using IdahoRP.UI;
using Sandbox;

namespace IdahoRP;

public partial class Idahoid
{
	public enum MessageLevel
	{
		Info,
		Error
	}

	private Hud _uiHudPage;

	[ClientRpc] 
	public void ShowHud()
	{
		_uiHudPage = new Hud();
	}

	[ClientRpc]
	public void UpdateTime(int hour, int minute )
	{
		_uiHudPage.MoneyPanel.CurrentHour = hour;
		_uiHudPage.MoneyPanel.CurrentMinute = minute;
	}

	[ClientRpc] 
	public void ShowModalMessage(string message ) 
		=> Log.Info( $"Modal message: {message}" );
	[ClientRpc] 
	public void ShowToastMessage( string message, MessageLevel level = MessageLevel.Info ) 
		=> Log.Trace( $"Toast message: {message}" );
}
