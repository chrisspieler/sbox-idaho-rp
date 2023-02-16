namespace IdahoRP.Api;

public interface IDirtyable
{
	/// <summary>
	/// Should be true if this record has been modified such that it no longer
	/// matches its persisted copy.
	/// </summary>
	bool IsDirty { get; set; }
}
