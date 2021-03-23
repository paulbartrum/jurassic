﻿using Jurassic.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Jurassic.TestSuiteRunner
{
    /// <summary>
    /// A test suite that tests newer ES features.
    /// Source: https://github.com/kangax/compat-table
    /// </summary>
    public class KangaxTestSuite : TestSuite
    {
        /// <summary>
        /// The ID of the test suite.
        /// </summary>
        public override string Id
        {
            get { return "kangax"; }
        }

        /// <summary>
        /// The name of the test suite.
        /// </summary>
        public override string Name
        {
            get { return "Kangax"; }
        }

        private class CompatTableEntry
        {
            public string group { get; set; }
            public string name { get; set; }
            public string detail { get; set; }
            public string script { get; set; }
            public bool Success { get; set; }
            public WorkerProcessResponse Response { get; set; }
        }

        /// <summary>
        /// Executes the test suite.
        /// </summary>
        public override void Run()
        {
            using (var pipeServer = new PipeServer(Id))
            {
                var dir = @"..\..\..\Kangax\";
                var globalScript = File.ReadAllText(Path.Combine(dir, "global.js"));
                var entries = JsonConvert.DeserializeObject<CompatTableEntry[]>(File.ReadAllText(Path.Combine(dir, "compat-table.json")));
                foreach (var testCase in entries)
                {
                    try
                    {
                        testCase.Response = Send(pipeServer, new WorkerProcessRequest
                        {
                            VariablesToReturn = new[] { "__asyncTestPassed" },
                            Script = globalScript + Environment.NewLine + $"(function () {{ {testCase.script} }})();"
                        });
                        if (testCase.Response.JsonResult == "true" || testCase.Response.Variables?["__asyncTestPassed"] == "true")
                            testCase.Success = true;
                        if (int.TryParse(testCase.Response.JsonResult, out int resultAsInteger) && resultAsInteger != 0)
                            testCase.Success = true;
                        if (testCase.Success == false &&
                            !testCase.name.StartsWith("generators") &&
                            !testCase.name.StartsWith("arrow functions") &&
                            !testCase.name.StartsWith("destructuring") &&
                            !testCase.name.StartsWith("spread syntax for iterable objects"))
                        {
                            Console.WriteLine($"{testCase.name} -- {testCase.detail}, result: {testCase.Response.JsonResult ?? $"{testCase.Response.ErrorType}: {testCase.Response.ErrorMessage}"}");
                        }

                        if (testCase.Success == false &&
                            (testCase.name.StartsWith("Symbol")))
                        {
                            Console.WriteLine(testCase.script);
                        }
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("*** Worker process crashed ***");
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
                foreach (var group in entries.GroupBy(e => e.group))
                {
                    Console.WriteLine($"**{group.Key}**|");
                    foreach (var test in group.GroupBy(e => e.name))
                    {
                        int successCount = test.Count(t => t.Success);
                        int totalCount = test.Count();

                        string status = ":x:";
                        if (successCount == 0)
                            status = ":x:";
                        else if (successCount == totalCount)
                            status = $":white_check_mark: {successCount}/{totalCount}";
                        else
                            status = $"{successCount}/{totalCount}";
                        Console.WriteLine($"&nbsp;&nbsp;{test.Key.Replace("_", "\\_")}|{status}");
                    }
                }
            }
        }

        /// <summary>
        /// Sends a request to a child process, then waits for the response.
        /// </summary>
        /// <param name="pipe"> The pipe to use to send the message. </param>
        /// <param name="request"> The request to send. </param>
        /// <returns> The response. </returns>
        private WorkerProcessResponse Send(PipeServer pipe, WorkerProcessRequest request)
        {
            return JsonConvert.DeserializeObject<WorkerProcessResponse>(pipe.Send(JsonConvert.SerializeObject(request)));
        }

        /// <summary>
        /// Processes worker process requests.
        /// </summary>
        /// <param name="request"> The request to process. </param>
        public override string ProcessRequest(string request)
        {
            var message = JsonConvert.DeserializeObject<WorkerProcessRequest>(request);
            var response = ProcessRequest(message);
            return JsonConvert.SerializeObject(response);
        }

        /// <summary>
        /// Processes worker process requests.
        /// </summary>
        /// <param name="request"> The request to process. </param>
        public WorkerProcessResponse ProcessRequest(WorkerProcessRequest request)
        {
            var engine = new ScriptEngine();

            var response = new WorkerProcessResponse();
            try
            {
                // Execute the provided script.
                object result = engine.Evaluate(request.Script);
                response.JsonResult = TypeConverter.ToString(JSONObject.Stringify(engine, result));
                if (request.VariablesToReturn != null)
                {
                    response.Variables = new Dictionary<string, string>();
                    foreach (var variableName in request.VariablesToReturn)
                        response.Variables[variableName] = engine.GetGlobalValue<string>(variableName);
                }
            }
            catch (Exception e)
            {
                // There was an error.
                response.ErrorType = e.GetType().Name;
                response.ErrorMessage = e.Message;
            }

            return response;
        }
    }
}