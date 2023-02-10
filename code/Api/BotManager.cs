using IdahoRP.Bots;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

public static class BotManager
{
	private static Dictionary<int, CitizenData> _botCitizenData = new();
	private static Dictionary<int, CitizenBot> _bots = new();
	private static int _nextBotId = 0;

	[ConCmd.Admin( "irp_bot_add" )]
	public static void AddCitizenBotCmd()
	{
		AddCitizenBot( null );
	}

	public static CitizenBot AddCitizenBot()
	{
		return AddCitizenBot( null );
	}

	public static CitizenBot AddCitizenBot( CitizenData inputCitizenData = null )
	{
		var citizenData = inputCitizenData;
		citizenData ??= CitizenData.GenerateRandom();
		var botId = _nextBotId;
		_nextBotId++;
		_botCitizenData[botId] = citizenData;
		Log.Info( $"Creating citizen bot named: {citizenData.Name}" );
		Log.Info( $"{ConsoleSystem.Caller} - Created bot: {botId}" );
		var bot = new CitizenBot( $"Bot{botId}_{citizenData.Name}" );
		_bots[botId] = bot;
		bot.Client.SetInt( "bot_id", botId );

		return bot;
	}

	[ConCmd.Admin("irp_bot_delete")]
	public static void DeleteBot( int botId )
	{
		Log.Info( $"{ConsoleSystem.Caller} - Deleted bot: {botId}" );
		_botCitizenData.Remove( botId );
		var botInstance = _bots[botId];
		_bots.Remove( botId );
		botInstance.Client.Pawn.Delete();
		botInstance.Client.Delete();
	}

	[ConCmd.Admin("irp_bot_delete_all")]
	public static void DeleteAllBots()
	{
		Log.Info( $"{ConsoleSystem.Caller} - Deleted all bots." );
		foreach (var botId in _bots.Keys )
		{
			DeleteBot( botId );
		}
	}

	public static CitizenData GetCitizenData( int botId ) => _botCitizenData[botId];
}
