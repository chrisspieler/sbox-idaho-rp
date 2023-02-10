using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdahoRP.UI;

public static class WorldPanelTracker
{
	private static Dictionary<WorldPanel, Entity> _entityParents = new();
	private static Dictionary<WorldPanel, Vector3> _positionOffsets = new();


	public static void AddWorldPanel<T>( T panel, Entity parent, Vector3 positionOffset ) where T : WorldPanel
	{
		_entityParents[panel] = parent;
		_positionOffsets[panel] = positionOffset;
		UpdatePanel( panel, parent );
	}
	public static void DestroyWorldPanel( WorldPanel panel )
	{
		_entityParents.Remove( panel );
		_positionOffsets.Remove( panel );
		panel.Delete();
	}

	public static void DestroyWorldPanels( Entity parent )
	{
		IEnumerable<WorldPanel> panels = _entityParents
			.Where( kvp => kvp.Value == parent )
			.Select( kvp => kvp.Key);
		foreach (var panel in panels )
		{
			DestroyWorldPanel( panel );
		}
	}

	[Event.Client.Frame]
	public static void Update()
	{
		foreach ( var kvp in _entityParents )
		{
			WorldPanel panel = kvp.Key;
			Entity parent = kvp.Value;
			UpdatePanel( panel, parent );
		}
	}

	private static void UpdatePanel( WorldPanel panel, Entity parent )
	{
		var newPosition = parent.Transform.Position + _positionOffsets[panel];
		var localPawn = Game.LocalPawn as Idahoid;
		var newRotation = Rotation.LookAt( localPawn.EyeRotation.Backward, Vector3.Up );
		panel.Transform = parent.Transform
			.WithPosition( newPosition )
			.WithRotation( newRotation );
	}
}
