﻿using AIGames.HeadsUpOmaha.Platform;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Troschuetz.Random.Generators;

namespace AIGames.HeadsUpOmaha.Game
{
	/// <summary>Represents the game state.</summary>
	[Serializable]
	public class GameState
	{
		/// <summary>Constructs a new game state.</summary>
		public GameState()
		{
			this.Player1 = new PlayerState();
			this.Player2 = new PlayerState();
			this.HandsPerLevel = 10;
			this.SmallBlind = 15;
		}

		/// <summary>Constructs a new game state based on settings..</summary>
		public GameState(Settings settings): this()
		{
			Update(settings);
		}
		/// <summary>The player of this bot.</summary>
		public PlayerType YourBot { get; set; }

		/// <summary>The number of the currently played hand, counting starts at 1.</summary>
		public int Round { get; set; }

		/// <summary>The number of hands per (blind) level.</summary>
		public int HandsPerLevel { get; set; }

		/// <summary>The card that are on the table.</summary>
		/// <remarks>
		/// This can be 0, 3, 4, or 5 cards.
		/// </remarks>
		public Cards Table { get; set; }

		/// <summary>The name of the bot that currently has the dealer button.</summary>
		public PlayerType OnButton { get; set; }

		/// <summary>The current size of the small blind.</summary>
		public int SmallBlind { get; set; }

		/// <summary>The current size of the big blind.</summary>
		public int BigBlind { get { return SmallBlind * 2; } }

		/// <summary>Updates the blind when the number of hands per level is passed..</summary>
		public void UpdateBlind(int round)
		{
			if (round % HandsPerLevel == 0)
			{
				this.SmallBlind += 5;
			}
		}

		/// <summary>Get player based on the player type.</summary>
		public PlayerState this[PlayerType tp]
		{
			get
			{
				switch (tp)
				{
					case PlayerType.player1: return this.Player1;
					case PlayerType.player2:
					default: return this.Player2;
				}
			}
		}

		/// <summary>Gets own player.</summary>
		public PlayerState Own { get { return this[this.YourBot]; } }
		
		/// <summary>Gets other player.</summary>
		public PlayerState Opp { get { return this[this.YourBot.Other()]; } }

		/// <summary>Gets the player with the button (small blind).</summary>
		public PlayerState Button { get { return this[this.OnButton]; } }

		/// <summary>Gets the player with the big blind.</summary>
		public PlayerState Blind { get { return this[this.OnButton.Other()]; } }

		/// <summary>Represents player1 specific data.</summary>
		public PlayerState Player1 { get; set; }
		/// <summary>Represents player2 specific data.</summary>
		public PlayerState Player2 { get; set; }

		/// <summary>Gets the pot.</summary>
		public int Pot { get { return this.Player1.Pot + this.Player2.Pot; } }

		/// <summary>The result of the match.</summary>
		public RoundResult Result { get; set; }

		/// <summary>Total amount of chips currently in the pot (plus sidepot).</summary>
		public int MaxWinPot
		{
			get
			{
				return Player1.Pot + Player2.Pot;
			}
		}

		/// <summary>amountToCall i	The amount of chips your bot has to put in to call.</summary>
		public int AmountToCall
		{
			get
			{
				return Math.Abs(Player1.Pot - Player2.Pot);
			}
		}

		/// <summary>Updates the state based on the settings.</summary>
		public void Update(Settings settings)
		{
			this.YourBot = settings.YourBot;
			this.HandsPerLevel = settings.HandsPerLevel;
			this.Player1.Update(settings);
			this.Player2.Update(settings);
		}

		/// <summary>Updates the state based on an action instruction.</summary>
		public void UpdateAction(Instruction instruction)
		{
			switch (instruction.Action)
			{
				case "player1": this.Player1.TimeBank = TimeSpan.FromMilliseconds(instruction.Int32Value); break;
				case "player2": this.Player2.TimeBank = TimeSpan.FromMilliseconds(instruction.Int32Value); break;
			}
		}

		/// <summary>Updates the state based on a match instruction.</summary>
		public void UpdateMatch(Instruction instruction)
		{
			switch (instruction.Action)
			{
				case "round":
					this.Round = instruction.Int32Value;
					Reset();
					break;
				case "smallBlind":
					this.SmallBlind = instruction.Int32Value;
					break;
				// ignore, should be two times the small blind.
				case "bigBlind": break;
				// ignore, is the sum of the pot of player1 and player2.
				case "maxWinPot": break;
				// ignore, is difference of the pot of player1 and player2.
				case "amountToCall": break;

				case "onButton":
					this.OnButton = instruction.PlayerTypeValue;
					break;

				case "table":
					this.Table = instruction.CardsValue;
					break;
			}
		}

		/// <summary>Updates the state based on a player instruction.</summary>
		public void UpdatePlayer(Instruction instruction)
		{
			var player = instruction.Player;

			switch (instruction.Action)
			{
				// Nothing need to be done.
				case "fold":
				// Nothing need to be done.
				case "check": break;
				case "hand": this[player].Hand = instruction.CardsValue; break;
				case "call": this[player].Call(this.AmountToCall); break;
				case "raise": this[player].Raise(instruction.Int32Value); break;
				case "stack": this[player].SetStack(instruction.Int32Value); break;
				case "post": this[player].Post(player == this.OnButton ? this.SmallBlind : this.BigBlind); break;
				case "wins":
					if (this.Result == RoundResult.NoResult)
					{
						this.Result = player == PlayerType.player1 ? RoundResult.Player1Wins : RoundResult.Player2Wins;
					}
					// if set, and another results follows, its a draw.
					else
					{
						this.Result = RoundResult.Draw;
					}
					break;
			}
		}

		/// <summary>Starts a new round.</summary>
		/// <param name="rnd">
		/// The randomizer to shuffle the deck.
		/// </param>
		public void StartNewRound(MT19937Generator rnd)
		{
			var deck = Cards.GetShuffledDeck(rnd);
			this.Player1.Hand = Cards.Create(deck.Take(4));
			this.Player2.Hand = Cards.Create(deck.Skip(4).Take(4));
			this.Table = Cards.Create(deck.Skip(8).Take(5));
			this.OnButton = this.OnButton.Other();
		}

		/// <summary>Apply the round result.</summary>
		public int ApplyRoundResult(RoundResult result)
		{
			int compare = 0;
			if (result == RoundResult.NoResult)
			{
				var pokerhand1 = PokerHand.CreateFromHeadsUpOmaha(this.Table, this.Player1.Hand);
				var pokerhand2 = PokerHand.CreateFromHeadsUpOmaha(this.Table, this.Player2.Hand);

				compare = PokerHandComparer.Instance.Compare(pokerhand1, pokerhand2);
			}
			else
			{
				switch (result)
				{
					case RoundResult.Player1Wins: compare = 1; break;
					case RoundResult.Player2Wins: compare = -1; break;
				}
			}
			int pot = this.Pot;

			if (compare == 0)
			{
				 pot /= 2;
				this.Player1.Win(pot);
				this.Player2.Win(pot);
			}
			else if (compare > 0)
			{
				this.Player1.Win(pot);
				this.Player2.Win(0);
			}
			else
			{
				this.Player2.Win(pot);
				this.Player1.Win(0);
			}
			return pot;
		}

		/// <summary>Makes a full copy of the settings for a specified player.</summary>
		public GameState Personalize(PlayerType player)
		{
			this.YourBot = player;
			return this;
		}

		/// <summary>Resets the state.</summary>
		/// <remarks>
		/// Tables, hands an result are being reset.
		/// </remarks>
		public void Reset()
		{
			this.Table = Cards.Empty;
			this.Player1.Reset();
			this.Player2.Reset();
			this.Result = RoundResult.NoResult;
		}

		#region I/O operations

		/// <summary>Saves the game to a file.</summary>
		/// <param name="fileName">
		/// The name of the file.
		/// </param>
		/// <param name="mode">
		/// The file mode.
		/// </param>
		public void Save(string fileName, FileMode mode = FileMode.Create) { Save(new FileInfo(fileName), mode); }

		/// <summary>Saves the game to a file.</summary>
		/// <param name="file">
		/// The file to save to.
		/// </param>
		/// <param name="mode">
		/// The file mode.
		/// </param>
		public void Save(FileInfo file, FileMode mode = FileMode.Create)
		{
			using (var stream = new FileStream(file.FullName, mode, FileAccess.Write))
			{
				Save(stream);
			}
		}

		/// <summary>Saves the game to a stream.</summary>
		/// <param name="stream">
		/// The stream to save to.
		/// </param>
		public void Save(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(GameState));
			serializer.Serialize(stream, this);
		}

		/// <summary>Loads the game from a file.</summary>
		/// <param name="fileName">
		/// The name of the file.
		/// </param>
		public static GameState Load(string fileName) { return Load(new FileInfo(fileName)); }

		/// <summary>Loads the game from a file.</summary>
		/// <param name="file">
		/// The file to load from.
		/// </param>
		public static GameState Load(FileInfo file)
		{
			using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
			{
				return Load(stream);
			}
		}

		/// <summary>Loads the game from a stream.</summary>
		/// <param name="stream">
		/// The stream to load from.
		/// </param>
		public static GameState Load(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(GameState));
			return (GameState)serializer.Deserialize(stream);
		}

		#endregion
	}
}
