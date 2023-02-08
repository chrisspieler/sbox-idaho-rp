using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.Api;

public partial class CitizenData : BaseNetworkable
{
	[Net] public string Name { get; set; }
	public ClothingContainer DefaultOutfit { get; set; }
	public Job CurrentJob { get; set; }

	private static IEnumerable<Clothing> _allClothing;

	public static CitizenData GenerateRandom()
	{
		return new CitizenData()
		{
			Name = GenerateRandomName(),
			DefaultOutfit = GenerateRandomOutfit(),
			CurrentJob = null
		};
	}

	private static List<string> _nameFragments = new()
	{
		"a",
		"ag",
		"ban",
		"bor",
		"bumo",
		"cer",
		"crow",
		"dam",
		"dro",
		"e",
		"ech",
		"en",
		"eth",
		"fan",
		"geb",
		"gor",
		"gun",
		"hai",
		"hoor",
		"i",
		"ine",
		"ing",
		"jaim",
		"jo",
		"jong",
		"krap",
		"kur",
		"la",
		"li",
		"lu",
		"le",
		"lo",
		"man",
		"mort",
		"mu",
		"nen",
		"nin",
		"o",
		"orth",
		"pe",
		"po",
		"ran",
		"ros",
		"roth",
		"ser",
		"sta",
		"tan",
		"than",
		"thin",
		"thos",
		"u",
		"ung",
		"urt",
		"vai",
		"vo",
		"wo",
		"yu",
		"ze"
	};

	private static string GenerateRandomName()
	{
		var rng = new Random();

		var name = string.Empty;
		var fragCount = rng.Int( 2, 3 );
		for ( int i = 0; i < fragCount; i++ )
		{
			var randomFragIdx = rng.Int( 0, _nameFragments.Count - 1 );
			var randomFrag = _nameFragments[ randomFragIdx ];
			if (i == 0 )
			{
				var firstLetter = randomFrag.First();
				firstLetter = char.ToUpper( firstLetter );
				// Capitalize the first letter.
				randomFrag = firstLetter + randomFrag.Substring( 1, randomFrag.Length - 1 );
			}
			name += randomFrag;
		}
		return name;
	}

	private static ClothingContainer GenerateRandomOutfit()
	{
		if ( _allClothing == null )
			_allClothing = ResourceLibrary.GetAll<Clothing>();

		var outfit = new ClothingContainer();

		Clothing top = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Tops );
		outfit.Add( top );

		Clothing otherTop = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Tops && c.CanBeWornWith( top ) );
		if (otherTop != null)
			outfit.Add( otherTop );

		Clothing bottom = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Bottoms );
		outfit.Add( bottom );

		Clothing footwear = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Footwear );
		outfit.Add( footwear );

		Clothing skin = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Skin);
		outfit.Add( skin );

		Clothing facial = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Facial );
		outfit.Add( facial );

		Clothing hair = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Hair );
		outfit.Add( hair );

		return outfit;

		Clothing GetRandomClothing(Func<Clothing, bool> predicate )
		{
			return _allClothing
				.Where( predicate )
				.Random();
		}
	}
}
