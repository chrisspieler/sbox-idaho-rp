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
			player.ShowToastMessage( "You have left your previous job." );
			oldJob.OffboardPlayer( player );
		}
		_workers[job].Add( player );
		player.CurrentJob = job;
		player.ShowModalMessage( $"Your new job title is: {job.Title}" );
		player.CurrentJob.OnboardPlayer( player );
	}
	private void SetJob( string jobId, Idahoid player ) => SetJob( _jobs[jobId], player );

	public int GetWorkerCount( Job job ) => _workers[job].Count;
}
