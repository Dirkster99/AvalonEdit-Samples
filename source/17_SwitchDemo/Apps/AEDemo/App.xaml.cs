namespace AEDemo
{
    using Models;
    using System;
    using System.Diagnostics;
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region fields
        private ViewModels.AppViewModel _appVM = null;
        private MainWindow _mainWindow = null;
        #endregion fields

        #region constructors
        static App()
        {
        }
        #endregion constructors

        #region methods
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // Set shutdown mode here (and reset further below) to enable showing custom dialogs (messageboxes)
                // durring start-up without shutting down application when the custom dialogs (messagebox) closes
                ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            }
            catch
            {
            }

            AppLifeCycleViewModel lifeCycle = null;

            try
            {
                lifeCycle = new AppLifeCycleViewModel();

                // Construct Application ViewMOdel and mainWindow
                _appVM = new ViewModels.AppViewModel(lifeCycle);
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }

            Application.Current.MainWindow = _mainWindow = new MainWindow();

            AppCore.CreateAppDataFolder();

            if (MainWindow != null && _appVM != null)
            {
                // and show it to the user ...
                MainWindow.Loaded += MainWindow_Loaded;
                MainWindow.Closing += OnClosing;

                // When the ViewModel asks to be closed, close the window.
                // Source: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
                MainWindow.Closed += delegate
                {
                    // Save session data and close application
                    OnClosed(_appVM, _mainWindow);

                    var dispose = _appVM as IDisposable;
                    if (dispose != null)
                        dispose.Dispose();

                    _mainWindow.DataContext = null;
                    _appVM = null;
                    _mainWindow = null;
                };

                ConstructMainWindowSession(_appVM, _mainWindow);
                MainWindow.Show();
            }
        }

        /// <summary>
        /// Method is invoked when the mainwindow is loaded and visble to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.StackTrace);
            }

            var appVM = MainWindow.DataContext as AppViewModel;

            // Load 2 test text files with these paths from App Resources folder
            appVM.LoadFiles(DocumentRootViewModel.src_path_1, DocumentRootViewModel.src_path_2);
        }

        /// <summary>
        /// COnstruct MainWindow an attach datacontext to it.
        /// </summary>
        /// <param name="workSpace"></param>
        /// <param name="win"></param>
        private void ConstructMainWindowSession(AppViewModel workSpace, MainWindow win)
        {
            try
            {
                MainWindow.DataContext = _appVM;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }

        /// <summary>
        /// Save session data on closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                AppViewModel wsVM = base.MainWindow.DataContext as AppViewModel;

                if (wsVM != null)
                {
                    // Close all open files and check whether application is ready to close
                    if (wsVM.AppLifeCycle.Exit_CheckConditions(wsVM) == true)
                    {
                        // (other than exception and error handling)
                        wsVM.AppLifeCycle.OnRequestClose(true);

                        e.Cancel = false;
                    }
                    else
                    {
                        wsVM.AppLifeCycle.CancelShutDown();
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }

        /// <summary>
        /// Execute closing function and persist session data to be reloaded on next restart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(AppViewModel appVM, MainWindow win)
        {
            try
            {
                // Use this handler to perform any action AFTER the MainWindow was closed
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.StackTrace);
            }
        }
        #endregion methods
    }
}
