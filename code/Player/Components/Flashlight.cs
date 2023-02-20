using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Player.Components;

public partial class Flashlight : EntityComponent<Idahoid>
{
	[Net] public SpotLightEntity Spotlight { get; set; }

	protected override void OnActivate()
	{
		if ( !Game.IsServer )
			return;
		Log.Info( "Activated flashlight component :)" );
		Spotlight = new SpotLightEntity()
		{
			Enabled = false,
			DynamicShadows = true,
			Range = 1024f,
			Falloff = 1.0f,
			LinearAttenuation = 0.0f,
			QuadraticAttenuation = 1.0f,
			Color = Color.White
		};
		Spotlight.SetParent( Entity );
		Spotlight.LocalPosition = Spotlight
			.LocalPosition
			.WithZ( 72f )
			.WithX( 10f );
	}

	public void Simulate()
	{
		Spotlight.Rotation = Entity.EyeRotation;

		if ( Input.Pressed( InputButton.Flashlight ) )
		{
			Log.Info( "Pressed flashlight button" );
			Spotlight.Enabled = !Spotlight.Enabled;
		}
	}
}
