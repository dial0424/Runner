public class Loby : nGUI	//nGUI는 iGUI 상속받음
{
	RenderTexture rt;
	public override void load()
	{
		//shader와 관련없는 코드 생략
		//다른 요소들 load

		loadPopOpt();
	}
	
	public override void draw(float dt)
	{
		//shader와 관련없는 코드 생략
		//다른 요소들 draw

		drawPopOpt(dt);
	}

	void loadPopOpt()
	{
		//shader와 관련없는 코드 생략
		//pop이 그려지기 전에 실행될 함수 델리게이트 등록

		pop.methodPaintBefore = paintPopOptBefore;
	}
	

	bool drawPopOpt(float dt)
	{
		popOpt.paint(dt);

		return popOpt.bShow;
	}

	void paintPopOptBefore(float dt, iPopup pop)
	{
		drawPopBlur(pop);
	}
	
	void drawPopBlur(iPopup pop)
	{
		setBlendFunc(2);	//iGUI 함수
		float blur = 5;
		if (pop.state == iPopupState.open)
			blur = 5 * pop.aniDt / pop._aniDt;
		else if (pop.state == iPopupState.close)
			blur = 5 * (1 - pop.aniDt / pop._aniDt);
		setBlur(blur);	//iGUI 함수
		drawImage(rt, 0, 0, TOP | LEFT);	//iGUI 함수, 
		setBlendFunc(0);
	}
}