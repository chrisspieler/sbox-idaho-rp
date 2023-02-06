using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP;

public partial class CitizenBot : Bot
{
	public CitizenBot(string name) : base(name)
	{

	}

	public override void BuildInput()
	{
		Input.SetButton( InputButton.PrimaryAttack, true );
		var pawn = Client.Pawn as Idahoid;
		pawn.MoveInput = Vector3.Forward;
		pawn.LookInput = pawn.CalculateLookInput(new Angles( 0, 30 * Time.Delta, 0 ));
	}
}
