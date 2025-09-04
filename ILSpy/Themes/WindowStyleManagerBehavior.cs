// Copyright (c) 2021 Tom Englert
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

using ICSharpCode.ILSpy.Options;

using TomsToolbox.Essentials;
using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition;
using TomsToolbox.Wpf.Interactivity;

namespace ICSharpCode.ILSpy.Themes
{
	public class WindowStyleManagerBehavior : FrameworkElementBehavior<Window>
	{
		private static readonly DispatcherThrottle restartNotificationThrottle = new DispatcherThrottle(ShowRestartNotification);

		private INotifyChanged _foreground;
		private INotifyChanged _background;

		protected override void OnAttached()
		{
			base.OnAttached();

			MessageBus<SettingsChangedEventArgs>.Subscribers += (sender, e) => Settings_PropertyChanged(sender, e);

			_foreground = AssociatedObject.Track(Control.ForegroundProperty);
			_background = AssociatedObject.Track(Control.BackgroundProperty);

			_foreground.Changed += Color_Changed;
			_background.Changed += Color_Changed;

			UpdateWindowStyle(AssociatedObject.GetExportProvider().GetExportedValue<SettingsService>().DisplaySettings);
			ApplyThemeToWindowCaption();
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			_foreground.Changed -= Color_Changed;
			_background.Changed -= Color_Changed;
		}

		private void Color_Changed(object sender, EventArgs e)
		{
			ApplyThemeToWindowCaption();
		}

		private void UpdateWindowStyle(DisplaySettings displaySettings)
		{
			var window = AssociatedObject;

			if (displaySettings.StyleWindowTitleBar)
			{
				window.Style = (Style)window.FindResource(TomsToolbox.Wpf.Styles.ResourceKeys.WindowStyle);
			}
		}

		private static void ShowRestartNotification()
		{
			MessageBox.Show(Properties.Resources.SettingsChangeRestartRequired);
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is not DisplaySettings displaySettings)
				return;

			if (e.PropertyName != nameof(DisplaySettings.StyleWindowTitleBar))
				return;

			if (!displaySettings.StyleWindowTitleBar)
			{
				restartNotificationThrottle.Tick();
				return;
			}

			UpdateWindowStyle(displaySettings);
		}

		private void ApplyThemeToWindowCaption()
		{
			var window = AssociatedObject;

			IntPtr hwnd = new WindowInteropHelper(window).Handle;

			if (hwnd != IntPtr.Zero)
			{
				var foreground = ((window.Foreground as SolidColorBrush)?.Color).ToGray();
				var background = ((window.Background as SolidColorBrush)?.Color).ToGray();

				var isDarkTheme = background < foreground;

				NativeMethods.UseImmersiveDarkMode(hwnd, isDarkTheme);
			}
			else
			{
				void Initialized(object o, EventArgs eventArgs)
				{
					ApplyThemeToWindowCaption();
					window.SourceInitialized -= Initialized;
				}

				window.SourceInitialized += Initialized;
			}
		}
	}
}
