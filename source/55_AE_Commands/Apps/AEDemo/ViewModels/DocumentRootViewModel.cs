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
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DocumentRootViewModel()
        {
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
        #endregion properties

        #region methods
        public bool LoadDocument(string paramFilePath)
        {
            if (File.Exists(paramFilePath))
            {
                var hlManager = HighlightingManager.Instance;

                if (Document == null)
                    Document = new TextDocument();
                else
                    Document.Text = string.Empty;

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
                        Document.Text = reader.ReadToEnd();
                    }
                }

                FilePath = paramFilePath;

                // Setting this to null and then to some useful value ensures that the Foldings work
                // Installing Folding Manager is invoked via HighlightingChange
                // (so this works even when changing from test.XML to test1.XML)
                HighlightingDefinition = null;
                string extension = System.IO.Path.GetExtension(paramFilePath);
                HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);

                return true;
            }

            return false;
        }
        #endregion methods
    }
}

