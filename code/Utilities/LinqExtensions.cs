using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP;

public static class LinqExtensions
{
	public static TSource Random<TSource>(this IEnumerable<TSource> source)
	{
		return source.OrderBy( _ => Guid.NewGuid() ).FirstOrDefault();
	}
}
