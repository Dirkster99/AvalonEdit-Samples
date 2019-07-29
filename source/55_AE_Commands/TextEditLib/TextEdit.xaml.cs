namespace TextEditLib
{
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Folding;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Rendering;
    using System.Windows;
    using System.Windows.Input;
    using TextEditLib.Foldings;

    /// <summary>
    /// Implements an AvalonEdit control textedit control with extensions.
    /// </summary>
    public class TextEdit : TextEditor
    {
        #region fields
        public static readonly new DependencyProperty SyntaxHighlightingProperty =
            TextEditor.SyntaxHighlightingProperty.AddOwner(typeof(TextEdit),
                new FrameworkPropertyMetadata(OnSyntaxHighlightingChanged));

        /// <summary>
        /// Document property.
        /// </summary>
        public static readonly new DependencyProperty DocumentProperty
            = TextView.DocumentProperty.AddOwner(
                typeof(TextEdit), new FrameworkPropertyMetadata(OnDocumentChanged));

        FoldingManager mFoldingManager = null;
        object mFoldingStrategy = null;
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
            CommandBindings.Add(new CommandBinding(TextEditCommands.FoldsCollapseAll, TextEdit.FoldsCollapseAll, TextEdit.FoldsColapseExpandCanExecute));
            CommandBindings.Add(new CommandBinding(TextEditCommands.FoldsExpandAll, TextEdit.FoldsExpandAll, TextEdit.FoldsColapseExpandCanExecute));
        }
        #endregion ctors

        #region methods
        private static void OnSyntaxHighlightingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textEdit = d as TextEdit;
            if (textEdit != null)
                textEdit.OnChangedFoldingInstance(e);
        }

        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textEdit = d as TextEdit;
            if (textEdit != null)
                textEdit.OnDocumentChanged(e);
        }

        private void OnDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            // Clean up and re-install foldings to avoid exception 'Invalid Document' being thrown by StartGeneration
            OnChangedFoldingInstance(e);
        }

        /// <summary>
        /// Method is invoked when the Document or SyntaxHighlightingDefinition dependency property is changed.
        /// This change should always lead to removing and re-installing the correct folding manager and strategy.
        /// </summary>
        /// <param name="e"></param>
        private void OnChangedFoldingInstance(DependencyPropertyChangedEventArgs e)
        {
            try
            {
                // Clean up last installation of folding manager and strategy
                if (mFoldingManager != null)
                {
                    FoldingManager.Uninstall(mFoldingManager);
                    mFoldingManager = null;
                }

                this.mFoldingStrategy = null;
            }
            catch
            {
            }

            if (e == null)
                return;

            var syntaxHighlighting = e.NewValue as IHighlightingDefinition;
            if (syntaxHighlighting == null)
                return;

            switch (syntaxHighlighting.Name)
            {
                case "XML":
                    mFoldingStrategy = new XmlFoldingStrategy();
                    this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                    break;

                case "C#":
                case "C++":
                case "PHP":
                case "Java":
                    this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(this.Options);
                    mFoldingStrategy = new BraceFoldingStrategy();
                    break;

                default:
                    this.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                    mFoldingStrategy = null;
                    break;
            }

            if (mFoldingStrategy != null)
            {
                if (mFoldingManager == null)
                    mFoldingManager = FoldingManager.Install(this.TextArea);

                UpdateFoldings();
            }
            else
            {
                if (mFoldingManager != null)
                {
                    FoldingManager.Uninstall(mFoldingManager);
                    mFoldingManager = null;
                }
            }
        }

        private void UpdateFoldings()
        {
            if (mFoldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy)mFoldingStrategy).UpdateFoldings(mFoldingManager, this.Document);
            }

            if (mFoldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy)mFoldingStrategy).UpdateFoldings(mFoldingManager, this.Document);
            }
        }

        #region Fold Unfold Command
        /// <summary>
        /// Determines whether a folding command can be executed or not and sets correspondind
        /// <paramref name="e"/>.CanExecute property value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FoldsColapseExpandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;

            var editoredi = sender as TextEdit;

            if (editoredi == null)
                return;

            if (editoredi.mFoldingManager == null)
                return;

            if (editoredi.mFoldingManager.AllFoldings == null)
                return;

            e.CanExecute = true;
        }

        /// <summary>
        /// Executes the collapse all folds command (which folds all text foldings but the first).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FoldsCollapseAll(object sender, ExecutedRoutedEventArgs e)
        {
            var editor = sender as TextEdit;

            if (editor == null)
                return;

            editor.CollapseAllTextfoldings();
        }

        /// <summary>
        /// Executes the collapse all folds command (which folds all text foldings but the first).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FoldsExpandAll(object sender, ExecutedRoutedEventArgs e)
        {
            var editor = sender as TextEdit;

            if (editor == null)
                return;

            editor.ExpandAllTextFoldings();
        }

        /// <summary>
        /// Goes through all foldings in the displayed text and folds them
        /// so that users can explore the text in a top down manner.
        /// </summary>
        private void CollapseAllTextfoldings()
        {
            if (this.mFoldingManager == null)
                return;

            if (this.mFoldingManager.AllFoldings == null)
                return;

            foreach (var loFolding in this.mFoldingManager.AllFoldings)
            {
                loFolding.IsFolded = true;
            }

            // Unfold the first fold (if any) to give a useful overview on content
            FoldingSection foldSection = this.mFoldingManager.GetNextFolding(0);

            if (foldSection != null)
                foldSection.IsFolded = false;
        }

        /// <summary>
        /// Goes through all foldings in the displayed text and unfolds them
        /// so that users can see all text items (without having to play with folding).
        /// </summary>
        private void ExpandAllTextFoldings()
        {
            if (this.mFoldingManager == null)
                return;

            if (this.mFoldingManager.AllFoldings == null)
                return;

            foreach (var loFolding in this.mFoldingManager.AllFoldings)
            {
                loFolding.IsFolded = false;
            }
        }
        #endregion Fold Unfold Command
        #endregion methods
    }
}
