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
	MaxClimbAngle
}

public partial class IdahoidStats : EntityComponent<Idahoid>, ISingletonComponent
{
	private Dictionary<PlayerStat, Action<float>> _statSetters;
	private Dictionary<PlayerStat, Func<float>> _statGetters;
	private Dictionary<PlayerStat, float> _baseStats = new()
	{
		{ PlayerStat.MaxClimbAngle, 33.0f }
	};
	private Dictionary<PlayerStat, List<PlayerStatModifier>> _activeModifiers = new()
	{
		{ PlayerStat.MaxClimbAngle, new List<PlayerStatModifier>() }
	};

	public float GetStat( PlayerStat stat )
	{
		return _statGetters[stat]();
	}
	private void SetStat( PlayerStat stat, float value )
	{
		_statSetters[stat]( value );
	}

	private WalkMechanic _walkMechanic;

	public void RecalculateStat(PlayerStat stat )
	{
		float adjustedValue = _baseStats[stat];
		foreach(var modifier in _activeModifiers[stat])
		{
			adjustedValue += modifier.Value;
		}
		SetStat( stat, adjustedValue );
	}

	public void AddModifier(PlayerStatModifier statMod, bool recalculateStat = true)
	{
		_activeModifiers[statMod.TargetStat].Add(statMod);
		if ( recalculateStat )
		{
			RecalculateStat( statMod.TargetStat );
		}
	}

	public void RemoveModifier(PlayerStatModifier statMod, bool recalculateStat = true)
	{
		_activeModifiers[statMod.TargetStat].Remove( statMod );
		if ( recalculateStat )
		{
			RecalculateStat( statMod.TargetStat );
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();

		_walkMechanic = Entity.Components.Get<WalkMechanic>();
		_statSetters = new()
		{
			{ PlayerStat.MaxClimbAngle, (p) => _walkMechanic.WalkSpeed = p }
		};
		_statGetters = new()
		{
			{ PlayerStat.MaxClimbAngle, () => _walkMechanic.WalkSpeed  }
		};
	}
}
