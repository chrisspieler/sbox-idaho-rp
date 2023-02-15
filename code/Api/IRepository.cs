﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

/// <summary>
/// Represents a database that may be used to store records of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of record to be stored.</typeparam>
/// <typeparam name="K">The type of the primary key of records stored in this database.</typeparam>
public interface IRepository<T,K> where T : IDbRecord<K>
{
	/// <summary>
	/// Returns the record with the specified <paramref name="Id"/>, or <c>null</c>
	/// if no such record exists.
	/// </summary>
	/// <param name="Id">The <c>Id</c> of the record that may be returned.</param>
	T Get( K Id );
	/// <summary>
	/// Returns all records of type <typeparamref name="T"/>.
	/// </summary>
	IEnumerable<T> GetAll();
	/// <summary>
	/// Returns all records of type <typeparamref name="T"/> for which the
	/// evaluation of <paramref name="predicate"/> is true.
	/// </summary>
	/// <param name="predicate">The filter used to choose which records are returned.
	/// </param>
	IEnumerable<T> Where( Func<T, bool> predicate );
	/// <summary>
	/// Adds the specified <paramref name="record"/>, or overwrites the record if
	/// it already exists.
	/// </summary>
	/// <param name="record">The record that shall be added or modified.</param>
	void Write( T record );
	/// <summary>
	/// Deletes the specified <paramref name="record"/>.
	/// </summary>
	/// <param name="record">The record that sahll be deleted.</param>
	void Delete( T record );
	bool Exists( K id );
	int Count { get; }
	T this[K id]
	{
		get => Get( id );
		set => Write( value );
	}
}
