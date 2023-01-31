using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STD
{ 
	public struct SortData
	{
		public float y;
		public int index;
	}

	public class iSort
	{
		SortData[] sd;
		public static int sdNum;

		private iSort(int max)
		{
			sd = new SortData[max];
			sdNum = 0;
		}

		private static iSort sort = null;
		private static iSort instance()
		{
			if (sort == null)
				sort = new iSort(100);
			return sort;
		}

		private void _add(float y)
		{
			sd[sdNum].y = y;
			sd[sdNum].index = sdNum;
			sdNum++;
		}
		public static void add(float y)
		{
			instance()._add(y);
		}

		private int _get(int i)
		{
			return sd[i].index;
		}
		public static int get(int i)
		{
			return instance()._get(i);
		}

		private void _init()
		{
			sdNum = 0;
		}
		public static void init()
		{
			instance()._init();
		}

		private void _update()
		{
			int i, j, n = sdNum - 1;
			for (i = 0; i < n; i++)
			{
				for (j = i + 1; j < sdNum; j++)
				{
					if (sd[i].y > sd[j].y)// > increase , < decrease
					{
						SortData t = sd[i];
						sd[i] = sd[j];
						sd[j] = t;
					}
				}
			}
		}
		public static void update()
		{
			instance()._update();
		}

	}
}

