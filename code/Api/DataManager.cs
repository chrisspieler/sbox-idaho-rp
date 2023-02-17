﻿using Sandbox;
using IdahoRP.Repositories.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

public static class DataManager
{
	[Event.Entity.PostSpawn]
	private static void Initialize()
	{
		CitizenDb = new FileRepository<CitizenData, long>().ToCached();
	}
	[Event.Tick.Server]
	public static void Tick()
	{
		CitizenDb.Tick();
	}

	private static IRepositorySet _repos;

	public static RepositoryCache<CitizenData, long> CitizenDb { get; set; }
}