namespace Jurassic
{
    /// <summary>
    /// Contains stack frame properties which can be transformed when formatting the
    /// stack trace.
    /// </summary>
    public class StackFrameTransformContext
    {
        /// <summary>
        /// Gets or sets the line number. A value of <c>0</c> means the line number
        /// is unknown.
        /// </summary>
        public int Line {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the path of the javascript script file. A value of
        /// <c>null</c> means that the path is unknown.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the function. A value of <c>null</c> or
        /// the empty string (<c>""</c>) means that the path is unknown.
        public string Function
        {
            get;
            set;
        }
    }
}
