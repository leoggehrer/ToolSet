using PlantUML.ConApp;

namespace ToolSet.ConApp
{
    /// <summary>
    /// Represents the UMLWatcherApp class.
    /// </summary>
    internal partial class UMLWatcherApp : CommonTool.ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static UMLWatcherApp()
        {
            ClassConstructing();
            DocumentsPath = SourcePath;
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

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DocConversionApp"/> class.
        /// </summary>
        public UMLWatcherApp()
        {
            Constructing();
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called when the object is constructed.
        /// </summary>
        partial void Constructed();
        #endregion Instance-Constructors

        #region app properties
        /// <summary>
        /// Gets or sets the selected diagram type.
        /// </summary>
        public static DiagramBuilderType DiagramBuilder { get; set; } = DiagramBuilderType.Activity;
        /// <summary>
        /// Gets or sets the path to the documents.
        /// </summary>
        private static string DocumentsPath { get; set; }
        /// <summary>
        /// Gets or sets the list of folder watchers.
        /// </summary>
        private static List<FolderWatcher> Watchers { get; set; } = new();
        #endregion app properties

        #region overrides
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var mnuIdx = 0;
            var menuItems = new List<MenuItem>
            {
                CreateMenuSeparator(),
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Force", "Change force flag"),
                    Action = (self) => ChangeForce(),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Path", "Change source path"),
                    Action = (self) => 
                    {
                        var savePath = DocumentsPath;
                        
                        DocumentsPath = SelectOrChangeToSubPath(DocumentsPath, [ SourcePath ]);
                        if (savePath != DocumentsPath)
                        {
                            PageIndex = 0;
                        }
                    },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Builder", "Change diagram builder"),
                    Action = (self) => ChangeDiagramBuilder(),
                },
                CreateMenuSeparator(),
            };
            for (var i = 0; i < Watchers.Count; i++)
            {
                var watcher = Watchers[i];
                var path = watcher.WatchPath;
                var menuItem = new MenuItem
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Remove watcher", $"{path.Replace(DocumentsPath, string.Empty)} - Force={watcher.Force} - {watcher.DiagramBuilder}"),
                    Tag = "watcher",
                    Action = (self) => DeleteWatcher(self),
                    Params = new() { { "index", i } },
                    ForegroundColor = ConsoleColor.Yellow,
                };
                menuItems.Add(menuItem);
            }
            menuItems.Add(CreateMenuSeparator());

            var files = GetSourceCodePaths(DocumentsPath, ["*.cs"]).ToArray();

            menuItems.AddRange(CreatePageMenuItems(ref mnuIdx, files, (item, menuItem) =>
            {
                menuItem.Text = ToLabelText("Path", $"{item.Replace(DocumentsPath, string.Empty)}");
                menuItem.Tag = "path";
                menuItem.Action = (self) => CreateWatcher(self, Force);
                menuItem.Params = new() { { "path", item } };
            }));

            return [.. menuItems.Union(CreateExitMenuItems())];
        }

        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        /// <param name="sourcePath">The path of the solution.</param>
        protected override void PrintHeader()
        {
            var count = 0;
            var saveForeColor = ForegroundColor;

            ForegroundColor = ConsoleColor.Green;

            count = PrintLine(nameof(UMLWatcherApp));
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
            PrintLine($"Force flag:      {Force}");
            PrintLine($"Document path:   {DocumentsPath}");
            PrintLine($"Diagram builder: {DiagramBuilder} [{DiagramBuilderType.Activity}|{DiagramBuilderType.Class}|{DiagramBuilderType.Sequence}]");
            PrintLine();
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Changes the diagram builder type based on the current value of DiagramBuilder.
        /// </summary>
        private static void ChangeDiagramBuilder()
        {
            DiagramBuilder = DiagramBuilder.ToString().ToLower() switch
            {
                "activity" => DiagramBuilderType.Class,
                "class" => DiagramBuilderType.Sequence,
                "sequence" => DiagramBuilderType.Activity,
                _ => DiagramBuilder,
            };
        }
        /// <summary>
        /// Creates a new folder watcher based on the provided menu item and options.
        /// </summary>
        /// <param name="menuItem">The menu item containing the necessary parameters.</param>
        /// <param name="force">A boolean value indicating whether to force the creation of the watcher.</param>
        private static void CreateWatcher(MenuItem menuItem, bool force)
        {
            var pathOrFilePath = menuItem.Params[menuItem.Tag]?.ToString() ?? string.Empty;

            Watchers.Add(new FolderWatcher(pathOrFilePath, "Diagrams", DiagramBuilder, "*.cs", force));
        }
        /// <summary>
        /// Deletes a watcher from the list of watchers.
        /// </summary>
        /// <param name="menuItem">The menu item representing the watcher to be deleted.</param>
        private static void DeleteWatcher(MenuItem menuItem)
        {
            var index = (int)menuItem.Params["index"];

            Watchers[index].Dispose();
            Watchers.RemoveAt(index);
        }
        #endregion app methods
    }
}
