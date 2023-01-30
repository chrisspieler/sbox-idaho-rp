using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP;

public partial class Idahoid
{
	[ClientRpc] public void ShowModalMessage(string message ) => Log.Error( message );
	[ClientRpc] public void ShowToastMessage( string message ) => Log.Error( message );
}
