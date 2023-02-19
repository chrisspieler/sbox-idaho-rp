using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api
{
	public static class ClockManager 
	{
		[ConVar.Replicated( "sv_clock_timescale" )]
		public static float sv_clock_timescale { get; set; } = 20f;
		public static DateTime CurrentTime { get; set; } = new DateTime( 2253, 8, 20, 8, 26, 23 );

		[ConCmd.Server("print_time")]
		public static void PrintCurrentTime()
		{
			Log.Info( $"The simulation time is: {CurrentTime.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ")}" );
		}

		[Event.Tick.Server]
		public static void Tick()
		{
			var scaledDeltaTime = Time.Delta * sv_clock_timescale;
			CurrentTime += TimeSpan.FromSeconds( scaledDeltaTime );
		}
	}
}
