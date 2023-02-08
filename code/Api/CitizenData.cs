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
		"ack",
		"ad",
		"ag",
		"aho",
		"al",
		"am",
		"an",
		"and",
		"ant",
		"ap",
		"ar",
		"arm",
		"ash",
		"at",
		"ban",
		"bar",
		"be",
		"beet",
		"ber",
		"bo",
		"bob",
		"boi",
		"bor",
		"bot",
		"boy",
		"bro",
		"butt",
		"ca",
		"can",
		"cano",
		"cer",
		"ch",
		"co",
		"con",
		"cor",
		"cud",
		"cum",
		"cur",
		"da",
		"dah",
		"dam",
		"der",
		"des",
		"do",
		"dom",
		"dor",
		"dry",
		"dum",
		"dump",
		"e",
		"el",
		"en",
		"eth",
		"er",
		"ex",
		"fad",
		"fan",
		"fast",
		"fat",
		"field",
		"fish",
		"fin",
		"fec",
		"fem",
		"free",
		"fry",
		"fur",
		"gan",
		"gape",
		"gar",
		"gay",
		"gee",
		"gem",
		"ger",
		"gi",
		"go",
		"goat",
		"god",
		"gold",
		"gun",
		"gus",
		"guy",
		"ha",
		"hit",
		"ho",
		"hum",
		"i",
		"ica",
		"id",
		"ida",
		"in",
		"ine",
		"ion",
		"is",
		"it",
		"ius",
		"job",
		"kar",
		"la",
		"les",
		"lin",
		"lingus",
		"lu",
		"ler",
		"lo",
		"load",
		"man",
		"me",
		"meat",
		"mel",
		"mer",
		"mid",
		"mo",
		"mod",
		"mogus",
		"mor",
		"morb",
		"mon",
		"mu",
		"mur",
		"n",
		"na",
		"ne",
		"ner",
		"nin",
		"nis",
		"no",
		"nu",
		"nut",
		"o",
		"od",
		"of",
		"off",
		"oid",
		"old",
		"on",
		"or",
		"os",
		"pe",
		"pi",
		"po",
		"pud",
		"ra",
		"ran",
		"red",
		"ri",
		"ros",
		"roth",
		"s",
		"sa",
		"san",
		"se",
		"sec",
		"sex",
		"sh",
		"sha",
		"sil",
		"so",
		"sos",
		"son",
		"sta",
		"suc",
		"sus",
		"ta",
		"tan",
		"tato",
		"th",
		"tit",
		"to",
		"tom",
		"tot",
		"u",
		"uck",
		"un",
		"und",
		"unt",
		"urn",
		"ver",
		"vo",
		"vol",
		"wad",
		"yu",
		"za",
		"zi"
	};

	[ConCmd.Server("debug_name_gen")]
	private static void NameGenTest()
	{
		for ( int i = 0; i < 50; i++ )
		{
			Log.Info( GenerateRandomName() );
		}
	}

	public static string GenerateRandomName()
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

	public static ClothingContainer GenerateRandomOutfit()
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
