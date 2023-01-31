using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class nGUI : iGUI
{
    RenderTexture texBack, texFbo;
    Rect rtBack;
    
	void Start()
    {
        init();

        texFbo = new RenderTexture(	MainCamera.devWidth,
									MainCamera.devHeight, 32,
									RenderTextureFormat.ARGB32);

        Camera.onPreCull = onPreCull;
        Camera.onPreRender = onPrev;
        Camera.onPostRender = onEnd;
		//Camera.main.rect = new Rect(0, 0, 1, 1);// default;

		bLoad = false;
    }
	bool bLoad;

    void onPreCull(Camera c)
    {
        preCull();
    }

    public void onPrev(Camera c)
    {
        texBack = c.targetTexture;
        c.targetTexture = texFbo;

        rtBack = Camera.main.rect;
        Camera.main.rect = new Rect(0, 0, 1, 1);
    }
    // void OnRenderObject(){}
    public void onEnd(Camera c)
    {
        c.targetTexture = texBack;

        Camera.main.rect = rtBack;
    }

	//void Update() { }

	class iFPS
	{
		iStrTex st;
		int frame, _frame;
		float takeTime, _taketime;

		private iFPS()
		{
			frame = 0;
			_frame = 0;
			takeTime = 0f;
			_taketime = 0F;

			st = new iStrTex();
			st.setStringSize(30);
			st.setStringRGBA(1, 0, 1, 1);
			st.setString("60");
		}

		static iFPS instance = null;
		public static void draw(float dt, iPoint p)
		{
			if( instance==null )
				instance = new iFPS();
			instance._draw(dt, p);
		}

		private void _draw(float dt, iPoint p)
		{
			st.drawString(p, TOP | LEFT);
			if (dt > 0.1f)
				return;

			_frame++;
			_taketime += dt;
			int fpsAvg = (int)(_frame / _taketime);

			frame++;
			takeTime += dt;
			if (takeTime >= 1f)
			{
				int fps = (int)(frame / takeTime);
				st.setString("avg fps: " + fpsAvg + " fps : " + fps);
				takeTime -= 1f;
				frame = (int)(fps * takeTime);
			}
		}
	}

	void OnGUI()
	{
		if (Event.current.type != EventType.Repaint)
			return;
		float delta = Time.deltaTime;


#if true// rt : onPrev() ~ onEnd()
		GL.Clear(true, true, Color.black);
		setProject();
		setRGBA(1, 1, 1, 1);
		drawImage(texFbo, 0, 0, TOP | LEFT);
#endif

		// Camera.main.rect = default;// xxxxxxx  new Rect(0, 0, 1, 1);

		// 0. onPrev : c.targetTexture = rt;
		texBack = RenderTexture.active;
		RenderTexture.active = texFbo;
		//rtBack = Camera.main.rect;
		//Camera.main.rect = new Rect(0, 0, 1, 1);
		GUI.matrix = Matrix4x4.TRS(
			Vector3.zero, Quaternion.identity, new Vector3(1, 1, 1));


		// 1. OnRenderObject
		if ( bLoad==false )
        {
			load();
			MainCamera.methodMouse = key;
			MainCamera.methodKeyboard = keyboard;
			bLoad = true;
		}
		iStrTex.runSt();
		draw(delta);
		drawLoading(delta);

		//iFPS.draw(delta, new iPoint(10, 10));

		// 2. onEnd : c.targetTexture = backupRt;
		RenderTexture.active = texBack;
		//Camera.main.rect = rtBack;
		setProject();

#if false//true// rt : 0 ~ 2 : drawGui
		setRGBA(1, 1, 1, 1);
		drawImage(texFbo, 0, 0, TOP | LEFT);
#elif true
		int w = Screen.width,
			h = Screen.height;
		GUI.matrix = Matrix4x4.TRS(
			// translation
			new Vector3(Camera.main.rect.x * w,
						Camera.main.rect.y * h, 0),
			// rotation
			Quaternion.identity,
			// scaling matrix
			new Vector3(Camera.main.rect.width * w / MainCamera.devWidth,
						Camera.main.rect.height * h / MainCamera.devHeight, 1)
			);
		setRGBA(1, 1, 1, 1);
		drawImage(texFbo, 0, 0, TOP | LEFT);
#elif true
		GL.Clear(true, true, Color.red);
		GUI.matrix = Matrix4x4.TRS(
			new Vector3(100*Mathf.Sin(test*180*Mathf.Deg2Rad), 0, 0),
			Quaternion.identity, new Vector3(1, 1, 1));
		//int w = Display.main.systemWidth,// Screen.width,
		//	h = Display.main.systemHeight;// Screen.height;
		int w = Screen.width,
			h = Screen.height;
		drawImage(texFbo, Camera.main.rect.x * w, Camera.main.rect.y * h,
							Camera.main.rect.width * w     / texFbo.width,
							Camera.main.rect.height * h     / texFbo.height,
							TOP | LEFT, 2, 0, REVERSE_NONE);

		test += delta;
#else
		setRGBA(1, 1, 1, 1);

		test += delta;
		while (test > 3)
			test -= 3;

		float t = test * 0.333333f;	//3초동안 0 ~ 1
		t = 1 - Mathf.Abs((((t - Mathf.Floor(t)) - 0.5f) * 2)); //3초동안 0 ~ 1, 3초동안 1 ~ 0
		
		//particle
		//setBlendFunc(1);

		//blur
		//setBlendFunc(2);
		//setBlur(t * 10);

		//moz
		//setBlendFunc(3);
		//setMoz(t * 20);

		drawImage(texFbo, 0, 0, TOP | LEFT);
		setBlendFunc(0);
#endif
	}
	float test = 0f;

#if false
	AAA: 11111111111111111111111111111111111111111111111111111111111111111111111111111111111
	BBB: 333333333333
	AAA * BBB
	AAA / BBB
//# include <stdio.h>
	char* dhagi(const char* a, const char* b);
	char* bbaegi(const char* a, const char* b);
	char* gob(const char* a, const char* b);
	char* nanugi(const char* a, const char* b);
#endif

    public virtual void load()
	{
	}

	public virtual void draw(float dt)
	{
	}

	public virtual void key(iKeystate stat, iPoint point) { }
	public virtual void keyboard(iKeystate stat, int key) { }

	private static float _loadDt = 1f;
	float loadDt = 1f;
	bool loadFadeIn = true;
	string nameScene;
	public bool fadeInEnd = false;

	public bool setLoading(string nameScene_)
    {
		if (loadDt > 0)
			return false;

		loadFadeIn = false;
		loadDt = 0.000001f;
		nameScene = nameScene_;
		return true;
	}

	void drawLoading(float dt)
    {
		if (loadDt == 0f)
			return;

		setRGBA(0, 0, 0, loadDt / _loadDt);
		fillRect(0, 0, MainCamera.devWidth, MainCamera.devHeight);

		if(loadFadeIn)
        {
			loadDt -= dt;
			if (loadDt < 0f)
			{
				loadDt = 0f;
				fadeInEnd = true;
			}
			
        }
		else
        {
			loadDt += dt;
			if (loadDt > _loadDt)
            {
				loadDt = _loadDt;
				MainCamera.setGameScene(nameScene);
				GameObject.Destroy(gameObject);
            }
		}
    }

}

