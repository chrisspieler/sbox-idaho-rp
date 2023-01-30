﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Utilities;

internal static class StringUtilities
{
	/// <summary>
	/// Returns true if all of the characters in the provided string <c>str</c> are
	/// letters, numbers, underscores, hyphens, or periods. Returns false if an
	/// invalid character is found.
	/// </summary>
	/// <param name="str">The string that will be scanned for invalid characters.</param>
	/// <returns></returns>
	public static bool IsConsoleFriendly(this string str )
	{
		if ( string.IsNullOrWhiteSpace( str ) )
			return false;
		foreach(char c in str )
		{
			if ( char.IsLetter( c ) || char.IsNumber( c ))
				continue;
			if ( new[] { '_', '-', '.' }.Contains( c ) )
				continue;
			return false;
		}
		return true;
	}
}
