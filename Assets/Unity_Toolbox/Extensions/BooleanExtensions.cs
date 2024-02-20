using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Toolbox
{
	public static class BooleanExtensions
	{
		public static bool UnderCondition(this bool b, bool condition)
		{
			return b || !condition;
		}
	}

}
