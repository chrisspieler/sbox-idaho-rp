using System;

namespace IdahoRP.Api;

/// <summary>
/// Represents a uniquely identifiable record that may fall out of sync with a
/// persistent store.
/// </summary>
public interface IDbRecord<K>
{
	/// <summary>
	/// A unique identifier for this record.
	/// </summary>
	K Id { get; }
	/// <summary>
	/// Should be true if this record has been modified such that it no longer
	/// matches its persisted copy.
	/// </summary>
	bool IsDirty { get; }
}
