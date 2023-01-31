using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STD
{
	public struct iPoint
	{
		public float x, y;

		public iPoint(float newX, float newY)
		{
			x = newX;
			y = newY;
		}

		// iPoint p = new iPoint(1, 2);
		// Vector3 v = p;
		public static implicit operator Vector3(iPoint p)
		{
			return new Vector3(p.x, p.y, 0f);
		}

		// Vector3 v = new Vector3(1, 2, 3);
		// iPoint p = (iPoint)v;
		public static explicit operator iPoint(Vector3 v)
		{
			return new iPoint(v.x, v.y);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public static bool operator ==(iPoint p0, iPoint p1)
		{
			return (p0.x == p1.x && p0.y == p1.y);
		}
		public static bool operator !=(iPoint p0, iPoint p1)
		{
			return (p0.x != p1.x || p0.y != p1.y);
		}

		public static iPoint operator +(iPoint p0, iPoint p1)
		{
			iPoint p;
			p.x = p0.x + p1.x;
			p.y = p0.y + p1.y;
			return p;
		}
		public static iPoint operator -(iPoint p0, iPoint p1)
		{
			iPoint p;
			p.x = p0.x - p1.x;
			p.y = p0.y - p1.y;
			return p;
		}
		public static iPoint operator *(iPoint p0, float s)
		{
			iPoint p;
			p.x = p0.x * s;
			p.y = p0.y * s;
			return p;
		}
		public static iPoint operator /(iPoint p0, float s)
		{
			iPoint p;
			p.x = p0.x / s;
			p.y = p0.y / s;
			return p;
		}

		public float getLength()
		{
			return Mathf.Sqrt(x * x + y * y);
		}

		public void setLength(float len)
		{
			float length = getLength();
			if( length > 0f )
			{
				length /= len;
				x /= length;
				y /= length;
			}
		}
	}
}