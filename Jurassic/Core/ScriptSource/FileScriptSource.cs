using System;
using System.IO;

namespace Jurassic
{
    /// <summary>
    /// Represents a script file.
    /// </summary>
    public class FileScriptSource : ScriptSource
    {
        private string path;

        /// <summary>
        /// Creates a new FileScriptSource instance.
        /// </summary>
        /// <param name="path"> The path of the script file. </param>
        public FileScriptSource(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            this.path = path;
        }

        /// <summary>
        /// Gets the path of the source file (either a path on the file system or a URL).  This
        /// can be <c>null</c> if no path is available.
        /// </summary>
        public override string Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// Returns a reader that can be used to read the source code for the script.
        /// </summary>
        /// <returns> A reader that can be used to read the source code for the script, positioned
        /// at the start of the source code. </returns>
        /// <remarks> If this method is called multiple times then each reader must return the
        /// same source code. </remarks>
        public override TextReader GetReader()
        {
            return new StreamReader(this.Path);
        }
    }
}
