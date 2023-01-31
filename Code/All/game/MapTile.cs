using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;

public class MapTile
{
    public delegate void methodMap();


    public int extend;
    public RenderTexture rt;

    public bool normal;

    public int[] tiles;
    public int tileX, tileY, tileW, tileH;
    public float moveSpeed;
    public float[] moveRate;
    public Texture[] texsBg;
    public Texture texFloor;
    public float arriveX;

    public iPoint off, offMin, offMax;
    public iPoint heroPos;

    public bool arrived;

    public methodMap methodMapArrived;

    public iItem item;
    public iBlock block;


    public MapTile(bool n, int[] _tiles, int _x, int _y, int _w, int _h, float _ms, float[] _mr, Texture[] _texs, Texture _tex, float _arriveX)
    {
        extend = (int)((float)MainCamera.devWidth * 720 / MainCamera.devHeight - MainCamera.devWidth); //720;
        //MainCamera.devWidth : MainCamera.devHeight = w : 720; 
        //w - dw = dw * 720 / dh - dw;
		rt = new RenderTexture(MainCamera.devWidth + extend, 720, 32, RenderTextureFormat.ARGB32);
        
        normal = n;

        tiles = _tiles;
        tileX = _x;
        tileY = _y;
        tileW = _w;
        tileH = _h;
        moveSpeed = _ms;
        moveRate = _mr;
        texsBg = _texs;
        texFloor = _tex;
        arriveX = tileX * tileW - _arriveX - extend;

        off = new iPoint(0, 0);
        offMax = off;
        offMin.x = MainCamera.devWidth + extend - tileX * tileW;

        heroPos = new iPoint(MainCamera.devWidth / 3, 519);

        arrived = false;

        methodMapArrived = null;

        item = new iItem(this);
        item.load(normal);

        block = new iBlock(this);
        block.load();
    }

    public void reset(bool fever)
    {
        if (fever)
        {
            off = new iPoint(0, 0);
            arrived = false;

            heroPos = new iPoint(MainCamera.devWidth / 3, 519);

            // item initialize
            item.load(normal);

			MapTile mt = Proc.me.currMap;
			Proc.me.currMap = Proc.me.maps[1];
            update(0.000001f);
			Proc.me.currMap = mt;
		}
	}

    void update(float dt)
    {
        if (dt == 0)
            return;

        RenderTexture bk = RenderTexture.active;
        RenderTexture.active = rt;

        //real paint
        Texture t;

        iGUI.instance.setRGBA(1, 1, 1, 1);

        for (int i = 0; i < texsBg.Length; i++)
        {
            float x = off.x * moveRate[i];

            t = texsBg[i];

            float w = t.width;
            if (x < -w)
                x += w;

            for (int j = 0; j < 6; j++)
            {
                iGUI.instance.drawImage(t, x + w * j, off.y, iGUI.TOP | iGUI.LEFT);
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            int x = tileW * (i % tileX);
            int y = tileH * (i / tileX);
            if (tiles[i] != 1) continue;
            iGUI.instance.drawImage(texFloor, x + off.x, y + off.y, iGUI.TOP | iGUI.LEFT);
        }

        block.draw(dt);
        item.draw(dt);

        if (arrived)
        {
            if (methodMapArrived != null)
                methodMapArrived();
        }

        RenderTexture.active = bk;
    }

	public void paint(float dt, float y)
	{
		update(dt);
		//iGUI.instance.drawImage(rt, 0, y, iGUI.TOP | iGUI.LEFT);
		iGUI.instance.drawImage(rt, 0, y, MainCamera.resolutionRate, MainCamera.resolutionRate, iGUI.TOP | iGUI.LEFT, 2, 0, iGUI.REVERSE_NONE);
	}

}
