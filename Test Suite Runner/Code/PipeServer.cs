using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

namespace Jurassic.TestSuiteRunner
{
    /// <summary>
    /// A class that can send messages over a pipe.
    /// </summary>
    public class PipeServer : IDisposable
    {
        private string clientParams;
        private AnonymousPipeServerStream inPipe;
        private AnonymousPipeServerStream outPipe;
        private Process childProcess;

        /// <summary>
        /// Initializes a new pipe server.
        /// </summary>
        /// <param name="clientParams"> One-time initialization data that will be passed to the
        /// child process. </param>
        public PipeServer(string clientParams)
        {
            this.clientParams = clientParams;
        }

        /// <summary>
        /// Cleans up resources.
        /// </summary>
        public void Dispose()
        {
            if (childProcess != null)
            {
                childProcess.Dispose();
                childProcess = null;
                inPipe.Dispose();
                inPipe = null;
                outPipe.Dispose();
                outPipe = null;
            }
        }

        /// <summary>
        /// Sends a message to the child process.
        /// </summary>
        /// <typeparam name="TRequest"> The type of the outgoing request message. </typeparam>
        /// <typeparam name="TResponse"> The type of the incoming response message. </typeparam>
        /// <param name="message"> The message to send. </param>
        /// <returns> A response message from the child process. </returns>
        public string Send(string message)
        {
            // Star the child process if it's not running.
            if (this.childProcess == null)
                StartChildProcess();

            try
            {
                // Send the message.
                var writer = new BinaryWriter(this.outPipe);    // Note: disposing closes the stream.
                writer.Write(message);
                writer.Flush();

                // Read the incoming response.
                var reader = new BinaryReader(this.inPipe);     // Note: disposing closes the stream.
                return reader.ReadString();
            }
            catch (IOException)
            {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// Starts the child (worker) process.
        /// </summary>
        private void StartChildProcess()
        {
            this.inPipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            this.outPipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

            // Start a copy of this process and pass it the pipe handle.
            var commandLineArgs = Environment.GetCommandLineArgs();
            var childProcess = new Process();
            childProcess.StartInfo.FileName = "dotnet";
            childProcess.StartInfo.Arguments = $"\"{commandLineArgs[0]}\" --pipe {this.outPipe.GetClientHandleAsString()} {this.inPipe.GetClientHandleAsString()} \"{clientParams}\"";
            childProcess.StartInfo.UseShellExecute = false;
            childProcess.Start();
            this.childProcess = childProcess;

            // Clean up.
            this.inPipe.DisposeLocalCopyOfClientHandle();
            this.outPipe.DisposeLocalCopyOfClientHandle();
        }
    }
}