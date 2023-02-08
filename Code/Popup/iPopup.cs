public delegate void MethodPopOpenClose(iPopup pop);

public delegate void MethodPopDraw(float dt, iPopup pop, iPoint zero);
public delegate void MethodPopPaint(float dt, iPopup pop);

public enum iPopupStyle
{
	alpha = 0,
	move,
	zoom,
	zoomRotate,
}

public enum iPopupState
{
	open = 0,
	proc,
	close
}

public class iPopup
{
	static RenderTexture renderTexture = null;

	public List<iImage> listImg;

	public iPopupStyle style;
	public iPoint openPoint, closePoint;
	public iPopupState state;
	public float aniDt, _aniDt;
	public bool bShow;
	public MethodPopOpenClose methodOpen, methodClose;
	public MethodPopDraw methodDrawBefore, methodDrawAfter;
	public MethodPopPaint methodPaintBefore, methodPaintAfter;
	public int selected;
	public bool zoomReverse;
	public float popScale;


	public iPopup()
	{
		if (renderTexture == null)
		{
			renderTexture = new RenderTexture(
				MainCamera.devWidth, MainCamera.devHeight, 32,
				RenderTextureFormat.ARGB32);
		}

		listImg = new List<iImage>();

		style = iPopupStyle.alpha;
		state = iPopupState.close;
		openPoint = new iPoint(0, 0);
		closePoint = new iPoint(0, 0);
		_aniDt = 0.5f;
		aniDt = 0.0f;
		bShow = false;
		methodOpen = null;
		methodClose = null;
		methodDrawBefore = null;
		methodDrawAfter = null;
		methodPaintBefore = null;
		methodPaintAfter = null;
		selected = -1;
		zoomReverse = false;
		popScale = 1f;
	}

	public void add(iImage i)
	{
		listImg.Add(i);
	}

	public void show(bool show)
	{
		if (show)
		{
			bShow = true;
			state = iPopupState.open;
		}
		else
		{
			state = iPopupState.close;
		}
		aniDt = 0.0f;// _aniDt = 0.5f default
	}

	public void paint(float dt, float a = 1f)
	{
		if (bShow == false)
			return;

		float alpha = a;
		iPoint position = new iPoint(0, 0);
		float scale = 1.0f;
		float degree = 0.0f;

		if (style == iPopupStyle.alpha)
		{
			if (state == iPopupState.open)	//열리는 중
			{
				aniDt += dt;
				if (aniDt >= _aniDt)
				{
					aniDt = _aniDt;
					state = iPopupState.proc;
					if (methodOpen != null)
						methodOpen(this);
				}
				alpha = aniDt / _aniDt;
				position = openPoint * (1 - alpha) + closePoint * alpha;
			}
			else if (state == iPopupState.proc)
			{
				alpha = a;
				position = closePoint;
			}
			else if (state == iPopupState.close)	//닫히는 중
			{
				aniDt += dt;
				if (aniDt >= _aniDt)
				{
					aniDt = _aniDt;
					bShow = false;
					if (methodClose != null)
					{
						methodClose(this);
						return;
					}
				}

				alpha = 1f - aniDt / _aniDt;// alpha 1 -> 0
				position = openPoint * (1 - alpha) + closePoint * alpha;
			}
		}
		else if (style == iPopupStyle.move)
		{
			if (state == iPopupState.open)
			{
				aniDt += dt;
				if (aniDt >= _aniDt)
				{
					aniDt = _aniDt;
					state = iPopupState.proc;
					if (methodOpen != null)
						methodOpen(this);
				}
				float r = Math.easeOut(aniDt / _aniDt, 0, 1);
				position = openPoint * (1 - r) + closePoint * r;
			}
			else if (state == iPopupState.proc)
			{
				position = closePoint;
			}
			else if (state == iPopupState.close)
			{
				aniDt += dt;
				if (aniDt >= _aniDt)
				{
					aniDt = _aniDt;
					bShow = false;
					if (methodClose != null)
					{
						methodClose(this);
						return;
					}
				}

				float r = Math.easeIn(aniDt / _aniDt, 1, 0);// alpha 1 -> 0
				position = openPoint * (1 - r) + closePoint * r;
			}
		}
		else if (style == iPopupStyle.zoom)
		{
			if (state == iPopupState.open)
			{
				aniDt += dt;
				if (aniDt >= _aniDt)
				{
					aniDt = _aniDt;
					state = iPopupState.proc;
					if (methodOpen != null)
						methodOpen(this);
				}
				float r = aniDt / _aniDt;
				position = openPoint * (1 - r) + closePoint * r;
				if (!zoomReverse)
					scale = r;
				else scale = 2f - r;    // 2 ~ 1

				popScale = scale;
			}
			else if (state == iPopupState.proc)
			{
				alpha = a;
				position = closePoint;
				scale = 1f;
				popScale = scale;
			}
			else if (state == iPopupState.close)
			{
				aniDt += dt;
				if (aniDt >= _aniDt)
				{
					aniDt = _aniDt;
					bShow = false;
					if (methodClose != null)
					{
						methodClose(this);
						return;
					}
				}

				float r = 1f - aniDt / _aniDt;// alpha 1 -> 0
				position = openPoint * (1 - r) + closePoint * r;
				scale = r;
				popScale = scale;
			}
		}
		Color c = iGUI.instance.getStringRGBA();
		iGUI.instance.setRGBA(1, 1, 1, alpha);

		if (methodPaintBefore != null)
			methodPaintBefore(dt, this);
#if false
			for (int i = 0; i < listImg.Count; i++)
			{
				iImage img = listImg[i];
				img.paint(dt, position);
			}
#else
		RenderTexture bkT = RenderTexture.active;
		RenderTexture.active = renderTexture;

		Matrix4x4 matrixBk = GUI.matrix;
		GUI.matrix = Matrix4x4.TRS(
			Vector3.zero, Quaternion.identity, new Vector3(1, 1, 1));
#if !RenterTextureScale
		Matrix4x4 mv = Matrix4x4.TRS(
			Vector3.zero, Quaternion.identity,
			new Vector3(0.5f, 0.5f, 1));
		GUI.matrix *= mv;
#endif

		GL.Clear(true, true, Color.clear);

		float left = 1000f, right = 0f, top = 1000f, bottom = 0f;
		for (int i = 0; i < listImg.Count; i++)
		{
			iImage img = listImg[i];
			if (img.tex == null)
				break;

			if (left > img.position.x)
				left = img.position.x;
			if (right < img.position.x + img.tex.tex.width)
				right = img.position.x + img.tex.tex.width;
			if (top > img.position.y)
				top = img.position.y;
			if (bottom < img.position.y + img.tex.tex.height)
				bottom = img.position.y + img.tex.tex.height;
		}
		float w = (right - left);
		float h = (bottom - top);
		iPoint gCenter = new iPoint(left + w / 2, top + h / 2);
		iPoint mCenter = new iPoint(renderTexture.width / 2, renderTexture.height / 2);
		iPoint move = mCenter - gCenter;

		if (methodDrawBefore != null)
			methodDrawBefore(dt, this, move);
		for (int i = 0; i < listImg.Count; i++)
		{
			iImage img = listImg[i];
			img.paint(dt, move);
		}
		if (methodDrawAfter != null)
			methodDrawAfter(dt, this, move);

		RenderTexture.active = bkT;
		GUI.matrix = matrixBk;

		if (methodPaintAfter != null)
			methodPaintAfter(dt, this);

		iGUI.instance.setRGBA(1, 1, 1, alpha);
		iPoint p = position - move * scale;
		//degree = 0;
#if RenterTextureScale
			iGUI.instance.drawImage(renderTexture, p.x, p.y, scale, scale,
				iGUI.TOP | iGUI.LEFT, 2, degree, iGUI.REVERSE_NONE);
#else
		iGUI.instance.drawImage(renderTexture, p.x, p.y, scale * 2, scale * 2,
			iGUI.TOP | iGUI.LEFT, 2, degree, iGUI.REVERSE_NONE);
#endif

#endif
		iGUI.instance.setRGBA(c.r, c.g, c.b, c.a);
	}
}