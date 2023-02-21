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
		Spotlight = new SpotLightEntity()
		{
			Enabled = false,
			DynamicShadows = true,
			Range = 1024f,
			Falloff = 1.0f,
			LinearAttenuation = 0.0f,
			QuadraticAttenuation = 1.0f,
			Color = Color.White,
			Brightness = 0.5f
		};
		Spotlight.SetParent( Entity );
		Spotlight.LocalPosition = Spotlight
			.LocalPosition
			.WithZ( 72f )
			.WithX( 15f );
	}

	public void Simulate()
	{
		var eyePitch = Entity.EyeRotation.Pitch();
		if ( eyePitch != float.NaN )
		{
			var currentAngles = Spotlight.Rotation.Angles();
			Spotlight.Rotation = Rotation.From( currentAngles.WithPitch( eyePitch ) ).Normal;
		}

		if ( Input.Pressed( InputButton.Flashlight ) )
		{
			Spotlight.Enabled = !Spotlight.Enabled;
		}
	}
}
