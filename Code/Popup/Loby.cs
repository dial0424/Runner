public class Loby : nGUI
{
	public override void load()
	{
		loadPopMenu();
	}

	iPopup popMenu;
	iImage[] imgMenu;

	//팝업 중 하나
	//로비의 게임시작, 옵션, 게임종료 버튼 코드
	//==================================================
	//ui - Menu
	//==================================================
	iPopup popMenu;
	iImage[] imgMenu;

	float disappearMenuDt;
	float _disappearMenuDt = 0.4f;

	void loadPopMenu()
	{
		iImage img;
		iTexture tex = null;

		iPopup pop = new iPopup();

		//팝업이 가지는 이미지, 글씨 넣기
		imgMenu = new iImage[3];
		for (int i = 0; i < 3; i++)
		{
			img = new iImage();
			for (int j = 0; j < 2; j++)
			{
				iStrTex st = new iStrTex(methodStMenuBtn, 100, 100);
				st.setString(i + "\n" + j);
				img.add(st.tex);
				tex = st.tex;
			}
			img.position = new iPoint((tex.tex.width + 20) * i, 0);
			imgMenu[i] = img;
			pop.add(img);
		}

		//팝업의 스타일, 여닫는 위치, 델리게이트 함수 설정
		pop.style = iPopupStyle.move;
		pop.openPoint = new iPoint(MainCamera.devWidth - (tex.tex.width + 20) * 3 - 20, MainCamera.devHeight);
		pop.closePoint = new iPoint(MainCamera.devWidth - (tex.tex.width + 20) * 3 - 20, MainCamera.devHeight - tex.tex.height - 20);
		pop.methodClose = closePopMenu;
		popMenu = pop;

		disappearMenuDt = _disappearMenuDt;
	}

	//버튼 위 글씨
	void methodStMenuBtn(iStrTex st)
	{
		string[] s = st.str.Split('\n');
		int index = int.Parse(s[0]);
		int select = int.Parse(s[1]);

		string[][] str = new string[][] { new string[] { "게임시작", "옵션", "게임종료" },
										  new string[] { "Game start", "option", "game end" },
										  new string[] { "Démarrer le jeu", "Options", "Fin du jeu" },};

		Texture tex = Resources.Load<Texture>("lobyicon0");// + index
		drawImage(tex, 0, 0, TOP | LEFT);

		setStringName("ProcPopFont");
		setStringSize(20);
		if (select == 0)
			setStringRGBA(0, 0, 0, 1);
		else
			setStringRGBA(1, 0, 1, 1);

		drawString(str[language][index], 50, 50, VCENTER | HCENTER);
	}

	void drawPopMenu(float dt)
	{
		if (popMenu.state == iPopupState.proc)
		{
			for (int i = 0; i < imgMenu.Length; i++)
			{
				imgMenu[i].frame = (i == popMenu.selected ? 1 : 0);
			}
		}

		//닫힐때 점점 투명해지도록
		if (disappearMenuDt < _disappearMenuDt)
		{
			disappearMenuDt += dt;
			float a = 1 - disappearMenuDt / _disappearDt; //1~0

			popMenu.paint(dt, a);
		}

		popMenu.paint(dt);
	}

	bool keyPopMenu(iKeystate stat, iPoint point)
	{
		if (popMenu == null ||
			popMenu.bShow == false)
			return false;
		if (popMenu.state != iPopupState.proc)
			return false;// return true;

		int i, j = -1;

		switch (stat)
		{
			case iKeystate.Began:

				for (i = 0; i < 3; i++)
				{
					if (imgMenu[i].touchRect(popMenu.closePoint, new iSize(0, 0)).containPoint(point))
					{
						j = i;
						break;
					}
				}

				popMenu.selected = j;

				if (j == -1)
				{
					MainCamera.soundManager.playTouchSound(0);
					break;
				}

				imgMenu[j].setBtnState(0);
				//버튼선택 효과음
				MainCamera.soundManager.playTouchSound(1);

				break;

			case iKeystate.Moved:

				for (i = 0; i < 3; i++)
				{
					if (imgMenu[i].touchRect(popMenu.closePoint, new iSize(0, 0)).containPoint(point))
					{
						j = i;
						break;
					}
				}

				if (popMenu.selected != j)
				{
					if (popMenu.selected != -1)
					{
						imgMenu[popMenu.selected].btnState[0] = false;
						imgMenu[popMenu.selected].btnState[1] = false;
					}
					if (j != -1)
						imgMenu[j].setBtnState(1);
					popMenu.selected = j;
				}

				break;

			case iKeystate.Ended:
				i = popMenu.selected;
				if (i == -1) break;

				imgMenu[i].setBtnState(2);

				if (i == 0)
				{
					popMenu.show(false);
				}
				else if (i == 1)
				{
					popOpt.show(true);
					resetPopOpt();
				}
				else if (i == 2)
				{
					popQuit.show(true);
				}

				break;
		}

		return false;// return true;
	}

	void play()
	{
		Hero.heroIdx = centerIdx;
		setLoading("Proc");
	}
}