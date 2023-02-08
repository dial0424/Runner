public class Result : nGUI
{
    public override void load()
    {
        loadPopStar();
    }

    public override void draw(float dt)
    {
        drawPopStar(dt);
    }

    //팝업중 하나
    //최상단의 별 팝업
    //==================================================
    //ui - starPop
    //==================================================

    iPopup popStarBg;
    iPopup[] popStar;

    int starNum;
    int openIdx;

    void loadPopStar()
    {
        //별 배경팝업
        //팝업이 가지는 이미지 넣기
        //위치 지정
        iTexture iTex = new iTexture(Resources.Load<Texture>("starBg"));

        iPopup pop = new iPopup();
        for (int i = 0; i < 3; i++)
        {
            iImage img = new iImage();
            img.add(iTex);
            pop.add(img);
            img.position = new iPoint(-iTex.tex.width / 2 * 3 - 10 + (iTex.tex.width + 10) * i,
                                      0);
        }

        //팝업의 스타일, 여닫는 위치, 델리게이트 함수 설정
        pop.style = iPopupStyle.zoom;
        pop.openPoint = new iPoint(MainCamera.devWidth / 2, 40);
        pop.closePoint = pop.openPoint;
        pop.methodOpen = openPopStar;
        popAll[3] = pop;
        popStarBg = pop;

        //별 팝업
        //팝업이 가지는 이미지 넣기
        //위치 지정
        iTex = new iTexture(Resources.Load<Texture>("star"));

        iPopup[] pops = new iPopup[3];
        for (int i = 0; i < 3; i++)
        {
            iImage img = new iImage();
            pops[i] = new iPopup();
            img.add(iTex);
            pops[i].add(img);
            img.position = new iPoint(0, 0);

            //팝업의 스타일, 여닫는 위치, 델리게이트 함수 설정
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
        //starNum => 0~3
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
        else if (resultScore >= 10000 && resultMoney >= 2000) return 3;
        else return 2;
    }

    void drawPopStar(float dt)
    {
        popStarBg.paint(dt);

        for (int i = 0; i < starNum; i++)
            popStar[i].paint(dt);
    }

}