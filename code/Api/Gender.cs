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
	/// Used by <c>GetSubjectVerb()</c> to achieve achieve proper subject-verb agreement.
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
	
	/// <summary>
	/// Returns the subject and object pronouns in "pronouns in bio" format.
	/// For example: "she/her", "they/them", "he/him"
	/// </summary>
	/// <returns></returns>
	public string SimplePronouns()
	{
		var sp = SubjectPronoun ?? "null";
		var op = ObjectPronoun ?? "null";
		return $"{sp}/{op}";
	}

	/// <summary>
	/// Given the singular and verbal conjugation of a verb, returns a conjugation appropriate
	/// for the <c>SubjectIsPlural</c> value of this pronoun.
	/// </summary>
	/// <param name="singularVerb"></param>
	/// <param name="pluralVerb"></param>
	/// <returns></returns>
	public string GetSubjectVerb(string singularVerb, string pluralVerb )
	{
		return SubjectIsPlural
			? $"{pluralVerb}"
			: $"{singularVerb}";
	}
}
