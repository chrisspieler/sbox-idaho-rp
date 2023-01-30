﻿using Sandbox;
using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

public partial class JobManager
{
	/// <summary>
	/// Admin-defined overrides to default job capacities.
	/// </summary>
	private Dictionary<string, int> _jobCapacityOverrides { get; set; } = new();
	/// <summary>
	/// Allows for a job to be easily looked up by its identifier.
	/// </summary>
	private Dictionary<string, Job> _jobs { get; set; } = new();
	/// <summary>
	/// Tracks the current workers for each job type in this session.
	/// </summary>
	private Dictionary<Job, List<Idahoid>> _workers = new();

	private static JobManager _instance;


	public JobManager()
	{
		_instance = this;
		Initialize();
	}

	public void Initialize()
	{
		IEnumerable<TypeDescription> jobTypes = TypeLibrary.GetTypes<Job>()
			.Where( p => !p.IsAbstract );
		foreach ( var jobType in jobTypes )
		{
			// [POLITICIAN NAME HERE] wishes they could do this! Ha ha!
			// [OTHER POLITICAN NAME HERE] for [PUBLIC OFFICE] [NEAR FUTURE YEAR]!
			var jobInstance = jobType.Create<Job>();
			var jobIdentifier = jobInstance.InternalName;
			// To look up jobs by their identifiers.
			_jobs[jobIdentifier] = jobInstance;
			// The source of record for all employee/job assignments.
			_workers[jobInstance] = new List<Idahoid>();
		}
	}

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

	/// <summary>
	/// Returns current maximum worker capacity of a given <c>Job</c>.
	/// </summary>
	/// <param name="job"></param>
	/// <returns></returns>
	public int GetJobCapacity( Job job )
	{
		if ( _jobCapacityOverrides.Keys.Contains( job.InternalName ) )
			return _jobCapacityOverrides[job.InternalName];
		else
			return job.DefaultWorkerMax;
	}

	public bool IsMaxCapacity( Job job )
	{
		var capacity = GetJobCapacity( job );
		var workerCount = GetWorkerCount( job );
		return capacity >= 0 && workerCount >= capacity;
	}

	public bool IsMaxCapacity( string jobId ) => IsMaxCapacity( _jobs[jobId] );

	private void SetJob( Job job, Idahoid player )
	{
		if (player.CurrentJob != null )
		{
			var oldJob = player.CurrentJob;
			_workers[oldJob].Remove( player );
		}
		_workers[job].Add( player );
		player.CurrentJob = job;
	}
	private void SetJob( string jobId, Idahoid player ) => SetJob( _jobs[jobId], player );

	public int GetWorkerCount( Job job ) => _workers[job].Count;

	[ConCmd.Admin("setjobcapacity")]
	public static void SetJobCapacity(string jobIdentifier, int jobCapacity )
	{
		if ( !IdExists( jobIdentifier ) ) return;
		var job = _instance._jobs[jobIdentifier];
		_instance._jobCapacityOverrides[jobIdentifier] = jobCapacity;
		Log.Info( $"{ConsoleSystem.Caller} - Maximum worker capacity for job {job.InternalName} set to {jobCapacity}" );
	}

	[ConCmd.Server("setjob")]
	public static void TrySetJob(string jobId )
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
			player.ShowToastMessage( $"You are already working the job \"{job.Title}\"!" );
			return;
		}
		// Is this job full?
		if ( _instance.IsMaxCapacity(jobId) )
		{
			player.ShowToastMessage( $"Job \"{job.Title}\" is already at its max capacity." );
			return;
		}
		// Is the player otherwise ineligible to hold this job in particular?
		if ( !job.CheckRequirements( player, out string rejectionReason ) )
		{
			player.ShowToastMessage( $"You are not qualified for the position of \"{job.Title}\".\nReason: {rejectionReason}" );
			return;
		}
		_instance.SetJob(job, player);
	}

	[ConCmd.Admin("getworkers")]
	public static void GetWorkers(string jobIdentifier )
	{
		if ( !IdExists( jobIdentifier ) ) return;
		var job = _instance._jobs[jobIdentifier];
		var workers = _instance._workers[job];
		Log.Info( $"{jobIdentifier} worker count: {workers.Count}" );
		Log.Info( $"---Enumerating {jobIdentifier} workers---" );
		foreach(var worker in workers )
		{
			Log.Info( worker.Name );
		}
		Log.Info( $"---End of list---" );
	}

	private static bool IdExists(string jobIdentifier)
	{
		if ( !_instance._jobs.Keys.Contains( jobIdentifier ) )
		{
			Log.Error( $"The specified job identifier {jobIdentifier} was not found." );
			return false;
		}
		return true;
	}
}
