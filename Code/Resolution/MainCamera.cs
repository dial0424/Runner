public class MainCamera : MonoBehaviour
{
	// 개발 해상도(1280p, 720p)
	public static int devWidth = 1280, devHeight = 720;// 640~??
    public static float resolutionRate;

	//관련없는 코드생략

	void Start()
	{
		// minHeight, maxHeight 제한
		devHeight = (int)((float)devWidth * Display.main.systemHeight / Display.main.systemWidth + 0.5f);
		if (devHeight < 540)// 2 : 1
			devHeight = 540;
		else if (devHeight > 960)// 4 : 3
			devHeight = 960;
		// devWidth : h = 4 ; 3
		resolutionRate = (float)devHeight / 720;

		//관련없는 코드생략
	}

	void Update()
	{
		iGUI.setResolution(devWidth, devHeight);

		//관련없는 코드생략
	}
}