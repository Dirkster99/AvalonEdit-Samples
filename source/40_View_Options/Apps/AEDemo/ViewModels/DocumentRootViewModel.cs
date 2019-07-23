namespace AEDemo.ViewModels
{
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Utils;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Windows.Input;
    using AEDemo.ViewModels.Base;
    using ICSharpCode.AvalonEdit;
    using AEDemo.Enums;

    public class DocumentRootViewModel : Base.ViewModelBase
    {
        #region fields
        private string _FilePath;

        private TextDocument _Document;
        private bool _IsDirty;
        private bool _IsReadOnly;
        private string _IsReadOnlyReason = string.Empty;

        private ICommand _HighlightingChangeCommand;
        private IHighlightingDefinition _HighlightingDefinition;
        private ICommand _DisableHighlightingCommand;
        private ICommand _toggleEditorOptionCommand;

        private bool _IsContentLoaded;
        private bool _WordWrap;
        private bool _ShowLineNumbers;
        private readonly TextEditorOptions _TextOptions;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DocumentRootViewModel()
        {
            _TextOptions = new TextEditorOptions();
            _TextOptions.AllowToggleOverstrikeMode = true;

            Document = new TextDocument();
        }
        #endregion ctors

        #region properties
        public TextDocument Document
        {
            get { return _Document; }
            set
            {
                if (_Document != value)
                {
                    _Document = value;
                    RaisePropertyChanged(() => Document);
                }
            }
        }

        public bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                if (_IsDirty != value)
                {
                    _IsDirty = value;
                    RaisePropertyChanged(() => IsDirty);
                }
            }
        }

        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                if (_FilePath != value)
                {
                    _FilePath = value;

                    RaisePropertyChanged(() => FilePath);
                }
            }
        }

        public bool IsContentLoaded
        {
            get { return _IsContentLoaded; }
            set
            {
                if (_IsContentLoaded != value)
                {
                    _IsContentLoaded = value;

                    RaisePropertyChanged(() => IsContentLoaded);
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            protected set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;
                    RaisePropertyChanged(() => IsReadOnly);
                }
            }
        }

        public string IsReadOnlyReason
        {
            get
            {
                return _IsReadOnlyReason;
            }

            protected set
            {
                if (_IsReadOnlyReason != value)
                {
                    _IsReadOnlyReason = value;
                    RaisePropertyChanged(() => IsReadOnlyReason);
                }
            }
        }

        public ICommand ToggleEditorOptionCommand
        {
            get
            {
                return _toggleEditorOptionCommand ??
                            (_toggleEditorOptionCommand = new RelayCommand<ToggleEditorOption>
                               ((p) => OnToggleEditorOption(p),
                                (p) => { return OnToggleEditorOptionCanExecute(p); }
                               )
                            );
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
                    RaisePropertyChanged(() => HighlightingDefinition);
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
        /// Gets a command that turns off editors syntax highlighting.
        /// </summary>
        public ICommand DisableHighlightingCommand
        {
            get
            {
                if (_DisableHighlightingCommand == null)
                {
                    _DisableHighlightingCommand = new RelayCommand<object>(
                        (p) => { HighlightingDefinition = null; },
                        (p) =>
                        {
                            if (HighlightingDefinition != null)
                                return true;

                            return false;
                        }
                        );
                }

                return _DisableHighlightingCommand;
            }
        }

        /// <summary>
        /// Get/set whether word wrap is currently activated or not.
        /// </summary>
        public bool WordWrap
        {
            get { return _WordWrap; }

            set
            {
                if (_WordWrap != value)
                {
                    _WordWrap = value;
                    RaisePropertyChanged(() => WordWrap);
                }
            }
        }

        /// <summary>
        /// Get/set whether line numbers are currently shown or not.
        /// </summary>
        public bool ShowLineNumbers
        {
            get { return _ShowLineNumbers; }

            set
            {
                if (_ShowLineNumbers != value)
                {
                    _ShowLineNumbers = value;
                    RaisePropertyChanged(() => ShowLineNumbers);
                }
            }
        }

        /// <summary>
        /// Get/set whether the end of each line is currently shown or not.
        /// </summary>
        public bool ShowEndOfLine               // Toggle state command
        {
            get { return TextOptions.ShowEndOfLine; }

            set
            {
                if (TextOptions.ShowEndOfLine != value)
                {
                    TextOptions.ShowEndOfLine = value;
                    RaisePropertyChanged(() => ShowEndOfLine);
                }
            }
        }

        /// <summary>
        /// Get/set whether the spaces are highlighted or not.
        /// </summary>
        public bool ShowSpaces               // Toggle state command
        {
            get { return TextOptions.ShowSpaces; }

            set
            {
                if (TextOptions.ShowSpaces != value)
                {
                    TextOptions.ShowSpaces = value;
                    RaisePropertyChanged(() => ShowSpaces);
                }
            }
        }

        /// <summary>
        /// Get/set whether the tabulator characters are highlighted or not.
        /// </summary>
        public bool ShowTabs               // Toggle state command
        {
            get { return TextOptions.ShowTabs; }

            set
            {
                if (TextOptions.ShowTabs != value)
                {
                    TextOptions.ShowTabs = value;
                    RaisePropertyChanged(() => ShowTabs);
                }
            }
        }

        /// <summary>
        /// Get/Set texteditor options frmo <see cref="AvalonEdit"/> editor as <see cref="TextEditorOptions"/> instance.
        /// </summary>
        public TextEditorOptions TextOptions
        {
            get { return _TextOptions; }
        }
        #endregion properties

        #region methods
        public bool LoadDocument(string paramFilePath)
        {
            IsContentLoaded = false;

            if (File.Exists(paramFilePath))
            {
                var hlManager = HighlightingManager.Instance;

                Document = new TextDocument();
                string extension = System.IO.Path.GetExtension(paramFilePath);
                HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);

                IsDirty = false;
                IsReadOnly = false;

                // Check file attributes and set to read-only if file attributes indicate that
                if ((System.IO.File.GetAttributes(paramFilePath) & FileAttributes.ReadOnly) != 0)
                {
                    IsReadOnly = true;
                    IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                                       "Change the file access permissions or save the file in a different location if you want to edit it.";
                }

                using (FileStream fs = new FileStream(paramFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                    {
                        Document = new TextDocument(reader.ReadToEnd());
                    }
                }

                FilePath = paramFilePath;
                IsContentLoaded = true;
            }

            return IsContentLoaded;
        }

        private void OnToggleEditorOption(object parameter)
        {
            if (parameter == null)
                return;

            if ((parameter is ToggleEditorOption) == false)
                return;

            ToggleEditorOption t = (ToggleEditorOption)parameter;

            switch (t)
            {
                case ToggleEditorOption.WordWrap:
                    this.WordWrap = !this.WordWrap;
                    break;

                case ToggleEditorOption.ShowLineNumber:
                    this.ShowLineNumbers = !this.ShowLineNumbers;
                    break;

                case ToggleEditorOption.ShowSpaces:
                    this.TextOptions.ShowSpaces = !this.TextOptions.ShowSpaces;
                    break;

                case ToggleEditorOption.ShowTabs:
                    this.TextOptions.ShowTabs = !this.TextOptions.ShowTabs;
                    break;

                case ToggleEditorOption.ShowEndOfLine:
                    this.TextOptions.ShowEndOfLine = !this.TextOptions.ShowEndOfLine;
                    break;

                default:
                    break;
            }
        }

        private bool OnToggleEditorOptionCanExecute(object parameter)
        {
            if (parameter == null)
                return false;

            if (parameter is ToggleEditorOption)
                return true;

            return false;
        }
        #endregion methods
    }
}

