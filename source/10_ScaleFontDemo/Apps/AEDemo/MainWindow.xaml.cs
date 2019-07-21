namespace AEDemo
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            // Attach mouse wheel CTRL-key zoom support
            this.avEditor.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(textEditor_PreviewMouseWheel);
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= MainWindow_Unloaded;

            // Detach mouse wheel CTRL-key zoom support
            this.PreviewMouseWheel -= textEditor_PreviewMouseWheel;
        }


        /// <summary>
        /// This method is triggered on a MouseWheel preview event to check if the user
        /// is also holding down the CTRL Key and adjust the current font size if so.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEditor_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double fontSize = this.avEditor.FontSize + e.Delta / 25.0;

                if (fontSize < 6)
                    this.avEditor.FontSize = 6;
                else
                {
                    if (fontSize > 200)
                        this.avEditor.FontSize = 200;
                    else
                        this.avEditor.FontSize = fontSize;
                }

                e.Handled = true;
            }
        }
    }
}
