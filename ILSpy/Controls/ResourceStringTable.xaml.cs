// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ICSharpCode.ILSpy.Controls
{
	/// <summary>
	/// Interaction logic for ResourceStringTable.xaml
	/// </summary>
	public partial class ResourceStringTable : UserControl
	{
		ICollectionView filteredView;
		string filter;

		public ResourceStringTable(IEnumerable strings, FrameworkElement container)
		{
			InitializeComponent();
			// set size to fit decompiler window
			container.SizeChanged += OnParentSizeChanged;
			if (!double.IsNaN(container.ActualWidth))
				Width = Math.Max(container.ActualWidth - 45, 0);
			MaxHeight = container.ActualHeight;

			filteredView = CollectionViewSource.GetDefaultView(strings);
			filteredView.Filter = OnResourceFilter;
			resourceListView.ItemsSource = filteredView;
		}

		private bool OnResourceFilter(object obj)
		{
			if (string.IsNullOrEmpty(filter))
				return true;

			if (obj is KeyValuePair<string, string> item)
				return item.Key?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true ||
					   item.Value?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true;

			return false; // make it obvious search is not working
		}

		private void OnParentSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.WidthChanged && !double.IsNaN(e.NewSize.Width))
				Width = Math.Max(e.NewSize.Width - 45, 0);
			if (e.HeightChanged)
				MaxHeight = e.NewSize.Height;
		}

		private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
		{
			filter = resourceFilterBox.Text;
			filteredView?.Refresh();
		}

		void ExecuteCopy(object sender, ExecutedRoutedEventArgs args)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var item in resourceListView.SelectedItems)
			{
				if (item is KeyValuePair<string, string> pair)
				{
					switch (args.Parameter)
					{
						case "Key":
							sb.AppendLine(pair.Key);
							continue;

						case "Value":
							sb.AppendLine(pair.Value);
							continue;

						default:
							sb.AppendLine($"{pair.Key}\t{pair.Value}");
							continue;
					}
				}
				sb.AppendLine(item.ToString());
			}
			Clipboard.SetText(sb.ToString());
		}

		void CanExecuteCopy(object sender, CanExecuteRoutedEventArgs args)
		{
			args.CanExecute = true;
		}
	}
}