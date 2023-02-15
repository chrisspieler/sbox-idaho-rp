using System;

namespace IdahoRP.Api.Data;

public interface IDbRecord
{
	Guid Id { get; }
	bool IsDirty { get; }
}
