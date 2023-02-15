using IdahoRP.Api;
using Sandbox;
using System.Runtime.CompilerServices;

namespace IdahoRP.Repositories.FileStorage;

internal abstract class FileRepository
{
	public const string FILE_DATA_BASE_DIR = "db";

	public string FolderPath => $"{FILE_DATA_BASE_DIR}/{_repoSubdirectory}";
	private string _repoSubdirectory;
	

	public FileRepository(string repoSubdirectory)
	{
		_repoSubdirectory = repoSubdirectory;
		if ( !FileSystem.Data.DirectoryExists( FILE_DATA_BASE_DIR ) )
		{
			Log.Info( $"Creating {nameof( FileStorage )} database." );
		}
		Log.Info( $"{nameof(FileStorage)} database path: {FileSystem.Data.GetFullPath(FILE_DATA_BASE_DIR)}" );
		if ( !FileSystem.Data.DirectoryExists( FolderPath ) )
		{
			FileSystem.Data.CreateDirectory( FolderPath );
			Log.Info( $"Created repo subdirectory: {_repoSubdirectory}" );
		}
	}
}
