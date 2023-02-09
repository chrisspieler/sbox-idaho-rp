using Sandbox;

namespace IdahoRP;

public partial class CitizenData : BaseNetworkable
{
	[Net] public string Name { get; set; }
	public ClothingContainer DefaultOutfit { get; set; }
	public Job CurrentJob { get; set; }

	public static CitizenData GenerateRandom()
	{
		return new CitizenData()
		{
			Name = RandomNameGenerator.GenerateRandomName(),
			DefaultOutfit = RandomOutfitGenerator.GenerateRandomOutfit(),
			CurrentJob = null
		};
	}
}
