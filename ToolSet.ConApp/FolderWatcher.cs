using System.Diagnostics;
using PlantUML.ConApp;

namespace ToolSet.ConApp
{
    public partial class FolderWatcher : IDisposable
    {
        #region properties
        public DiagramBuilderType DiagramBuilder { get; private set; }
        public string WatchPath { get; private set; }
        public string DiagramFolder { get; private set; }
        public string Filter { get; private set; }
        public bool Force { get; private set; }
        private FileSystemWatcher Watcher { get; set; }
        #endregion properties

        #region Instance-Constructors
        public FolderWatcher(string path, string diagramFolder, DiagramBuilderType diagramBuilderType, string filter, bool force)
        {
            Constructing();
            WatchPath = path;
            DiagramFolder = diagramFolder;
            DiagramBuilder = diagramBuilderType;
            Filter = filter;
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
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var path = Path.GetDirectoryName(e.FullPath);

            if (Path.Exists(path))
            {
                UMLDiagramBuilder diagram = DiagramBuilder switch
                {
                    DiagramBuilderType.Activity => new ActivityDiagramBuilder(path, DiagramFolder, Force),
                    DiagramBuilderType.Class => new ClassDiagramBuilder(path, DiagramFolder, Force),
                    DiagramBuilderType.Sequence => new SequenceDiagramBuilder(path, DiagramFolder, Force),
                    _ => throw new InvalidOperationException("Invalid diagram builder type."),
                };
                diagram.CreateFromPath();
            }
            Debug.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            Debug.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            Debug.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Debug.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }
        private void OnError(object source, ErrorEventArgs e)
        {
            Debug.WriteLine($"Error: {e.GetException()}");
        }
        #endregion Event-Handlers

        #region dispose
        public void Dispose()
        {
            Watcher.Dispose();
        }
        #endregion dispose
    }
}
