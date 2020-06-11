using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System.IO;
using System.Text;

namespace AEDemo.ViewModels
{
	public class DocumentViewModel : Base.ViewModelBase
	{
        #region fields
        private TextDocument _Document;
        private bool _IsDirty;
        private bool _IsReadOnly;
        private string _IsReadOnlyReason = string.Empty;
		private string _FileName;
		#endregion fields

		#region ctors
		/// <summary>
		/// Class constructor
		/// </summary>
		public DocumentViewModel()
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
                    NotifyPropertyChanged(() => Document);
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
                    NotifyPropertyChanged(() => IsDirty);
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
                    NotifyPropertyChanged(() => IsReadOnly);
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
                    NotifyPropertyChanged(() => IsReadOnlyReason);
                }
            }
        }

        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;

                    NotifyPropertyChanged(() => FileName);
                }
            }
        }
        #endregion properties

        #region methods

        public bool LoadDocument(string filePath, bool asReadOnly)
        {
            if (File.Exists(filePath))
            {
                Document = new TextDocument();

                IsDirty = false;
                IsReadOnly = asReadOnly;

                // Check file attributes and set to read-only if file attributes indicate that
                if ((System.IO.File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0)
                {
                    IsReadOnly = true;
                    IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                                       "Change the file access permissions or save the file in a different location if you want to edit it.";
                }

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                    {
                        Document = new TextDocument(reader.ReadToEnd());
                    }
                }

                FileName = filePath;

                return true;
            }

            return false;
        }
        #endregion methods
    }
}
