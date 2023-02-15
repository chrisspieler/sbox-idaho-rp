using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

public class RepositoryCache<T, K> : IRepository<T, K> where T : IDbRecord<K>
{
	public RepositoryCache(IRepository<T,K> dataSource)
	{
		_dataSource = dataSource;
	}

	private IRepository<T,K> _dataSource;
	private Dictionary<K, T> _cache = new();

	public T this[K id] 
	{ 
		get => throw new NotImplementedException(); 
		set => throw new NotImplementedException(); 
	}

	public int Count => throw new NotImplementedException();

	public void Delete( T record )
	{
		throw new NotImplementedException();
	}

	public bool Exists( K id )
	{
		throw new NotImplementedException();
	}

	public T Get( K Id )
	{
		throw new NotImplementedException();
	}

	public IEnumerable<T> GetAll()
	{
		throw new NotImplementedException();
	}

	public IEnumerable<T> Where( Func<T, bool> predicate )
	{
		throw new NotImplementedException();
	}

	public void Write( T record )
	{
		throw new NotImplementedException();
	}
}
