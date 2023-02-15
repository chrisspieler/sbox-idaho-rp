using IdahoRP.Api;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdahoRP.Repositories.FileStorage
{
	/// <summary>
	/// A "database" that uses the Sbox FileSystem API. Each record is its own file and  
	/// each table is a folder.
	/// </summary>
	internal class FileRepository<T,K> : FileRepository, IRepository<T,K> where T : IDbRecord<K>
	{
		public FileRepository(string repoFolderName = null) 
			: base( repoFolderName ?? typeof( T ).Name ) { }

		private string GetRecordPath( T record ) => GetRecordPath( record.Id );
		private string GetRecordPath( K id ) => $"{FolderPath}/{id}";

		public void Write( T record )
		{
			string recordPath = GetRecordPath( record );
			FileSystem.Data.WriteJson(recordPath, record);
		}

		public void Delete( T record )
		{
			string recordPath = GetRecordPath( record );
			if ( !FileSystem.Data.FileExists( recordPath ) )
			{
				throw new InvalidOperationException( $"Attempted to delete {typeof( T ).Name} record {record}, but the record does not exist!" );
			}
			FileSystem.Data.DeleteFile( recordPath );
		}

		public int Count => FileSystem.Data.FindFile( FolderPath ).Count();

		public bool Exists(K id )
		{
			string recordPath = GetRecordPath( id );
			return FileSystem.Data.FileExists( recordPath );
		}

		public T Get( K id )
		{
			string recordPath = GetRecordPath( id );
			return FileSystem.Data.ReadJsonOrDefault<T>( recordPath, default );
		}

		public IEnumerable<T> GetAll()
		{
			IEnumerable<string> foundFiles = FileSystem.Data.FindFile( FolderPath );
			foreach( string file in foundFiles )
			{
				yield return FileSystem.Data.ReadJson<T>( file );
			}
		}

		public IEnumerable<T> Where( Func<T, bool> predicate )
		{
			// It's not possible for the "database engine" to filter results here
			// since we're just using the file system.
			return GetAll().Where(predicate);
		}
	}
}
