using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

public class DirtyChecker<T>
{
	private Dictionary<T, int> _lastHashCode = new();
	private List<PropertyDescription> _dirtyableProperties;

	public DirtyChecker()
	{
		var targetType = TypeLibrary.GetType<T>();
		_dirtyableProperties = targetType
			.Properties
			.Where( m => m.HasAttribute<DirtyableAttribute>() )
			.ToList();
	}

	public bool HasDirtied(T instance)
	{
		int hashCode = 0;
		foreach(var property in _dirtyableProperties )
		{
			hashCode ^= property.GetHashCode();
		}
		bool hasChanged;
		if (!_lastHashCode.ContainsKey(instance))
		{
			_lastHashCode[instance] = hashCode;
			// It is assumed that entirely new items are not dirty.
			return false;
		}
		hasChanged = _lastHashCode[instance] != hashCode;
		_lastHashCode[instance] = hashCode;
		return hasChanged;
	}
}
