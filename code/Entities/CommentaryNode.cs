using Editor;
using Sandbox;
using System.Collections.Generic;

namespace IdahoRP.Entities;

[Library("ent_commentary"), HammerEntity]
[Title("Commentary node"), Category(""), Icon("place")]
public partial class CommentaryNode : ModelEntity
{
	public class CommentaryStep
	{
		public string Caption { get; set; }
		public SoundEvent Audio { get; set; }
		public List<string> OnBeginCmd { get; set; }
		public List<string> OnEndCmd { get; set; }
	}
	public List<CommentaryStep> lines { get; set; }
}
