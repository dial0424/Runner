using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;
using UnityEditor;

namespace STD
{
	public class iImage
	{
		public List<iTexture> listTex;

		public iTexture tex;
		public iPoint position;

		public bool animation;
		public int repeatIdx, repeatNum; // 0 : loop, 1 ~ : count
		public int frame;
		public float frameDt, _frameDt;
		public float scale;

		public bool[] btnState;
		//public bool clickBegan;
		//public bool select;
		//public bool clickEnd;
		public float selectDt, _selectDt;
		public float selectScale;

		public bool once;

		public int anc;
		public bool leftRight;
		public float red, green, blue, alpha;

		public iImage()
		{
			listTex = new List<iTexture>();

			//tex;
			position = new iPoint(0, 0);

			animation = false;
			repeatIdx = 0;
			repeatNum = 0;
			frame = 0;
			_frameDt = 0.0167f;// 1 / 60
			frameDt = 0.0f;
			scale = 1.0f;

			btnState = new bool[3];	//0 : began, 1: select, 2 : end
			for (int i = 0; i < btnState.Length; i++)
				btnState[i] = false;
			//clickBegan = false;
			//select = false;
			//clickEnd = false;
			selectDt = 0.0f;
			_selectDt = 0.2f;
			selectScale = 0.2f;

			once = true;

			anc = iGUI.TOP | iGUI.LEFT;
			leftRight = false;

			red = 1f;
			green = 1f;
			blue = 1f;
			alpha = 1f;
		}

		public iImage clone()
		{
			iImage img = new iImage();
			for(int i=0; i < listTex.Count; i++)
				img.add(listTex[i]);

			img.tex = tex;
			img.position = position;

			img.animation = animation;
			img.repeatIdx = repeatIdx;
			img.repeatNum = repeatNum;
			img.frame = frame;
			img.frameDt = frameDt;
			img._frameDt = _frameDt;
			img.scale = scale;

			img.btnState = btnState;
			//img.clickBegan = clickBegan;
			//img.select = select;
			//img.clickEnd = clickEnd;
			img.selectDt = selectDt;
			img._selectDt = _selectDt;
			img.selectScale = selectDt;

			img.once = once;

			img.anc = anc;
			img.leftRight = leftRight;

			img.alpha = alpha;

			return img;
		}

		public void add(iTexture t)
		{
			if (tex == null)
				tex = t;
			listTex.Add(t);
			t.retainCount++;
		}

		public void set(int index)
		{
			frame = index;
		}

		public void paint(float dt, float r=1, float g=1, float b=1, float alpha = 1)
		{
			paint(dt, new iPoint(0, 0), r, g, b, alpha);
		}
		public void paint(float dt, iPoint off, float r = 1, float g = 1, float b = 1, float alpha = 1)
		{
			if (animation)
			{
				frameDt += dt;
				if (frameDt >= _frameDt)
				{
					frameDt -= _frameDt;
					frame++;

					if (frame == listTex.Count)
					{
						frame = 0;
						repeatIdx++;

						if (repeatNum == 0)
							;// loop
						else// if (repeatNum != 0)
						{
							if (repeatIdx == repeatNum)
							{
								animation = false;
								frame = listTex.Count - 1;
							}
						}
					}
				}
			}

			tex = listTex[frame];
			Texture t = tex.tex;
			//iGUI.instance.drawImage(t, position + off, iGUI.TOP | iGUI.LEFT);
			off += position;
			
			setSelectSclae();
#if true
			if( btnState[0] || btnState[1] || btnState[2])
			{
				selectDt += dt;
				if (selectDt > _selectDt)
					selectDt = _selectDt;
			}
			else
			{
				selectDt -= dt;
				if (selectDt < 0)
					selectDt = 0;
			}

			//float s = scale  + selectScale * selectDt / _selectDt;
			float s = scale;
			if( selectDt > 0 )
			{
				s += selectScale * selectDt / _selectDt;
				if (s != 1.0f)
				{
					off.x += (1 - s) * t.width / 2;
					off.y += (1 - s) * t.height / 2;
				}
			}
#endif
			Color bk = iGUI.instance.getRGBA();
			iGUI.instance.setRGBA(red*r, green*g, blue*b, alpha * this.alpha);
			iGUI.instance.drawImage(t, off.x, off.y, s, s,
				anc, 2, 0, leftRight ? iGUI.REVERSE_WIDTH : iGUI.REVERSE_NONE);
			iGUI.instance.setRGBA(bk.r, bk.g, bk.b, bk.a);
		}

		public void setBtnState(int idx)
        {
			for(int i = 0; i < btnState.Length; i++)
            {
				if (i == idx)
				{
					btnState[i] = true;
					continue;
				}
				btnState[i] = false;
            }
			once = true;
        }

		void setSelectSclae()
		{
			if (once)
			{
				selectDt = 0f;
				once = false;
			}
			if (btnState[0]) selectScale = -0.2f;
			if (btnState[1]) selectScale = 0.2f;
			if (btnState[2]) selectScale = 0;
		}

		public void startAnimation()
		{
			animation = true;
			repeatIdx = 0;
			frame = 0;
			frameDt = 0.0f;
		}

		public iRect touchRect()
		{
			return touchRect(new iPoint(0, 0), new iSize(0, 0));
		}

		public iRect touchRect(iPoint off, iSize s)
		{
			iGUI.instance.setRGBA(1, 1, 1, 0.5f);
			iGUI.instance.fillRect(position.x + off.x - s.width / 2,
								position.y + off.y - s.height / 2,
								tex.tex.width + s.width,
								tex.tex.height + s.height);

			return new iRect(	position.x + off.x - s.width / 2,
								position.y + off.y - s.height / 2,
								tex.tex.width + s.width,
								tex.tex.height + s.height);
		}

		public iPoint center()
		{
			return new iPoint(	position.x + tex.tex.width/2,
								position.y + tex.tex.height/2);
		}
		public iPoint center(iPoint off)
		{
			return new iPoint(	off.x + position.x + tex.tex.width / 2,
								off.x + position.y + tex.tex.height / 2);
		}
	}
}

