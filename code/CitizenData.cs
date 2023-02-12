using IdahoRP.Api;
using Sandbox;

namespace IdahoRP;

public partial class CitizenData : BaseNetworkable
{
	[Net] public string Name { get; set; }
	public ClothingContainer DefaultOutfit { get; set; }
	public Job CurrentJob { get; set; }
	public Gender Gender { get; set; }

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
		var genders = ResourceLibrary.GetAll<Gender>();
		foreach (var gender in genders)
		{
			_genderPicker.AddItem( gender, 100 / gender.RarityFactor );
		}
	}
}
