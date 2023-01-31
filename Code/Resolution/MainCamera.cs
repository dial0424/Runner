public class MainCamera : MonoBehaviour
{
	// ���� �ػ�(1280p, 720p)
	public static int devWidth = 1280, devHeight = 720;// 640~??
    public static float resolutionRate;

	//���þ��� �ڵ����

	void Start()
	{
		// minHeight, maxHeight ����
		devHeight = (int)((float)devWidth * Display.main.systemHeight / Display.main.systemWidth + 0.5f);
		if (devHeight < 540)// 2 : 1
			devHeight = 540;
		else if (devHeight > 960)// 4 : 3
			devHeight = 960;
		// devWidth : h = 4 ; 3
		resolutionRate = (float)devHeight / 720;

		//���þ��� �ڵ����
	}

	void Update()
	{
		iGUI.setResolution(devWidth, devHeight);

		//���þ��� �ڵ����
	}
}