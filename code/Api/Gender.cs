using Sandbox;

namespace IdahoRP.Api;

[GameResource("Gender Definition", "gender", "Defines the name and pronouns associated with a gender.")]
public class Gender : GameResource
{
	/// <summary>
	/// Name of the gender (e.g., "male," "female," "nonbinary," etc.)
	/// </summary>
	public string Name { get; set; }
	/// <summary>
	/// A subject pronoun. Example: "He/she/they is/are an interesting person."
	/// </summary>
	public string SubjectPronoun { get; set; }
	/// <summary>
	/// Used to achieve achieve proper subject-verb agreement.
	/// </summary>
	public bool SubjectIsPlural { get; set; }
	/// <summary>
	/// An object pronoun. Example: "You'd probably like to meet him/her/them."
	/// </summary>
	public string ObjectPronoun { get; set; }
	/// <summary>
	/// A possessive adjective. Example: "It is his/her/their birthday."
	/// </summary>
	public string PosessiveAdjective { get; set; }
	/// <summary>
	/// A possessive pronoun. Example: "The pleasure is all his/hers/theirs."
	/// </summary>
	public string PosessivePronoun { get; set; }
	/// <summary>
	/// A reflexive pronoun. Example: "I'm afraid that [name] just blue himself/herself/themselves."
	/// </summary>
	public string ReflexivePronoun { get; set; }
	public float RarityFactor { get; set; } = 20f;
	
	public string SimplePronouns()
	{
		var sp = SubjectPronoun ?? "null";
		var op = ObjectPronoun ?? "null";
		return $"{sp}/{op}";
	}

	public string GetSubjectVerb(string singularVerb, string pluralVerb )
	{
		return SubjectIsPlural
			? $"{SubjectPronoun} {pluralVerb}"
			: $"{SubjectPronoun} {singularVerb}";
	}
}
