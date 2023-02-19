using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Internal;

namespace IdahoRP.Api
{
	public static class ClockManager 
	{
		[ConVar.Replicated( "sv_clock_timescale" )]
		public static float sv_clock_timescale { get; set; } = 20f;
		public static DateTime CurrentTime { get; set; } = new DateTime( 2153, 8, 20, 8, 26, 23 );
		private static DateTime _nextMinute;

		static ClockManager()
		{
			SetNextMinute();
		}

		private static void SetNextMinute()
		{
			_nextMinute = new DateTime(
				year: CurrentTime.Year,
				month: CurrentTime.Month,
				day: CurrentTime.Day,
				hour: CurrentTime.Hour,
				minute: CurrentTime.Minute,
				second: 0
				);
			_nextMinute = _nextMinute.AddMinutes( 1 );
		}

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
			if (CurrentTime >= _nextMinute )
			{
				Event.Run( ClockEvent.MinuteElapsed, CurrentTime.Hour, CurrentTime.Minute );
				SetNextMinute();
			}
		}
	}
}
