using System;

namespace AIGames.HeadsUpOmaha
{
	/// <summary>Represents a staticial math helper class.</summary>
	public static class StatisticalMath
	{
		/// <summary>Gets the number of permutations.</summary>
		public static int Permutation(ulong n, ulong k)
		{
			ulong p = 1;

			var n1 = n - k > k ? n - k : k;
			var k1 = n - k < k ? n - k : k;


			for (ulong nI = n; nI > n1; nI--)
			{
				p *= nI;
			}
			for (ulong kI = k1; k1 > 1; kI--)
			{
				p /= kI;
			}
			return (int)p;
		}

		/// <summary>Gets the next permutation given a array of comparable elements.</summary>
		public static bool NextPermutation<T>(this T[] elements) where T : IComparable<T>
        {
            /*
          Knuths
          1. Find the largest index j such that a[j] < a[j + 1]. If no such index exists, the permutation is the last permutation.
          2. Find the largest index l such that a[j] < a[l]. Since j + 1 is such an index, l is well defined and satisfies j < l.
          3. Swap a[j] with a[l].
          4. Reverse the sequence from a[j + 1] up to and including the final element a[n].

          */
            var largestIndex = -1;
            for (var i = elements.Length - 2; i >= 0; i--)
            {
                if (elements[i].CompareTo(elements[i + 1]) < 0)
                {
                    largestIndex = i;
                    break;
                }
            }

            if (largestIndex < 0) return false;

            var largestIndex2 = -1;
            for (var i = elements.Length - 1; i >= 0; i--)
            {
                if (elements[largestIndex].CompareTo(elements[i]) < 0)
                {
                    largestIndex2 = i;
                    break;
                }
            }

            var tmp = elements[largestIndex];
            elements[largestIndex] = elements[largestIndex2];
            elements[largestIndex2] = tmp;

            for (int i = largestIndex + 1, j = elements.Length - 1; i < j; i++, j--)
            {
                tmp = elements[i];
                elements[i] = elements[j];
                elements[j] = tmp;
            }

            return true;
        }
	}
}
