public class Loby : nGUI    //nGUI는 iGUI 상속받음
{
	RenderTexture rt;
	public override void load()
	{
		rt = new RenderTexture(MainCamera.devWidth, MainCamera.devHeight, 32, RenderTextureFormat.ARGB32);

		//shader와 관련없는 코드 생략
		//다른 요소들 load

		loadPopOpt();
	}

	public override void draw(float dt)
	{
		RenderTexture backUp = RenderTexture.active;
		RenderTexture.active = rt;
		GL.Clear(true, true, Color.clear);

		//shader와 관련없는 코드 생략
		//다른 요소들 draw

		RenderTexture.active = backUp;

		drawImage(rt, 0, 0, TOP | LEFT);

		drawPopOpt(dt);
	}

	void paintPopOptBefore(float dt, iPopup pop)
	{
		drawPopBlur(pop);
	}

	void drawPopBlur(iPopup pop)
	{
		setBlendFunc(2);    //iGUI 함수
							//블러처리 후
		float blur = 5;
		if (pop.state == iPopupState.open)
			blur = 5 * pop.aniDt / pop._aniDt;
		else if (pop.state == iPopupState.close)
			blur = 5 * (1 - pop.aniDt / pop._aniDt);
		setBlur(blur);  //iGUI 함수
		drawImage(rt, 0, 0, TOP | LEFT);    //iGUI 함수
											//팝업 이전 이미지 그림

		setBlendFunc(0);	//일반 섀이더로 바꿈
							//백업 활용
	}
}