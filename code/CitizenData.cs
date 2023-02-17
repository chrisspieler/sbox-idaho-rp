using IdahoRP.Api;
using IdahoRP.Repositories.FileStorage;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace IdahoRP;

public partial class CitizenData : BaseNetworkable, IDbRecord<long>
{
	[JsonRequired]
	[Net, Dirtyable] public string Name { get; set; }
	[JsonRequired]
	[Dirtyable] public ClothingContainer DefaultOutfit { get; set; }
	[JsonRequired]
	[Net, Dirtyable] public Job CurrentJob { get; set; }
	[JsonIgnore]
	[Dirtyable] public Gender Gender 
	{
		get => _gender;
		set
		{
			_gender = value;
			GenderId = value.ResourceId;
		}
	}
	private Gender _gender;
	[JsonRequired]
	public int GenderId { get; set; }

	public CitizenData()
	{

	}

	[JsonConstructor]
	public CitizenData(string name, ClothingContainer defaultOutfit, Job currentJob, int genderId, long id)
	{
		(Name, DefaultOutfit, CurrentJob, Id) = (name, defaultOutfit, currentJob, id);
		Gender = ResourceLibrary.GetAll<Gender>().FirstOrDefault( g => g.ResourceId == genderId );
		if (Gender == null )
		{
			Log.Error( $"No gender found for gender ID: {genderId}" );
		}
	}

	// IDbRecord
	public long Id { get; private set; }
	[JsonIgnore] public bool IsDirty { get; set; } = false;
	[JsonIgnore] public bool ShouldDelete { get; set; } = false;

	/// <summary>
	/// Given a Steam ID, returns the existing <c>CitizenData</c> for that player or bot, 
	/// or provides a randomized default if no data exists yet.
	/// </summary>
	/// <param name="steamId">The client of the player or bot whose data shall be retrieved.</param>
	public static CitizenData GetData(long steamId)
	{
		var citizenDb = DataManager.CitizenDb;
		if ( !citizenDb.Exists( steamId ) )
		{
			Log.Info( $"No existing CitizenData found for Steam ID {steamId}. Starting fresh." );
			CitizenData newData = GenerateRandom();
			newData.Id = steamId;
			citizenDb[steamId] = newData;
			return newData;
		}
		else
		{
			return citizenDb[steamId];
		}
	}

	[ConCmd.Admin("print_citizen_data")]
	public static void PrintCitizenData()
	{
		var citizenDb = DataManager.CitizenDb;
		Log.Info( $"Printing data for {citizenDb.Count} citizens." );
		foreach(var citizen in citizenDb.GetAll() )
		{
			Log.Info( $"SteamID: {citizen.Id}, Name: \"{citizen.Name}\", Gender: \"{citizen.Gender.Name}\", Job: \"{citizen.CurrentJob?.Title}\"" );
		}
	}

	public static CitizenData GenerateRandom()
	{
		return new CitizenData()
		{
			Name = RandomNameGenerator.GenerateRandomName(),
			DefaultOutfit = RandomOutfitGenerator.GenerateRandomOutfit(),
			CurrentJob = null,
			Gender = GenderPicker.GetNext()
		};
	}

	public static RandomChancer<Gender> GenderPicker
	{
		get
		{
			if ( _genderPicker == null )
				BuildRandomGenderList();
			return _genderPicker;
		}
	}

	private static RandomChancer<Gender> _genderPicker;

	[ConCmd.Server("genderlist")]
	private static void GetGenderList()
	{
		Log.Info( $"{GenderPicker.Count} genders loaded:" );
		foreach ( var gender in _genderPicker )
		{
			Log.Info( $"Gender: {gender.Item.Name}, Ratio: {gender.Ratio}" );
		}
	}

	private static void BuildRandomGenderList()
	{
		_genderPicker = new RandomChancer<Gender>();
		var allGenders = ResourceLibrary.GetAll<Gender>();
		foreach (var gender in allGenders)
		{
			_genderPicker.AddItem( gender, 100 / gender.RarityFactor );
		}
	}
}
