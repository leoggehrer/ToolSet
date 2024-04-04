//@BaseCode
//MdStart
using PlantUML.ConApp;

namespace ToolSet.ConApp
{
    public partial class UMLWatcher : FolderWatcher
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the class.
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static UMLWatcher()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called during the construction of the class.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// Represents a method that is called when a class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region properties
        /// <summary>
        /// Gets or sets the type of diagram builder.
        /// </summary>
        public DiagramBuilderType DiagramBuilder { get; private set; }
        /// <summary>
        /// Gets or sets the folder path for diagrams.
        /// </summary>
        public string DiagramFolder { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether to create a complete diagram.
        /// </summary>
        public bool CreateCompleteDiagram { get; set; }
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
        /// Initializes a new instance of the <see cref="FolderWatcher"/> class with the specified parameters.
        /// </summary>
        /// <param name="path">The path of the folder to watch.</param>
        /// <param name="diagramFolder">The folder where the diagrams will be saved.</param>
        /// <param name="diagramBuilderType">The type of diagram builder to use.</param>
        /// <param name="filter">The filter for the files to watch.</param>
        /// <param name="createCompleteDiagram">A flag indicating whether to create a complete diagram.</param>
        /// <param name="force">A flag indicating whether to force the creation of diagrams.</param>
        public UMLWatcher(string path, string diagramFolder, DiagramBuilderType diagramBuilderType, string filter, bool createCompleteDiagram, bool force)
            : base(path, filter )
        {
            Constructing();

            DiagramFolder = diagramFolder;
            DiagramBuilder = diagramBuilderType;
            CreateCompleteDiagram = createCompleteDiagram;
            Force = force;

            Constructed();
        }
        partial void Constructing();
        partial void Constructed();
        #endregion Instance-Constructors

        #region Event-Handlers
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

                var path = Path.GetDirectoryName(e.FullPath);

                if (Path.Exists(path))
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        while (IsPathAccessible(path, [Filter]) == false)
                        {
                            Thread.Sleep(500);
                        }
                    }

                    UMLDiagramBuilder diagram = DiagramBuilder switch
                    {
                        DiagramBuilderType.Activity => new ActivityDiagramBuilder(path, DiagramFolder, CreateCompleteDiagram, Force),
                        DiagramBuilderType.Class => new ClassDiagramBuilder(path, DiagramFolder, CreateCompleteDiagram, Force),
                        DiagramBuilderType.Sequence => new SequenceDiagramBuilder(path, DiagramFolder, CreateCompleteDiagram, Force),
                        _ => throw new InvalidOperationException("Invalid diagram builder type."),
                    };
                    diagram.CreateFromPath();
                }
                InProcess = false;
            }
        }
        #endregion Event-Handlers
    }
}
//MdEnd