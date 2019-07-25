namespace TextEditLib
{
    using ICSharpCode.AvalonEdit;
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using TextEditLib.Extensions;

    /// <summary>
    /// Implements an AvalonEdit control textedit control with extensions.
    /// </summary>
    public class TextEdit : TextEditor
    {
        #region fields
        #region EditorCurrentLine Highlighting Colors
        private static readonly DependencyProperty EditorCurrentLineBackgroundProperty =
            DependencyProperty.Register("EditorCurrentLineBackground",
                                         typeof(Brush),
                                         typeof(TextEdit),
                                         new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty EditorCurrentLineBorderProperty =
            DependencyProperty.Register("EditorCurrentLineBorder", typeof(Brush),
                typeof(TextEdit), new PropertyMetadata(new SolidColorBrush(
                    Color.FromArgb(0x60, SystemColors.HighlightBrush.Color.R,
                                         SystemColors.HighlightBrush.Color.G,
                                         SystemColors.HighlightBrush.Color.B))));

        public static readonly DependencyProperty EditorCurrentLineBorderThicknessProperty =
            DependencyProperty.Register("EditorCurrentLineBorderThickness", typeof(double),
                typeof(TextEdit), new PropertyMetadata(2.0d));
        #endregion EditorCurrentLine Highlighting Colors

        #region CaretPosition
        private static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register("Column", typeof(int),
                typeof(TextEdit), new UIPropertyMetadata(1));

        private static readonly DependencyProperty LineProperty =
            DependencyProperty.Register("Line", typeof(int),
                typeof(TextEdit), new UIPropertyMetadata(1));
        #endregion CaretPosition
        #endregion fields

        #region ctors
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
        #endregion ctors

        #region properties
        #region EditorCurrentLine Highlighting Colors
        /// <summary>
        /// Gets/sets the background color of the current editor line.
        /// </summary>
        public Brush EditorCurrentLineBackground
        {
            get { return (Brush)GetValue(EditorCurrentLineBackgroundProperty); }
            set { SetValue(EditorCurrentLineBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets/sets the border color of the current editor line.
        /// </summary>
        public Brush EditorCurrentLineBorder
        {
            get { return (Brush)GetValue(EditorCurrentLineBorderProperty); }
            set { SetValue(EditorCurrentLineBorderProperty, value); }
        }

        /// <summary>
        /// Gets/sets the the thickness of the border of the current editor line.
        /// </summary>
        public double EditorCurrentLineBorderThickness
        {
            get { return (double)GetValue(EditorCurrentLineBorderThicknessProperty); }
            set { SetValue(EditorCurrentLineBorderThicknessProperty, value); }
        }
        #endregion EditorCurrentLine Highlighting Colors

        #region CaretPosition
        /// <summary>
        /// Get/set the current column of the editor caret.
        /// </summary>
        public int Column
        {
            get
            {
                return (int)GetValue(ColumnProperty);
            }

            set
            {
                SetValue(ColumnProperty, value);
            }
        }

        /// <summary>
        /// Get/set the current line of the editor caret.
        /// </summary>
        public int Line
        {
            get
            {
                return (int)GetValue(LineProperty);
            }

            set
            {
                SetValue(LineProperty, value);
            }
        }
        #endregion CaretPosition
        #endregion properties

        #region methods
        private void TextEdit_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= TextEdit_Unloaded;

            // Detach mouse wheel CTRL-key zoom support
            this.PreviewMouseWheel -= textEditor_PreviewMouseWheel;
            this.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
        }

        private void TextEdit_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= TextEdit_Loaded;

            AdjustCurrentLineBackground();

            // Attach mouse wheel CTRL-key zoom support
            this.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(textEditor_PreviewMouseWheel);
            this.TextArea.Caret.PositionChanged += Caret_PositionChanged;
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

        /// <summary>
        /// Update Column and Line position properties when caret position is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            // this.TextArea.TextView.InvalidateLayer(KnownLayer.Background); //Update current line highlighting

            if (this.TextArea != null)
            {
                this.Column = this.TextArea.Caret.Column;
                this.Line = this.TextArea.Caret.Line;
            }
            else
            {
                this.Column = 0;
                this.Line = 0;
            }
        }

        /// <summary>
        /// Reset the <seealso cref="SolidColorBrush"/> to be used for highlighting the current editor line.
        /// </summary>
        private void AdjustCurrentLineBackground()
        {
            HighlightCurrentLineBackgroundRenderer oldRenderer = null;

            // Make sure there is only one of this type of background renderer
            // Otherwise, we might keep adding and WPF keeps drawing them on top of each other
            foreach (var item in this.TextArea.TextView.BackgroundRenderers)
            {
                if (item != null)
                {
                    if (item is HighlightCurrentLineBackgroundRenderer)
                    {
                        oldRenderer = item as HighlightCurrentLineBackgroundRenderer;
                    }
                }
            }

            if (oldRenderer != null)
                this.TextArea.TextView.BackgroundRenderers.Remove(oldRenderer);

            this.TextArea.TextView.BackgroundRenderers.Add(new HighlightCurrentLineBackgroundRenderer(this));
        }
		#endregion methods
    }
}
