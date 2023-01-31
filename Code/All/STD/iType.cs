using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STD
{
	public enum iKeystate
	{
		Began = 0,	// pressed
		Moved,		// moved
		Ended,		// released
		Double,
	};

	public delegate void MethodMouse(iKeystate stat, iPoint point);
	public delegate void MethodWheel(iPoint wheel);

	public enum iKeyboard
	{
		Left = 0,// a, A, 4, <-
		Right,
		Up,
		Down,
		Space
	};

	public delegate void MethodKeyboard(iKeystate stat, int key);

}

