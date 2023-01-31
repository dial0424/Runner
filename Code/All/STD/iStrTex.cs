using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;

public class iTexture
{
	public Texture tex;
	// etc
	public int retainCount;

	public iTexture(Texture t)
	{
		tex = t;
		retainCount = (t == null ? 0 : 1);
	}
}

public class iStrTex
{
	// ============================================================
	// static : runSt()를 void OnGUI()함수 내 서두 부분에 호출해야 함.
	// ============================================================
	public static void runSt()
	{
		for (int i = 0; i < stInfoNum; i++)
		{
			ref StInfo info = ref stInfo[i];
			cb(info.cls, info.method);
		}
		stInfoNum = 0;
	}

	private static void cb(iStrTex st, MethodSt method)
	{
#if true// #issue
		if (st.tex.tex != st.texReserve)
			((RenderTexture)st.tex.tex).Release();
#endif
		st.tex.tex = st.texReserve;

		RenderTexture bkT = RenderTexture.active;
		RenderTexture.active = (RenderTexture)st.tex.tex;
		//Rect bkR = Camera.main.rect;
		//Camera.main.rect = new Rect(0, 0, 1, 1);
		Matrix4x4 matrixBk = GUI.matrix;
		GUI.matrix = Matrix4x4.TRS(
			Vector3.zero, Quaternion.identity, new Vector3(1, 1, 1));

		GL.Clear(true, true, Color.clear);// add

		string sn = iGUI.instance.getStringName();
		float ss = iGUI.instance.getStringSize();
		Color sc = iGUI.instance.getStringRGBA();
		iGUI.instance.setStringName(st.stringName);
		iGUI.instance.setStringSize(st.stringSize);
		iGUI.instance.setStringRGBA(st.stringColor.r,
									st.stringColor.g,
									st.stringColor.b,
									st.stringColor.a);

		if( method!=null )
			method(st);

		iGUI.instance.setStringName(sn);
		iGUI.instance.setStringSize(ss);
		iGUI.instance.setStringRGBA(sc.r,
									sc.g,
									sc.b,
									sc.a);

		RenderTexture.active = bkT;
		//Camera.main.rect = bkR;
		GUI.matrix = matrixBk;
	}


	public delegate void MethodSt(iStrTex st);
	public struct StInfo
	{
		public MethodSt method;
		public iStrTex cls;

		public StInfo(MethodSt m, iStrTex c)
		{
			method = m;
			cls = c;
		}
	}
	static StInfo[] stInfo = new StInfo[100];
	static int stInfoNum;

	// ============================================================
	// member
	// ============================================================
	public iTexture tex; //public RenderTexture tex;
	public string str, strNew;
	string stringName;
	float stringSize;
	Color stringColor;

	MethodSt methodSt;
	public float wid, hei;
	bool fixedWH;
	public RenderTexture texReserve;

	public iStrTex(float w = 0f, float h = 0f)
	{
		tex = new iTexture(null);
		str = null;
		strNew = null;
		stringName = null;
		stringSize = 30;
		stringColor = Color.clear;
		methodSt = null;
		wid = w;
		hei = h;
		fixedWH = !(w == 0f && h == 0f);
		texReserve = null;
	}

	public iStrTex(MethodSt m, float w = 0f, float h = 0f)
	{
		tex = new iTexture(null);
		str = null;
		strNew = null;
		stringName = null;
		stringSize = 30;
		stringColor = Color.clear;
		methodSt = m;
		wid = w;
		hei = h;
		fixedWH = !(w == 0f && h == 0f);
		texReserve = null;
	}

	public void setStringName(string name)
	{
		stringName = name;
	}
	public void setStringSize(float s)
	{
		stringSize = s;
	}
	public void setString(string s)
	{
		//strNew = s;
		getTexture(s);
	}
	public void setStringRGBA(float r, float g, float b, float a)
	{
		stringColor = new Color(r, g, b, a);
	}

	public void drawString(iPoint p, int anc)
	{
		Texture t = getTexture();
		iGUI.instance.drawImage(t, p.x, p.y, anc);
	}
	public void drawString(float x, float y, int anc)
	{
		Texture t = getTexture();
		iGUI.instance.drawImage(t, x, y, anc);
	}

	public void drawString(string str, iPoint p, int anc)
	{
		Texture t = getTexture(str);
		iGUI.instance.drawImage(t, p.x, p.y, anc);
	}
	public void drawString(string str, float x, float y, int anc)
	{
		Texture t = getTexture(str);
		iGUI.instance.drawImage(t, x, y, anc);
	}
	public void drawString(string str, iPoint p, int anc, int xyz, float degree)
	{
		Texture t = getTexture(str);
		iGUI.instance.drawImage(t, p.x, p.y, 1f, 1f, anc, xyz, degree, iGUI.REVERSE_NONE);
	}
	public void drawString(string str, float x, float y, int anc, int xyz, float degree)
	{
		Texture t = getTexture(str);
		iGUI.instance.drawImage(t, x, y, 1f, 1f, anc, xyz, degree, iGUI.REVERSE_NONE);
	}

	public void drawString(string str, iPoint p, float sx, float sy, int anc, int xyz, float degree)
	{
		Texture t = getTexture(str);
		iGUI.instance.drawImage(t, p.x, p.y, sx, sy, anc, xyz, degree, iGUI.REVERSE_NONE);
	}
	public void drawString(string str, float x, float y, float sx, float sy, int anc, int xyz, float degree)
	{
		Texture t = getTexture(str);
		iGUI.instance.drawImage(t, x, y, sx, sy, anc, xyz, degree, iGUI.REVERSE_NONE);
	}


	public Texture getTexture(string s)
	{
		strNew = s;
		return getTexture();
	}

	public Texture getTexture()
	{
		if (str == strNew)
			return tex.tex;
		str = strNew;

		string sn = iGUI.instance.getStringName();
		float ss = iGUI.instance.getStringSize();
		Color sc = iGUI.instance.getStringRGBA();
		iGUI.instance.setStringName(stringName);
		iGUI.instance.setStringSize(stringSize);
		iGUI.instance.setStringRGBA(stringColor.r,
									stringColor.g,
									stringColor.b,
									stringColor.a);

		iSize size = iGUI.instance.sizeOfString(str);

		iGUI.instance.setStringName(sn);
		iGUI.instance.setStringSize(ss);
		iGUI.instance.setStringRGBA(sc.r, sc.g, sc.b, sc.a);

		if ( fixedWH==false )
		{
			wid = size.width;
			hei = size.height;
		}
		// realsize 1000 x 100 w = 1000, h = 100
		// use size 830 x 100 w = 830
		texReserve = new RenderTexture(
						(int)(wid + 0.5f),
						(int)(hei + 0.5f), 32,
						RenderTextureFormat.ARGB32);
		if (tex.tex == null)
		{
			tex.tex = texReserve;

			// backup
			RenderTexture bkT = RenderTexture.active;
			RenderTexture.active = (RenderTexture)tex.tex;

			GL.Clear(true, true, Color.clear);

			// rollback
			RenderTexture.active = bkT;
		}

		StInfo info;
		if ( methodSt==null )
			info = new StInfo(new MethodSt(cbGetTexture), this);
		else
			info = new StInfo(methodSt, this);

		stInfo[stInfoNum] = info;
		stInfoNum++;

		return tex.tex;
	}

	public static void cbGetTexture(iStrTex st)
	{
		iGUI.instance.setStringName(st.stringName);
		iGUI.instance.setStringSize(st.stringSize);
		iGUI.instance.setStringRGBA(st.stringColor.r,
									st.stringColor.g,
									st.stringColor.b,
									st.stringColor.a);

		iGUI.instance.drawString(st.str, 0, 0);
	}

}
