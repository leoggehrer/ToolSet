//@BaseCode
//MdStart
namespace ToolSet.ConApp
{
    /// <summary>
    /// Represents the main program class.
    /// </summary>
    partial class Program
    {
        #region Class-Constructors
        static Program()
        {
            ClassConstructing();
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();
        #endregion Class-Constructors

        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        static void Main(string[] args)
        {
            new ToolSetApp().Run(args);
        }

        #region app methods
        #endregion app methods
    }
 }
 //MdEnd