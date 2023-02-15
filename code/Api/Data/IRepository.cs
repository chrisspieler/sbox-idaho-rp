using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api.Data;

public interface IRepository<T> where T : IDbRecord
{
	T Get( Guid Id );
	IEnumerable<T> GetAll();
	IEnumerable<T> Where( Func<T, bool> predicate );
	void Add( T record );
	void Delete( T record );
	void Edit( T record );
}
