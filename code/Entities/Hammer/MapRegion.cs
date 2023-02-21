using Editor;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Entities.Hammer;

[Library( "irp_mapregion" ), HammerEntity]
[Title( "Map Region" ), Category( "Idaho RP" ), Icon( "activity_zone" )]
public partial class MapRegion : BaseTrigger
{
	public MapRegion()
	{
		ActivationTags = new TagList( "player" );
	}

	[Property]
	[Net] public string RegionName { get; set; }

	public override void OnTouchStart( Entity toucher )
	{
		var pawn = (Idahoid)toucher;
		pawn.CurrentRegion = this;
		pawn.UpdateRegion( RegionName );
	}

	public override void OnTouchEnd( Entity toucher )
	{
		var pawn = (Idahoid)toucher;
		if (pawn.CurrentRegion == this)
		{
			pawn.CurrentRegion = null;
			pawn.UpdateRegion( null );
		}
	}
}
