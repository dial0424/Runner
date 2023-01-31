using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STD;

public class Loby : nGUI
{
	RenderTexture rt;
	
	public override void load()
	{
		rt = new RenderTexture(MainCamera.devWidth, MainCamera.devHeight, 32, RenderTextureFormat.ARGB32);

		loadBg();

		loadPopChar();
		loadPopMenu();
		loadPopCharInfo();
		loadPopOpt();
		loadPopQuit();

		popMenu.show(true);
		popChar.show(true);

		//loadParticle();
	}

	public override void draw(float dt)
	{
		RenderTexture backUp = RenderTexture.active;
		RenderTexture.active = rt;
		GL.Clear(true, true, Color.clear);

		drawBg(dt);

		drawPopChar(dt);
		drawPopMenu(dt);
		drawPopCharInfo(dt);

		RenderTexture.active = backUp;

		drawImage(rt, 0, 0, TOP | LEFT);

		drawPopOpt(dt);
		drawPopQuit(dt);

		//drawParticle(dt);
	}

	public override void key(iKeystate stat, iPoint point)
	{
		//point_ = point;

		// key.....
		if (keyPopOpt(stat, point) ||
			keyPopQuit(stat, point))
			return;
		if (keyPopChar(stat, point) ||
			keyPopMenu(stat, point))
			return;

		// 배경 터치 스크린 처리...
	}

	//==================================================
	// Bg
	//==================================================
	Texture texBg;

	void loadBg()
	{
		//background
		texBg = Resources.Load<Texture>("l_bg0");

	}

	void drawBg(float dt)
	{
		setRGBA(1, 1, 1, 1);
		drawImage(texBg, 0, 0, TOP | LEFT);
	}


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

		pop.style = iPopupStyle.move;
		pop.openPoint = new iPoint(MainCamera.devWidth - (tex.tex.width + 20) * 3 - 20, MainCamera.devHeight);
		pop.closePoint = new iPoint(MainCamera.devWidth - (tex.tex.width + 20) * 3 - 20, MainCamera.devHeight - tex.tex.height - 20);
		pop.methodClose = closePopMenu;
		popMenu = pop;

		disappearMenuDt = _disappearMenuDt;
	}

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

	void closePopMenu(iPopup pop)
	{
		disappearDt = 0f;
		imgChar[centerIdx].alpha = 0f;
		popChar.show(false);
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

		if (disappearMenuDt < _disappearMenuDt)
		{
			disappearMenuDt += dt;
			float a = 1 - disappearMenuDt / _disappearDt; //1~0

			popMenu.paint(dt, a);
		}
		popMenu.paint(dt);
	}

	void resetPopOpt()
	{
		page = 0;
		imgOptBtn[1].alpha = 0;
		imgOptBtn[2].alpha = 1;
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

				if(popMenu.selected != j)
				{
					if(popMenu.selected != -1)
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
#if false
		chacData c = new chacData(centerIdx);
		byte[] bytes = FileIO.struct2bytes(c);
		FileIO.save("chacData", bytes);
#endif
		Hero.heroIdx = centerIdx;
		setLoading("Proc");
	}

	//==================================================
	//ui - Char
	//==================================================
	iPopup popChar;
	iImage[] imgChar;
	iImage[] imgCharBtn;

	// currChar == 0, imgChar[2], [0], [1]
	// currChar == 1, imgChar[0], [1], [2]
	// currChar == 2, imgChar[1], [2], [0]
	int currChar, nextChar;
	float moveCharDt;
	float _moveCharDt = 0.5f;

	iImage[] imgCharRun;

	iPoint[] posChar = new iPoint[3]
	{
		new iPoint(MainCamera.devWidth/2 - 250 - 35, 30),
		new iPoint(MainCamera.devWidth/2 - 35, 30),
		new iPoint(MainCamera.devWidth/2 + 250 - 35, 30),
	};
	float[] scaleChar = new float[] { 1.0f, 1.7f, 1.0f };

	int centerIdx;
	public bool run;
	float disappearDt;
	float _disappearDt = 0.4f;

	void loadPopChar()
	{
		iImage img;
		iTexture tex = null;

		iPopup pop = new iPopup();

		imgChar = new iImage[3];

		for (int i = 0; i < 3; i++)
		{
			img = new iImage();
			for (int j = 0; j < 4; j++)
			{
				tex = new iTexture(Resources.Load<Texture>("cookie" + i + "_" + j));
				img.add(tex);
			}

			img.position = posChar[i];
			img._frameDt = 0.1f;// <<<<<<
			img.startAnimation();
			if (i == 1)
				img.scale = 1.7f;
			pop.add(img);
			img.anc = VCENTER | HCENTER;
			imgChar[i] = img;
		}

		imgCharBtn = new iImage[2];
		for (int i = 0; i < 2; i++)
		{
			img = new iImage();
			tex = new iTexture(Resources.Load<Texture>("arrow" + i));
			img.tex = tex;
			img.add(tex);
			img.position = new iPoint((MainCamera.devWidth - 100 - img.tex.tex.width) * i, 0);
			imgCharBtn[i] = img;
			pop.add(img);
		}

		pop.style = iPopupStyle.alpha;
		pop.openPoint = new iPoint(50, MainCamera.devHeight / 2 - tex.tex.height / 2 - 100);
		pop.closePoint = new iPoint(50, MainCamera.devHeight / 2 - tex.tex.height / 2);
		pop.methodClose = closePopChar;
		popChar = pop;

		imgCharRun = new iImage[3];
		for (int i = 0; i < 3; i++)
		{
			img = imgChar[i].clone();
			img.position = new iPoint(posChar[1].x, posChar[1].y + popChar.closePoint.y);
			img._frameDt = -0.05f;// <<<<<< 1300f 속로 오른쪽으로 이동
			img.scale = 1.7f;
			imgCharRun[i] = img;
		}

		currChar = nextChar = 1;
		moveCharDt = 0f;

		centerIdx = 1;
		run = false;
		disappearDt = _disappearDt;
	}
	void closePopChar(iPopup pop)
	{
		disappearDt = 0f;
		play();
	}

	int[][] array = new int[][]
	{
		new int[] { 2, 0, 1 },
		new int[] { 0, 1, 2 },
		new int[] { 1, 2, 0 },
	};

	void drawPopChar(float dt)
	{
		if (currChar != nextChar)
		{
			bool lr = imgChar[0].leftRight;
			moveCharDt += dt;
			// currChar == 0, imgChar[2], [0], [1]
			// currChar == 1, imgChar[0], [1], [2]
			// currChar == 2, imgChar[1], [2], [0]
			if (moveCharDt < _moveCharDt)
			{
				iSort.init();
				float r = moveCharDt / _moveCharDt;
				for (int i = 0; i < 3; i++)
				{
					imgChar[i].position = Math.linear(r, posChar[array[currChar][i]],
														posChar[array[nextChar][i]]);
					imgChar[i].scale = Math.linear(r, scaleChar[array[currChar][i]],
													  scaleChar[array[nextChar][i]]);

					iSort.add(imgChar[i].scale);
				}
				iSort.update();
				for (int i = 0; i < 3; i++)
				{
					int j = iSort.get(i);
					popChar.listImg[i] = imgChar[j];
				}
			}
			else
			{
				moveCharDt = 0f;
#if false
				currChar = nextChar;
#else
				if (lr)
				{
					currChar = (currChar + 2) % 3;
					centerIdx = (centerIdx + 1) % 3;
				}
				else
				{
					currChar = (currChar + 1) % 3;
					centerIdx = (centerIdx + 2) % 3;
				}
				
#endif
				for (int i = 0; i < 3; i++)
				{
					imgChar[i].position = posChar[array[currChar][i]];
					imgChar[i].scale = scaleChar[array[currChar][i]];
					if (i != currChar)
						imgChar[array[currChar][i]].animation = false;
					if (imgChar[i].leftRight) imgChar[i].leftRight = false;
				}
			}
		}

		if (disappearDt < _disappearDt)
		{
			run = true;
			disappearDt += dt;
			float a = 1 - disappearDt / _disappearDt; //1~0

			popChar.paint(dt, a);
		}

		if (run)
		{
			iImage img = imgCharRun[centerIdx];
			img.position.x += dt * 1300f;
			img.paint(dt);
		}

		popChar.paint(dt);
	}

	bool showPopCharInfo = false;//popCharInfo.show(true);

	bool keyPopChar(iKeystate stat, iPoint point)
	{
		if (popChar == null ||
			popChar.bShow == false)
			return false;
		if (popChar.state != iPopupState.proc)
			return false;// return true;

		int i, j = -1;

		switch (stat)
		{
			case iKeystate.Began:

				if (imgChar[centerIdx].touchRect(popChar.closePoint, new iSize(0, 0)).containPoint(point))
					showPopCharInfo = true;

				for (i = 0; i < imgCharBtn.Length; i++)
				{
					if (imgCharBtn[i].touchRect(popChar.closePoint, new iSize()).containPoint(point))
					{
						j = i;
						break;
					}
				}

				popChar.selected = j;

				if (j == -1)
				{
					MainCamera.soundManager.playTouchSound(0);
					break;
				}

				imgCharBtn[j].setBtnState(0);
				MainCamera.soundManager.playTouchSound(1);

				if (j == 0)
				{
					nextChar = (currChar + 2) % 3;

					for (i = 0; i < 3; i++)
						imgChar[i].leftRight = true;

				}
				else if (j == 1)
				{
					nextChar = (currChar + 1) % 3;

					for (i = 0; i < 3; i++)
						imgChar[i].leftRight = false;

				}

				for (i = 0; i < 3; i++)
					imgChar[i].startAnimation();

				break;

			case iKeystate.Moved:
				
				if (!imgChar[centerIdx].touchRect(popChar.closePoint, new iSize(0, 0)).containPoint(point))
						showPopCharInfo = false; 

				for (i = 0; i < 2; i++)
				{
					if (imgCharBtn[i].touchRect(popChar.closePoint, new iSize(0, 0)).containPoint(point))
					{
						j = i;
						break;
					}
				}

				if (popChar.selected != j)
				{
					if (popChar.selected != -1)
					{
						imgCharBtn[popChar.selected].btnState[0] = false;
						imgCharBtn[popChar.selected].btnState[1] = false;
					}
					if (j != -1)
					{
						imgCharBtn[j].setBtnState(1);
					}
					popChar.selected = j;
				}

				break;

			case iKeystate.Ended:
				showPopCharInfo = false;

				i = popChar.selected;
				if (i == -1)
					break;

				imgCharBtn[i].setBtnState(2);

				break;
		}
		return false;// return true;
	}

	//==================================================
	//ui - popCharInfo
	//==================================================
	iPopup popCharInfo;
	iStrTex popInfoStrTex;

	void loadPopCharInfo()
	{
		iPopup pop = new iPopup();

		// bg (include exp)
		iImage img = new iImage();

		iStrTex st = new iStrTex(methodCharInfo, 500, 200);
		st.setString(centerIdx + "");
		img.add(st.tex);
		popInfoStrTex = st;
		pop.add(img);

		pop.style = iPopupStyle.zoom;
		pop.openPoint = new iPoint(MainCamera.devWidth / 2, (720 - 140) / 2 - 20);
		pop.closePoint = new iPoint((MainCamera.devWidth - 500) / 2, pop.openPoint.y - 200);
		pop.methodDrawBefore = drawPopCharInfoBefore;
		popCharInfo = pop;
	}

	void methodCharInfo(iStrTex st)
	{
		Texture tex = Resources.Load<Texture>("info");
		drawImage(tex, 0, 0, TOP | LEFT);

		string[][][] charInfos = new string[][][]
		{
			new string[][]{ new string[]{ "용감한쿠키", "매우 용감함"},
							new string[]{ "명량한쿠키", "매우 명량함"},
							new string[]{ "허브맛쿠키", "허브향이 남"},},
			new string[][]{ new string[]{ "Brave cookies", "Very brave"},
							new string[]{ "Bright cookie", "Very bright"},
							new string[]{ "Herb-flavored cookie", "It's smells like herbs"},},
			new string[][]{ new string[]{ "Cookies braves", "Très courageux"},
							new string[]{ "Bright Cookie", "Très brillant" },
							new string[]{ "Cookie au goût d'herbe", "Ça sent l'herbe"},},
		};
		int idx = int.Parse(st.str);

		setStringSize(40f);
		setStringRGBA(0.2f, 0.2f, 0.2f, 1);
		setStringName("ProcPopFont");
		drawString(charInfos[language][idx][0], 250, 50, VCENTER | HCENTER);
		setStringSize(27f);
		drawString(charInfos[language][idx][1], 100, 100, VCENTER | LEFT);
	}

	void drawPopCharInfoBefore(float dt, iPopup pop, iPoint zero)
	{
		popInfoStrTex.setString(centerIdx + "");
	}

	void drawPopCharInfo(float dt)
	{
		if (showPopCharInfo)
		{
			if (popCharInfo.bShow == false)
				popCharInfo.show(true);
		}
		else
		{
			if (popCharInfo.state == iPopupState.open)
			{
				popCharInfo.aniDt = popCharInfo._aniDt - popCharInfo.aniDt;
				popCharInfo.state = iPopupState.close;
			}
			else if (popCharInfo.state == iPopupState.proc)
				popCharInfo.show(false);
		}
		popCharInfo.paint(dt);
	}
	//==================================================
	//ui - popOpt
	//==================================================
	GameData gd;
	string path;
	string[] resultVal;

	iImage[] imgOptBtn;
	iPopup popOpt;
	iStrTex stOptBg;
	iStrTex[] stCheck;

	int page;

	public static int language; //0 : 한국어, 1 : 영어, 2 : 프랑스어
	int setLanguage;

	void loadPopOpt()
	{
		gd = new GameData();
		
		path = "save.sav";
		gd = readResult(path);
		resultVal = gd.value2string();
		language = int.Parse(resultVal[0]);
		setLanguage = language;

		imgOptBtn = new iImage[6];

		iPopup pop;
		iTexture tex;
		iStrTex st;
		iImage img;

		pop = new iPopup();
		img = new iImage();

		st = new iStrTex(methodPopOpt, 640, 400);
		st.setString(page + "");
		stOptBg = st;
		img.add(st.tex);
		img.position = new iPoint();
		pop.add(img);

		tex = new iTexture(Resources.Load<Texture>("close"));
		img = new iImage();
		img.add(tex);
		img.position = new iPoint(640 - tex.tex.width / 3 * 2, 0 - tex.tex.height / 3);
		imgOptBtn[0] = img;
		pop.add(img);

		for (int i = 0; i < 2; i++)
		{
			img = new iImage();

			tex = new iTexture(Resources.Load<Texture>("arrow" + i));
			img.add(tex);
			img.position = new iPoint(30 + (640 - 100 - 30 - 30) * i, 400 - 100 - 30);
			imgOptBtn[i + 1] = img;
			pop.add(img);
		}

		stCheck = new iStrTex[3];
		for (int i = 0; i < 3; i++)
		{
			img = new iImage();

			st = new iStrTex(methodCheckBtn, 60, 60);
			if (i == 0)
				st.setString("" + 0);
			else 
				st.setString("" + 1);
			stCheck[i] = st;
			img.add(st.tex);

			img.position = new iPoint(640 - img.tex.tex.width - 30, 80 + 65 * i);
			img.alpha = 0f;
			imgOptBtn[i + 3] = img;
			pop.add(img);
		}

		pop.style = iPopupStyle.zoom;
		pop.openPoint = new iPoint(MainCamera.devWidth / 2, MainCamera.devHeight / 2);
		pop.closePoint = new iPoint((MainCamera.devWidth - 640) / 2, (MainCamera.devHeight - 400) / 2);
		pop.methodDrawBefore = drawPopOptBefore;
		pop.methodPaintBefore = paintPopOptBefore;
		pop.methodClose = methodPopOptClose;
		popOpt = pop;

		page = 0;
		imgOptBtn[1].alpha = 0;
	}
	GameData readResult(string p)
	{
		GameData v;
		byte[] b = FileIO.load(p);
		if(b == null)
		{
			v = new GameData(0);
			b = FileIO.struct2bytes(v);
			FileIO.save(p, b);

			return v;
		}
		
		v = FileIO.bytes2struct<GameData>(b);

		return v;
	}

	void methodPopOpt(iStrTex st)
	{
		int page = int.Parse(st.str);

		Texture tex = Resources.Load<Texture>("OptBg");
		drawImage(tex, 0, 0, TOP | LEFT);

		string[][] strs = new string[][] { new string[]{ "내일이 기대되는 (미래)개발자, 김란입니다\n\n\nemail : dial0424@daum.net", 
														 "총 개발기간 : 1.5개월\n만든도구 : Unity / Visual Studio",
														 "주언어 : C# \n유니티, 코코스2D-X",
														 "언어설정\n\n한국어\n\nEnglish\n\nFrançais"},
										   new string[]{ "(future)Developers looking forward to tomorrow,\nI'm Kim Ran\n\n\nemail : dial0424@daum.net",
														 "Total development period: 1.5 months\nMade tools : Unity / Visual Studio",
														 "Main language: C# \nUnity, Cocos2D-X",
														 "Language Settings\n\n한국어\n\nEnglish\n\nFrançais"},
										   new string[]{ "(avenir)Les développeurs attendent demain,\nje suis Kim Ran.\n\n\nemail : dial0424@daum.net",
														 "Durée totale de développement : 1,5 mois\n outil créé : Unity / Visual Studio",
														 "Langue principale : C# \nUnité, Cocos2D-X",
														 "Paramètres linguistiques\n\n한국어\n\nEnglish\n\nFrançais" } };
		string[] str = new string[] { "언어를 바꾸고 창을 닫으면 재로딩됩니다.",
									  "If you change the language\nand close the window,\nit will reload.",
									  "Si vous changez de langue\net fermez la fenêtre,\nelle se recharge.",};
		string s = "";

		s = strs[language][page];

		setStringName("ProcPopFont");
		setStringSize(30f);
		setStringRGBA(0, 0, 0, 1f);
		drawString(s, new iPoint(30, 30), TOP | LEFT);

		if (page == 3)
		{
			s = str[language];
			setStringSize(30f);
			setStringRGBA(1f, 0f, 0.3f, 1f);
			drawString(s, new iPoint(610, 370), BOTTOM | RIGHT);
		}
	}

	void methodCheckBtn(iStrTex st)
	{
		string strs = st.str;
		int chk = int.Parse(strs);

		Texture tex = Resources.Load<Texture>("check" + chk);
		drawImage(tex, 0, 0, TOP | LEFT);
	}

	void drawPopOptBefore(float dt, iPopup pop, iPoint zero)
	{
		stOptBg.setString("" + page);
		for(int i=0;i<stCheck.Length;i++)
		{
			if (i == setLanguage)
				stCheck[i].setString("0");
			else 
				stCheck[i].setString("1");
		}
	}

	void drawPopBlur(iPopup pop)
	{
		setBlendFunc(2);
		float blur = 5;
		if (pop.state == iPopupState.open)
			blur = 5 * pop.aniDt / pop._aniDt;
		else if (pop.state == iPopupState.close)
			blur = 5 * (1 - pop.aniDt / pop._aniDt);
		setBlur(blur);
		drawImage(rt, 0, 0, TOP | LEFT);
		setBlendFunc(0);
	}
	void paintPopOptBefore(float dt, iPopup pop)
	{
		drawPopBlur(pop);
	}
	bool drawPopOpt(float dt)
	{
		popOpt.paint(dt);

		return popOpt.bShow;
	}
	void methodPopOptClose(iPopup pop)
	{
		for(int i = 3; i < imgOptBtn.Length; i++)
		{
			imgOptBtn[i].alpha = 0;
		}
	}

	bool keyPopOpt(iKeystate stat, iPoint point)
	{
		if (popOpt == null || !popOpt.bShow)
			return false;

		int i, j = -1;

		switch (stat)
		{
			case iKeystate.Began:
				for (i = 0; i < 6; i++)
				{
					if (imgOptBtn[i].alpha == 0f) continue;
					if (imgOptBtn[i].touchRect(popOpt.closePoint, new iSize()).containPoint(point))
					{
						j = i;
						break;
					}
				}

				popOpt.selected = j;

				if(j == -1)
					break;
				
				imgOptBtn[j].setBtnState(0);
				if(j == 0)
					MainCamera.soundManager.playTouchSound(1);
				else
					MainCamera.soundManager.playTouchSound(2);

				break;

			case iKeystate.Moved:
				for (i = 0; i < 6; i++)
				{
					if (imgOptBtn[i].touchRect(popOpt.closePoint, new iSize()).containPoint(point))
					{
						j = i;
						break;
					}
				}

				if (popOpt.selected != j)
				{
					//버튼 취소음, 선택음
					if (popOpt.selected != -1)
					{
						imgOptBtn[popOpt.selected].btnState[0] = false;
						imgOptBtn[popOpt.selected].btnState[1] = false;
					}
					if (j != -1)
						imgOptBtn[j].setBtnState(1);
					popOpt.selected = j;
				}

				break;

			case iKeystate.Ended:
				i = popOpt.selected;
				if (i == -1) break;

				imgOptBtn[i].setBtnState(2);

				j = page;
				if (i == 0)
				{
					popOpt.show(false);
					if (language != setLanguage)
					{
						gd.setValue(setLanguage);

						string path = "save.sav";
						byte[] bytes = FileIO.struct2bytes(gd);

						FileIO.save(path, bytes);

						setLoading("Loby");
					}
				}
				else if (i == 1)
				{
					if (page > 0)
						page--;
				}
				else if (i == 2)
				{
					if (page < 3)
						page++;
				}
				else
				{
					setLanguage = i - 3;
				}

				if (j != page)
				{
					if (page == 0)
					{
						imgOptBtn[1].alpha = 0;
						imgOptBtn[2].alpha = 1;
					}
					else if (page == 1)
					{
						imgOptBtn[1].alpha = 1;
						imgOptBtn[2].alpha = 1;
					}
					else if (page == 2)
					{
						imgOptBtn[1].alpha = 1;
						imgOptBtn[2].alpha = 1;
						imgOptBtn[3].alpha = 0;
						imgOptBtn[4].alpha = 0;
						imgOptBtn[5].alpha = 0;
					}
					else if (page == 3)
					{
						imgOptBtn[1].alpha = 1;
						imgOptBtn[2].alpha = 0;
						imgOptBtn[3].alpha = 1;
						imgOptBtn[4].alpha = 1;
						imgOptBtn[5].alpha = 1;
					}
				}
				break;
		}
		return true;
	}

	//==================================================
	//ui - popQuit
	//==================================================
	iPopup popQuit;
	iImage[] imgQuitBtn;

	void loadPopQuit()
	{
		iPopup pop = new iPopup();
		iImage img = new iImage();

		iStrTex st = new iStrTex(methodPopQuit, 400, 250);
		st.setString("");
		img.add(st.tex);
		img.position = new iPoint(0, 0);
		pop.add(img);

		imgQuitBtn = new iImage[2];
		for (int i = 0; i < 2; i++)
		{
			img = new iImage();
			st = new iStrTex(methodPopQuitBtn, 150, 60);
			st.setString(i + "\n");
			img.add(st.tex);
			img.position = new iPoint(20 + (400 - 150 - 20 - 20) * i, 250 - 30);
			imgQuitBtn[i] = img;
			pop.add(img);
		}

		pop.style = iPopupStyle.zoom;
		pop.openPoint = new iPoint(MainCamera.devWidth / 2, MainCamera.devHeight / 2);
		pop.closePoint = new iPoint((MainCamera.devWidth - 400) / 2, (MainCamera.devHeight - 250) / 2);
		pop.methodPaintBefore = paintPopQuitBefore;
		popQuit = pop;
	}

	void methodPopQuit(iStrTex st)
	{
		Texture tex = Resources.Load<Texture>("optQuitBg");
		drawImage(tex, 0, 0, TOP | LEFT);

		string[] s = new string[]{ "종료하시겠습니까?",
								   "You want to exit?",
								   "Tu veux sortir ?",};

		setStringSize(50f);
		setStringName("ProcPopFont");
		setStringRGBA(0, 0, 0, 1);
		drawString(s[language], new iPoint(tex.width / 2, tex.height / 2), VCENTER | HCENTER);
	}

	void methodPopQuitBtn(iStrTex st)
	{
		string[] s = st.str.Split('\n');
		int index = int.Parse(s[0]);

		Texture tex = Resources.Load<Texture>("optQuitBtn");
		drawImage(tex, 0, 0, TOP | LEFT);

		string[][] str = new string[][] { new string[] { "종료", "취소" },
										  new string[] { "Exit", "Cancel" },
										  new string[] { "Sortir", "annuler" },};

		setStringSize(30f);
		setStringName("ProcPopFont");
		setStringRGBA(0, 0, 0, 1);
		drawString(str[language][index], new iPoint(tex.width / 2, tex.height / 2), VCENTER | HCENTER);
	}

	void paintPopQuitBefore(float dt, iPopup pop)
	{
		drawPopBlur(pop);
	}
	bool drawPopQuit(float dt)
	{
		popQuit.paint(dt);

		return popQuit.bShow;
	}

	bool keyPopQuit(iKeystate stat, iPoint point)
	{
		if (popQuit == null || !popQuit.bShow) return false;

		int i, j = -1;

		switch (stat)
		{
			case iKeystate.Began:
				for (i = 0; i < 2; i++)
				{
					if (imgQuitBtn[i].touchRect(popQuit.closePoint, new iSize()).containPoint(point))
					{
						j = i;
						break;
					}
				}

				popQuit.selected = j;

				if(j == -1)
					break;
				
				imgQuitBtn[i].setBtnState(0);
				MainCamera.soundManager.playTouchSound(1);

				break;

			case iKeystate.Moved:

				for(i = 0; i < 2; i++) 
				{
					if (imgQuitBtn[i].touchRect(popQuit.closePoint, new iSize()).containPoint(point))
					{
						j = i;
						break;
					} 
				}

				if(popQuit.selected != j)
				{
					if(popQuit.selected != -1)
					{
						imgQuitBtn[popQuit.selected].btnState[0] = false;
						imgQuitBtn[popQuit.selected].btnState[1] = false;
					}
					if (j != -1)
						imgQuitBtn[j].setBtnState(1);
					popQuit.selected = j;
				}

				break;

			case iKeystate.Ended:
				i = popQuit.selected;
				if(i == -1) break;

				imgQuitBtn[i].setBtnState(2);

				if(i == 0)
				{
					Application.Quit();
					Debug.Log("게임종료");
				}
				else //if(i == 1)
				{
					popQuit.show(false);
				}

				break;
		}

		return true;
	}

	//==================================================
	//ui - particle
	//==================================================

	Particle[] _particle;
	Particle[] particle;
	int particleNum;
	const int _particleNum = 1000;

	float createDt, _createDt;

	Texture texParticle;

	void loadParticle()
	{
		_particle = new Particle[_particleNum];
		for (int i = 0; i < _particleNum; i++)
			_particle[i] = new Particle();

		particle = new Particle[_particleNum];
		particleNum = 0;

		_createDt = 0.002f;
		createDt = 0f;

		texParticle = Resources.Load<Texture>("one");
	}

	void drawParticle(float dt)
	{
		setBlendFunc(1);
		for(int i=0; i<particleNum; i++)
		{
			if (particle[i].paint(dt))
			{
				particleNum--;
				particle[i] = particle[particleNum];
				i--;
			}
		}
		setBlendFunc(0);

		// 0.05f 생성
		createDt += dt;
		while( createDt > _createDt )
		{
			createDt -= _createDt;

			// add
			for(int i=0; i<_particleNum; i++)
			{
				if(_particle[i].life >= _particle[i]._life)
				{
					float life = 0.5f;
					iPoint sp = new iPoint(MainCamera.devWidth / 2 + Random.RandomRange(-25, 25), 100 + MainCamera.devHeight * 0.66f + Random.RandomRange(-15, 15));
					iPoint ep = new iPoint(MainCamera.devWidth / 2 + Random.RandomRange(-8, 8), 150 + MainCamera.devHeight / 2 + Random.RandomRange(-25, 25));
					float ss = 0.2f + Random.RandomRange(0, 1), es = Random.RandomRange(0, 1);
					Color sc = new Color(0f, Random.RandomRange(0.1f, 0.3f), Random.RandomRange(0.5f, 1f), 1f);
					Color ec = new Color(0f, Random.RandomRange(0.1f, 0.3f), Random.RandomRange(0.5f, 1f), 0f);
					//Color sc = new Color(1, 1, 1, Random.RandomRange(0.5f, 1f));
					//Color ec = new Color(1, 1, 1, Random.RandomRange(0f, 0.2f));
					_particle[i].set(texParticle, life, sp, ep, ss, es, sc, ec);

					particle[particleNum] = _particle[i];
					particleNum++;
					return;
				}
			}
		}
	}

}

class Particle
{
	Texture tex;
	public float life, _life;
	iPoint sp, ep;// position
	float ss, es;// scale
	Color sc, ec;// color

	public void set(Texture tex, float life, iPoint sp, iPoint ep, float ss, float es, Color sc, Color ec)
	{
		this.tex = tex;
		_life = life;
		this.life = 0f;
		this.sp = sp;
		this.ep = ep;
		this.ss = ss;
		this.es = es;
		this.sc = sc;
		this.ec = ec;
	}

	public bool paint(float dt)
	{
		float r = life / _life;
		iPoint p = sp * (1 - r) + ep * r;
		float s = ss * (1 - r) + es * r;
		Color c = sc * (1 - r) + ec * r;

		iGUI.instance.setRGBA(c.r, c.g, c.b, c.a);
		iGUI.instance.drawImage(tex, p.x, p.y, s, s, iGUI.VCENTER | iGUI.HCENTER, 2, 0, iGUI.REVERSE_NONE);

		life += dt;
		if (life > _life)
			return true;
		return false;
	}
}



public struct chacData
{
    public int centerIdx;
    public chacData(int Idx)
    {
        centerIdx = Idx;
    }
}