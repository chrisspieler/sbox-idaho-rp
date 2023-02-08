using Sandbox;
using System;
using System.Collections.Generic;
using IdahoRP.Mechanics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Internal;

namespace IdahoRP;

public enum PlayerStat
{
	Health,
	MaxHealth,
	Magic,
	MagicRegen,
	MaxMagic,
	Stamina,
	StaminaRegen,
	MaxStamina,
	MaxClimbAngle,
}

public partial class Idahoid
{

	private Dictionary<PlayerStat, float> _baseStats { get; set; }
	private Dictionary<PlayerStat, Action<float>> _statSetters { get; set; }
	private Dictionary<PlayerStat, Func<float>> _statGetters { get; set; }
	private Dictionary<PlayerStat, List<PlayerStatModifier>> _activeBaseModifiers { get; set; }
	private Dictionary<PlayerStat, List<PlayerStatModifier>> _activeTickModifiers { get; set; }
	private Dictionary<PlayerStat, PlayerStat> _maxMappings { get; set; }
	private Dictionary<PlayerStat, PlayerStat> _minMappings { get; set; }
	private Dictionary<PlayerStat, PlayerStat> _regenMappings { get; set; }

	private void InitializeStats()
	{
		_baseStats = new()
		{
			{ PlayerStat.Health, 100.0f },
			{ PlayerStat.MaxHealth, 100.0f },
			{ PlayerStat.Magic, 60.0f },
			{ PlayerStat.MaxMagic, 60.0f },
			{ PlayerStat.MagicRegen, 2.0f },
			{ PlayerStat.Stamina, 75.0f },
			{ PlayerStat.StaminaRegen, 10.0f },
			{ PlayerStat.MaxStamina, 75.0f },
			{ PlayerStat.MaxClimbAngle, 33.0f }
		};
		_statSetters = new()
		{
			{ PlayerStat.Health, p => Health = p },
			{ PlayerStat.MaxHealth, p => _maxHealth = p },
			{ PlayerStat.Magic, p => _magic = p },
			{ PlayerStat.MagicRegen, p => _magicRegen = p },
			{ PlayerStat.MaxMagic, p => _maxMagic = p },
			{ PlayerStat.Stamina, p => _stamina = p },
			{ PlayerStat.StaminaRegen, p => _staminaRegenRate = p },
			{ PlayerStat.MaxStamina, p => _maxStamina = p },
			{ PlayerStat.MaxClimbAngle, (p) => Controller.MaxGroundAngle = p }
		};
		_statGetters = new()
		{
			{ PlayerStat.Health, () => Health },
			{ PlayerStat.MaxHealth, () => _maxHealth },
			{ PlayerStat.Magic, () => _magic },
			{ PlayerStat.MagicRegen, () => _magicRegen },
			{ PlayerStat.MaxMagic, () => _maxMagic },
			{ PlayerStat.Stamina, () => _stamina },
			{ PlayerStat.StaminaRegen, () => _staminaRegenRate },
			{ PlayerStat.MaxStamina, () => _maxStamina },
			{ PlayerStat.MaxClimbAngle, () => Controller.MaxGroundAngle  }
		};
		_activeBaseModifiers = new();
		_activeTickModifiers = new();
		foreach(var statType in Enum.GetValues( typeof( PlayerStat ) )){
			PlayerStat stat = (PlayerStat)statType;
			_activeBaseModifiers[stat] = new List<PlayerStatModifier>();
			_activeTickModifiers[stat] = new List<PlayerStatModifier>();
		}
		_maxMappings = new()
		{
			{PlayerStat.Health, PlayerStat.MaxHealth },
			{PlayerStat.Magic, PlayerStat.MaxMagic },
			{PlayerStat.Stamina, PlayerStat.MaxStamina }
		};
		_minMappings = new()
		{

		};
		_regenMappings = new()
		{
			{PlayerStat.Stamina, PlayerStat.StaminaRegen },
			{PlayerStat.Magic, PlayerStat.MagicRegen }
		};
	}

	public float GetStat( PlayerStat stat )
	{
		return _statGetters[stat]();
	}

	public void SetStat( PlayerStat stat, float value )
	{
		_statSetters[stat]( value );
	}

	public void OffsetStat( PlayerStat stat, float offset ) => SetStat(stat, GetStat( stat ) + offset );

	public void TickRegen()
	{
		foreach(var kvp in _regenMappings )
		{
			// The stat that we are regenerating
			PlayerStat target = kvp.Key;
			// The stat that determines how much to regenerate by
			PlayerStat regen = kvp.Value;
			var initialValue = _statGetters[target]();
			var regenValue = _statGetters[regen]();
			var adjustedValue = initialValue + regenValue * Time.Delta;
			adjustedValue = ClampStat( target, adjustedValue );
			_statSetters[target]( adjustedValue );
		}
	}

	public void TickStatChanges()
	{
		// For each stat that exists...
		foreach(var kvp in _activeTickModifiers )
		{
			PlayerStat stat = kvp.Key;
			// ...get the list of active tick modifiers on that stat...
			List<PlayerStatModifier> statMods = kvp.Value;
			// ...and get the current value of the stat.
			float adjustedValue = _statGetters[stat]();
			// For each active tick modifier on the stat...
			foreach(var mod in statMods )
			{
				// ...offset the value of that stat by the value of the active tick modifier.
				adjustedValue += mod.GetResult( adjustedValue) * Time.Delta;
				adjustedValue = ClampStat( stat, adjustedValue );
				// Log.Info( $"{Client} - Stat \"{stat}\" adjusted value after applying TICK modifier \"{mod.Name}\" is: {adjustedValue}" );
			}

			// Write the adjusted value back to the stat only after ALL modifiers are applied.
			_statSetters[stat]( adjustedValue );
		}
	}

	/// <summary>
	/// Updates a stat by applying all of the active base modifiers on that stat to the base value
	/// of the stat.
	/// </summary>
	/// <param name="stat"></param>
	public void RecalculateStat(PlayerStat stat )
	{
		float adjustedValue = _baseStats[stat];
		foreach(var modifier in _activeBaseModifiers[stat])
		{
			adjustedValue += modifier.GetResult(adjustedValue);
			// Log.Info( $"{Client} - Stat \"{stat}\" adjusted value after applying BASE modifier \"{modifier.Name}\" is: {adjustedValue}" );
		}
		SetStat( stat, adjustedValue );
	}

	public void AddTickModifier( PlayerStatModifier statMod )
	{
		_activeTickModifiers[statMod.TargetStat].Add( statMod );
	}

	public void RemoveTickModifier(PlayerStatModifier statMod )
	{
		_activeTickModifiers[statMod.TargetStat].Remove( statMod );
	}

	public void AddBaseModifier(PlayerStatModifier statMod, bool recalculateStat = true)
	{
		_activeBaseModifiers[statMod.TargetStat].Add( statMod );
		// Log.Info( $"{Client} - Added base modifier \"{statMod.Name}\"." );
		if ( recalculateStat )
		{
			RecalculateStat( statMod.TargetStat );
		}
	}

	public void RemoveBaseModifier(PlayerStatModifier statMod, bool recalculateStat = true)
	{
		// If there's nothing to remove, just move on peacefully.
		if ( !_activeBaseModifiers[statMod.TargetStat].Contains( statMod ) )
		{
			// Log.Info( $"{Client} - Cannot remove base modifier \"{statMod.Name}\", as it does not exist!" );
			return;
		}

		_activeBaseModifiers[statMod.TargetStat].Remove( statMod );
		// Log.Info( $"{Client} - Removed base modifier \"{statMod.Name}\"." );
		if ( recalculateStat )
		{
			RecalculateStat( statMod.TargetStat );
		}
	}

	private float ClampStat(PlayerStat stat, float value )
	{
		float max = float.PositiveInfinity;
		if ( _maxMappings.ContainsKey( stat ) )
		{
			var maxTarget = _maxMappings[stat];
			max = _statGetters[maxTarget]();
		}
		float min = 0.0f;
		if ( _minMappings.ContainsKey( stat ) )
		{
			var minTarget = _minMappings[stat];
			min = _statGetters[minTarget]();
		}
		return Math.Clamp( value, min, max );
	}
}
