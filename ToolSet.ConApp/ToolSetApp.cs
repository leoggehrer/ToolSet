namespace ToolSet.ConApp
{
    /// <summary>
    /// Represents the main application class for the ToolSet application.
    /// </summary>
    internal partial class ToolSetApp : CommonTool.ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static ToolSetApp()
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

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DocConversionApp"/> class.
        /// </summary>
        public ToolSetApp()
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
                    Text = ToLabelText("UML-Builder", "Runs the PlantUML builder tool."),
                    Action = (self) => { new PlantUML.ConApp.PlantUMLApp().Run([]); },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("DOC-Conversion", "Runs the document conversion tool."),
                    Action = (self) => { new DocConversion.ConApp.DocConversionApp().Run([]); },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("UML-Watcher", "Watches a folder for changes and creates UML diagrams."),
                    Action = (self) => { new UMLWatcherApp().Run([]); },
                },
            };
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

            count = PrintLine(nameof(ToolSet));
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
        }
        #endregion overrides

        #region app methods
        #endregion app methods
    }
}
