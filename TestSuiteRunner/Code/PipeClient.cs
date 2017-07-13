using System;
using System.IO;
using System.IO.Pipes;
using Newtonsoft.Json;

namespace Jurassic.TestSuite
{
    /// <summary>
    /// Listens for messages on a pipe from the parent process and sends responses.
    /// </summary>
    /// <typeparam name="TIn"> The type of the incoming message. </typeparam>
    /// <typeparam name="TOut"> The type of the outgoing message. </typeparam>
    public class PipeClient<TIn, TOut>
    {
        /// <summary>
        /// Starts reading from the pipe with the given handle.
        /// </summary>
        /// <param name="inPipeHandle"> The pipe handle identifier for incoming data. </param>
        /// <param name="outPipeHandle"> The pipe handle identifier for outgoing data. </param>
        /// <param name="callback"> The callback that handles incoming messages. </param>
        public static void Start(string inPipeHandle, string outPipeHandle, Func<TIn, TOut> callback)
        {
            try
            {
                using (var inPipe = new AnonymousPipeClientStream(PipeDirection.In, inPipeHandle))
                using (var reader = new BinaryReader(inPipe))
                using (var outPipe = new AnonymousPipeClientStream(PipeDirection.Out, outPipeHandle))
                using (var writer = new BinaryWriter(outPipe))
                {
                    while (true)
                    {
                        // Read the incoming message.
                        var message = JsonConvert.DeserializeObject<TIn>(reader.ReadString());

                        // Call the callback.
                        var response = callback(message);

                        // Send the response message.
                        writer.Write(JsonConvert.SerializeObject(response));
                        writer.Flush();
                    }
                }
            }
            catch (EndOfStreamException)
            {
                // Expected when the pipe is closed by the server.
            }
        }
    }

}
