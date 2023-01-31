using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;
class Hero
{
	public enum State
	{
		RUN = 0,
		SLIDE,
		JUMP,
		CRASH,
		DEAD
	}

	public State state;

	public float moveSpeed;

	bool land;
	public float dmgDt;

	float fallSpeed;
	private static float gravity = 1200;

	public bool lie;

	int jumpN;
	int _jumpN;

	public bool mapChanging;

	//chacData cd;

	iImage img;
	iImage[] imgs;

	float rtRate;
	public iRect rt;
	
	public static int heroIdx;

	public Hero(iPoint p, float ms)
	{
		state = State.RUN;

		moveSpeed = ms;

		land = false;
		dmgDt = 0f;

		fallSpeed = 0;

		lie = false;

		jumpN = 0;
		_jumpN = 2;

		mapChanging = false;

#if false
		byte[] bytes = FileIO.load("chacData");
		cd = FileIO.bytes2struct<chacData>(bytes);

		int idx = cd.centerIdx;
#else
		int idx = heroIdx;
#endif
		//0 : RUN, 1 : SLIDE, 2 : JUMP, 3 : CRASH
		imgs = new iImage[5] { new iImage(), new iImage(), new iImage(), new iImage(), new iImage()};

		for (int i = 0; i < 14; i++)
		{
			Texture tex = Resources.Load<Texture>("cookie" + idx + "_" + i);
			iTexture iTex = new iTexture(tex);
			if (i < 4)
				imgs[0].add(iTex);
			else if (i < 6)
				imgs[1].add(iTex);
			else if (i < 8)
				imgs[2].add(iTex);
			else if (i < 10)
				imgs[3].add(iTex);
			else
				imgs[4].add(iTex);
		}
		for (int i = 0; i < 5; i++)
			imgs[i].animation = true;
		imgs[3].repeatNum = 3;
		imgs[4].repeatNum = 1;

		img = imgs[0];

		img._frameDt = 0.1f;
		img.startAnimation();
		img.position = p;

		rtRate = 0.70f;
		rt.origin = new iPoint(p.x + img.listTex[0].tex.width * rtRate / 2, p.y);
		rt.size = new iSize(img.listTex[0].tex.width * rtRate, img.listTex[0].tex.height)* MainCamera.resolutionRate;
	}

	public void paint(float dt)
	{
		setImg((int)state);

		MapTile map = Proc.me.currMap;

#if true
		ref iPoint off = ref map.off;
		ref iPoint offMin = ref map.offMin;
		ref iPoint offMax = ref map.offMax;

		float x = rt.origin.x + off.x;// == MainCamera.devWidth / 3
		off.x = MainCamera.devWidth / 3 - rt.origin.x;
		if (off.x < offMin.x)
			off.x = offMin.x;
		else if (off.x > offMax.x)
			off.x = offMax.x;
#endif
		if (mapChanging == false)
			collision(dt);
#if true// ctrl
		iPoint v = new iPoint(0, 0);
		if (v.x != 0 || v.y != 0)
			v /= v.getLength();

		iPoint t = v * moveSpeed * dt;
		t.y += fall(dt);

#if false   //¾ÈµÊ....
		if (t.y > map.tileH)
		{
			dt *= map.tileH / t.y;
			v *= moveSpeed * dt;
			v.y += fall(dt);
		}
		else 
#endif
		v = t;

		float c = 1;
		if (state != State.DEAD)
		{
			if (rt.origin.x < map.arriveX)
			{
				if (mapChanging == false)
					rt.origin.x += moveSpeed * dt;
				if (rt.origin.x >= map.arriveX)
				{
					rt.origin.x = map.arriveX;
					map.arrived = true;
					Debug.Log("arrive");
				}
			}
#endif
			c = 1f;
			if (dmgDt > 0f)
			{
				c = Mathf.Abs(Mathf.Sin(360 * dmgDt * Mathf.Deg2Rad));
			}
		}
		
		if (dt > 0)
			move(v, map.tileX, map.tileY, map.tileW, map.tileH, map.tiles);

		//img.position = rt.origin + map.off;
		img.position.x = rt.origin.x + map.off.x;
		img.position.y = rt.origin.y;

		img.position *= MainCamera.resolutionRate;
		img.scale = MainCamera.resolutionRate;
		img.paint(dt, 1, c, c, 1);

		run();
	}
	void setImg(int idx)
	{
		if (idx == (int)State.CRASH)
		{
			if (img.repeatIdx == img.repeatNum)
			{
				state = State.RUN;
				idx = (int)State.RUN;
			}
			else if (!imgs[idx].animation)
				imgs[idx].startAnimation();
		}
		rt.origin.y += img.listTex[0].tex.height - imgs[idx].listTex[0].tex.height;
		img = imgs[idx];
		img._frameDt = 0.1f;

		rt.size = new iSize(img.listTex[0].tex.width * rtRate, img.listTex[0].tex.height);
	}

	public void move(iPoint v, int tX, int tY, int tW, int tH, int[] tiles)
	{
		bool meet = false;

		//     y
		// x        x2
		//
		float x = rt.origin.x;
		float y = rt.origin.y;

		int x_ = (int)x; x_ /= tW;
		int y_ = (int)y; y_ /= tH;

		float x2 = x + rt.size.width;
		int x2_ = (int)x2; x2_ /= tW;


		y += v.y;

		int nX = x2_ - x_ + 1;

		float minY = 0;
		for (int j = y_ - 1; j > -1; j--)
		{
			for (int i = x_; i < x_ + nX; i++)
			{
				if (tiles[tX * j + i] == 1)
				{
					minY = tH * (j + 1);
					meet = true;
					break;
				}
			}
			if (meet) break;
		}

		if (y < minY)
		{
			y = minY;
			fallSpeed = 0;
		}
		rt.origin.y = y;

		y = rt.origin.y + rt.size.height;
		y_ = (int)y; y_ /= tH;

		for (int j = y_ + 0; j < tY; j++)
		{
			for (int i = x_; i <= x_ + nX; i++)
			{
				if (tiles[tX * j + i] == 1)
				{
					maxY = tH * j - 1;
					meet = true;
					break;
				}
			}

			if (meet) break;
		}

		if (y > maxY)
		{
			y = maxY;
			land = true;
			fallSpeed = 0;
			jumpN = 0;

			
		}
		else
		{
			land = false;
		}

		rt.origin.y = y - rt.size.height;
	}
	float maxY = 659;

	public float fall(float dt)
	{
		if (land == false)
			fallSpeed += gravity * dt;
		else
		{
			fallSpeed = 0;
		}
		return fallSpeed * dt;
	}

	public void run()
	{
		if (land && !lie && state != State.CRASH)
		{
			state = State.RUN;
		}
	}

	public void slide()
	{
		if (state != State.CRASH)
		{
			state = State.SLIDE;
			lie = true;
		}
	}

	public void jump()
	{
		if (jumpN < _jumpN)
		{
			state = State.JUMP;
			land = false;
			fallSpeed = -600;
			jumpN++;
		}
		else
		{
			Debug.Log("¿­¹Þ¾Æ!!!!!");
		}
	}

	public void dead()
	{
		state = State.DEAD;
	}

	public void crash()
	{
		state = State.CRASH;
		MainCamera.soundManager.playCookieSound(2);
	}

	void collision(float dt)
	{
		Proc p = Proc.me;
		iItem item = p.currMap.item;
		iBlock block = p.currMap.block;

		// item
		for (int i = 0; i < item.num; i++)
		{
			ref iItem.Item it = ref item.items[i];

			if (it.rect().contianRect(rt))
			{
				int type = it.type;
				p.addItemEffect(type, 3, it.position + Proc.me.currMap.off, it.value);
				p.addItemEffect(type, 1, it.position + Proc.me.currMap.off, it.value);

				if (type > 2 && type < 6)
				{
					p.popFeverOpenClose(true);
					type = 0;
				}

				it.alive = false;
				item.num--;
				item.items[i] = item.items[item.num];
				i--;

				MainCamera.soundManager.playProcSound(type);
			}
		}

		// block
		if (dmgDt == 0f)
		{
			for (int i = 0; i < block.num; i++)
			{
				ref iBlock.Block b = ref block.blocks[i];
				if (b.rect().contianRect(rt))
				{
					crash();
					p.loseLife(b.damage);
					
					dmgDt = 3.0f;
				}
			}
		}
		else
		{
			dmgDt -= dt;
			if (dmgDt < 0f)
				dmgDt = 0f;
		}
	}
}
