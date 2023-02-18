using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP;

public static partial class Commands
{
	[ConCmd.Admin("worldsound")]
	public static void PlaySound(string soundName, Vector3 position )
	{
		Sound.FromWorld( soundName, position );
	}
}
