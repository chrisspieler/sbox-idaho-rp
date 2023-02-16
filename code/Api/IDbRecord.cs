using System;

namespace IdahoRP.Api;

/// <summary>
/// Represents a uniquely identifiable record that may fall out of sync with a
/// persistent store.
/// </summary>
public interface IDbRecord<K> : IDirtyable
{
	/// <summary>
	/// A unique identifier for this record.
	/// </summary>
	K Id { get; }
	bool ShouldDelete { get; set; }
}
