using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace IdahoRP.Api;

public partial class RepositoryCache<T, K> : IRepository<T, K> where T : IDbRecord<K>
{
	public RepositoryCache(IRepository<T,K> dataSource)
	{
		_dataSource = dataSource;
	}

	private IRepository<T,K> _dataSource;
	private Dictionary<K, T> _cache = new();
	private ChangeChecker<T> _changeChecker = new ChangeChecker<T>();

	public void Tick()
	{
		UpdateIsDirty();
		HonorShouldDelete();
		WriteDirtyValues();
	}

	private void UpdateIsDirty()
	{
		foreach ( var item in _cache.Values )
		{
			if ( _changeChecker.HasChanged( item ) )
			{
				item.IsDirty = true;
			}
		}
	}

	private void HonorShouldDelete()
	{
		// Delete any item for which deletion has been requested.
		var toDelete = _cache.Values.Where( p => p.ShouldDelete );
		foreach ( var doomedItem in toDelete )
		{
			Delete( doomedItem );
		}
	}

	private void WriteDirtyValues()
	{
		// Write all dirty items to the database.
		foreach ( var item in _cache.Values )
		{
			if ( item.IsDirty )
			{
				_dataSource.Write( item );
				item.IsDirty = false;
			}
		}
	}

	public T this[K id] 
	{ 
		get => Get(id); 
		set => Write(value); 
	}

	public int Count => _dataSource.Count;

	public void Delete( T record )
	{
		_dataSource.Delete( record );
		if ( _cache.ContainsKey( record.Id ) )
		{
			_cache.Remove( record.Id );
		}
	}

	public bool Exists( K id )
	{
		if (_cache.ContainsKey(id))
		{
			return !_cache[id].ShouldDelete;
		}
		else
		{
			if ( _dataSource.Exists( id ) )
			{
				_cache[id] = _dataSource.Get( id );
				return true;
			}
			else
			{
				return false;
			}
		}

	}

	public T Get( K Id )
	{
		if ( _cache.ContainsKey( Id ) )
		{
			// The database is not the source of record, so the cache is returned as-is.
			return _cache[Id];
		}
		else
		{
			var item = _dataSource.Get( Id );
			if (item != null )
			{
				_cache[Id] = item;
			}
			return item;
		}
	}

	public IEnumerable<T> GetAll()
	{
		// The database will not contain any data that the cache does not contain.
		foreach(var item in _cache )
		{
			if ( item.Value.ShouldDelete )
				continue;
			yield return item.Value;
		}
	}

	public IEnumerable<T> Where( Func<T, bool> predicate )
	{
		return GetAll().Where( predicate );
	}

	public void Write( T record )
	{
		_cache[record.Id] = record;
		_dataSource.Write( record );
	}
}
