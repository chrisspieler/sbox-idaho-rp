using IdahoRP.Api;
using Sandbox;

namespace IdahoRP.Jobs;

[Library]
public partial class NEETJob : Job
{
	const string JOB_TITLE = "NEET";
	const string JOB_DESC = "NEET is an acronym that stands for \"Not employed, in education, or in training.\" It's essentially the default state of a human being. Will this be just a temporary stop on your journey to a fulfilling career, or will you dig your heels in and make being a NEET your identity? No judgements - this is a video game.";
	const string JOB_INTERNAL_NAME = "job_neet";

	public NEETJob() : base( JOB_TITLE, JOB_DESC, JOB_INTERNAL_NAME ) { }

	public override void OffboardPlayer( Idahoid player )
	{
		throw new System.NotImplementedException();
	}

	public override void OnboardPlayer( Idahoid player )
	{
		throw new System.NotImplementedException();
	}
}
