using Sandbox;
using System.Linq;

namespace IdahoRP;

public static class ClientExtensions
{
	public static int GetBotId(this IClient cl)
	{
		char[] botIdChars = cl.Name
			.Split( '_' )
			.First()
			.Skip( 3 )
			.ToArray();
		var botIdString = new string( botIdChars );
		return int.Parse( botIdString );
	}
}
