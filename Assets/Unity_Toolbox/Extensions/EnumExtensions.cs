using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class EnumUtils
{

	public static T[] GetValues<T>() where T : Enum
	{
		return (T[])System.Enum.GetValues(typeof(T));
	}
}
