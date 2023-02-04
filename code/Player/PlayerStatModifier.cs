using Sandbox;
using System;

namespace IdahoRP;

public enum StatModifierOperation
{
	Addition,
	Multiplication
}

[GameResource("Player Stat Modifier", "statmod", "Defines a modifier applied to player stats by items, genetics, or some other source.")]
public partial class PlayerStatModifier : GameResource
{
	/// <summary>
	/// A brief, player-facing name for the stat modifier.
	/// </summary>
	public string Name { get; set; }
	/// <summary>
	/// A player-facing description of the stat modifier.
	/// </summary>
	public string Description { get; set; }
	/// <summary>
	/// The stat that shall be modified by <c>Value</c>.
	/// </summary>
	public PlayerStat TargetStat { get; set; }
	/// <summary>
	/// The value of the operation that shall be applied to the player stat targeted by <c>TargetStat</c>.
	/// </summary>
	public float Value { get; set; }
	public StatModifierOperation Operation { get; set; }

	/// <summary>
	/// Creates a new instance of PlayerStatModifier that targets <c>targetStat</c>.
	/// </summary>
	/// <param name="targetStat">The stat upon which this modifier shall act.</param>
	public static PlayerStatModifier Upon(PlayerStat targetStat )
	{
		return new PlayerStatModifier()
		{
			TargetStat = targetStat
		};
	}

	/// <summary>
	/// Configures this instance of PlayerStatModifier to use the specified name.
	/// </summary>
	/// <param name="name">The name that shall be used by this PlayerStatModifier.</param>
	public PlayerStatModifier WithName(string name )
	{
		Name = name;
		return this;
	}

	/// <summary>
	/// Configures this instance of PlayerStatModifier to use the specified description.
	/// </summary>
	/// <param name="description">The description that shall be used by this PlayerStatModifier.</param>
	public PlayerStatModifier WithDescription(string description)
	{
		Description = description;
		return this;
	}

	/// <summary>
	/// Configures this instance of PlayerStatModifier to apply the specified offset as its operation.
	/// </summary>
	/// <param name="offset">The value that shall be applied to the target stat.</param>
	public PlayerStatModifier AsOffset(float offset )
	{
		Value = offset;
		Operation = StatModifierOperation.Addition;
		return this;
	}

	/// <summary>
	/// Configures this instance of PlayerStatModifier to apply the specified multiplier as its operation.
	/// </summary>
	/// <param name="multiplier">The multiplier that shall be applied to the target stat.</param>
	public PlayerStatModifier AsMultiplier(float multiplier )
	{
		Value = multiplier;
		Operation = StatModifierOperation.Multiplication;
		return this;
	}

	/// <summary>
	/// Configures this instance of PlayerStatModifier to negate (invert) the value of the target stat.
	/// </summary>
	public PlayerStatModifier AsNegation() => AsMultiplier( -1.0f );
	/// <summary>
	/// Configures this instance of PlayerStatModifier to zero out the value of the target stat.
	/// </summary>
	public PlayerStatModifier AsNullification() => AsMultiplier( 0.0f );

	/// <summary>
	/// Given an <c>initialValue</c>, returns a float equal to the offset that must be applied to <c>intialValue</c>
	/// for it to be properly affected by this modifier.
	/// </summary>
	/// <param name="initialValue">The value of <c>TargetStat</c> to which this modifier shall be applied.</param>
	/// <exception cref="InvalidOperationException">Thrown if <c>Operation</c> is set to an invalid value.</exception>
	public float GetResult(float initialValue )
	{
		return Operation switch
		{
			StatModifierOperation.Addition => (Value + initialValue) - initialValue,
			StatModifierOperation.Multiplication => (Value * initialValue) - initialValue,
			_ => throw new InvalidOperationException( "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo." )
		};
	}
}
