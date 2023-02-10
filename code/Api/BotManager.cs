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
	private static int _nextBotId = 0;

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
		var bot = new CitizenBot( $"Bot{botId}_{citizenData.Name}" );
		bot.Client.SetInt( "bot_id", botId );

		return bot;
	}

	public static CitizenData GetCitizenData( int botId ) => _botCitizenData[botId];
}
