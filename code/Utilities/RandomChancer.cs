using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP;

public class RandomChancer<T> : IEnumerable<RandomChancer<T>.RandomItem>
{
	public class RandomItem
	{
		public RandomItem(T item, float ratio)
		{
			Item = item;
			Ratio = ratio;
		}

		public T Item { get; set; }
		public float Ratio { get; set; }
	}
	private List<RandomItem> _ratios = new();
	private Random _rand = new Random();

	private float _ratioSum => _ratios.Sum( p => p.Ratio );

	public int Count => _ratios.Count;

	public void AddItem(T item, float percentChance)
	{
		_ratios.Add( new RandomItem( item, percentChance ) );
	}

	public T GetNext()
	{
		var numValue = _rand.NextSingle() * _ratioSum;
		T selectedItem = default;
		foreach(RandomItem item in _ratios )
		{
			numValue -= item.Ratio;

			if ( numValue <= 0 )
			{
				selectedItem = item.Item;
				break;
			}
		}
		return selectedItem;
	}

	public IEnumerator<RandomItem> GetEnumerator()
	{
		return _ratios.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _ratios.GetEnumerator();
	}
}
