namespace IdahoRP;

public static class CollisionTags
{
	/// <summary>
	/// Never collides with anything.
	/// </summary>
	public const string NotSolid = "notsolid";
	/// <summary>
	/// Everything that is solid.
	/// </summary>
	public const string Solid = "solid";
	/// <summary>
	/// Trigger that isn't collideable but can still send touch events.
	/// </summary>
	public const string Trigger = "trigger";
	/// <summary>
	/// A ladder.
	/// </summary>
	public const string Ladder = "ladder";
	/// <summary>
	/// Water pool.
	/// </summary>
	public const string Water = "water";
	/// <summary>
	/// Never collides with anything except solid and other debris.
	/// </summary>
	public const string Debris = "debris";
	/// <summary>
	/// Just like debris, but also sends touch events to players.
	/// </summary>
	public const string Interactable = "interactable";
	/// <summary>
	/// This is a player.
	/// </summary>
	public const string Player = "player";
	/// <summary>
	/// A fired projectile.
	/// </summary>
	public const string Projectile = "projectile";
	/// <summary>
	/// This is a weapon players can interact with.
	/// </summary>
	public const string Weapon = "weapon";
	/// <summary>
	/// Driveable vehicle.
	/// </summary>
	public const string Vehicle = "vehicle";
	/// <summary>
	/// Physics prop, collideable by player movement by default.
	/// </summary>
	public const string Prop = "prop";
	/// <summary>
	/// A non playable entity.
	/// </summary>
	public const string NPC = "npc";

	public const string Clip = "clip";
	public const string PlayerClip = "playerclip";
	public const string BulletClip = "bulletclip";
	public const string ProjectileClip = "projectileclip";
	public const string NPCClip = "npcclip";
}
