using System;
using UIKit;
using System.Collections.Generic;

namespace DelegatePro.iOS
{
	public class ItemPickerModel<TItemType> : UIPickerViewModel
	{
		public TItemType SelectedItem { get; set; }

		public event EventHandler ValueChanged;
		private void OnValueChanged()
		{
			if (ValueChanged != null)
				ValueChanged (this, EventArgs.Empty);
		}

		List<TItemType> _items = null;

		public ItemPickerModel (List<TItemType> items)
		{
			_items = items;
		}

		public override nint GetComponentCount (UIPickerView pickerView)
		{
			return 1;
		}

		public override nint GetRowsInComponent (UIPickerView pickerView, nint component)
		{
			return _items.Count;
		}

		public override string GetTitle (UIPickerView pickerView, nint row, nint component)
		{
            return _items[(int)row].ToString();
		}

		public override void Selected (UIPickerView pickerView, nint row, nint component)
		{
			SelectedItem = _items[(int)row];
			OnValueChanged ();
		}
	}
}

