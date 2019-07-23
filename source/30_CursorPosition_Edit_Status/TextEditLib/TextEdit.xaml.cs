namespace TextEditLib
{
    using ICSharpCode.AvalonEdit;
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Implements an AvalonEdit control textedit control with extensions.
    /// </summary>
    public class TextEdit : TextEditor
    {
        #region fields
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
            this.Loaded += TextEdit_Loaded;
            this.Unloaded += TextEdit_Unloaded;
        }
        #endregion ctors

        #region properties
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
        private void TextEdit_Loaded(object sender, RoutedEventArgs e)
        {
            this.TextArea.Caret.PositionChanged += Caret_PositionChanged;
        }

        private void TextEdit_Unloaded(object sender, RoutedEventArgs e)
        {
            this.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
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
        #endregion methods
    }
}
