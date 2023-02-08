public class Proc : nGUI
{
    public override void load()
    {
        loadState();
    }

    public override void draw(float dt)
    {
        drawPopState(dt);
    }

	public override void key(iKeystate stat, iPoint point)
	{
		if (keyPopPause(stat, point) ||
			keyPopCount(stat, point))
			return;
	}

	//팝업 중 하나
	//게임 중 최상단의 일시정지버튼, 목숨 ui, 점수 ui 코드
	// =============================================================
	// ui - show state/score
	// =============================================================
	iPopup popState;
	iImage[] imgStateBtn;

	iImage imgHpBar;

	iStrTex[] stScoreMoney;
	public int score, money;
	public float life, _life;
	float damage;
	public float delay;

	void loadState()
	{
		iPopup pop = new iPopup();

		//팝업이 가지는 이미지, 글씨 넣기
		// btn
		imgStateBtn = new iImage[1];

		iImage img = new iImage();
		iTexture tex = new iTexture(Resources.Load<Texture>("pause"));
		img.add(tex);
		img.position = new iPoint(10, 10);
		imgStateBtn[0] = img;
		pop.add(img);

		// hp
		img = new iImage();
		tex = new iTexture(Resources.Load<Texture>("item2"));
		img.add(tex);
		img.position = new iPoint(300, 5);
		pop.add(img);

		img = new iImage();
		tex = new iTexture(Resources.Load<Texture>("hpbar"));
		img.add(tex);
		img.position = new iPoint(370, 20);
		img.alpha = 0.5f;
		imgHpBar = img;
		pop.add(img);

		// cookie, money
		stScoreMoney = new iStrTex[2];
		for (int i = 0; i < 2; i++)
		{
			img = new iImage();
			tex = new iTexture(Resources.Load<Texture>("item" + i));
			img.add(tex);
			img.position = new iPoint(1000, 10 + 60 * i);
			pop.add(img);

			img = new iImage();
			iStrTex st = new iStrTex(methodStScoreMoney, 200, 50);
			st.setString("0");
			img.add(st.tex);
			img.position = new iPoint(1060, 20 + 60 * i);
			pop.add(img);
			stScoreMoney[i] = st;
		}

		//팝업의 스타일, 여닫는 위치, 델리게이트 함수 설정
		pop.style = iPopupStyle.move;
		pop.openPoint = new iPoint(0, -70);
		pop.closePoint = new iPoint(0, 0);
		pop.methodPaintAfter = paintPopStateAfter;
		popState = pop;

		score = 0;
		money = 0;
		life = _life = maps[0].tileX / 10 * 3; // tileX : life = 130 : 39  =  160 : 48
		damage = 0f;
		delay = 0f;
	}

	//점수표기
	void methodStScoreMoney(iStrTex st)
	{
		setStringSize(40f);
		setStringRGBA(1, 1, 1, 1);
		drawString(st.str, new iPoint(200, 0), TOP | RIGHT);
	}


	void paintPopStateAfter(float dt, iPopup pop)
	{
		//점수 설정
		stScoreMoney[0].setString("" + score);
		stScoreMoney[1].setString("" + money);

		//목숨 설정
		if (life <= 0)
		{
			hero.dead();

			delay += dt;
			if (delay >= 0.7f)
				setGameover();
		}
		else
		{
			//feverMap에서 목숨 안줄어들도록
			if (currMap == maps[0])
			{
				life -= dt;
				if (life < 0)
					life = 0;
			}

			float r = life / _life;

			iPoint pPopup = pop.closePoint;
			if (pop.state == iPopupState.open)
				pPopup = Math.easeOut(pop.aniDt / pop._aniDt, pop.openPoint, pop.closePoint);
			else if (pop.state == iPopupState.close)
				pPopup = Math.easeIn(pop.aniDt / pop._aniDt, pop.closePoint, pop.openPoint);
			iPoint p = imgHpBar.position + pPopup;
			setRGBA(0f, 0.2f, 1f, 1f);
			drawImage(imgHpBar.tex.tex, p, r, 1f, TOP | LEFT);

			//데미지 표시
			if (damage > 0)
			{
				p.x += imgHpBar.tex.tex.width * r;
				r = damage / _life;
				setRGBA(1f, 0f, 0f, 1f);
				drawImage(imgHpBar.tex.tex, p, r, 1f, TOP | LEFT);
				damage -= dt * 5f;
				if (damage <= 0)
					damage = 0f;
			}
		}
	}

	void drawPopState(float dt)
	{
		popState.paint(dt);
	}

	bool keyPopState(iKeystate stat, iPoint point)
	{
		if (imgStateBtn[0].touchRect(popState.closePoint, new iSize(40, 40)).containPoint(point) == false)
			return false;

		switch (stat)
		{
			case iKeystate.Began:
				if (imgStateBtn[0].touchRect(popState.closePoint, new iSize(0, 0)).containPoint(point))
				{
					MainCamera.soundManager.playTouchSound(1);
					MainCamera.soundManager.stopBgSound();
					popPause.show(true);
				}

				break;

			case iKeystate.Moved:
				break;

			case iKeystate.Ended:
				break;
		}

		return true;
	}

	void setGameover()
	{
		if (setLoading("Result"))
		{
			Result.result = maps[0].arrived;
			Result.resultScore = score;
			Result.resultMoney = money;
		}
	}
}