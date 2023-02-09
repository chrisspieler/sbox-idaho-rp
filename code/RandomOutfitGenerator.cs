using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdahoRP;

public static class RandomOutfitGenerator
{
	private static IEnumerable<Clothing> _allClothing;

	public static ClothingContainer GenerateRandomOutfit()
	{
		if ( _allClothing == null )
			_allClothing = ResourceLibrary.GetAll<Clothing>();

		var outfit = new ClothingContainer();

		Clothing top = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Tops );
		outfit.Add( top );

		Clothing otherTop = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Tops && c.CanBeWornWith( top ) );
		if ( otherTop != null )
			outfit.Add( otherTop );

		Clothing bottom = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Bottoms );
		outfit.Add( bottom );

		Clothing footwear = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Footwear );
		outfit.Add( footwear );

		Clothing skin = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Skin );
		outfit.Add( skin );

		Clothing facial = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Facial );
		outfit.Add( facial );

		Clothing hair = GetRandomClothing( c => c.Category == Clothing.ClothingCategory.Hair );
		outfit.Add( hair );

		return outfit;

		Clothing GetRandomClothing( Func<Clothing, bool> predicate )
		{
			return _allClothing
				.Where( predicate )
				.Random();
		}
	}
}
