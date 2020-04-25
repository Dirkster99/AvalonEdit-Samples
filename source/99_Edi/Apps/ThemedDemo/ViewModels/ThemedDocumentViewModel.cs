using HL.Interfaces;
using System.Windows;
using System.Windows.Media;
using TextEditLib.Interfaces;
using TextEditLib.ViewModels;

namespace ThemedDemo.ViewModels
{

	/// <summary>
	/// Implements a viewmodel for a themed (Dark/Light) display of the TextEditor in AvalonEdit.
	/// </summary>
	public class ThemedDocumentViewModel : DocumentViewModel
	{
		#region ctors
		/// <summary>
		/// Class constructor from AvalonEdit <see cref="HighlightingManager"/> instance.
		/// </summary>
		public ThemedDocumentViewModel(IHighLightingManagerAdapter hlManager)
			: base(hlManager)
		{
		}
		#endregion ctors

		/// <summary>
		/// Invoke this method to apply a change of theme to the content of the document
		/// (eg: Adjust the highlighting colors when changing from "Dark" to "Light"
		///      WITH current text document loaded.)
		/// </summary>
		internal void OnAppThemeChanged(IThemedHighlightingManager hlManager)
		{
			if (hlManager == null)
				return;

			// Does this highlighting definition have an associated highlighting theme?
			if (hlManager.CurrentTheme.HlTheme != null)
			{
				// A highlighting theme with GlobalStyles?
				// Apply these styles to the resource keys of the editor
				foreach (var item in hlManager.CurrentTheme.HlTheme.GlobalStyles)
				{
					switch (item.TypeName)
					{
						case "DefaultStyle":
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorBackground, item.backgroundcolor);
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorForeground, item.foregroundcolor);
							break;

						case "CurrentLineBackground":
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorCurrentLineBackgroundBrushKey, item.backgroundcolor);
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorCurrentLineBorderBrushKey, item.bordercolor);
							break;

						case "LineNumbersForeground":
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorLineNumbersForeground, item.foregroundcolor);
							break;

						case "Selection":
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorSelectionBrush, item.backgroundcolor);
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorSelectionBorder, item.bordercolor);
							break;

						case "Hyperlink":
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorLinkTextBackgroundBrush, item.backgroundcolor);
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorLinkTextForegroundBrush, item.foregroundcolor);
							break;

						case "NonPrintableCharacter":
							ApplyToDynamicResource(TextEditLib.Themes.ResourceKeys.EditorNonPrintableCharacterBrush, item.foregroundcolor);
							break;

						default:
							throw new System.ArgumentOutOfRangeException("GlobalStyle named '{0}' is not supported.", item.TypeName);
					}
				}
			}

			// 1st try: Find highlighting based on currently selected highlighting
			// The highlighting name may be the same as before, but the highlighting theme has just changed
			if (HighlightingDefinition != null)
			{
				// Reset property for currently select highlighting definition
				HighlightingDefinition = hlManager.GetDefinition(HighlightingDefinition.Name);
				NotifyPropertyChanged(() => this.HighlightingDefinitions);

				if (HighlightingDefinition != null)
					return;
			}

			// 2nd try: Find highlighting based on extension of file currenlty being viewed
			if (string.IsNullOrEmpty(FilePath))
				return;

			string extension = System.IO.Path.GetExtension(FilePath);

			if (string.IsNullOrEmpty(extension))
				return;

			// Reset property for currently select highlighting definition
			HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);
			NotifyPropertyChanged(() => this.HighlightingDefinitions);
		}

		/// <summary>
		/// Re-define an existing <seealso cref="SolidColorBrush"/> and backup the originial color
		/// as it was before the application of the custom coloring.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="newColor"></param>
		private void ApplyToDynamicResource(ComponentResourceKey key, Color? newColor)
		{
			if (Application.Current.Resources[key] == null || newColor == null)
				return;

			// Re-coloring works with SolidColorBrushs linked as DynamicResource
			if (Application.Current.Resources[key] is SolidColorBrush)
			{
				//backupDynResources.Add(resourceName);

				var newColorBrush = new SolidColorBrush((Color)newColor);
				newColorBrush.Freeze();

				Application.Current.Resources[key] = newColorBrush;
			}
		}
	}
}
