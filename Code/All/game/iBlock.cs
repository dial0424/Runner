using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;
public class iBlock
{
	public struct Block
	{
		public bool alive;
		public int type;      //데미지가 다름
		public float damage;
		public iSize size;
		public iPoint position;
		public Texture texture;

		public iRect rect()
		{
			if (type == 1)
				return new iRect(position.x + texture.width / 5 * 2, position.y, size.width / 5, size.height);
			else
				return new iRect(position.x, position.y, size.width, size.height);
		}
	}

	public Block[] blocks;
	public int num;
	Texture[] texs;

	MapTile map;

	public iBlock(MapTile m)
	{
		map = m;
	}

	public void load()
	{
		num = 20;
		blocks = new Block[num];
		for (int i = 0; i < num; i++)
		{
			blocks[i] = new Block();
			blocks[i].alive = false;
		}

		texs = new Texture[2];
		for (int i = 0; i < 2; i++)
			texs[i] = Resources.Load<Texture>("block" + i);

		int[] tiles = map.tiles;
		for (int i = 0, j = 0; i < tiles.Length; i++)
		{
			if (j >= num) break;
			if (tiles[i] == 2 || tiles[i] == 3)
			{
				ref Block b = ref blocks[j];
				int x = map.tileW * (i % map.tileX);
				int y = map.tileW * (i / map.tileX);

				b.alive = true;
				b.type = tiles[i] - 2;
				b.texture = texs[b.type];
				if (b.type == 1)
					b.position = new iPoint(x, y - b.texture.height / 3 * 2);
				else
					b.position = new iPoint(x, y - b.texture.height / 2);
				b.damage = tiles[i] * 2f;
				j++;
			}
		}
	}

	public void draw(float dt)
	{
		for (int i = 0; i < num; i++)
		{
			ref Block bl = ref blocks[i];

			if (!bl.alive)
				continue;

			Texture t = bl.texture;
			bl.size = new iSize(t.width, t.height);

			iPoint p = bl.position + map.off;

			iGUI.instance.setRGBA(1, 1, 1, 1);
			iGUI.instance.drawImage(t, p.x, p.y, iGUI.TOP | iGUI.LEFT);
		}
	}
}
