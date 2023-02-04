using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

public partial class JobManager
{
	/// <summary>
	/// Prints to the console a list of all of the available jobs in this session.
	/// </summary>
	[ConCmd.Server( "joblist" )]
	public static void GetJobList()
	{
		Log.Info( $"Caller: {ConsoleSystem.Caller}" );
		foreach ( var kvp in _instance._jobs )
		{
			var job = kvp.Value;
			var jobId = job.InternalName;
			var workerCount = _instance.GetWorkerCount( job );
			var capacity = _instance.GetJobCapacity( job );
			string strCapacity = capacity >= 0 ? capacity.ToString() : "Infinity";
			Log.Info( $"{jobId,-16}:{workerCount,2}/{strCapacity,2}" );
		}
	}

	[ConCmd.Server( "setjob" )]
	public static void TrySetJob( string jobId )
	{
		// Is the job ID invalid?
		if ( !IdExists( jobId ) ) return;
		var player = ConsoleSystem.Caller.Pawn as Idahoid;
		// Is there no player pawn for this caller?
		if ( player == null )
			return;
		var job = _instance._jobs[jobId];
		// Is the player already working this job?
		if ( player.CurrentJob == job )
		{
			player.ShowToastMessage(
				message: $"You are already working the job \"{job.Title}\"!",
				level: Idahoid.MessageLevel.Error
				);
			return;
		}
		// Is this job full?
		if ( _instance.IsMaxCapacity( jobId ) )
		{
			player.ShowToastMessage(
				message: $"Job \"{job.Title}\" is already at its max capacity.",
				level: Idahoid.MessageLevel.Error
				);
			return;
		}
		// Is the player otherwise ineligible to hold this job in particular?
		if ( !job.CheckRequirements( player, out string rejectionReason ) )
		{
			player.ShowToastMessage(
				message: $"You are not qualified for the position of \"{job.Title}\".\nReason: {rejectionReason}",
				level: Idahoid.MessageLevel.Error
				);
			return;
		}
		_instance.SetJob( job, player );
	}

	[ConCmd.Admin( "setjobcapacity" )]
	public static void SetJobCapacity( string jobIdentifier, int jobCapacity )
	{
		if ( !IdExists( jobIdentifier ) ) return;
		var job = _instance._jobs[jobIdentifier];
		_instance._jobCapacityOverrides[jobIdentifier] = jobCapacity;
		Log.Info( $"{ConsoleSystem.Caller} - Maximum worker capacity for job {job.InternalName} set to {jobCapacity}" );
	}

	[ConCmd.Admin( "getworkers" )]
	public static void GetWorkers( string jobIdentifier )
	{
		if ( !IdExists( jobIdentifier ) ) return;
		var job = _instance._jobs[jobIdentifier];
		var workers = _instance._workers[job];
		Log.Info( $"{jobIdentifier} worker count: {workers.Count}" );
		Log.Info( $"---Enumerating {jobIdentifier} workers---" );
		foreach ( var worker in workers )
		{
			Log.Info( worker.Name );
		}
		Log.Info( $"---End of list---" );
	}

	private static bool IdExists( string jobIdentifier )
	{
		if ( !_instance._jobs.Keys.Contains( jobIdentifier ) )
		{
			Log.Error( $"The specified job identifier {jobIdentifier} was not found." );
			return false;
		}
		return true;
	}
}
