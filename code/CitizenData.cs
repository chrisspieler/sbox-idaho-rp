using IdahoRP.Api;
using Sandbox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IdahoRP;

public partial class CitizenData : BaseNetworkable
{
	private static Dictionary<long, CitizenData> _citizenDatabase = new();
	[Net] public string Name { get; set; }
	public ClothingContainer DefaultOutfit { get; set; }
	[Net] public Job CurrentJob { get; set; }
	[Net] public Gender Gender { get; set; }
	
	/// <summary>
	/// Given a Steam ID, returns the existing <c>CitizenData</c> for that player or bot, 
	/// or provides a randomized default if no data exists yet.
	/// </summary>
	/// <param name="steamId">The client of the player or bot whose data shall be retrieved.</param>
	public static CitizenData GetData(long steamId)
	{
		if ( !_citizenDatabase.ContainsKey( steamId ) )
		{
			Log.Info( $"No existing CitizenData found for Steam ID {steamId}. Starting fresh." );
			CitizenData newData = GenerateRandom();
			_citizenDatabase[steamId] = newData;
			return newData;
		}
		else
		{
			return _citizenDatabase[steamId];
		}
	}

	[ConCmd.Admin("print_citizen_data")]
	public static void PrintCitizenData()
	{
		Log.Info( $"Printing data for {_citizenDatabase.Count} citizens." );
		foreach(KeyValuePair<long, CitizenData> kvp in _citizenDatabase )
		{
			long steamId = kvp.Key;
			CitizenData citizen = kvp.Value;
			Log.Info( $"SteamID: {steamId}, Name: \"{citizen.Name}\", Gender: \"{citizen.Gender.Name}\", Job: \"{citizen.CurrentJob?.Title}\"" );
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
