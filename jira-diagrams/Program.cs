using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.IO;

namespace jira_diagrams
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "Jira diagrams";
            app.HelpOption("-?|-h|--help");

            app.OnExecute(() => {
                Console.WriteLine("Hello World!");
                return 0;
            });

            app.Command("milestone", command =>
            {
                command.Description = "Generate dependecy graph based on milestones";
//                var type = command.Option("-t|--type <diagram-type>", "Diagram type", CommandOptionType.SingleValue);
                var inputFile = command.Option("-f|--file <file-path>", "Path to a json file with input data", CommandOptionType.SingleValue);
                command.OnExecute(() =>
                {
                    //if(!type.HasValue())
                    //{
                    //    Console.WriteLine("Type is required");
                    //    return -1;
                    //}
                    string milestoneData = null;
                    if (inputFile.HasValue())
                    {
                        if (!File.Exists(inputFile.Value()))
                        {
                            Console.WriteLine($"File {inputFile} could not be found.");
                            return -1;
                        }
                        milestoneData = File.ReadAllText(inputFile.Value());
                    }
                    else
                    {
                        milestoneData = Console.In.ReadToEnd();
                    }
                    new MilestoneGraphCommand(null, JsonConvert.DeserializeObject<MilestoneData>(milestoneData)).Execute();
                    return 0;
                });
            });

            app.Execute(args);
        }
    }
}
