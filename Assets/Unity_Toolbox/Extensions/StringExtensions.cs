using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
	public static class StringExtensions
	{
		public static string FirstCharToUpper(this string input) =>
			input switch
			{
				null => throw new System.ArgumentNullException(nameof(input)),
				"" => throw new System.ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
				_ => input[0].ToString().ToUpper() + input.Substring(1)
			};
	}
}