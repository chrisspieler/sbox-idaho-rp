using IdahoRP.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Jobs
{
	public partial class ClickerJob : Job
	{
		const string JOB_TITLE = "Dopamine Farmer";
		const string JOB_DESC = "Money is great, but you know what's better? Numbers that get larger when you click on stuff. Tickle your pink to produce the lootbox chemical. You won't get paid, but you'll feel great.";
		const string JOB_INTERNAL_NAME = "job_clicker";

		public ClickerJob() : base( JOB_TITLE, JOB_DESC, JOB_INTERNAL_NAME ) { }

		public override bool CheckRequirements( Idahoid player, out string reason )
		{
			reason = null;
			return true;
		}

		public override void OffboardPlayer( Idahoid player )
		{

		}

		public override void OnboardPlayer( Idahoid player )
		{

		}
	}
}
