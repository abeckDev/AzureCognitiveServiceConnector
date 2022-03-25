using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Collections.Generic;

namespace AbeckDev.AppliedAiConnector
{
    public static class AppliedAiConnectorFunction
    {
        [FunctionName("AppliedAiConnectorFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //General error handling
            try
            {
                //File Uri of the file to be browsed
                Uri fileUri = new Uri(req.Query["fileUri"]);

                //Read Cognitive Service Endpoint and Key from Environment Variable (Config)
                string endpoint = System.Environment.GetEnvironmentVariable("endpoint");
                string key = System.Environment.GetEnvironmentVariable("key");

                //Generate Credentials via SDK 
                AzureKeyCredential credential = new AzureKeyCredential(key);
                DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

                //Analyse the document and wait for its completion 
                AnalyzeDocumentOperation operation = await client.StartAnalyzeDocumentFromUriAsync(System.Environment.GetEnvironmentVariable("modelId"), fileUri);
                await operation.WaitForCompletionAsync();
                AnalyzeResult result = operation.Value;

                //Identify all found KeyValue pairs and write them in a dictionary 
                var keyValuePairs = result.KeyValuePairs;
                Dictionary<string, string> keyValueResults = new Dictionary<string, string>();

                //Generate the output to only contain key and value pairs without the boilerplate
                foreach (var item in keyValuePairs)
                {
                    try
                    {
                        keyValueResults.Add(item.Key.Content, item.Value.Content);
                    }
                    catch
                    {
                        //Do nothing in case there is an error with mapping one of the key value pairs 
                    }
                }

                //Return the generated view as a result
                return new OkObjectResult(JsonSerializer.Serialize(keyValueResults));
            }
            catch (Exception ex)
            {
                //In case something goes wrong, return BadRequest and express error. 
                return new BadRequestObjectResult(ex);
            }
            
        }
    }
}

