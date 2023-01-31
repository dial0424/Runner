using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STD
{
	public struct iRect
	{
		public iPoint origin;
		public iSize size;

		public iRect(float x, float y, float width, float height)
		{
			origin = new iPoint(x, y);
			size = new iSize(width, height);
		}


		public bool containPoint(iPoint p)
		{
			return !(	p.x < origin.x ||
						p.x > origin.x + size.width ||
						p.y < origin.y ||
						p.y > origin.y + size.height);
		}

		public bool contianRect(iRect rt)
		{
			return !(	rt.origin.x + rt.size.width < origin.x ||
						rt.origin.x > origin.x + size.width ||
						rt.origin.y + rt.size.height < origin.y ||
						rt.origin.y > origin.y + size.height);
		}
	}
}

