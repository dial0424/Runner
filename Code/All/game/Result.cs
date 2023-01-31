using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using STD;

public class Result : nGUI
{
    public override void load()
    {
        loadBg();

        loadPopResult();
        loadPopStar();
        loadPopBtn();

        popResult[0].show(true);
    }
    public override void draw(float dt)
    {
        drawBg(dt);

        drawPopResult(dt);
        drawPopStar(dt);
        drawPopBtn(dt);
    }

    public override void key(iKeystate stat, iPoint point)
    {
        if (keyPopBtn(stat, point))
            return;

        keyBg(stat, point);
    }

    //==================================================
    //Bg
    //==================================================

    Texture bg;
    iPopup[] popAll;

    void loadBg()
    {
        bg = Resources.Load<Texture>("r_bg0");

        popAll = new iPopup[8];
    }

    void drawBg(float dt)
    {
        setRGBA(1, 1, 1, 1);
        drawImage(bg, new iPoint(0, 0), TOP | LEFT);
    }

    bool keyBg(iKeystate stat, iPoint point)
    {
        if(stat == iKeystate.Began)
        {
            if (popBtn.bShow)
                return false;

            for (int i = 0; i < popAll.Length; i++)
                popAll[i].aniDt = popAll[i]._aniDt;
        }
        return false;
    }

    //==================================================
    //ui - resultPop
    //==================================================

    //GameData gd;
    //string path;
    //string[] resultVal;

    int language;
    
    iImage[] imgResult;
    iPopup[] popResult;

	iNumber[] numberScore;
	iStrTex[] stResultScore;
	float resultDelay;

    void loadPopResult()
    {
#if false
        gd = new GameData();

        path = "save.sav";
        gd = readResult(path);
        resultVal = gd.value2string();
#else
#endif
        language = Loby.language;

        imgResult = new iImage[3];
        popResult = new iPopup[3];
		stResultScore = new iStrTex[2];

		iImage img;
        iPopup[] pop = new iPopup[3];
		MethodPopOpenClose[] openPop = new MethodPopOpenClose[3] {
			openPopResultTitle, openPopResultScore, openPopResultMoney
		};
		MethodPopDraw[] drawPop = new MethodPopDraw[3] {
			null, drawPopResultScore, drawPopResultMoney
		};

	    iPoint[] resultPoint;
        resultPoint = new iPoint[]
        {
            new iPoint(MainCamera.devWidth/ 2 - 200f, 160),
            new iPoint(MainCamera.devWidth/ 2 - 200f, 300),
            new iPoint(MainCamera.devWidth/ 2 - 200f, 380),
        };

		for (int i = 0; i < imgResult.Length; i++)
        {
            pop[i] = new iPopup();

            img = new iImage();
            iStrTex st;
            if (i == 0)
                st = new iStrTex(methodPopResult, 400, 80);
            else st = new iStrTex(methodPopResult, 400, 50);
			st.setString(i + "\n" + 0);
			//st.setString("" + i);
			img.add(st.tex);
			if (i > 0)
				stResultScore[i - 1] = st;

			pop[i].add(img);
            img.position = new iPoint(0, 0);
            imgResult[i] = img;
            
            if(i == 0)
            {
                pop[i].style = iPopupStyle.alpha;
                pop[i].openPoint = new iPoint(MainCamera.devWidth/ 2 - 200f, -100);
            }
            else
            {
                pop[i].style = iPopupStyle.zoom;
                pop[i].openPoint = resultPoint[i] + new iPoint(st.wid/2, st.hei/2);
            }
            pop[i].closePoint = resultPoint[i];
			pop[i].methodOpen = openPop[i];
			pop[i].methodDrawBefore = drawPop[i];
            popAll[i] = pop[i];
		}
		popResult = pop;

		numberScore = new iNumber[2];
		numberScore[0] = new iNumber(0, 0.3f);
		numberScore[1] = new iNumber(0, 0.3f);
	}

	void openPopResultTitle(iPopup pop)
	{
        if(popResult[1].bShow == false)
		    popResult[1].show(true);
	}
    //void drawPopResultTitle(float dt, iPopup pop, iPoint zero)

    public static bool result;
	public static int resultScore, resultMoney;

	void openPopResultScore(iPopup pop)
	{
		numberScore[0].add(resultScore);
		resultDelay = 0f;
	}
	void drawPopResultScore(float dt, iPopup pop, iPoint zero)
	{
		resultDelay += dt;
		if (resultDelay < 0.2f)
			return;

		int n = numberScore[0].get(dt);
		if (n == resultScore)
		{
			if (popResult[2].bShow == false)
				popResult[2].show(true);
		}

		stResultScore[0].setString(1 + "\n" + n);
	}
	void openPopResultMoney(iPopup pop)
	{
		numberScore[1].add(resultMoney);
		resultDelay = 0f;
	}
	void drawPopResultMoney(float dt, iPopup pop, iPoint zero)
	{
		resultDelay += dt;
		if (resultDelay < 0.2f)
			return;

		int n = numberScore[1].get(dt);
		if (n == resultMoney)
		{
			if (popStarBg.bShow == false)
				popStarBg.show(true);
		}

		stResultScore[1].setString(2 + "\n" + n);
	}

    void methodPopResult(iStrTex st)
    {
		string[] s = st.str.Split('\n');
		int index = int.Parse(s[0]);
		string strScore = s[1];
		float[] size = new float[] { 60f, 40f, 40f };
		string[][] str = new string[][] { new string[] { "게임결과", "점수        ", "돈        " },
                                          new string[] { "Result",   "Score       ", "Money     " },
                                          new string[] { "Résultat", "Score       ", "Argent    " },};

        Texture tex = Resources.Load<Texture>("result0");
        drawImage(tex, 0, 0, TOP|LEFT);

		setStringSize(size[index]);
        setStringRGBA(0, 0, 0, 1);
        setStringName("ProcPopFont");

        if (index == 0)
			drawString(str[language][index], st.wid / 2, st.hei / 2, VCENTER | HCENTER);
        else
		{
            drawString(str[language][index], 20, 5, TOP | LEFT);
			drawString(strScore, 400, 5, TOP | RIGHT);
		}
    }

    void drawPopResult(float dt)
    {
        for (int i = 0; i < popResult.Length; i++)
            popResult[i].paint(dt);
    }

    //==================================================
    //ui - starPop
    //==================================================

    iPopup popStarBg;
    iPopup[] popStar;

    int starNum;
    int openIdx;

    void loadPopStar()
    {
        iTexture iTex = new iTexture(Resources.Load<Texture>("starBg"));
        
        iPopup pop = new iPopup();
        for(int i = 0; i < 3; i++)
        {
            iImage img = new iImage();
            img.add(iTex);
            pop.add(img);
            img.position = new iPoint(- iTex.tex.width / 2 * 3 - 10 + (iTex.tex.width + 10) * i,
                                      0);
        }
        pop.style = iPopupStyle.zoom;
        pop.openPoint = new iPoint(MainCamera.devWidth / 2, 40);
        pop.closePoint = pop.openPoint;
        pop.methodOpen = openPopStar;
        popAll[3] = pop;
        popStarBg = pop;

        iTex = new iTexture(Resources.Load<Texture>("star"));

        iPopup[] pops = new iPopup[3];
        for(int i = 0; i < 3; i++)
        {
            iImage img = new iImage();
            pops[i] = new iPopup();
            img.add(iTex);
            pops[i].add(img);
            img.position = new iPoint(0, 0);

            pops[i].style = iPopupStyle.zoom;
            pops[i].zoomReverse = true;
            pops[i].methodOpen = openPopStar;
            pops[i].openPoint = new iPoint(MainCamera.devWidth / 2 - iTex.tex.width / 2 * 3 - 10 + (iTex.tex.width + 10) * i - iTex.tex.width / 2,
                                      40 - iTex.tex.height / 2);
            pops[i].closePoint = new iPoint(MainCamera.devWidth / 2 - iTex.tex.width / 2 * 3 - 10 + (iTex.tex.width + 10) * i,
                                      40);
            popAll[i + 4] = pops[i];
        }
        popStar = pops;

        starNum = setStarNum();
        openIdx = 0;
    }

    void openPopStar(iPopup pop)
    {
        if (starNum != openIdx)
        {
            popStar[openIdx].show(true);
            openIdx++;
        }
        else
        {
            if (popBtn.bShow == false)
                popBtn.show(true);
        }
    }

    int setStarNum()
    {   
        if (result == false) return 0;

        if (resultScore < 5000 && resultMoney < 1500)
            return 1;
        else if(resultScore >= 10000 && resultMoney >= 2000) return 3;
        else return 2;
    }

    void drawPopStar(float dt)
    {
        popStarBg.paint(dt);

        for(int i=0;i<starNum;i++)
            popStar[i].paint(dt);
    }

    //==================================================
    //ui - Btn
    //==================================================

    iPopup popBtn;
    iImage[] imgBtn;

    void loadPopBtn()
    {
        imgBtn = new iImage[2];

        iImage img;
        iTexture tex = null;
        iPopup pop = new iPopup();
        for(int i = 0; i < imgBtn.Length; i++)
        {
            img = new iImage();
            for(int j = 0; j < 2; j++)
            {
                iStrTex st = new iStrTex(methodStPopBtn, 100, 100);
                st.setString(i + "\n" + j);
                img.add(st.tex);
                tex = st.tex;
            }
            pop.add(img);
            img.position = new iPoint((MainCamera.devWidth - 300 - 300 - tex.tex.width) * i, 0);
            imgBtn[i] = img;
        }
        pop.style = iPopupStyle.alpha;
        pop.openPoint = new iPoint(300, MainCamera.devHeight);
        pop.closePoint = new iPoint(300, MainCamera.devHeight - 150);
        popAll[7] = pop;
        popBtn = pop;
    }

    void methodStPopBtn(iStrTex st)
    {
        string[] s = st.str.Split('\n');
        int index = int.Parse(s[0]);
        int select = int.Parse(s[1]);

        Texture tex = Resources.Load<Texture>("resulticon0");
        drawImage(tex, 0, 0, TOP | LEFT);

        string[][] strs = new string[][] { new string[] { "다시", "확인", },
                                           new string[] { "Again", "Check", },
                                           new string[] { "Encore", "Vérifiez", },};
        setStringName("ProcPopFont");
        setStringSize(20f);
        if (select == 0)
            setStringRGBA(0, 0, 0, 1);
        else 
            setStringRGBA(1, 0, 1, 1);

        drawString(strs[language][index], 50, 50, VCENTER | HCENTER);
    }

    void drawPopBtn(float dt)
    {
        if(popBtn.state == iPopupState.proc)
        {
            for(int i = 0; i < imgBtn.Length; i++)
            {
                imgBtn[i].frame = (popBtn.selected == i ? 1 : 0);
            }
        }
        popBtn.paint(dt);
    }

    bool keyPopBtn(iKeystate stat, iPoint point)
    {
        if (popBtn.state != iPopupState.proc)
            return false;

        int i, j = -1;

        switch (stat)
        {
            case iKeystate.Began:
                for (i = 0; i < imgBtn.Length; i++)
                {
                    if (imgBtn[i].touchRect(popBtn.closePoint, new iSize(0, 0)).containPoint(point))
                    {
                        j = i;
                        break;
                    }
                }

                popBtn.selected = j;

                if (j == -1)
                {
                    MainCamera.soundManager.playTouchSound(0);
                    break;
                }

                imgBtn[j].setBtnState(0);
                imgBtn[j].once = true;
                MainCamera.soundManager.playTouchSound(1);

                break;
            case iKeystate.Moved:
                for(i = 0; i < imgBtn.Length; i++)
                {
                    if (imgBtn[i].touchRect(popBtn.closePoint, new iSize(0, 0)).containPoint(point))
                    {
                        j = i;
                        break;
                    }
                }

                if(popBtn.selected != j)
                {
                    if (popBtn.selected != -1)
                    {
                        imgBtn[popBtn.selected].btnState[0] = false;
                        imgBtn[popBtn.selected].btnState[1] = false;
                    }
                    if(j != -1)
                    {
                        imgBtn[j].setBtnState(1);
                    }
                    popBtn.selected = j;
                }

                break;
            case iKeystate.Ended:
                i = popBtn.selected;
                if (i == -1) break;

                imgBtn[i].setBtnState(2);

                if(i == 0)
                {
                    setLoading("Proc");
                }
                else
                {
                    setLoading("Loby");
                }

                for (i = 0; i < popAll.Length; i++)
                    popAll[i].show(false);
                
                break;
        }

        return true;
    }
}
