using IdahoRP.Api;
using Sandbox;

namespace IdahoRP;

public partial class Idahoid
{
	[Net] public Job CurrentJob { get; set; }
	[Net] private float _maxHealth { get; set; } = 100f;
	[Net] private float _magic { get; set; } = 75f;
	[Net] private float _magicRegen { get; set; } = 2.0f;
	[Net] private float _maxMagic { get; set; } = 75f;
	[Net] private float _stamina { get; set; } = 60f;
	[Net] private float _maxStamina { get; set; } = 60f;
	[Net] private float _staminaRegenRate { get; set; } = 10f;

	bool TrySpendStat(PlayerStat stat, float amount )
	{
		var currentValue = GetStat( stat );
		if ( currentValue < amount )
			return false;
		OffsetStat( stat, -amount );
		return true;
	}

	public void GiveCash(float amount )
	{
		Data.PocketMoney += amount;
	}
}
