//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace Jurassic
//{
//    /// <summary>
//    /// Represents a stack trace for javascript functions.
//    /// </summary>
//    public class ExecutionStack
//    {
//        [ThreadStatic]
//        private static ExecutionStack instance;

//        private struct RegisteredFunction
//        {
//            public string Name;
//            public Func<int, DebugInfoExpression> SequencePointCallback;
//        }

//        private Dictionary<int, RegisteredFunction> registeredFunctions = new Dictionary<int, RegisteredFunction>();

//        public static ExecutionStack Current
//        {
//            get
//            {
//                if (instance == null)
//                    instance = new ExecutionStack();
//                return instance;
//            }
//        }

//        /// <summary>
//        /// Registers a javascript function.
//        /// </summary>
//        /// <param name="functionName"> The name of the function.  This can be different from the
//        /// .NET method name. An empty string indicates the function is global or eval code. </param>
//        /// <param name="method"> The method that implements the function. </param>
//        /// <param name="name"> A callback </param>
//        public void RegisterFunction(string name, MethodBase method, Func<int, DebugInfoExpression> sequencePointCallback)
//        {
//            if (name == null)
//                throw new ArgumentNullException("name");
//            if (method == null)
//                throw new ArgumentNullException("method");
//            this.registeredFunctions.Add(method.MetadataToken, new RegisteredFunction() { Name = name, SequencePointCallback = sequencePointCallback });
//        }

//        /// <summary>
//        /// Gets the current stack trace.
//        /// </summary>
//        /// <returns> The current stack trace. </returns>
//        public string GetStackTrace()
//        {
//            var result = new System.Text.StringBuilder();
//            var stackTrace = new StackTrace();
//            foreach (var frame in stackTrace.GetFrames())
//            {
//                // Check if the frame is inside a registered function.
//                var method = frame.GetMethod();
//                if (this.registeredFunctions.ContainsKey(method.MetadataToken))
//                {
//                    var registeredFunction = this.registeredFunctions[method.MetadataToken];
//                    var debugInfo = registeredFunction.SequencePointCallback(frame.GetILOffset());
//                    if (debugInfo != null)
//                    {
//                        if (result.Length > 0)
//                            result.Append(Environment.NewLine);
//                        if (registeredFunction.Name == string.Empty)
//                            result.AppendFormat("    at {0}:{1}", debugInfo.Document.FileName, debugInfo.StartLine);
//                        else
//                            result.AppendFormat("    at {0} ({1}:{2})", registeredFunction.Name, debugInfo.Document.FileName, debugInfo.StartLine);
//                    }
//                }
//            }
//            return result.ToString();
//        }
//    }
//}
