using Sandbox.Razor;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdahoRP.UI
{
	public class ListBox : Panel
	{
		public Panel SelectedItem 
		{
			get => _selectedItem;
			set
			{
				var oldSelection = _selectedItem;
				if (oldSelection != null )
					oldSelection.RemoveClass( "selected" );
				_selectedItem = value;
				_selectedItem.AddClass( "selected" );
				if ( _selectedItem == oldSelection )
					return;
				OnSelectionChanged?.Invoke( this, _selectedItem);
			}
		}
		private Panel _selectedItem;

		private bool _selectionIsInitialized = false;

		public override void Tick()
		{
			base.Tick();
			if ( !_selectionIsInitialized && ChildrenCount > 0 )
				InitializeSelection();
		}

		private void InitializeSelection()
		{
			var selectedItems = Children.Where( c => c.HasClass( "selected" ) );
			SelectedItem = selectedItems.Count() switch
			{
				> 1 => throw new InvalidOperationException( $"{nameof( ListBox )} does not support multiple selection yet." ),
				1 => selectedItems.FirstOrDefault(),
				<= 0 => Children.FirstOrDefault(c => !c.HasClass("noselect")),
			};
			_selectionIsInitialized = true;
		}

		protected override void OnChildAdded( Panel child )
		{
			base.OnChildAdded( child );
			if (!child.HasClass( "noselect" ) && child.ElementName != "button" )
			{
				child.AddEventListener( "onclick", _ => SelectedItem = child );
			}
		}

		public event EventHandler<Panel> OnSelectionChanged;
	}
}
