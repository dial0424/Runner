using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;
public class iItem
{
	public struct Item
	{
		public bool alive;
		public int type;    //0 : 일반, 1 : 돈, 2 : 특수, ~~~
		public int value;

		public iPoint position;
		public iSize size;

		public float aniDt;

		public iRect rect()
		{
			return new iRect(position.x, position.y, size.width, size.height);
		}
	}

	int[][] pattern = new int[][]
	{
		new int[] {
			2, 10*0, 10*0,
		},
		new int[] {
			4, 10*0, 10*0, 10*0, 10*0,
		},
		new int[]
		{
			3, 10*0, 10*1, 10*1,
		},
		new int[]
		{
			9, 10*0, 10*1, 10*2, 10*3, 10*4,
			   10*4, 10*3, 10*2, 10*1,
		},
		new int[]
		{
			12, 10*0, 10*1, 10*2, 10*3,
				10*3, 10*4, 10*5, 10*5,
				10*4, 10*3, 10*2, 10*1,
		},
	};

	Item[] _items;
	int _num;
	public Item[] items;
	public int num;
	Texture[] texs;

	MapTile map;

	bool mapTypeNormal;

	public iItem(MapTile m)
	{
		map = m;
	}

	public void load(bool mt)
	{
		_num = 200;
		_items = new Item[_num];
		for (int i = 0; i < _num; i++)
		{
			_items[i] = new Item();
			_items[i].alive = false;
		}

		items = new Item[_num];
		num = 0;

		texs = new Texture[7];
		for (int i = 0; i < 7; i++)
			texs[i] = Resources.Load<Texture>("item" + i);

		mapTypeNormal = mt;

		layoutItem();
	}

	void layoutItem()
	{
		int n = 9;
		if (map.normal)
			n = 80;
		int h = 600;

		iPoint p = new iPoint(n, h);

		while (p.x < _num)
		{
			p = setPattern((int)p.x, (int)p.y);
			if (p.x >= (int)map.arriveX / map.tileW)
				break;
		}
	}

	iPoint setPattern(int xx, int yy)
	{
		int[] p = pattern[Random.Range(0, pattern.Length)];
		bool changeY = false;

		int y = -1;
		int j = 0;
		int idx = 0;
		for (int i = 0; i < p[0]; i++)
		{
			int n = p[1 + j];
			int what = Random.Range(0, 2);
			y = n / 10;

			if (mapTypeNormal)
			{
				if (xx % 7 == 0)
					what = Random.Range(3, 6);
			}
			else what = 6;

			idx = map.tileX * (10 - y) + xx - (600 - yy) / map.tileH * map.tileX;
			if(map.tiles[idx] == 1)
			{
				if(p[j]/10 < y)
				{
					y = p[j] / 10;
					j--;
				}
				else
				{
					return new iPoint(xx, yy - map.tileH * (y + 1));
				}
			}


			int value = 100;
			if (what == 6)
				value = 300;
			addItem(what, new iPoint(map.tileW * xx, yy - map.tileH * y), value);

			j = (j == i) ? i + 1 : j - 1;
			xx++;
			if(j < 0)
				return new iPoint(xx, yy);
		}
		//다 끝났는데 아래에 타일이 없으면 타일 만날때까지 떨어지도록
		while(map.tiles[idx + map.tileX] != 1)
		{
			yy += map.tileH;
			int what = Random.Range(0, 2);
			if (!mapTypeNormal)
				what = 6;
			addItem(what, new iPoint(map.tileW * xx, yy - map.tileH * y), 100);
			xx++;
			idx = map.tileX * (10 - y) + xx - (600 - yy) / map.tileH * map.tileX;
		}

		return new iPoint(xx, yy);
	}

	void addItem(int type, iPoint p, int value)
	{
		for (int i = 0; i < _num; i++)
		{
			ref Item it = ref _items[i];
			if (it.alive == false)
			{
				it.alive = true;
				it.type = type;
				it.position = p;
				it.value = value;

				items[num] = it;
				num++;
				return;
			}
		}
	}

	public void draw(float dt)
	{
		for (int i = 0; i < num; i++)
		{
			ref Item it = ref items[i];

			// draw
			Texture t = texs[it.type];
			it.size = new iSize(t.width, t.height);

			iPoint p = it.position + Proc.me.currMap.off;

			it.aniDt += dt;
			if (it.aniDt > 0.3f)
				it.aniDt -= 0.3f;

			float s = it.aniDt * 0.4f + 1f;

			iGUI.instance.setRGBA(1, 1, 1, 1);
			iGUI.instance.drawImage(t,
									p.x + t.width / 2, p.y + t.height / 2,
									s, s,
									iGUI.VCENTER | iGUI.HCENTER,
									1, 0, iGUI.REVERSE_NONE);
		}
	}
}
