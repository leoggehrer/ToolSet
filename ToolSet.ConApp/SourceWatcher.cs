//@BaseCode
//MdStart
namespace ToolSet.ConApp
{
    public partial class SourceWatcher : FolderWatcher
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="SourceWatcher"/> class.
        /// This static constructor sets up the necessary properties for the program.
        /// </summary>
        static SourceWatcher()
        {
            ClassConstructing();
            ClassConstructed();
        }

        /// <summary>
        /// This method is called during the construction of the <see cref="UMLWatcher"/> class.
        /// </summary>
        static partial void ClassConstructing();

        /// <summary>
        /// Represents a method that is called when the <see cref="UMLWatcher"/> class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region properties
        /// <summary>
        /// Gets the target path.
        /// </summary>
        public string TargetPath { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether the force flag is enabled.
        /// </summary>
        public bool Force { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether the process is currently in progress.
        /// </summary>
        private bool InProcess { get; set; } = false;
        #endregion properties

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceWatcher"/> class with the specified parameters.
        /// </summary>
        /// <param name="sourcePath">The path of the folder to watch.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="filter">The filter for the files to watch.</param>
        /// <param name="force">A flag indicating whether to force the creation of diagrams.</param>
        public SourceWatcher(string sourcePath, string targetPath, string filter, bool force)
            : base(sourcePath, filter)
        {
            Constructing();

            TargetPath = targetPath;
            Force = force;

            Constructed();
        }

        partial void Constructing();
        partial void Constructed();
        #endregion Instance-Constructors

        #region Event-Handlers
        /// <summary>
        /// Event handler for the file creation event.
        /// </summary>
        /// <param name="source">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        protected override void OnCreated(object source, FileSystemEventArgs e)
        {
            base.OnCreated(source, e);

            if (File.Exists(e.FullPath) == false && InProcess == false)
            {
                InProcess = true;
                UpdateTarget(e.FullPath);
                InProcess = false;
            }
        }

        /// <summary>
        /// Event handler for the file system change event.
        /// </summary>
        /// <param name="source">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        override protected void OnChanged(object source, FileSystemEventArgs e)
        {
            base.OnChanged(source, e);

            if (InProcess == false)
            {
                InProcess = true;
                UpdateTarget(e.FullPath);
                InProcess = false;
            }
        }
        /// <summary>
        /// Event handler for the "Deleted" event.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        protected override void OnDeleted(object source, FileSystemEventArgs e)
        {
            base.OnDeleted(source, e);

            if (InProcess == false)
            {
                InProcess = true;
                DeleteTargetFile(e.FullPath);
                InProcess = false;
            }
        }
        #endregion Event-Handlers

        #region app-methods
        /// <summary>
        /// Updates the diagrams based on the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the file that triggered the update.</param>
        private void UpdateTarget(string filePath)
        {
            if (File.Exists(filePath))
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    while (IsFileInUse(filePath) == false)
                    {
                        Thread.Sleep(500);
                    }
                }

                var fileName = Path.GetFileName(filePath);
                var targetFilePath = Path.Combine(TargetPath, fileName);

                if (Force || File.Exists(targetFilePath) == false)
                {
                    var targetPath = Path.GetDirectoryName(targetFilePath);

                    if (Directory.Exists(targetPath) == false)
                    {
                        Directory.CreateDirectory(targetPath!);
                    }

                    File.Copy(filePath, targetFilePath, true);
                }
            }
        }
        /// <summary>
        /// Deletes the target file at the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the file to delete.</param>
        private void DeleteTargetFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var targetFilePath = Path.Combine(TargetPath, fileName);

            if (Force && File.Exists(targetFilePath))
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    while (IsFileInUse(filePath) == false)
                    {
                        Thread.Sleep(500);
                    }
                }

                File.Delete(targetFilePath);
            }
        }
        #endregion app-methods
    }
}
//MdEnd