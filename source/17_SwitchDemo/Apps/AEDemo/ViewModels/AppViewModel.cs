namespace AEDemo.ViewModels
{
    using Base;
    using ICSharpCode.AvalonEdit.Highlighting;
    using Microsoft.Win32;
    using System;
	using System.Collections.Generic;
	using System.Windows.Documents;
	using System.Windows.Input;

    /// <summary>
    /// Main ViewModel vlass that manages session start-up, life span, and shutdown
    /// of the application.
    /// </summary>
    internal class AppViewModel : Base.ViewModelBase, IDisposable
    {
        #region private fields
        private bool mDisposed = false;
        private AppLifeCycleViewModel _AppLifeCycle;

        private bool _isInitialized = false;       // application should be initialized through one method ONLY!
        private object _lockObject = new object(); // thread lock semaphore

        private ICommand _OpenFileCommand;
        private IHighlightingDefinition _HighlightingDefinition;
        private ICommand _HighlightingChangeCommand;
		private readonly DocumentRootViewModel _demo;
		#endregion private fields

		#region constructors
		/// <summary>
		/// Standard Constructor
		/// </summary>
		public AppViewModel(AppLifeCycleViewModel lifecycle)
            : this()
        {
            _AppLifeCycle = lifecycle;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected AppViewModel()
        {
            _demo = new DocumentRootViewModel();
        }
        #endregion constructors

        #region properties
        public AppLifeCycleViewModel AppLifeCycle
        {
            get
            {
                return _AppLifeCycle;
            }
        }

        /// <summary>
        /// Gets the demo viewmodel and all its properties and commands
        /// </summary>
        public DocumentRootViewModel DocumentRoot
        {
            get
            {
                return _demo;
            }
        }

        public ICommand OpenFileCommand
        {
            get
            {
                if (_OpenFileCommand == null)
                {
                    _OpenFileCommand = new RelayCommand<object>((p) =>
                    {
                        var dlg = new OpenFileDialog();
                        if (dlg.ShowDialog().GetValueOrDefault())
                        {
                            var fileViewModel = DocumentRoot.LoadDocument(dlg.FileName, string.Empty);
                        }
                    });
                }

                return _OpenFileCommand;
            }
        }

        #region Highlighting Definition
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

            protected set
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
        #endregion properties

        #region methods
        public bool LoadFiles(string fileName_1, string fileName_2)
		{
            return DocumentRoot.LoadDocument(fileName_1, fileName_2);
        }

        /// <summary>
        /// Call this method if you want to initialize a headless
        /// (command line) application. This method will initialize only
        /// Non-WPF related items.
        /// 
        /// Method should not be called after <seealso cref="InitForMainWindow"/>
        /// </summary>
        public void InitWithoutMainWindow()
        {
            lock (_lockObject)
            {
                if (_isInitialized == true)
                    throw new Exception("AppViewModel initizialized twice.");

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed == false)
            {
                if (disposing == true)
                {
                    // Dispose of the curently displayed content
                    ////mContent.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            mDisposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion methods
    }
}
