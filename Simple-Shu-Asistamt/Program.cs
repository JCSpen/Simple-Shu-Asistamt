﻿using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.Assistant.v2;
using IBM.Watson.Assistant.v2.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Reflection.Emit;
using IBM.Cloud.SDK.Core.Http;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using Microsoft.Win32.SafeHandles;
using System.Runtime.CompilerServices;

namespace Simple_Shu_Asistamt
{
    internal class Program
    {
        // Simple Shu assistant
        //"apikey": "0LTlYh3-Kt6uIe1eQ8ytijsuzdnEKq_jUs8pff49fXeM",
        //"iam_apikey_description": "Auto-generated for key crn:v1:bluemix:public:conversation:eu-gb:a/f5532d34a1324f0fa245f2c4399ae5ea:9105472d-0990-4acc-a349-661d4607d608:resource-key:c39c4170-0c37-48ad-bc10-a2bacb61cfb1",
        //"iam_apikey_name": "Auto-generated service credentials",
        //"iam_role_crn": "crn:v1:bluemix:public:iam::::serviceRole:Manager",
        //"iam_serviceid_crn": "crn:v1:bluemix:public:iam-identity::a/f5532d34a1324f0fa245f2c4399ae5ea::serviceid:ServiceId-455ef5af-5f8b-49ca-a533-7cbf3f807127",
        //"url": "https://api.eu-gb.assistant.watson.cloud.ibm.com/instances/9105472d-0990-4acc-a349-661d4607d608"

        static void Main(string[] args)
        {
            // extract the source titles and links


            string userQuery = " ";
            IamAuthenticator authenticator = new IamAuthenticator(
            apikey: "0LTlYh3-Kt6uIe1eQ8ytijsuzdnEKq_jUs8pff49fXeM"
            );

            AssistantService assistant = new AssistantService("2023-01-17", authenticator);
            assistant.SetServiceUrl("https://api.eu-gb.assistant.watson.cloud.ibm.com/instances/9105472d-0990-4acc-a349-661d4607d608");

            var result = assistant.CreateSession(
            assistantId: "74e78bca-b878-4493-92ad-f31e048b92cd"
            );
            List<string> sourceTitles = new List<string>();
            List<string> sourceLinks = new List<string>();
            List<string> lables = new List<string>();
            bool resolved = false;
            JArray optionsArray = null;
            var sessionId = result.Result.SessionId;
            string titleOfText = "";
            string mainTitle = "";
            bool overrideReset = false;


            while (userQuery != "0")
            {
                if (overrideReset) //Clears Console and outputs message when exception is found
                {
                    Console.Clear();
                    Console.WriteLine("Sorry I did not catch that please try again!");
                    overrideReset= false;
                }
                Console.WriteLine("Chat with me : \n");
                userQuery = Console.ReadLine();


                var result2 = assistant.Message(
                 assistantId: "74e78bca-b878-4493-92ad-f31e048b92cd",
                 sessionId,
                 input: new MessageInput()
                 {
                     Text = userQuery
                 }
                 );
                //JObject output = JObject.Parse(result2.Response);
                //string txt = (string)output["output"]["generic"][0]["text"];
                //txt = txt.Replace("\n", "\n").Replace("- ", "");
                //var ouput = new {result2.Response};  // replace with the actual output



                // parse the JSON response
                JObject response = JObject.Parse(result2.Response);
                //Console.WriteLine(result2.Response);
                // extract the main title

                try //Fetch values from response
                {
                    titleOfText = response?["output"]?["generic"]?[0]?["text"]?.ToString();


                    mainTitle = response?["output"]?["generic"]?[0]?["title"]?.ToString();
                }
                catch (Exception e) //If a value is out of range or not found, the override statement is called
                {
                    overrideReset = true;
                }

                //for (int i = 0; i < response["output"]["generic"].Count(); i++)
                //{

                //     titleOfText = (string)suggestions[0]["value"]["input"]["suggestion_id"];
                //     lables.Add((string)suggestions[0]["label"]);
                //    titleOfText = response["output"]["generic"][i]["title"].Value<string>();
                //}

                if (titleOfText != null && titleOfText.Length > 20) //Finds links by title length
                {
                    int index = titleOfText.IndexOf(':');

                    titleOfText = index >= 0 ? titleOfText.Substring(0, index) : titleOfText;
                }



                Console.WriteLine(mainTitle);
                Console.WriteLine(titleOfText);
                linkSeprater(response);

                //sourceTitles.RemoveAll(s => s.Contains("Is there anything else I can help you with"));

                // print the results


                //for (int i = 0; i < sourceTitles.Count; i++)
                //{
                //    Console.WriteLine(sourceTitles[i] + "\n" + sourceLinks[i]);
                //}
                try //Fetches values from JSON Response
                {

                    resolved = false;
                    for (int i = 0; i < response?["output"]?["generic"]?[0]?["suggestions"]?.Count(); i++)
                    {
                        if (!resolved)
                        {
                            resolved = true;
                            Console.WriteLine("Select one of the following options \n");
                        }
                        string lablles = response?["output"]?["generic"]?[0]?["suggestions"]?[i]?["label"].ToString();
                        Console.WriteLine(lablles + "\n");
                    }

                    for (int i = 0; i < response?["output"]?["generic"]?.Count(); i++)
                    {
                        optionsArray = response?["output"]?["generic"]?[i]?["options"] as JArray;
                        if (optionsArray != null)
                        {
                            JArray textsArray = response?["output"]?["generic"]?[i]?["text"] as JArray;
                            int count = 0;
                            if (!resolved && count !=2)
                            { 
                                Console.WriteLine("Please select one of the following options: \n");
                                resolved = true;
                            }
                            foreach (JToken option in optionsArray)
                            {
                                string extractedText = (string)option["label"];
                                if (extractedText.ToLower() == "yes" || extractedText.ToLower() == "no")
                                {
                                    count++;
                                }
                                if(count == 1)
                                {
                                    Console.WriteLine("Did this help you? \n");
                                }
                                Console.WriteLine(extractedText +"\n"); // Output: "I have no knowledge" and "I have some knowledge"
                            }

                        }

                    }
                }

                

                catch (Exception e)
                {

                }





            }
            void linkSeprater(JObject response) //Formats links and outputs it with the title
            {
                for (int i = 0; i < response["output"]["generic"].Count(); i++)
                {
                    string inputText = response?["output"]?["generic"]?[i]?["text"]?.ToString();
                    string linkPattern = @"\[(.*?)\]\((.*?)\)";
                    Regex regex = new Regex(linkPattern);
                    if (inputText != null)
                    {
                        MatchCollection matches = regex.Matches(inputText);
                        foreach (Match match in matches)
                        {
                            string link = Regex.Match(match.Value, linkPattern).Groups[2].Value;
                            string title = match.Groups[1].Value;
                            Console.WriteLine(title + "\n");
                            Console.WriteLine(link + "\n");

                        }
                    }
                }
            }
          
        }
    }
}

