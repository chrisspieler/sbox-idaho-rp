using IdahoRP.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP;

public static class RepositoryExtensions
{
	public static RepositoryCache<T,K> ToCached<T,K>(this IRepository<T,K> repo, bool updateNetwork = false)
		where T : IDbRecord<K>
	{
		return new RepositoryCache<T, K>( repo, updateNetwork );
	}
}
