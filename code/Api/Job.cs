using IdahoRP.Utilities;
using Sandbox;
using Sandbox.Diagnostics;
using Sandbox.UI.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

/// <summary>
/// The base class for all jobs in Idaho RP.
/// </summary>
public abstract partial class Job : BaseNetworkable
{
	public Job(string title, string description, string internalName)
	{
		Title = title;
		Description = description;
		InternalName = internalName;
	}

	/// <summary>
	/// A player-facing job title.
	/// </summary>
	public string Title { get; set; } = null;
	/// <summary>
	/// A player-facing description of a job. This description should help the player decide whether this job is 
	/// right for them.
	/// </summary>
	public string Description { get; set; } = null;
	/// <summary>
	/// An internal name for this job that shall be used by console commands.
	/// </summary>
	public string InternalName
	{
		get => _internalName;
		set
		{
			Assert.True( value.IsConsoleFriendly(), "Job internal name either contained invalid characters or was empty.");
			_internalName = value;
		}
	}
	private string _internalName = null;
	/// <summary>
	/// The default limit on how many players may concurrently hold the same job in the same session.
	/// If the value is below zero, there is no limit. This is just a suggestion that may be overridden 
	/// by the JobManager.
	/// </summary>
	[Net] public int DefaultWorkerMax { get; set; } = -1;

	/// <summary>
	/// Returns true if the specified player is eligible to work this job, and false if not.
	/// </summary>
	/// <param name="player">The player whose eligibility for this job shall be assessed.</param>
	/// <param name="reason">If the specified player does not meet the requirements for this
	/// job, <c>reason</c> shall contain a player-facing message explaining why.</param>
	public abstract bool CheckRequirements( Idahoid player, out string reason );

	/// <summary>
	/// The procedure that shall be followed whenever a player is assigned this job. 
	/// </summary>
	/// <param name="player">The player who shall be assigned this job.</param>
	public abstract void OnboardPlayer( Idahoid player );

	/// <summary>
	/// The procedure that shall be followed whenever a player is no longer assigned this job.
	/// </summary>
	/// <param name="player">The player who shall no longer be assigned this job.</param>
	public abstract void OffboardPlayer( Idahoid player );


}
