using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Bots
{
	public interface IBotAction
	{
		bool IsCompleted { get; }
		void Tick( CitizenBot bot );
	}
}
