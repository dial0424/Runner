using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;

namespace STD
{
	// Android : unique library(Activity, Manifest.xml)			<<java (graphics + paint)>>
	// Window : DX or OpenGL Vulkan Low Level API Hardware		<<Win32 API(rgb) + GDI+ (grphaics paint)>>
	// iOS, Mac : OS X(OpenGL), Latest(Mojave or loader Metal)	<<Cocoa (NSWindow, UIView)>>
	// 유니티													<<OnGUI>>

	public class iGUI : MonoBehaviour
	{
		public static iGUI instance = null;
		public iGUI()
		{
			instance = this;
		}

		// geometry
		Texture2D texDot;
		Color color;
		float lineWidth;

		// image
		public const int TOP = 1, BOTTOM = 2, VCENTER = 4,
						LEFT = 8, RIGHT = 16, HCENTER = 32;
		public const int REVERSE_NONE = 0, REVERSE_WIDTH = 1, REVERSE_HEIGHT = 2;

		// string
		string stringName = null;
		float stringSize = 20f;
		Color stringColor = Color.white;

		// ======================================================
		// init & buffer
		// ======================================================
		public void init()
		{
			Texture2D tex = new Texture2D(1, 1);
			tex.SetPixel(0, 0, new Color(1, 1, 1));
			tex.Apply();
			texDot = tex;
			lineWidth = 1f;
			color = Color.white;

			setBlendFunc(0);
		}

		public void preCull()
		{
			GL.Clear(true, true, Color.clear);
		}

		//
		// matrix
		//
		public void setProject()
		{
			int w = Display.main.systemWidth,// Screen.width,
				h = Display.main.systemHeight;// Screen.height;
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
		}

		//
		// viewport + matrix(projection)
		// 
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

			int width = Display.main.systemWidth,// Screen.width,//
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
		public static void setResolutionFull(int devWidth, int devHeight)
		{
			Camera.main.rect = new Rect(0, 0, 1, 1);

			int width = Screen.width, height = Screen.height;
			// width : height = 4 : 3 = devWidth : h
			// h = height / width x devWidth
			float dh = (float)height / width * devWidth;
			Screen.SetResolution(devWidth, (int)dh, true);
			//Debug.LogFormat($"devResolution({devWidth}, {dh})");
		}
		public static void setResolutionFix(int devWidth, int devHeight)
		{
			// 지원하는 비율 최대치, 최소치 못 넘게
#if false
		// 풀 스크린 : 가로 고정, 세로 민, 맥스 한계치
		int width = Screen.width, height = Screen.height;
		// width : height = 4 : 3 = devWidth : h
		devHeight = (int)((float)height / width * devWidth);
		if (devHeight < 640)// 2 : 1
			devHeight = 640;
		else if (devHeight > 960)
			devHeight = 960;// 4 : 3
#endif
			setResolutionClip(devWidth, devHeight);
		}
		// ======================================================
		// geometry
		// ======================================================
#if false// #issue clip width, height (실제 화면의 절반 최대)
		public void setClip(float x, float y, float w, float h)
		{
			if (x != 0 || y != 0 || w != 0 || h != 0)
			{
				x -= MainCamera.devWidth / 2;
				y -= MainCamera.devHeight / 2;
				GUI.BeginClip(new Rect(x, y, w, h));
			}
			else
				GUI.EndClip();
		}
#endif

		public Color getRGBA() { return color; }
		public void setRGBA(float r, float g, float b, float a)
		{
			color.r = r;
			color.g = g;
			color.b = b;
			color.a = a;
		}

		public void setLineWidth(float width)
		{
			lineWidth = width;
		}

		public void drawLine(iPoint s, iPoint e)
		{
			drawLine(s.x, s.y, e.x, e.y);
		}
		public void drawLine(float sx, float sy, float ex, float ey)
		{
			float cx = (sx + ex) / 2;
			float cy = (sy + ey) / 2;
			float dx = ex - sx;
			float dy = ey - sy;
			float len = Mathf.Sqrt(dx * dx + dy * dy);

			float degree = Math.angleDirection(sx, sy, ex, ey);
			drawImage(texDot, cx, cy, len, lineWidth, VCENTER | HCENTER, 2, -degree, REVERSE_NONE);
		}

		public void drawRect(float x, float y, float width, float height)
		{
			// top & bottom
			drawLine(x, y, x + width, y);
			drawLine(x, y + height - lineWidth, x + width, y + height - lineWidth);
			// left & right
			drawLine(x, y + lineWidth,
						x, y + height - lineWidth);
			drawLine(x + width - lineWidth, y + lineWidth,
						x + width - lineWidth, y + height - lineWidth);
		}

		public void fillRect(float x, float y, float width, float height)
		{
			drawImage(texDot, x, y, width, height, TOP | LEFT, 2, 0, REVERSE_NONE);
		}
		public void fillRect(iRect rt)
		{
			drawImage(texDot, rt.origin.x, rt.origin.y, rt.size.width, rt.size.height, TOP | LEFT, 2, 0, REVERSE_NONE);
		}

		// ======================================================
		// image
		// ======================================================
		public Texture createImage(string path)
		{
			Texture tex = Resources.Load<Texture>(path);
			return tex;
		}

		public void drawImage(Texture tex, iPoint p, int anc)
		{
			drawImage(tex, p.x, p.y, 1f, 1f, anc, 2, 0, REVERSE_NONE);
		}

		public void drawImage(Texture tex, float x, float y, int anc)
		{
			drawImage(tex, x, y, 1f, 1f, anc, 2, 0, REVERSE_NONE);
		}

		public void drawImage(Texture tex, iPoint p, float sx, float sy, int anc)
		{
			drawImage(tex, p.x, p.y, sx, sy, anc, 2, 0, REVERSE_NONE);
		}

		public void drawImage(Texture tex, iPoint p, float sx, float sy, int anc, int xyz, float degree, int reverse)
		{
			drawImage(tex, p.x, p.y, sx, sy, anc, xyz, degree, reverse);
		}

		public void drawImage(Texture tex, float x, float y, float sx, float sy, int anc, int xyz, float degree, int reverse, 
			float tx = 0f, float ty = 0f, float tw = 1f, float th = 1f)
		{
			float w = tex.width * sx;
			float h = tex.height * sy;

			switch (anc)
			{
				case TOP | LEFT:								break;
				case TOP | RIGHT:		x -= w;					break;
				case TOP | HCENTER:		x -= w / 2;				break;
				case BOTTOM | LEFT:					y -= h;		break;
				case BOTTOM | RIGHT:	x -= w;		y -= h;		break;
				case BOTTOM | HCENTER:	x -= w / 2; y -= h;		break;
				case VCENTER | LEFT:				y -= h / 2; break;
				case VCENTER | RIGHT:	x -= w;		y -= h / 2;	break;
				case VCENTER | HCENTER: x -= w / 2; y -= h / 2; break;
			}

			Matrix4x4 matrixPrjection = GUI.matrix;// matrixProjection(개발좌표에서 화면좌표)

			Matrix4x4 matrixModelview = Matrix4x4.TRS(
				new Vector3(x + w / 2, y + h / 2, 0),
				Quaternion.Euler(	xyz == 0 ? degree : 0,
									xyz == 1 ? degree : 0,
									xyz == 2 ? degree : 0),
				new Vector3(1, 1, 1)
				);
			GUI.matrix = GUI.matrix * matrixModelview;
			if (reverse == REVERSE_WIDTH)
			{
				Matrix4x4 m0 = Matrix4x4.TRS(
					new Vector3(0, 0, 0),
					Quaternion.Euler(0, 180, 0),
					new Vector3(1, 1, 1)
					);
				GUI.matrix = GUI.matrix * m0;
			}
			else if (reverse == REVERSE_HEIGHT)
			{
				Matrix4x4 m1 = Matrix4x4.TRS(
					new Vector3(0, 0, 0),
					Quaternion.Euler(180, 0, 0),
					new Vector3(1, 1, 1)
					);
				GUI.matrix = GUI.matrix * m1;
			}

#if false
			GUI.DrawTexture(new Rect(-w / 2, -h / 2, w, h), tex);
//#elif true
#elif false
			GUI.DrawTexture(new Rect(-w / 2, -h / 2, w, h), tex, ScaleMode.StretchToFill, true, h / w, color, 0, 0);
#elif false// prev
			GUI.color = color;// #issue
			GUI.DrawTextureWithTexCoords(new Rect(-w / 2, -h / 2, w, h), tex, new Rect(tx, ty, tw, th), true);
#elif true// curr
			Material m = mat[matIndex];
			m.SetTexture("_MainTex", tex);
			m.SetColor("color", color);
			m.SetFloat("blur", blur);
			m.SetFloat("moz", moz);
			Graphics.DrawTexture(new Rect(-w / 2, -h / 2, w, h), 
								 tex, new Rect(tx, ty, tw, th), 0, 0, 0, 0, m);
#else
			ScaleMode scaleMode = ScaleMode.ScaleToFit;
			float imageAspect = h / w;
			float borderWidth = 0, borderRadius = 0;
			GUI.DrawTexture(new Rect(-w / 2, -h / 2, w, h), tex, scaleMode, true, imageAspect, color, borderWidth, borderRadius);
#endif
			GUI.matrix = matrixPrjection;
		}

		Material[] mat = null;
		int matIndex = 0;
		public void setBlendFunc(int index)
		{
			if( mat==null )
			{
				string[] name = new string[] { "std", "add", "blur", "moz" };

				mat = new Material[name.Length];

				for (int i = 0; i < name.Length; i++)
				{
					Shader shader = Shader.Find("Unlit/" + name[i]);
					mat[i] = new Material(shader);
				}
			}

			matIndex = index;
		}

		float blur = 0;
		public void setBlur(float b)
		{
			blur = b;
		}

		float moz = 1;
		public void setMoz(float m)
		{
			if (m < 1)
				m = 1;
			moz = m;
		}
		// ======================================================
		// string
		// ======================================================
		public string getStringName()
		{
			return stringName;
		}
		public void setStringName(string name)
		{
			stringName = name;
		}

		public float getStringSize()
		{
			return stringSize;
		}
		public void setStringSize(float size)
		{
			stringSize = size;
		}

		public Color getStringRGBA()
		{
			return stringColor;
		}
		public void setStringRGBA(float r, float g, float b, float a)
		{
			stringColor = new Color(r, g, b, a);
		}

		public void drawString(string str, iPoint p, int anc = TOP | LEFT)
		{
			drawString(str, p.x, p.y, anc);
		}

		public iSize sizeOfString(string str)
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.font = Resources.Load<Font>(stringName);
			style.fontSize = (int)stringSize;
			style.fontStyle = FontStyle.Normal;
			style.normal.textColor = stringColor;
			Vector2 size = style.CalcSize(new GUIContent(str));
			return new iSize(size.x, size.y);
		}

		public void drawString(string str, float x, float y, int anc = TOP | LEFT)
		{
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.font = Resources.Load<Font>(stringName);
			style.fontSize = (int)stringSize;
			style.fontStyle = FontStyle.Normal;
			style.normal.textColor = stringColor;
			style.hover.textColor = stringColor;
			Vector2 size = style.CalcSize(new GUIContent(str));
			switch (anc)
			{
				case TOP | LEFT:										break;
				case TOP | RIGHT:		x -= size.x;					break;
				case TOP | HCENTER:		x -= size.x / 2;				break;
				case BOTTOM | LEFT:							y -= size.y; break;
				case BOTTOM | RIGHT:	x -= size.x;		y -= size.y; break;
				case BOTTOM | HCENTER:	x -= size.x / 2;	y -= size.y; break;
				case VCENTER | LEFT:						y -= size.y / 2; break;
				case VCENTER | RIGHT:	x -= size.x;		y -= size.y / 2; break;
				case VCENTER | HCENTER: x -= size.x / 2;	y -= size.y / 2; break;
			}
			GUI.color = color;// #issue
			GUI.Label(new Rect(x, y, size.x, size.y), str, style);
		}
	}

#if false
	void btn()
	{
		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.font = Resources.Load<Font>("Freshman");
		style.fontSize = 20;
		style.normal.textColor = Color.red;
		style.hover.textColor = new Color(1, 0, 1);
		if (GUI.Button(new Rect(0, 160, 150, 50), "Btn Test", style))
		{
			Debug.Log("GUI Button ====================");
		}
	}
#endif
}
