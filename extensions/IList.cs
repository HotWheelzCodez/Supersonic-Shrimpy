using System.Collections.Generic;
using System;

public static class IListExt {
	public static void Shuffle<T>(this IList<T> list, Random rng)
	{
		int n = list.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
