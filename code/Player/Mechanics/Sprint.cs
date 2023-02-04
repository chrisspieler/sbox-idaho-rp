using Sandbox;
using System;

namespace IdahoRP.Mechanics;

/// <summary>
/// The basic sprinting mechanic for players.
/// It shouldn't, though.
/// </summary>
public partial class SprintMechanic : PlayerControllerMechanic
{
	/// <summary>
	/// A cooldown applied whenever the player sprints until their stamina 
	/// dips below <c>SprintCooldownThreshold</c>. This is a longer cooldown
	/// intended to punish overexertion.
	/// </summary>
	public float SprintHardCooldownTime { get; set; } = 1.0f;
	/// <summary>
	/// A cooldown applied whenever the player stops sprinting and the hard
	/// cooldown would not apply. This is meant as a countermeasure against 
	/// strategies that involve fluttering the sprint key to sprint for free
	/// or reduced cost.
	/// </summary>
	public float SprintSoftCooldownTime { get; set; } = 0.1f;
	/// <summary>
	/// The stamina value below which ending a sprint results in an extra-penalizing
	/// cooldown period.
	/// </summary>
	public float SprintCooldownThreshold { get; set; } = 5.0f;
	/// <summary>
	/// The amount of stamina that once reached will immediately boot the player
	/// out of the sprinting state.
	/// </summary>
	public float MinSprintStamina { get; set; } = 0.0f;
	/// <summary>
	/// The number of stamina points per second that will be drained while sprinting.
	/// </summary>
	public float StaminaDrainPerSecond { get; set; } = 20.0f;
	/// <summary>
	/// The sprint speed in units per second under ideal conditions.
	/// </summary>
	public float SprintSpeed { get; set; } = 160f;
	/// <summary>
	/// Used in the <c>MaxSprintableAngle</c> calculation. Determines what portion of the player's 
	/// <c>MaxClimbAngle</c> is also sprintable.
	/// </summary>
	public float MaxSprintableAngleFactor { get; set; } = 0.3f;
	/// <summary>
	/// The angle of the most steep slope that a player may sprint on.
	/// </summary>
	public float MaxSprintableAngle => Player.GetStat( PlayerStat.MaxClimbAngle ) * MaxSprintableAngleFactor;
	/// <summary>
	/// Sprint has a higher priority than other mechanics.
	/// </summary>
	public override int SortOrder => 10;
	public override float? WishSpeed => SprintSpeed;

	private PlayerStatModifier _staminaDrainMod;
	private PlayerStatModifier _staminaRegenMod;

	protected override void OnStart()
	{
		if ( _staminaDrainMod == null )
			InitializeDrainMod();
		if ( _staminaRegenMod == null )
			InitializeRegenMod();
		// Prevent stamina from regenerating while sprinting is active.
		Player.AddBaseModifier( _staminaRegenMod );
		// Steadily drain stamina while sprinting is active.
		Player.AddTickModifier( _staminaDrainMod );
	}
	private void InitializeDrainMod()
	{
		_staminaDrainMod = PlayerStatModifier
			.Upon( PlayerStat.Stamina )
			.WithName( "Sprint Cost" )
			.WithDescription( "Running requires stamina." )
			.AsOffset( -StaminaDrainPerSecond );
	}

	private void InitializeRegenMod()
	{
		_staminaRegenMod = PlayerStatModifier
			.Upon( PlayerStat.StaminaRegen )
			.WithName( "Sprint Exhaustion" )
			.WithDescription( "You cannot recover stamina while sprinting." )
			.AsNullification();
	}

	protected override void OnStop()
	{
		if (Player.GetStat(PlayerStat.Stamina) < SprintCooldownThreshold)
		{
			TimeUntilCanStart = SprintHardCooldownTime;
		}
		else
		{
			TimeUntilCanStart = SprintSoftCooldownTime;
		}
		Player.RemoveBaseModifier( _staminaRegenMod );
		Player.RemoveTickModifier( _staminaDrainMod );
	}

	protected override bool ShouldStart()
	{
		if ( !TimeUntilCanStart ) return false;
		if ( !Input.Down( InputButton.Run ) ) return false;
		if ( Player.MoveInput.Length == 0 ) return false;
		if ( Player.Controller.CurrentGroundAngle > MaxSprintableAngle ) return false;
		if ( Player.GetStat(PlayerStat.Stamina) <= MinSprintStamina ) return false;
		return true;
	}
}
