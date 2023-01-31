using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STD
{
	public struct iSize
	{
		public float width, height;

		public iSize(float w, float h)
		{
			width = w;
			height = h;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public static bool operator ==(iSize s0, iSize s1)
		{
			return (s0.width == s1.width && s0.height == s1.height);
		}
		public static bool operator !=(iSize s0, iSize s1)
		{
			return (s0.width != s1.width || s0.height != s1.height);

		}

		public static iSize operator +(iSize s0, iSize s1)
		{
			iSize s;
			s.width = s0.width + s1.width;
			s.height = s0.height + s1.height;
			return s;
		}
		public static iSize operator -(iSize s0, iSize s1)
		{
			iSize s;
			s.width = s0.width - s1.width;
			s.height = s0.height - s1.height;
			return s;
		}
		public static iSize operator *(iSize s0, float s)
		{
			iSize r;
			r.width = s0.width * s;
			r.height = s0.height * s;
			return r;
		}
		public static iSize operator /(iSize s0, float s)
		{
			iSize r;
			r.width = s0.width / s;
			r.height = s0.height / s;
			return r;
		}
	}

}
