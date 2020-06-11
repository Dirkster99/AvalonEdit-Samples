namespace TextEditLib
{
    using ICSharpCode.AvalonEdit;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Implements an AvalonEdit control textedit control with extensions.
    /// </summary>
    public class TextEdit : TextEditor
    {
        /// <summary>
        /// Static class constructor
        /// </summary>
        static TextEdit()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEdit),
                new FrameworkPropertyMetadata(typeof(TextEdit)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public TextEdit()
        {
            Loaded += TextEdit_Loaded;
            Unloaded += TextEdit_Unloaded;
        }

        private void TextEdit_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= TextEdit_Unloaded;

            // Detach mouse wheel CTRL-key zoom support
            this.PreviewMouseWheel -= textEditor_PreviewMouseWheel;
        }

        private void TextEdit_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= TextEdit_Loaded;

            // Attach mouse wheel CTRL-key zoom support
            this.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(textEditor_PreviewMouseWheel);
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
                double fontSize = this.FontSize + e.Delta / 25.0;

                if (fontSize < 6)
                    this.FontSize = 6;
                else
                {
                    if (fontSize > 200)
                        this.FontSize = 200;
                    else
                        this.FontSize = fontSize;
                }

                e.Handled = true;
            }
        }
    }
}
