public class Proc : nGUI
{
	public override void draw(float dt)
	{
		// dt에는 Time.deltaTime이 들어옴
		float _dt = dt;
		if (pauseGame())
			dt = 0f;

		drawMap(dt);

		hero.paint(dt);
		drawItemEffect(dt);

		drawPopState(dt);
		drawPopHeroCtrlBtn(dt);
		drawPopFever(dt);

		drawPopPause(_dt);  //일시정지 상황에도 그려져야함
		drawPopCount(_dt);
	}

	bool pauseGame()
	{
		if (popPause.bShow || popCount.bShow) //일시정지 팝업, 카운트 팝업 상태
			return true;
		return false;
	}
}