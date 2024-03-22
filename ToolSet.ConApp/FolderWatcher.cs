using System.Diagnostics;
using PlantUML.ConApp;

namespace ToolSet.ConApp
{
    /// <summary>
    /// Represents a folder watcher that monitors changes in a specified directory.
    /// </summary>
    public partial class FolderWatcher : IDisposable
    {
        #region properties
        /// <summary>
        /// Gets or sets the type of diagram builder.
        /// </summary>
        public DiagramBuilderType DiagramBuilder { get; private set; }
        /// <summary>
        /// Gets the path being watched.
        /// </summary>
        public string WatchPath { get; private set; }
        /// <summary>
        /// Gets or sets the folder path for diagrams.
        /// </summary>
        public string DiagramFolder { get; private set; }
        /// <summary>
        /// Gets or sets the filter used to determine which files are monitored in the folder.
        /// </summary>
        public string Filter { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether to create a complete diagram.
        /// </summary>
        public bool CreateCompleteDiagram { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the force flag is enabled.
        /// </summary>
        public bool Force { get; private set; }
        private FileSystemWatcher Watcher { get; set; }
        #endregion properties

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderWatcher"/> class with the specified parameters.
        /// </summary>
        /// <param name="path">The path of the folder to watch.</param>
        /// <param name="diagramFolder">The folder where the diagrams will be saved.</param>
        /// <param name="diagramBuilderType">The type of diagram builder to use.</param>
        /// <param name="filter">The filter for the files to watch.</param>
        /// <param name="createCompleteDiagram">A flag indicating whether to create a complete diagram.</param>
        /// <param name="force">A flag indicating whether to force the creation of diagrams.</param>
        public FolderWatcher(string path, string diagramFolder, DiagramBuilderType diagramBuilderType, string filter, bool createCompleteDiagram, bool force)
        {
            Constructing();
            WatchPath = path;
            DiagramFolder = diagramFolder;
            DiagramBuilder = diagramBuilderType;
            Filter = filter;
            CreateCompleteDiagram = createCompleteDiagram;
            Force = force;
            Watcher = new FileSystemWatcher(WatchPath);

            Watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            Watcher.Changed += OnChanged;
            Watcher.Created += OnCreated;
            Watcher.Deleted += OnDeleted;
            Watcher.Renamed += OnRenamed;
            Watcher.Error += OnError;

            Watcher.Filter = Filter;
            Watcher.IncludeSubdirectories = true;
            Watcher.EnableRaisingEvents = true;

            Constructed();
        }
        partial void Constructing();
        partial void Constructed();
        #endregion Instance-Constructors

        #region Event-Handlers
        /// <summary>
        /// Event handler for the FileSystemWatcher's Changed event.
        /// </summary>
        /// <param name="source">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var path = Path.GetDirectoryName(e.FullPath);

            if (Path.Exists(path))
            {
                UMLDiagramBuilder diagram = DiagramBuilder switch
                {
                    DiagramBuilderType.Activity => new ActivityDiagramBuilder(path, DiagramFolder, CreateCompleteDiagram, Force),
                    DiagramBuilderType.Class => new ClassDiagramBuilder(path, DiagramFolder,  CreateCompleteDiagram, Force),
                    DiagramBuilderType.Sequence => new SequenceDiagramBuilder(path, DiagramFolder, CreateCompleteDiagram, Force),
                    _ => throw new InvalidOperationException("Invalid diagram builder type."),
                };
                diagram.CreateFromPath();
            }
            Debug.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }
        /// <summary>
        /// Event handler for the file creation event.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            Debug.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            Debug.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }
        /// <summary>
        /// Event handler for the "Renamed" event.
        /// </summary>
        /// <param name="source">The object that raised the event.</param>
        /// <param name="e">The event arguments containing information about the renamed file.</param>
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Debug.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }
        /// <summary>
        /// Event handler for handling errors that occur during the folder watching process.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">An <see cref="ErrorEventArgs"/> object that contains the event data.</param>
        private void OnError(object source, ErrorEventArgs e)
        {
            Debug.WriteLine($"Error: {e.GetException()}");
        }
        #endregion Event-Handlers

        #region dispose
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Watcher.Dispose();
        }
        #endregion dispose
    }
}
