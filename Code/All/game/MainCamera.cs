using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using STD;

public class MainCamera : MonoBehaviour
{
	// 개발 해상도(1280p, 720p)
	// (ipad) 1024 x 768
	// (iphone) 2.1:1
	// (android) 12:9, 2:1
	// (pc) 16:9
	public static int devWidth = 1280, devHeight = 720;// 640~??
	public static float resolutionRate;

	// 16:9 개발
	// 액자처리

	// 최적화 16:9 대응 4:3, 아이폰 2.1:1
	// 4:3 ~ 2.1:1

	public static MethodMouse methodMouse = null;
	bool drag;
	Vector3 prevV;

	public static MethodWheel methodWheel = null;

	public static MethodKeyboard methodKeyboard = null;

	public static SoundMgt soundManager;

	void Start()
    {
		// minHeight, maxHeight 제한....
		devHeight = (int)((float)devWidth * Display.main.systemHeight / Display.main.systemWidth + 0.5f);
		if (devHeight < 540)// 2 : 1
			devHeight = 540;
		else if (devHeight > 960)// 4 : 3
			devHeight = 960;
		// devWidth : h = 4 ; 3
		resolutionRate = (float)devHeight / 720;

		drag = false;

		loadGameHierachy();

		soundManager = new SoundMgt();
		GameObject go = new GameObject("Audio");

		for(int i=0;i<3;i++)
			soundManager.audioSources[i] = go.AddComponent<AudioSource>();


		Application.targetFrameRate = 60;
		setGameScene("Loby");
	}
	public static GameObject currScene;

	public static GameObject creatGameObject(string name)
	{
		GameObject go = new GameObject(name);
		var t = System.Type.GetType(name);
		Object ob = go.AddComponent(t);

		return go;
	}

	public static void setGameScene(string nameScene)
    {
		currScene = creatGameObject(nameScene);
		soundManager.playBgSound(nameScene);
	}


	void Update()
	{
		iGUI.setResolution(devWidth, devHeight);

		// ctrl
		updateMouse();
		updatewheel();
		updateKeyboard();

		// view
		drawGameHierachy();
	}

	void updateMouse()
    {
		if (methodMouse == null)
			return;

		int btn = 0;// 0:left, 1:right, 2:wheel, 3foward, 4back
		if (Input.GetMouseButtonDown(btn))
		{
			iPoint p = mousePosition();
			//Debug.LogFormat($"Began p({p.x},{p.y})");
			drag = true;
			prevV = Input.mousePosition;// 누르자 말자 Moved 안들어오게 방지

			if (methodMouse != null)
				methodMouse(iKeystate.Began, p);
		}
		else if (Input.GetMouseButtonUp(btn))
		{
			iPoint p = mousePosition();
			//Debug.LogFormat($"Ended p({p.x},{p.y})");
			drag = false;

			if (methodMouse != null)
				methodMouse(iKeystate.Ended, p);
		}

		//if (drag)
		{
			Vector3 v = Input.mousePosition;
			if (prevV == v)
				return;
			prevV = v;

			iPoint p = mousePosition();
			//Debug.LogFormat($"Moved p({p.x},{p.y})");

			if (methodMouse != null)
				methodMouse(iKeystate.Moved, p);
		}
	}

	void updatewheel()
    {
		if (methodWheel == null)
			return;

		if (Input.mouseScrollDelta != Vector2.zero)
		{
			methodWheel(new iPoint(Input.mouseScrollDelta.x,
									Input.mouseScrollDelta.y));
		}

	}

	public static KeyCode[] kc = new KeyCode[]{
		KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,
		KeyCode.Space
	};
	void updateKeyboard()
    {
		if (methodKeyboard == null)
			return;

		int keyDown = 0, keyUp = 0;
		for(int i=0; i<kc.Length; i++)
        {
			if( Input.GetKeyDown(kc[i]))
            {
				keyDown |= (int)(Mathf.Pow(2, i));
				keyState |= (int)(Mathf.Pow(2, i));
			}
			else if (Input.GetKeyUp(kc[i]))
			{
				keyUp |= (int)(Mathf.Pow(2, i));
				keyState &= ~(int)(Mathf.Pow(2, i));
			}
		}

		if( keyDown!=0 )
			methodKeyboard(iKeystate.Began, keyDown);
		methodKeyboard(iKeystate.Moved, keyState);
		if (keyUp != 0)
			methodKeyboard(iKeystate.Ended, keyUp);
	}
	int keyState = 0;

	public static bool checkKeyState(int key, KeyCode k)
    {
		int i = 0;
		for(; i<kc.Length; i++)
        {
			if (kc[i] == k)
				break;
        }
		int n = (int)Mathf.Pow(2, i);

		return (int)(key & n) == n;
    }

	public static Vector3 iPointToVector3(iPoint p)
	{
		return new Vector3(p.x - devWidth / 2, devHeight / 2 - p.y, 0f);
	}

	public static iPoint vector3ToiPoint(Vector3 v)
	{
		return new iPoint(v.x + devWidth / 2, devHeight / 2 - v.y);
	}

	public static iPoint mousePosition()
	{
		int sw = Screen.width, sh = Screen.height;
		float vx = Camera.main.rect.x * sw;
		float vy = Camera.main.rect.y * sh;
		float vw = Camera.main.rect.width * sw;
		float vh = Camera.main.rect.height * sh;

		Vector3 v = Input.mousePosition;
		iPoint p = new iPoint(	(v.x - vx) / vw * devWidth,
								(1f - (v.y - vy) / vh) * devHeight);
		//Debug.LogFormat($"screen({sw},{sh}) : input({v.x},{v.y}) => use({p.x},{p.y})");
		return p;
	}

	// ===========================================================
	// Game
	// ===========================================================
	public static MainCamera mc;


	//하이라키에서 그리는것(unity 내부적으로도)
	void loadGameHierachy()
	{
		mc = this;


	}
	void drawGameHierachy()
	{
	}

	void keyGameHierachy()
    {

    }
	// ===========================================================
	// 소프트키 숨기기
	// ===========================================================
	void OnApplicationFocus(bool focusStatus)

	{

		if (focusStatus)

		{

			DisableSystemUI.DisableNavUI();

		}

	}
	public class DisableSystemUI

	{

#if UNITY_ANDROID

		static AndroidJavaObject activityInstance;

		static AndroidJavaObject windowInstance;

		static AndroidJavaObject viewInstance;



		const int SYSTEM_UI_FLAG_HIDE_NAVIGATION = 2;

		const int SYSTEM_UI_FLAG_LAYOUT_STABLE = 256;

		const int SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION = 512;

		const int SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN = 1024;

		const int SYSTEM_UI_FLAG_IMMERSIVE = 2048;

		const int SYSTEM_UI_FLAG_IMMERSIVE_STICKY = 4096;

		const int SYSTEM_UI_FLAG_FULLSCREEN = 4;



		public delegate void RunPtr();



		public static void Run()

		{

			if (viewInstance != null)
			{

				viewInstance.Call("setSystemUiVisibility",

								  SYSTEM_UI_FLAG_LAYOUT_STABLE

								  | SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION

								  | SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN

								  | SYSTEM_UI_FLAG_HIDE_NAVIGATION

								  | SYSTEM_UI_FLAG_FULLSCREEN

								  | SYSTEM_UI_FLAG_IMMERSIVE_STICKY);

			}
		}

#endif
		public static void DisableNavUI()
		{
			if (Application.platform != RuntimePlatform.Android)

				return;

#if UNITY_ANDROID

			using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))

			{

				activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

				windowInstance = activityInstance.Call<AndroidJavaObject>("getWindow");

				viewInstance = windowInstance.Call<AndroidJavaObject>("getDecorView");



				AndroidJavaRunnable RunThis;

				RunThis = new AndroidJavaRunnable(new RunPtr(Run));

				activityInstance.Call("runOnUiThread", RunThis);

			}

#endif

		}



	}
#if false
		if (stPrint == null)
		{
			stPrint = new iStrTex[4];
			for(int i=0; i<4; i++)
			{
				stPrint[i] = new iStrTex();
				stPrint[i].setStringSize(30);
				stPrint[i].setStringRGBA(1, 0, 0, 1);
			}
		}
		stPrint[0].drawString(Screen.width + "," + Screen.height, 0, 0, TOP | LEFT);
		stPrint[1].drawString(Screen.currentResolution.width + "," + Screen.currentResolution.height, 0, 40, TOP | LEFT);
		stPrint[2].drawString(Display.main.systemWidth + "," + Display.main.systemHeight, 0, 80, TOP | LEFT);// ==Screen.currentResolution.width/height  != Screen.width/heiht
		stPrint[3].drawString($"{Camera.main.rect.x}, {Camera.main.rect.y}, {Camera.main.rect.width}, {Camera.main.rect.height}", 0, 120, TOP | LEFT);

		/*
		 <<Editor>>
		 1091, 585
		 1920, 1200
		 1091, 585
		 0.023, 0, 0.95, 1 => well

		 <<Application>>
		 1280, 720
		 1920, 1200
		 1920, 1200
		 0, 0.05, 1, 0.9 => 상단 까맣게

		Display.main.systemWidth/systemHeight 변경하지 않았을때, Po

		 <<android : auto ratation>>
		 potrait 
		 x display

		 landscape
		 1280, 720 // screen
		 1280, 720 // screen.currentResolution
		 2640, 1080 // Display
		 0.136, 0, 0.727, 1

		 <<android : landscape left>>
		 <<android : landscape right>>
		 
		 */
#endif
}