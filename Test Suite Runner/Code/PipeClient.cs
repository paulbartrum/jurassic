using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;

namespace Jurassic.TestSuiteRunner
{
    /// <summary>
    /// A class that can receive messages from a pipe.
    /// </summary>
    public static class PipeClient
    {
        /// <summary>
        /// Determines if the current process is a child process, and if so, starts processing messages.
        /// </summary>
        /// <param name="testSuiteFactory"> A callback that creates a test suite from the given
        /// client params string. </param>
        /// <returns> <c>true</c> if the current process is a child (worker) process; <c>false</c>
        /// otherwise. </returns>
        public static bool TryStartChildProcess(Func<string, TestSuite> testSuiteFactory)
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length >= 5 && commandLineArgs[1] == "--pipe")
            {
                string inPipeHandle = commandLineArgs[2];
                string outPipeHandle = commandLineArgs[3];
                TestSuite testSuite = testSuiteFactory(commandLineArgs[4]);

                // Disable the error reporting dialog.
                NativeMethods.SetErrorMode(
                    NativeMethods.SetErrorMode(0) |
                    ErrorModes.SEM_NOGPFAULTERRORBOX |
                    ErrorModes.SEM_FAILCRITICALERRORS |
                    ErrorModes.SEM_NOOPENFILEERRORBOX);

                try
                {
                    using (var inPipe = new AnonymousPipeClientStream(PipeDirection.In, inPipeHandle))
                    using (var reader = new BinaryReader(inPipe, Encoding.UTF8))
                    using (var outPipe = new AnonymousPipeClientStream(PipeDirection.Out, outPipeHandle))
                    using (var writer = new BinaryWriter(outPipe, Encoding.UTF8))
                    {
                        while (true)
                        {
                            // Read the incoming message.
                            var request = reader.ReadString();

                            // Call the callback.
                            var response = testSuite.ProcessRequest(request);

                            // Send the response message.
                            writer.Write(response);
                            writer.Flush();
                        }
                    }
                }
                catch (EndOfStreamException)
                {
                    // Expected when the pipe is closed by the server.
                }

                return true;
            }
            return false;
        }

        [Flags]
        private enum ErrorModes : uint
        {
            SYSTEM_DEFAULT = 0x0,
            SEM_FAILCRITICALERRORS = 0x0001,
            SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            SEM_NOOPENFILEERRORBOX = 0x8000
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            internal static extern ErrorModes SetErrorMode(ErrorModes mode);
        }
    }
}