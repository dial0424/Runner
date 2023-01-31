using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using STD;

public class Proc : nGUI
{
	public static Proc me = null;
	Hero hero;

	public override void load()
	{
		me = this;
		
		loadMap();

		hero = new Hero(new iPoint(80, 300), 300);

		loadState();
		loadPopPause();
		loadPopCount();
		loadHeroPopCtrlBtn();
		loadPopFever();

		loadItemEffect();

		popState.show(true);
		popHeroCtrlBtn.show(true);
		popFever.show(true);
	}

	public override void draw(float dt)
	{
		float _dt = dt;
		if (pauseGame())
			dt = 0f;

		drawMap(dt);

		hero.paint(dt);
		drawItemEffect(dt);		

		drawPopState(dt);
		drawPopHeroCtrlBtn(dt);
		drawPopFever(dt);

		drawPopPause(_dt);	//일시정지 상황에도 그려져야함
		drawPopCount(_dt);
	}

	public override void key(iKeystate stat, iPoint point)
	{
		if (keyPopPause(stat, point) ||
			keyPopCount(stat, point))
			return;
		if (pauseGame()) return;

		if (keyPopState(stat, point))
			return;

		if (keyPopHeroCtrlBtn(stat, point))
			return;

		keyPopFever(stat, point);
	}

	public override void keyboard(iKeystate stat, int key)
	{

	}

	// =============================================================
	// map
	// =============================================================

	//0 : 빈공간, 1 : 바닥, 2 : 장애물1, 3 : 장애물2, 4 : a, 5 : b, 6 : c
	int[][] listTile;

	public MapTile[] maps;
	public MapTile currMap;
	
	string[] feverStr;
	
	float currMapY, nextMapY;

	iStrTex stFever;
	iImage imgStFever;
	float feverDt;
	int language;

	void loadMap()
	{
		//0 : normal, 1 : fever
		listTile = new int[3][]
		{
			new int[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
			},
			new int[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			},
			new int[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			},
		};

		maps = new MapTile[2];

		int[] tiles;
		Texture[] texBg;
		Texture texFloor;

		tiles = listTile[2];
		texBg = new Texture[3];
		for (int i = 0; i < texBg.Length; i++)
			texBg[i] = Resources.Load<Texture>("bg2_" + i);
		texFloor = Resources.Load<Texture>("sand");

		maps[0] = new MapTile(true, tiles, 160, 12, 60, 60, 300, new float[] { 1.0f, 0.5f, 0.3f }, texBg, texFloor, 250);
		maps[0].methodMapArrived = setGameover;

		tiles = listTile[1];
		texBg = new Texture[2];
		for (int i = 0; i < texBg.Length; i++)
			texBg[i] = Resources.Load<Texture>("bg3_" + i);
		texFloor = Resources.Load<Texture>("cloud");

		maps[1] = new MapTile(false, tiles, 80, 12, 60, 60, 300, new float[] { 1.0f, 0.5f, 0.3f }, texBg, texFloor, MainCamera.devWidth / 3 * 2);
		maps[1].methodMapArrived = endFever;

		currMap = maps[0];
		currMapY = 0;
		nextMapY = 0;

		language = Loby.language;

		iStrTex st = new iStrTex();
		iImage img = new iImage();
		feverStr = new string[3];
		feverStr = new string[] { "피버타임!!!", "Fever time!!!", "Temps de fièvre!!!" };
		st.setStringName("ProcPopFont");
		st.setStringSize(60);
		st.setStringRGBA(1, 0, 1, 1);
		st.setString(feverStr[language]);
		stFever = st;
		feverDt = 0f;
		img.add(stFever.tex);
		img.alpha = 0f;
		img.position = new iPoint(MainCamera.devWidth / 2, MainCamera.devHeight / 2); //new iPoint((MainCamera.devWidth - st.tex.tex.width) / 2, (MainCamera.devHeight - st.tex.tex.height) / 2);
		img.anc = VCENTER | HCENTER;
		imgStFever = img;
	}

	void drawMap(float dt)
	{
		//prevMap.paint(dt, 0);

		float interval = MainCamera.devHeight;// 720
		float moveSpeed = interval / 1f;// 1초만에 이동
		bool move = false;
		float r;

		if (currMapY < nextMapY)// 피버모든 진입 상황
		{
			currMapY += moveSpeed * dt;
			r = currMapY / nextMapY;
			if (r < 0.4f)
			{
				imgStFever.alpha = Math.linear(r / 0.4f, 0, 1);
				imgStFever.scale = Math.linear(r / 0.4f, 0, 3);
			}
			else if (r < 1f)
			{
				imgStFever.scale = Math.linear((r - 0.4f) / 0.6f, 3, 1);
				imgStFever.position = Math.linear((r - 0.4f) / 0.6f, new iPoint(MainCamera.devWidth / 2, MainCamera.devHeight / 2), new iPoint(MainCamera.devWidth / 2, 90));
			}
			if (currMapY > nextMapY)
			{
				imgStFever.alpha = 0f;
				currMapY = nextMapY;

				currMap = maps[1];
				hero.rt.origin = currMap.heroPos;

				//popFever.show(true);
				popHeroCtrlBtn.show(true);
				popState.show(true);
			}
			move = true;
		}
		else if (currMapY > nextMapY)// 노멀모드 진입 상황
		{
			currMapY -= moveSpeed * dt;
			if (currMapY < nextMapY)
			{
				MainCamera.soundManager.playBgSound("Proc");
				currMapY = nextMapY;

				currMap = maps[0];
				hero.rt.origin = currMap.heroPos;

				popHeroCtrlBtn.show(true);
				popState.show(true);
			}
			move = true;
		}
		
		hero.mapChanging = move;

		if (move) 
		{
			float y = currMapY;
			if (nextMapY == 0)
				y = interval - currMapY;
			r = Mathf.Abs(y) / interval;// MainCamera.devHeight;
		}
		else 
			r = 0;

		for (int i = 0; i < 2; i++)
        {
			maps[i].paint((!move && currMap == maps[i]) ? dt : 0, currMapY - interval * i);
        }

		if(currMap == maps[1])
		{
			setBlendFunc(3);
			feverDt += dt;
			float m = 1 - Mathf.Abs(Math.fract(feverDt/3) - 0.5f) * 2;
			if (m < 0.4)
				m = 0;
			setMoz(m * 20);
			stFever.drawString(feverStr[language], MainCamera.devWidth/2, 60, TOP|HCENTER);
			setBlendFunc(0);
		}

		imgStFever.paint(dt);
	}

	// =============================================================
	// ui - jump / slide btn
	// =============================================================
	iPopup popHeroCtrlBtn;

	iImage[] imgHeroCtrlBtn;

	string[] strHeroCtrl;

	void loadHeroPopCtrlBtn()
	{
		iPopup pop = new iPopup();
		imgHeroCtrlBtn = new iImage[2];

		iImage img;
		Texture tex = null;

		for (int i = 0; i < imgHeroCtrlBtn.Length; i++)
		{
			img = new iImage();
			for (int j = 0; j < 2; j++)
			{
				iStrTex st = new iStrTex(methodCtrlBtn, 150, 150);
				st.setString(i + "\n" + j);
				img.add(st.tex);
				tex = st.tex.tex;
			}
			//슬라이드 점프 위치만 바꿔주면됨
			img.position = new iPoint((MainCamera.devWidth - tex.width) * (imgHeroCtrlBtn.Length - 1 - i), 0);
			imgHeroCtrlBtn[i] = img;
			pop.add(img);
		}

		pop.style = iPopupStyle.move;
		pop.openPoint = new iPoint(0, MainCamera.devHeight);
		pop.closePoint = new iPoint(0, MainCamera.devHeight - tex.height - 10);
		popHeroCtrlBtn = pop;

		strHeroCtrl = new string[2] { "SLIDE", "JUMP" };
	}

	void methodCtrlBtn(iStrTex st)
	{
		string[] strs = st.str.Split('\n');
		int index = int.Parse(strs[0]);
		int select = int.Parse(strs[1]);

		Texture tex = Resources.Load<Texture>("procbtn" + index);

		setRGBA(1, 1, 1, 1);
		drawImage(tex, 0, 0, TOP | LEFT);

		setStringSize(40);
		setStringName("ProcPopFont");
		if (select == 0)
			setStringRGBA(1, 1, 1, 1);
		else if (select == 1)
			setStringRGBA(0.5f, 0.5f, 1, 1);

		drawString(strHeroCtrl[index], tex.width / 2, tex.height / 2, VCENTER | HCENTER);
	}

	void drawPopHeroCtrlBtn(float dt)
	{
		if (popHeroCtrlBtn.state == iPopupState.proc)
		{
			for (int i = 0; i < imgHeroCtrlBtn.Length; i++)
			{
				imgHeroCtrlBtn[i].frame = (i == popHeroCtrlBtn.selected ? 1 : 0);
			}
		}

		popHeroCtrlBtn.paint(dt);
	}

	bool keyPopHeroCtrlBtn(iKeystate stat, iPoint point)
	{
		int i, j = -1;

		switch (stat)
		{
			case iKeystate.Began:
				for (i = 0; i < 2; i++)
				{
					if (imgHeroCtrlBtn[i].touchRect(popHeroCtrlBtn.closePoint, new iSize()).containPoint(point))
					{
						j = i;
						break;
					}
				}

				popHeroCtrlBtn.selected = j;
				if (j == -1) break;

				imgHeroCtrlBtn[j].setBtnState(0);

				if (j == 0)
					hero.slide();
				if (j == 1)
					hero.jump();

				MainCamera.soundManager.playCookieSound(j);

				return true;

			case iKeystate.Moved:
				for (i = 0; i < 2; i++)
				{
					if (imgHeroCtrlBtn[i].touchRect(popHeroCtrlBtn.closePoint, new iSize()).containPoint(point))
					{
						j = i;
						break;
					}
				}

				if (popHeroCtrlBtn.selected != j)
				{
					if (popHeroCtrlBtn.selected != -1)
					{
						imgHeroCtrlBtn[popHeroCtrlBtn.selected].btnState[0] = false;
						imgHeroCtrlBtn[popHeroCtrlBtn.selected].btnState[1] = false;
					}
					if (j != -1)
						imgHeroCtrlBtn[j].setBtnState(1);
					popHeroCtrlBtn.selected = j;
				}

				break;

			case iKeystate.Ended:
				i = popHeroCtrlBtn.selected;
				if (i == -1) break;

				imgHeroCtrlBtn[i].setBtnState(2);

				if (i == 0)
					hero.lie = false;

				break;
		}

		return false;
	}

	// =============================================================
	// ui - show state/score
	// =============================================================
	iPopup popState;
	iImage[] imgStateBtn;

	//Texture texHpBar;
	iImage imgHpBar;

	iStrTex[] stScoreMoney;
	public int score, money;
	public float life, _life;
	float damage;
	public float delay;

	void loadState()
	{
		iPopup pop = new iPopup();

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

		pop.style = iPopupStyle.move;
		pop.openPoint = new iPoint(0, -70);// -????
		pop.closePoint = new iPoint(0, 0);
		pop.methodPaintAfter = paintPopStateAfter;
		popState = pop;

		score = 0;
		money = 0;
		life = _life = maps[0].tileX / 10 * 3;//130 : 39  /  160 : 48
		damage = 0f;
		delay = 0f;
	}

	void methodStScoreMoney(iStrTex st)
	{
		setStringSize(40f);
		setStringRGBA(1, 1, 1, 1);
		drawString(st.str, new iPoint(200, 0), TOP | RIGHT);
	}

	void paintPopStateAfter(float dt, iPopup pop)
	{
		stScoreMoney[0].setString("" + score);
		stScoreMoney[1].setString("" + money);

		if (life <= 0)
		{
			hero.dead();

			delay += dt;
			if (delay >= 0.7f)
				setGameover();
		}
		else
		{
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

			if (damage > 0)
			{
				p.x += imgHpBar.tex.tex.width * r;
				r = damage / _life;
				setRGBA(1f, 0f, 0f, 1f);
				drawImage(imgHpBar.tex.tex, p, r, 1f, TOP | LEFT);
				damage -= dt * 5f;
				if(damage <= 0)
					damage = 0f;
			}
		}
	}

	public void loseLife(float dmg)
	{
		damage = dmg;
		life -= damage;
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
#if false
			gd.setValue(maps[0].arrived, score, money);
			
			string path = "save.sav";
			byte[] bytes = FileIO.struct2bytes(gd);

			FileIO.save(path, bytes);
#endif
			Result.result = maps[0].arrived;
			Result.resultScore = score;
			Result.resultMoney = money;
		}
	}

	// =============================================================
	// ui - pause
	// =============================================================
	iPopup popPause;
	iImage[] imgPopPause;

	void loadPopPause()
	{
		popPause = new iPopup();

		iPopup pop = new iPopup();
		iImage img;
		iTexture tex = null;

		
		imgPopPause = new iImage[3];
		for (int i = 0; i < 3; i++)
		{
			img = new iImage();
			for (int j = 0; j < 2; j++)
			{
				iStrTex st = new iStrTex(methodPopPause, 250, 96);
				st.setString(i + "\n" + j);
				img.add(st.tex);
				tex = st.tex;
			}
			img.position = new iPoint(0, (tex.tex.height + 20) * i);
			imgPopPause[i] = img;
			pop.add(img);
		}
		pop.style = iPopupStyle.zoom;
		pop.openPoint = new iPoint(MainCamera.devWidth / 2, MainCamera.devHeight / 2);
		pop.closePoint = new iPoint((MainCamera.devWidth - tex.tex.width) / 2,
									(MainCamera.devHeight - tex.tex.height * 3 - 40) / 2);
		pop.methodPaintBefore = paintPopPauseBefore;
		pop.methodClose = closePopPause;
		popPause = pop;
	}

	void methodPopPause(iStrTex st)
	{
		string[] s = st.str.Split('\n');
		int index = int.Parse(s[0]);
		int select = int.Parse(s[1]);

		string[][] str = new string[][] { new string[] { "계속하기", "다시하기", "그만하기" },
										  new string[] { "Continue", "Again", "Stop" },
										  new string[] { "Continue", "Refais-le", "Arrête" },};

		Texture tex = Resources.Load<Texture>("p_btn0");
		drawImage(tex, 0, 0, TOP | LEFT);

		setStringName("ProcPopFont");
		setStringSize(40);
		if (select == 0)
			setStringRGBA(0, 0, 0, 1);
		else
			setStringRGBA(0.7f, 0, 0.7f, 1);

		drawString(str[language][index], st.wid / 2, st.hei / 2, VCENTER | HCENTER);
	}

	void paintPopPauseBefore(float dt, iPopup pop)
	{
		float alpha = 1f;
		if (pop.state == iPopupState.open)
		{
			alpha = pop.aniDt / pop._aniDt;
		}
		else if (pop.state == iPopupState.close)
		{
			alpha = 1f - pop.aniDt / pop._aniDt;
		}
		setRGBA(0, 0, 0, alpha * 0.7f);

		fillRect(0, 0, MainCamera.devWidth, MainCamera.devHeight);
		setRGBA(1, 1, 1, 1);
	}

	void closePopPause(iPopup pop)
	{
		//popCount.show(true);
		showPopCount(true);
	}

	void drawPopPause(float dt)
	{
		popPause.paint(dt);
	}

	bool keyPopPause(iKeystate stat, iPoint point)
	{
		if (popPause == null) return false;
		if (popPause.bShow == false) return false;
		if (popPause.state != iPopupState.proc) return true;

		int i, j = -1;

		switch (stat)
		{
			//0 : began, 1: select, 2 : end
			case iKeystate.Began:
				for (i = 0; i < imgPopPause.Length; i++)
				{
					if (imgPopPause[i].touchRect(popPause.closePoint, new iSize(0, 0)).containPoint(point))
					{
						j = i;
						break;
					}
				}

				popPause.selected = j;
				
				if (j == -1)
					break;

				imgPopPause[j].setBtnState(0);
				MainCamera.soundManager.playTouchSound(1);

				break;

			case iKeystate.Moved:
				for (i = 0; i < imgPopPause.Length; i++)
				{
					if (imgPopPause[i].touchRect(popPause.closePoint, new iSize(0, 0)).containPoint(point))
					{
						j = i;
						break;
					}
				}

				if (j != popPause.selected)
				{
					if (popPause.selected != -1)
					{
						imgPopPause[popPause.selected].btnState[0] = false;
						imgPopPause[popPause.selected].btnState[1] = false;
					}
					if (j != -1)
						imgPopPause[j].setBtnState(1);
					popPause.selected = j;
				}

				break;

			case iKeystate.Ended:
				i = popPause.selected;
				if (i == -1) break;

				imgPopPause[i].setBtnState(2);

				if (i == 0)
				{
					popPause.show(false);
				}
				else if (i == 1)
				{
					setLoading("Proc");
				}
				else if (i == 2)
				{
					setGameover();
				}

				break;
		}

		return true;
	}

	bool pauseGame()
	{
		if (popPause.bShow || popCount.bShow)
			return true;
		return false;
	}

	// =============================================================
	// ui - popCount
	// =============================================================
	iPopup popCount;
	Texture[] texCount;
	float countDt;

	void loadPopCount()
	{
		iPopup pop = new iPopup();

		pop.methodPaintAfter = drawPopCountAfter;
		popCount = pop;

		texCount = new Texture[3];
		for (int i = 0; i < 3; i++)
			texCount[i] = Resources.Load<Texture>("count" + i);
		countDt = 0;
	}

	void showPopCount(bool show)
	{
		if (show)
		{
			countDt = 0;
		}

		popCount.show(show);
	}

	void drawPopCountAfter(float dt, iPopup pop)
	{
		//count 소리 어디 넣을지 고민해보깅
		float alpha = 1, scale = 1, degree = 0;

		float delta = countDt;
		int index = 0;
		while (delta > 1f)
		{
			delta -= 1f;
			index++;
		}
		Texture tex = texCount[index];

		if (delta < 0.2f)
		{
			float r = delta / 0.2f;
			scale = Math.linear(r, 3, 1);
			alpha = Math.linear(r, 0, 1);
			degree = 0;
		}
		else if (delta < 0.7f)
		{
			scale = 1;
			alpha = 1;
			degree = 0;
		}
		else// if(delta < 1.0f )
		{
			float r = (delta - 0.7f) / 0.3f;
			scale = Math.linear(r, 1, 0);
			alpha = 1;
			degree = 360 * r;
		}

		setRGBA(1, 1, 1, alpha);
		drawImage(tex, MainCamera.devWidth / 2, MainCamera.devHeight / 2,
			scale, scale, VCENTER | HCENTER, 2, degree, REVERSE_NONE);

		countDt += dt;
		if (countDt >= 3f)
		{
			//popCount.show(false);
			popCount.bShow = false;
			popCount.state = iPopupState.close;
			MainCamera.soundManager.playBgSound();
		}
	}

	void drawPopCount(float dt)
	{
		popCount.paint(dt);
	}

	bool keyPopCount(iKeystate stat, iPoint point)
	{
		if (popCount.bShow)
			return true;
		return false;
	}

	// =============================================================
	// ui - feverPop
	// =============================================================

	iPopup popFever;
	iImage[] imgFever;

	void loadPopFever()
	{
		popFever = new iPopup();
		imgFever = new iImage[3];

		iTexture tex = null;
		iImage img;
		iPopup pop = new iPopup();

		for (int i = 0; i < imgFever.Length; i++)
		{
			img = new iImage();
			tex = new iTexture(Resources.Load<Texture>("item" + (i + 3) + "_0"));
			img.add(tex);
			tex = new iTexture(Resources.Load<Texture>("item" + (i + 3)));
			img.add(tex);
			img.position = new iPoint(0, (tex.tex.height + 40) * i);
			imgFever[i] = img;
			pop.add(img);
		}

		pop.style = iPopupStyle.move;
		pop.openPoint = new iPoint(-tex.tex.width - 20, MainCamera.devHeight / 2 - (tex.tex.height / 2 * 3 + 40) - 40);
		pop.closePoint = new iPoint(10, MainCamera.devHeight / 2 - (tex.tex.height / 2 * 3 + 40) - 40);
		pop.methodDrawBefore = drawPopFeverBefore;
		popFever = pop;
	}

	void drawPopFeverBefore(float dt, iPopup pop, iPoint zero)
	{
		iRect rt = new iRect();
		rt.origin = new iPoint(zero.x - 10, zero.y - 20);
		Texture tex = pop.listImg[0].listTex[0].tex;
		rt.size = new iSize(tex.width + 20, (tex.height + 40) * 3);
		setRGBA(1, 1, 1, 0.7f);
		fillRect(rt);
	}

	void drawPopFever(float dt)
	{
		popFever.paint(dt);
	}

	public void setFever(int idx)
	{
		imgFever[idx].frame = 1;
		for (int i = 0; i < imgFever.Length; i++)
		{
			if (imgFever[i].frame == 0)
				return;
		}

		startFever();
	}

	void startFever()
	{
		Debug.Log("피버타임-!!시작");
		MainCamera.soundManager.playBgSound("Fever");
		MainCamera.soundManager.playCookieSound(3);

		currMap.heroPos = hero.rt.origin;

		maps[1].reset(true);

		nextMapY = MainCamera.devHeight;// 720; 

		popHeroCtrlBtn.show(false);
		popState.show(false);
	}

	void endFever()
	{
		Debug.Log("피버타임-!!끝");
		for (int i = 0; i < imgFever.Length; i++)
			imgFever[i].frame = 0;

		//currMap.heroPos = hero.rt.origin; 피버는 다시 불리면 초기화로 시작하니까, 백업 필요없음
		nextMapY = 0;

		popHeroCtrlBtn.show(false);
		popState.show(false);
	}
	bool keyPopFever(iKeystate stat, iPoint point)
	{
		switch (stat)
		{
			case iKeystate.Began:
				popFeverOpenClose(!popFever.bShow);

				break;

			case iKeystate.Moved:
				break;

			case iKeystate.Ended:
				break;
		}
		return false;
	}

	public void popFeverOpenClose(bool open)
	{
		if (open)
		{
			if (!popFever.bShow)
				popFever.show(true);
		}
		else if (popFever.bShow)
			popFever.show(false);
	}
	// =============================================================
	// ui - effect
	// =============================================================

	Texture[] texItemEffect;

	ItemEffect[][] _coin;// 4가지 * 100
	ItemEffect[][] _cookie;
	ItemEffect[][] _alphabetA;//4가지 * 5
	ItemEffect[][] _alphabetB;
	ItemEffect[][] _alphabetC;
	ItemEffect[][] _star;//4가지 * 100

	ItemEffect[] itEffects;// 960
	int itEffectNum;

	void loadItemEffect()
	{
		texItemEffect = new Texture[7];
		for (int i = 0; i < texItemEffect.Length; i++)
			texItemEffect[i] = Resources.Load<Texture>("item" + i);

		_coin = new ItemEffect[4][];
		for (int i = 0; i < _coin.Length; i++)
			_coin[i] = new ItemEffect[100];

		Texture tex = texItemEffect[0];
		for (int j = 0; j < 100; j++)
		{
			_coin[0][j] = new ItemDrop(tex);
			_coin[1][j] = new ItemFly(tex);
			_coin[2][j] = new ItemArrive(tex);
			_coin[3][j] = new ItemEat(tex);
		}

		_cookie = new ItemEffect[4][];
		for (int i = 0; i < _cookie.Length; i++)
			_cookie[i] = new ItemEffect[100];

		tex = texItemEffect[1];
		for (int j = 0; j < 100; j++)
		{
			_cookie[0][j] = new ItemDrop(tex);
			_cookie[1][j] = new ItemFly(tex);
			_cookie[2][j] = new ItemArrive(tex);
			_cookie[3][j] = new ItemEat(tex);
		}

		_alphabetA = new ItemEffect[4][];
		for (int i = 0; i < _alphabetA.Length; i++)
			_alphabetA[i] = new ItemEffect[5];

		tex = texItemEffect[3];
		for (int j = 0; j < 5; j++)
		{
			_alphabetA[0][j] = new ItemDrop(tex);
			_alphabetA[1][j] = new ItemFly(tex);
			_alphabetA[2][j] = new ItemArrive(tex);
			_alphabetA[3][j] = new ItemEat(tex);
		}

		_alphabetB = new ItemEffect[4][];
		for (int i = 0; i < _alphabetB.Length; i++)
			_alphabetB[i] = new ItemEffect[5];

		tex = texItemEffect[4];
		for (int j = 0; j < 5; j++)
		{
			_alphabetB[0][j] = new ItemDrop(tex);
			_alphabetB[1][j] = new ItemFly(tex);
			_alphabetB[2][j] = new ItemArrive(tex);
			_alphabetB[3][j] = new ItemEat(tex);
		}

		_alphabetC = new ItemEffect[4][];
		for (int i = 0; i < _alphabetC.Length; i++)
			_alphabetC[i] = new ItemEffect[5];

		tex = texItemEffect[5];
		for (int j = 0; j < 5; j++)
		{
			_alphabetC[0][j] = new ItemDrop(tex);
			_alphabetC[1][j] = new ItemFly(tex);
			_alphabetC[2][j] = new ItemArrive(tex);
			_alphabetC[3][j] = new ItemEat(tex);
		}

		_star = new ItemEffect[4][];
		for (int i = 0; i < _star.Length; i++)
			_star[i] = new ItemEffect[100];

		tex = texItemEffect[6];
		for (int j = 0; j < 100; j++)
		{
			_star[0][j] = new ItemDrop(tex);
			_star[1][j] = new ItemFly(tex);
			_star[2][j] = new ItemArrive(tex);
			_star[3][j] = new ItemEat(tex);
		}

		itEffects = new ItemEffect[960];
		itEffectNum = 0;
	}

	void drawItemEffect(float dt)
	{
		for (int i = 0; i < itEffectNum; i++)
		{
			if (itEffects[i].paint(dt))
			{
				itEffectNum--;
				itEffects[i] = itEffects[itEffectNum];
				i--;
			}
		}
	}

	public void addItemEffect(int index, int type, iPoint sp, int score)
	{
		if(type == 1 || type == 3)
			sp *= MainCamera.resolutionRate;

		for (int i = 0; i < 100; i++)
		{
			ItemEffect it;
			iPoint p;
			if (index == 0)
			{
				it = _cookie[type][i];
				p = new iPoint(1000, 10);
			}
			else if (index == 1)
			{
				it = _coin[type][i];
				p = new iPoint(1000, 70);
			}
			//나중에 abc 위치 찾아서 y값 바꾸기
			else if (index == 3)
			{
				it = _alphabetA[type][i];
				p = new iPoint(10, 205);
			}
			else if (index == 4)
			{
				it = _alphabetB[type][i];
				p = new iPoint(10, 295);
			}
			else if (index == 5)
			{
				it = _alphabetC[type][i];
				p = new iPoint(10, 385);
			}
			else
			{
				it = _star[type][i];
				p = new iPoint(1000, 10);
			}

			if (it.alive == false)
			{
				it.alive = true;

				it.tex = texItemEffect[index];
				it.num = score;
				it.sp = sp;
				it.ep = p;
				it.moveSpeed = 500;
				it.delta = 0f;
				it.index = index;
				it.off = Proc.me.currMap.off;

				itEffects[itEffectNum] = it;
				itEffectNum++;

				return;
			}
		}
	}
}

// 100
class ItemEffect
{
	public bool alive;

	public Texture tex;
	public int num;
	public iPoint sp, ep;
	public float moveSpeed;
	public float delta;
	public int index;   // 0 : cookie, 1 : coin, 3 : A, 4 : B, 5 : C, 6 : star
	public iPoint off;

	public ItemEffect(Texture t)
	{
		tex = t;
	}

	public virtual bool paint(float dt) { return false; }
}

class ItemDrop : ItemEffect
{
	public ItemDrop(Texture t) : base(t)
	{
	}

	public override bool paint(float dt)
	{
		// 0.5초만에 떨어진다.
		iPoint p;
		float a, s = 1f, degree = 0;
		if (delta < 0.5f)
		{
			p.x = sp.x;
			p.y = sp.y - 80 * Mathf.Sin(180 * delta / 0.5f * Mathf.Deg2Rad);
			a = 1.0f;
			if (delta < 0.25f)
				s = delta / 0.25f;
			Debug.Log(p.y);
		}
		// 2초간 딜레이(자동 먹거나, 사라진다)
		else
		{
			p = sp;
			a = Math.linear((delta - 0.5f) / 2.0f, 1, 0);
			degree = 360 * (delta - 0.5f) / 2.0f;
		}

		iGUI.instance.setRGBA(1, 1, 1, a);
#if true
		degree = 0;
		// issue  깊이 버퍼
#endif
		iGUI.instance.drawImage(tex, p.x, p.y, s, s, iGUI.TOP | iGUI.LEFT, 1, degree, iGUI.REVERSE_NONE);

		delta += dt;
		if (delta > 2.5f)
		{
			alive = false;
			Proc.me.addItemEffect(index, 1, sp, num);
			return true;
		}

		return false;
	}
}

class ItemFly : ItemEffect
{
	public ItemFly(Texture t) : base(t)
	{
	}

	public override bool paint(float dt)
	{
		// sp, ep 날아간다.(포물선)
		float r = delta / 0.5f;
		iPoint p = Math.linear(r, sp, ep);
		p.y += 300 * Mathf.Sin(180 * r * Mathf.Deg2Rad);

		float s = 0.7f;
		float degree = 0;

		iGUI.instance.setRGBA(1, 1, 1, 1);
		iGUI.instance.drawImage(tex, p.x, p.y, s, s, iGUI.TOP | iGUI.LEFT, 1, degree, iGUI.REVERSE_NONE);

		delta += dt;
		if (delta > 0.5f)
		{
			alive = false;
			if (index == 1)
				Proc.me.money += num;
			else
			{
				Proc.me.score += num;
				if (index > 2 && index < 6)
					Proc.me.setFever(index - 3);
			}

			Proc.me.addItemEffect(index, 2, ep, num);

			return true;
		}
		return false;
	}
}

class ItemArrive : ItemEffect
{
	public ItemArrive(Texture t) : base(t)
	{
	}

	public override bool paint(float dt)
	{
		// 큰게 생성되어서 원래 크기로 줄어든다.
		float s = 1f;
		float a = 1f;
		if (delta < 0.2f)
		{
			float r = delta / 0.2f;
			s = Math.linear(r, 1, 1.5f);
			a = Math.linear(r, 0.5f, 0);
		}
		else
		{
			float r = (delta - 0.2f) / 1.3f;
			s = Math.linear(r, 1.5f, 1);
			a = Math.linear(r, 0.5f, 0);
		}
		iPoint p = sp;
		float degree = 0f;

		iGUI.instance.setRGBA(1, 1, 1, a);
		iGUI.instance.drawImage(tex, p.x + tex.width / 2, p.y + tex.height / 2, s, s, iGUI.VCENTER | iGUI.HCENTER, 1, degree, iGUI.REVERSE_NONE);


		delta += dt;
		if (delta > 1.5f)
		{
			alive = false;
			return true;
		}

		return false;
	}
}

class ItemEat : ItemEffect
{
	public ItemEat(Texture t) : base(t)
	{

	}

	public override bool paint(float dt)
	{
		float a, s, _s = 0.6f, degree = 0f;
		iPoint p = sp;

		if (delta < 0.8f)
		{
			float r = delta / 0.8f;   
			a = Math.linear(r, 1f, 0f);
			s = Mathf.Cos(Math.linear(r, 0, 360 * Mathf.Deg2Rad)) * _s * MainCamera.resolutionRate;
			p = Math.linear(delta / 0.8f, sp, sp + (Proc.me.currMap.off - off) * MainCamera.resolutionRate);
		}
		else
		{
			a = 0f;
			s = _s;

			alive = false;
			return true;
		}

		delta += dt;

		iGUI.instance.setRGBA(1, 1, 1, a);
		iGUI.instance.drawImage(tex,
								p.x,
								p.y,
								s, _s,
								iGUI.TOP | iGUI.LEFT, 1, degree, iGUI.REVERSE_NONE);

		return false;
	}
}

public struct GameData
{
#if false
	int score;
	int money;
	string str;

	public GameData(int s = 0, int m = 0, string st = "")
	{
		score = s;
		money = m;
		str = st;
	}

	public void setValue(bool r, int s, int m)
	{
		if (r)
			str = "클리어!!";
		else
			str = "스테이지 실패,,";

		score = s;
		money = m;
	}

	public string[] value2string()
	{
		string[] s = new string[3];

		s[0] = str;
		s[1] = score + "";
		s[2] = money + "";

		return s;
	}
#else
	int langauge;
	public GameData(int l)
	{
		langauge = l;
	}
	public void setValue(int l)
	{
		langauge = l;
	}
	public int getValue()
	{
		return langauge;
	}
	public string[] value2string()
	{
		string[] s = new string[1];

		s[0] = langauge + "";

		return s;
	}
#endif
}
