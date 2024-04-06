﻿//@BaseCode
//MdStart
using PlantUML.ConApp;

namespace ToolSet.ConApp
{
    /// <summary>
    /// Represents the SourceWatcherApp class.
    /// </summary>
    internal partial class SourceWatcherApp : CommonTool.ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static SourceWatcherApp()
        {
            ClassConstructing();
            TargetPath = ProjectsPath = SourcePath;
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
        public SourceWatcherApp()
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
        /// Gets or sets the path to the documents.
        /// </summary>
        private static string ProjectsPath { get; set; }
        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        public static string TargetPath { get; set; }
        /// <summary>
        /// Gets or sets the selected diagram type.
        /// </summary>
        public static DiagramBuilderType DiagramBuilder { get; set; } = DiagramBuilderType.All;
        /// <summary>
        /// Gets or sets the folder path for diagrams.
        /// </summary>
        public static string DiagramFolder { get; set; } = "diagrams";
        /// <summary>
        /// Gets or sets the list of folder uml-watchers.
        /// </summary>
        private static List<SourceWatcher> Watchers { get; set; } = new();
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
                    Text = ToLabelText("Depth", "Change max sub path depth"),
                    Action = (self) => ChangeMaxSubPathDepth(),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Projects path", "Change projects path"),
                    Action = (self) =>
                    {
                        var savePath = ProjectsPath;

                        ProjectsPath = SelectOrChangeToSubPath(ProjectsPath, MaxSubPathDepth, [ SourcePath ]);
                        if (savePath != ProjectsPath)
                        {
                            PageIndex = 0;
                        }
                        if (savePath == TargetPath)
                        {
                            TargetPath = ProjectsPath;
                        }
                    },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Target path", "Change target path"),
                    Action = (self) => 
                    {
                        var savePath = TargetPath;
                        
                        TargetPath = SelectOrChangeToSubPath(TargetPath, MaxSubPathDepth, [ SourcePath ]);
                        if (savePath != TargetPath)
                        {
                            PageIndex = 0;
                        }
                    },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Folder", "Change diagram folder"),
                    Action = (self) => ChangeDiagramFolder(),
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
                    Text = ToLabelText("Remove watcher", $"{watcher.TargetPath} - Force={watcher.Force}"),
                    Tag = "watcher",
                    Action = (self) => DeleteWatcher(self),
                    Params = new() { { "index", i } },
                    ForegroundColor = ConsoleColor.Yellow,
                };
                menuItems.Add(menuItem);
            }
            menuItems.Add(CreateMenuSeparator());

            if (mnuIdx % 10 != 0)
            {
                mnuIdx += 10 - (mnuIdx % 10);
            }

            var files = GetSourceCodePaths(ProjectsPath, ["*.cs"]).ToArray();

            menuItems.AddRange(CreatePageMenuItems(ref mnuIdx, files, (item, menuItem) =>
            {
                var subPath = item.Replace(ProjectsPath, string.Empty);
                var targetPath = item.Replace(ProjectsPath, TargetPath);

                menuItem.Text = ToLabelText("Path", $"{subPath}");
                menuItem.Tag = "paths";
                menuItem.Action = (self) => CreateWatcher(self, Force);
                menuItem.Params = new() { { "sourcePath", item }, { "targetPath", targetPath } };
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

            count = PrintLine(nameof(SourceWatcherApp));
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
            PrintLine($"Force flag:       {Force}");
            PrintLine($"Max. path depth:  {MaxSubPathDepth}");
            PrintLine($"Projects path:    {ProjectsPath}");
            PrintLine();
            PrintLine($"Target path:      {TargetPath}");
            PrintLine($"Diagram folder:   {DiagramFolder}");
            PrintLine($"Diagram builder:  {DiagramBuilder} [{DiagramBuilderType.Activity}|{DiagramBuilderType.Class}|{DiagramBuilderType.Sequence}]");
            PrintLine();
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Changes the diagram folder name.
        /// </summary>
        private static void ChangeDiagramFolder()
        {
            DiagramFolder = ReadLine("Enter the diagram folder name: ").Trim();
        }
        /// <summary>
        /// Changes the diagram builder type based on the current value of DiagramBuilder.
        /// </summary>
        private static void ChangeDiagramBuilder()
        {
            DiagramBuilder = DiagramBuilder.ToString().ToLower() switch
            {
                "all" => DiagramBuilderType.Activity,
                "activity" => DiagramBuilderType.Class,
                "class" => DiagramBuilderType.Sequence,
                "sequence" => DiagramBuilderType.All,
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
            var sourcePath = menuItem.Params["sourcePath"]?.ToString() ?? string.Empty;
            var targetPath = menuItem.Params["targetPath"]?.ToString() ?? string.Empty;

            Watchers.Add(new SourceWatcher(sourcePath, targetPath, "*.cs", force));
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
//MdEnd