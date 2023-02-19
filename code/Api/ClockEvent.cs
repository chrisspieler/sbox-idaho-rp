using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

public static class ClockEvent
{
	public const string MinuteElapsed = "minute_elapsed";

	public class MinuteElapsedAttribute : EventAttribute
	{
		public MinuteElapsedAttribute() : base( MinuteElapsed ) { }
	}
}
