namespace AEDemo.ViewModels
{
	using ICSharpCode.AvalonEdit.Highlighting;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Windows.Input;
	using AEDemo.ViewModels.Base;
	using UnitComboLib.ViewModels;
	using UnitComboLib.Models.Unit;
	using UnitComboLib.Models.Unit.Screen;
	using System.Collections.Generic;

	public class DocumentRootViewModel : Base.ViewModelBase
	{
		#region fields
		public const string src_path_1 = @".\Resources\Options_1.cs";
		public const string src_path_2 = @".\Resources\Options_2.cs";
		private readonly List<string> _DocumentsList;
		private readonly ObservableCollection<DocumentViewModel> _DocumentViews;
		private DocumentViewModel _CurrentDocumentView;
		private int _DocumentsLists_SelectedIndex;
		private ICommand _DocumentsListChangeCommand;

		private ICommand _HighlightingChangeCommand;
		private IHighlightingDefinition _HighlightingDefinition;
		#endregion fields

		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		public DocumentRootViewModel()
		{
			var items = new ObservableCollection<UnitComboLib.Models.ListItem>(GenerateScreenUnitList());
			SizeUnitLabel = UnitComboLib.UnitViewModeService.CreateInstance(items, new ScreenConverter(), 0);

			_DocumentsList = new List<string>(new string[] { src_path_1, src_path_2 });
			_DocumentViews = new ObservableCollection<DocumentViewModel>();
		}
		#endregion ctors

		#region properties
		public List<string> DocumentsList => _DocumentsList;

		public IEnumerable<DocumentViewModel> DocumentViews
		{
			get
			{
				return _DocumentViews;
			}
		}

		public DocumentViewModel CurrentDocumentView
		{
			get
			{
				return _CurrentDocumentView;
			}

			protected set
			{
				if (_CurrentDocumentView != value)
				{
					_CurrentDocumentView = value;
					NotifyPropertyChanged(() => CurrentDocumentView);
				}
			}
		}

		public int DocumentsLists_SelectedIndex
		{
			get
			{
				return _DocumentsLists_SelectedIndex;
			}

			protected set
			{
				if (_DocumentsLists_SelectedIndex != value)
				{
					_DocumentsLists_SelectedIndex = value;
					NotifyPropertyChanged(() => DocumentsLists_SelectedIndex);
				}
			}
		}

		public ICommand DocumentsListChangeCommand
		{
			get
			{
				if (_DocumentsListChangeCommand == null)
				{
					_DocumentsListChangeCommand = new RelayCommand<object>((p) =>
					{
						if (p is int == false)
							return;

						int param = (int)p;

						if (DocumentsLists_SelectedIndex == param
						    || param < 0 || param >= _DocumentViews.Count)
							return;

						DocumentsLists_SelectedIndex = param;
						CurrentDocumentView = _DocumentViews[param];
					});
				}

				return _DocumentsListChangeCommand;
			}
		}

		#region Highlighting Definition
		/// <summary>
		/// Gets a copy of all highlightings.
		/// </summary>
		public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
		{
			get
			{
				var hlManager = HighlightingManager.Instance;

				if (hlManager != null)
					return hlManager.HighlightingDefinitions;

				return null;
			}
		}

		/// <summary>
		/// AvalonEdit exposes a Highlighting property that controls whether keywords,
		/// comments and other interesting text parts are colored or highlighted in any
		/// other visual way. This property exposes the highlighting information for the
		/// text file managed in this viewmodel class.
		/// </summary>
		public IHighlightingDefinition HighlightingDefinition
		{
			get
			{
				return _HighlightingDefinition;
			}

			set
			{
				if (_HighlightingDefinition != value)
				{
					_HighlightingDefinition = value;
					NotifyPropertyChanged(() => HighlightingDefinition);
				}
			}
		}

		/// <summary>
		/// Gets a command that changes the currently selected syntax highlighting in the editor.
		/// </summary>
		public ICommand HighlightingChangeCommand
		{
			get
			{
				if (_HighlightingChangeCommand == null)
				{
					_HighlightingChangeCommand = new RelayCommand<object>((p) =>
					{
						var parames = p as object[];

						if (parames == null)
							return;

						if (parames.Length != 1)
							return;

						var param = parames[0] as IHighlightingDefinition;
						if (param == null)
							return;

						HighlightingDefinition = param;
					});
				}

				return _HighlightingChangeCommand;
			}
		}
		#endregion Highlighting Definition

		/// <summary>
		/// Scale view of text in percentage of font size
		/// </summary>
		public IUnitViewModel SizeUnitLabel { get; }
		#endregion properties

		#region methods
		public bool LoadDocument(string paramFilePath_1, string paramFilePath_2)
		{
			DocumentsLists_SelectedIndex = 0;
			CurrentDocumentView = null;
			_DocumentViews.Clear();
			HighlightingDefinition = null;

			bool File_Exists = false;
			if (File.Exists(paramFilePath_1))
			{
				var item = new DocumentViewModel();
				bool result = item.LoadDocument(paramFilePath_1, true);

				if (result == true)
				{
					File_Exists = true;
					_DocumentViews.Add(item);
				}
			}

			if (File.Exists(paramFilePath_2))
			{
				var item = new DocumentViewModel();
				bool result = item.LoadDocument(paramFilePath_2, false);

				if (result == true)
				{
					File_Exists = (File_Exists && true);
					_DocumentViews.Add(item);
				}
			}

			if (File_Exists)
			{
				CurrentDocumentView = _DocumentViews[0];

				string extension = System.IO.Path.GetExtension(paramFilePath_1);
				var hlManager = HighlightingManager.Instance;
				HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Initialize Scale View with useful units in percent and font point size
		/// </summary>
		/// <returns></returns>
		private IEnumerable<UnitComboLib.Models.ListItem> GenerateScreenUnitList()
		{
			List<UnitComboLib.Models.ListItem> unitList = new List<UnitComboLib.Models.ListItem>();

			var percentDefaults = new ObservableCollection<string>() { "25", "50", "75", "100", "125", "150", "175", "200", "300", "400", "500" };
			var pointsDefaults = new ObservableCollection<string>() { "3", "6", "8", "9", "10", "12", "14", "16", "18", "20", "24", "26", "32", "48", "60" };

			unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenPercent, "Percent", "%", percentDefaults));
			unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenFontPoints, "Point", "pt", pointsDefaults));

			return unitList;
		}
		#endregion methods
	}
}

