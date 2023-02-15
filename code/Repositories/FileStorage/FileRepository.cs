using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Repositories.FileStorage;

internal class FileRepository
{
	public const string FILE_DATA_BASE_DIR = "db";

	public FileRepository(string repoSubdirectory)
	{
		if ( !FileSystem.Data.DirectoryExists( FILE_DATA_BASE_DIR ) )
		{
			Log.Info( $"Creating {nameof( FileStorage )} database." );
		}
		Log.Info( $"{nameof(FileStorage)} database path: {FileSystem.Data.GetFullPath(FILE_DATA_BASE_DIR)}" );
		var repoPath = $"{FILE_DATA_BASE_DIR}/{repoSubdirectory}";
		if ( !FileSystem.Data.DirectoryExists( repoPath ) )
		{
			FileSystem.Data.CreateDirectory( repoPath );
			Log.Info( $"Created repo subdirectory: {repoSubdirectory}" );
		}
	}
}
