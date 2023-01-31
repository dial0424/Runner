using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;

public struct Btn
{
    public Texture tex;
    public iRect rt;
    public bool touch;

    public delegate void btnMethod();
    public btnMethod method;

    public Btn(Texture t, iRect r)
    {
        tex = t;
        rt = r;
        method = null;
        touch = false;
    }
}

public class mButtonProc
{
}

public class mButtonResult
{
    public int num;
    public Btn[] btns;

    public iPoint[] btnPoints = new iPoint[]
    {
        new iPoint(mButtonLoby.dw - 130f, mButtonLoby.dh - 130f),
    };

    public mButtonResult(int n, Texture[] t)
    {
        num = n;
        btns = new Btn[num];
        iPoint[] p = btnPoints;
        for (int i = 0; i < num; i++)
        {
            Texture tex = t[i];
            iPoint pos = p[i];
            iRect rt = new iRect(pos.x, pos.y, tex.width, tex.height);
            btns[i] = new Btn(tex, rt);
        }
    }

    public void drawBtn()
    {
        for (int i = 0; i < num; i++)
        {
            Texture t = btns[i].tex;
            iRect r = btns[i].rt;

            iGUI.instance.setRGBA(1, 1, 1, 1);
            iGUI.instance.drawImage(t, r.origin, iGUI.TOP | iGUI.LEFT);
        }
    }

    public void clickBtn(iPoint mouseP)
    {
        for (int i = 0; i < num; i++)
        {
            bool t = btns[i].touch = btns[i].rt.containPoint(mouseP);
        }
    }

    public void callMethod()
    {
        for (int i = 0; i < num; i++)
        {
            if (btns[i].touch)
                btns[i].method();
        }
    }
}

public class mButtonLoby
{
    public int num;
    public Btn[] btns;
    float time = 0f;
    int w, h;

    public static float dw = MainCamera.devWidth, dh = MainCamera.devHeight;

    public iPoint[] btnPoints = new iPoint[]
    {
        new iPoint(dw - 130f, dh - 130f),   
    };
        

    public mButtonLoby(int n, Texture[] t)
    {
        num = n;
        btns = new Btn[num];
        iPoint[] p = btnPoints;
        for(int i = 0; i < num; i++)
        {
            Texture tex = t[i];
            iPoint pos = p[i];
            iRect rt = new iRect(pos.x, pos.y, tex.width, tex.height);
            btns[i] = new Btn(tex, rt);
        }
    }

    public void drawBtn()
    {
        for (int i = 0; i < num; i++)
        {
            Texture t = btns[i].tex;
            iRect r = btns[i].rt;

            iGUI.instance.setRGBA(1, 1, 1, 1);
            iGUI.instance.drawImage(t, r.origin, iGUI.TOP | iGUI.LEFT);
        }
    }

    public void clickBtn(iPoint mouseP,iKeystate stat)
    {
        for(int i = 0; i < num; i++)
        {
            Btn btn = btns[i];
            btn.touch = btn.rt.containPoint(mouseP);
            w = btn.tex.width;
            h = btn.tex.height;
            if (btn.touch) aniBtn(btn, stat, mouseP);
        }
    }

    public void callMethod()
    {
        for(int i=0;i<num;i++)
        {
            if (btns[i].touch)
                btns[i].method();
        }
    }


    public void aniBtn(Btn btn, iKeystate stat,iPoint point)
    {
        if(stat == iKeystate.Began)
        {
            if (btn.touch)
            {
                if (time > 0f)
                    time -= Time.deltaTime;
                else
                {
                    time = 0f;
                }

                btn.tex.width = (w - (int)time);
                btn.tex.height = (h - (int)time);
            }
            else
            {
                btn.tex.width = w;
                btn.tex.height = h;
            }
        }

        else if (stat == iKeystate.Moved)
        {
            if (!btn.rt.containPoint(point))
                btn.touch = false;
        }

        else if(stat == iKeystate.Ended)
        {
            if (btn.touch)
                btn.method();
        }
    }
}
