public class Loby : nGUI    //nGUI�� iGUI ��ӹ���
{
	RenderTexture rt;
	public override void load()
	{
		rt = new RenderTexture(MainCamera.devWidth, MainCamera.devHeight, 32, RenderTextureFormat.ARGB32);

		//shader�� ���þ��� �ڵ� ����
		//�ٸ� ��ҵ� load

		loadPopOpt();
	}

	public override void draw(float dt)
	{
		RenderTexture backUp = RenderTexture.active;
		RenderTexture.active = rt;
		GL.Clear(true, true, Color.clear);

		//shader�� ���þ��� �ڵ� ����
		//�ٸ� ��ҵ� draw

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
		setBlendFunc(2);    //iGUI �Լ�
							//��ó�� ��
		float blur = 5;
		if (pop.state == iPopupState.open)
			blur = 5 * pop.aniDt / pop._aniDt;
		else if (pop.state == iPopupState.close)
			blur = 5 * (1 - pop.aniDt / pop._aniDt);
		setBlur(blur);  //iGUI �Լ�
		drawImage(rt, 0, 0, TOP | LEFT);    //iGUI �Լ�
											//�˾� ���� �̹��� �׸�

		setBlendFunc(0);	//�Ϲ� ���̴��� �ٲ�
							//��� Ȱ��
	}
}