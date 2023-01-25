public class Proc : nGUI
{
	public override void draw(float dt)
	{
		// dt���� Time.deltaTime�� ����
		float _dt = dt;
		if (pauseGame())
			dt = 0f;

		drawMap(dt);

		hero.paint(dt);
		drawItemEffect(dt);

		drawPopState(dt);
		drawPopHeroCtrlBtn(dt);
		drawPopFever(dt);

		drawPopPause(_dt);  //�Ͻ����� ��Ȳ���� �׷�������
		drawPopCount(_dt);
	}

	bool pauseGame()
	{
		if (popPause.bShow || popCount.bShow) //�Ͻ����� �˾�, ī��Ʈ �˾� ����
			return true;
		return false;
	}
}