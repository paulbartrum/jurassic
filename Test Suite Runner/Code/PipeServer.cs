using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Jurassic.TestSuite
{
    /// <summary>
    /// Creates a pipe server that can send messages and receive replies.
    /// </summary>
    /// <typeparam name="TOut"> The type of the outgoing message. </typeparam>
    /// <typeparam name="TIn"> The type of the incoming message. </typeparam>
    public class PipeServer<TOut, TIn> : IDisposable
    {
        private AnonymousPipeServerStream inPipe;
        private AnonymousPipeServerStream outPipe;
        private Process childProcess;

        /// <summary>
        /// Cleans up resources.
        /// </summary>
        public void Dispose()
        {
            if (this.childProcess != null)
            {
                this.childProcess.Dispose();
                this.childProcess = null;
                this.inPipe.Dispose();
                this.inPipe = null;
                this.outPipe.Dispose();
                this.outPipe = null;
            }
        }

        /// <summary>
        /// Determines if the current process is a child process.
        /// </summary>
        /// <param name="childInPipeHandle"> If the return value is true, this is set to the handle
        /// for the pipe for incoming messages. </param>
        /// <param name="childOutPipeHandle"> If the return value is true, this is set to the
        /// handle for the pipe for outgoing messages. </param>
        /// <returns> <c>true</c> if the current process is a child (worker) process; <c>false</c>
        /// otherwise. </returns>
        public static bool IsChildProcess(out string childInPipeHandle, out string childOutPipeHandle)
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length >= 4 && commandLineArgs[1] == "--pipe")
            {
                childInPipeHandle = commandLineArgs[2];
                childOutPipeHandle = commandLineArgs[3];
                return true;
            }
            childInPipeHandle = null;
            childOutPipeHandle = null;
            return false;
        }

        /// <summary>
        /// Sends a message to the child process.
        /// </summary>
        /// <param name="message"> The message to send. </param>
        /// <returns> A response message from the child process. </returns>
        public TIn Send(TOut message)
        {
            // Star the child process if it's not running.
            if (this.childProcess == null)
                StartChildProcess();

            try
            {
                // Send the message.
                var writer = new BinaryWriter(this.outPipe);    // Note: disposing closes the stream.
                writer.Write(JsonConvert.SerializeObject(message));
                writer.Flush();

                // Read the incoming response.
                var reader = new BinaryReader(this.inPipe);     // Note: disposing closes the stream.
                return JsonConvert.DeserializeObject<TIn>(reader.ReadString());
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

            // Start a copy of this EXE and pass it the pipe handle.
            var childProcess = new Process();
            childProcess.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
            childProcess.StartInfo.Arguments = $"--pipe {this.outPipe.GetClientHandleAsString()} {this.inPipe.GetClientHandleAsString()}";
            childProcess.StartInfo.UseShellExecute = false;
            childProcess.Start();
            this.childProcess = childProcess;

            // Clean up.
            this.inPipe.DisposeLocalCopyOfClientHandle();
            this.outPipe.DisposeLocalCopyOfClientHandle();
        }
    }

}
