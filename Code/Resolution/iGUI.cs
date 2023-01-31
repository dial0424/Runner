public class iGUI : MonoBehaviour
{ 
	public static void setResolution(int devWidth, int devHeight)
	{
		// Screen.width, Screen.height; HWND(Client Rect != Windows Rect) 사이즈
		// Screen.currentResolution.width, Screen.currentResolution.height;(Monitor Size)
		// Display.main.systemWidth, Display.main.systemHeight(Activity Portrait, Landscape)
	
		setResolutionClip(devWidth, devHeight);
	}

	public static void setResolutionClip(int devWidth, int devHeight)
	{
		Screen.SetResolution(devWidth, devHeight, false);
		float r0 = (float)devWidth / devHeight;

		int width = Display.main.systemWidth,// Screen.width;//
			height = Display.main.systemHeight;// Screen.height;//
		float r1 = (float)width / height;

		if (r0 < r1)// 세로가 길때
		{
			float w = r0 / r1;
			float x = (1 - w) / 2;
			Camera.main.rect = new Rect(x, 0, w, 1);
		}
		else// 가로가 길때
		{
			float h = r1 / r0;
			float y = (1 - h) / 2;
			Camera.main.rect = new Rect(0, y, 1, h);
		}
	}
}