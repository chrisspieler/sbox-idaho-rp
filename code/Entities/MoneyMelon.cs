using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Entities
{
	public partial class MoneyMelon : ModelEntity, IUse
	{
		public float BaseValue { get; set; }

		public override void Spawn()
		{
			SetModel( "models/sbox_props/watermelon/watermelon.vmdl_c" );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			Tags.Add( "money" );
		}

		public bool IsUsable( Entity user )
		{
			var pawn = user as Idahoid;
			if ( pawn == null )
			{
				return false;
			}
			return true;
		}

		public bool OnUse( Entity user )
		{
			((Idahoid)user).GiveCash( BaseValue, "Money Melon" );
			Delete();
			return false;
		}
	}
}
