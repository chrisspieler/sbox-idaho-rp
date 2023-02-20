using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.UI;

public class Modal : Panel
{
	public event EventHandler OnOpen;
	public event EventHandler OnClose;

	public void Open()
	{
		SetClass( "visible", true );
		OnOpen?.Invoke( this, null );
	}

	public void Close()
	{
		SetClass( "visible", false );
		OnClose?.Invoke( this, null );
	}
}
