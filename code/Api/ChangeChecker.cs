using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

public class ChangeChecker<T>
{
	public bool WriteNetworkDataOnChange { get; set; } = false;

	private Dictionary<T, int> _lastHashCode = new();
	private List<PropertyDescription> _watchedProperties;

	public ChangeChecker( bool updateNetwork = false)
	{
		WriteNetworkDataOnChange = updateNetwork;
		var targetType = TypeLibrary.GetType<T>();
		_watchedProperties = targetType
			.Properties
			.Where( m => m.HasAttribute<WatchAttribute>() )
			.ToList();
	}

	public bool HasChanged(T instance)
	{
		int hashCode = 0;
		foreach(var property in _watchedProperties )
		{
			hashCode = HashCode.Combine( hashCode, property.GetValue(instance)?.GetHashCode() );
		}
		bool hasChanged;
		if (!_lastHashCode.ContainsKey(instance))
		{
			_lastHashCode[instance] = hashCode;
			// Entirely new items are unchanged from their initial form, so we return false here.
			return false;
		}
		hasChanged = _lastHashCode[instance] != hashCode;
		_lastHashCode[instance] = hashCode;
		if ( hasChanged && WriteNetworkDataOnChange)
			(instance as BaseNetworkable).WriteNetworkData();
		return hasChanged;
	}
}
