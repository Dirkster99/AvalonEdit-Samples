namespace AEDemo.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Input;

    /// <summary>
    /// Implements application life cycle relevant properties and methods,
    /// such as: state for shutdown, shutdown_cancel, command for shutdown,
    /// and methods for save and load application configuration.
    /// </summary>
    public class AppLifeCycleViewModel : Base.ViewModelBase
    {
        #region fields
        private bool? mDialogCloseResult = null;
        private bool mShutDownInProgress = false;
        private bool mShutDownInProgress_Cancel = false;

        private ICommand mExitApp = null;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets a string for display of the application title.
        /// </summary>
        public string Application_Title
        {
            get
            {
                return Models.AppCore.Application_Title;
            }
        }

        /// <summary>
        /// Get path and file name to application specific settings file
        /// </summary>
        public string DirFileAppSettingsData
        {
            get
            {
                return System.IO.Path.Combine(Models.AppCore.DirAppData,
                                              string.Format(CultureInfo.InvariantCulture, "{0}.App.settings",
                                              Models.AppCore.AssemblyTitle));
            }
        }

        /// <summary>
        /// This can be used to close the attached view via ViewModel
        /// 
        /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
        /// </summary>
        public bool? DialogCloseResult
        {
            get
            {
                return mDialogCloseResult;
            }

            private set
            {
                if (mDialogCloseResult != value)
                {
                    mDialogCloseResult = value;
                    RaisePropertyChanged(() => DialogCloseResult);
                }
            }
        }

        /// <summary>
        /// Gets a command to exit (end) the application.
        /// </summary>
        public ICommand ExitApp
        {
            get
            {
                if (mExitApp == null)
                {
                    mExitApp = new Base.RelayCommand<object>((p) => AppExit_CommandExecuted(),
                                                             (p) => Closing_CanExecute());
                }

                return mExitApp;
            }
        }

        /// <summary>
        /// Get a path to the directory where the user store his documents
        /// </summary>
        public static string MyDocumentsUserDir
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public bool ShutDownInProgress_Cancel
        {
            get
            {
                return mShutDownInProgress_Cancel;
            }

            set
            {
                if (mShutDownInProgress_Cancel != value)
                    mShutDownInProgress_Cancel = value;
            }
        }
        #endregion properties

        #region methods
        #region StartUp/ShutDown
        private void AppExit_CommandExecuted()
        {
            try
            {
                if (Closing_CanExecute() == true)
                {
                    mShutDownInProgress_Cancel = false;
                    OnRequestClose();
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }

        private bool Closing_CanExecute()
        {
            return true;
        }

        /// <summary>
        /// Check if pre-requisites for closing application are available.
        /// Save session data on closing and cancel closing process if necessary.
        /// </summary>
        /// <returns>true if application is OK to proceed closing with closed, otherwise false.</returns>
        public bool Exit_CheckConditions(object sender)
        {
            try
            {
                if (mShutDownInProgress == true)
                    return true;

                // this return is normally computed if there are documents open with unsaved data
                return true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.StackTrace);
            }

            return true;
        }

        #region RequestClose [event]
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        ////public event EventHandler ApplicationClosed;

        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        public void OnRequestClose(bool ShutDownAfterClosing = true)
        {
            try
            {
                if (ShutDownAfterClosing == true)
                {
                    if (mShutDownInProgress == false)
                    {
                        if (DialogCloseResult == null)
                            DialogCloseResult = true;      // Execute Closing event via attached property

                        if (mShutDownInProgress_Cancel == true)
                        {
                            mShutDownInProgress = false;
                            mShutDownInProgress_Cancel = false;
                            DialogCloseResult = null;
                        }
                    }
                }
                else
                    mShutDownInProgress = true;

                CommandManager.InvalidateRequerySuggested();

                ////EventHandler handler = ApplicationClosed;
                ////if (handler != null)
                ////  handler(this, EventArgs.Empty);
            }
            catch (Exception exp)
            {
                mShutDownInProgress = false;

                Debug.WriteLine(exp.StackTrace);
            }
        }

        public void CancelShutDown()
        {
            DialogCloseResult = null;
        }
        #endregion // RequestClose [event]
        #endregion StartUp/ShutDown
        #endregion methods
    }
}
