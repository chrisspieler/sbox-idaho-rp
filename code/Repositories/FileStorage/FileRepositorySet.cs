using IdahoRP.Api;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Repositories.FileStorage
{
	[Library]
	public class FileRepositorySet : IRepositorySet
	{
		public string Name => "File Storage";

		//public IRepository<CitizenData> _citizenDb => throw new NotImplementedException();
	}
}
