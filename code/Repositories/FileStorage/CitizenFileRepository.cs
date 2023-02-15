using IdahoRP.Api.Data;
using System;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Repositories.FileStorage
{
	internal class CitizenFileRepository : FileRepository, IRepository<CitizenData>
	{
		public const string CITIZEN_DATA_DIR = "citizen";
		public CitizenFileRepository() : base(CITIZEN_DATA_DIR)
		{
		}

		public void Add( CitizenData record )
		{
			throw new NotImplementedException();
		}

		public void Delete( CitizenData record )
		{
			throw new NotImplementedException();
		}

		public void Edit( CitizenData record )
		{
			throw new NotImplementedException();
		}

		public CitizenData Get( Guid Id )
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CitizenData> GetAll()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CitizenData> Where( Func<CitizenData, bool> predicate )
		{
			throw new NotImplementedException();
		}
	}
}
