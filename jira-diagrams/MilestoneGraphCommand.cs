using System;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace jira_diagrams
{
    internal class MilestoneGraphCommand
    {
        private string format;
        private readonly string inputFile;

        public MilestoneGraphCommand(string format, string inputFile)
        {
            this.format = format;
            this.inputFile = inputFile;
        }

        internal void Execute()
        {
            if(!File.Exists(inputFile))
            {
                Console.WriteLine($"File {inputFile} could not be found.");
                return;
            }
            var milestoneData = JsonConvert.DeserializeObject<MilestoneData>(File.ReadAllText(inputFile));
            var output = new StringBuilder();
            output.AppendLine("graph LR");
            var milestones = milestoneData.Milestones.ToDictionary(m => m.Id);

            foreach(var milestone in milestoneData.Milestones)
            {
                output.AppendLine($"milestone-{milestone.Id}(\"Milestone {milestone.Id} - {milestone.Title} - Due: {milestone.DueDate.ToString("yyyy-MM-dd")}\")");
                if(milestone.DependsOnMilestones != null)
                foreach(var dependsOn in milestone.DependsOnMilestones.Where(d => milestones.ContainsKey(d.Id)))
                {
                    output.AppendLine($"milestone-{dependsOn.Id} --> milestone-{milestone.Id}");
                }
            }
            foreach(var task in milestoneData.Tasks)
            {
                output.AppendLine($"task-{task.Id}(\"{task.Id} - {task.Title}\")");
                if(task.Implements != null)
                {
                    foreach(var implemented in task.Implements.Where(i => milestones.ContainsKey(i.Id)))
                    {
                        output.AppendLine($"task-{task.Id} --> milestone-{implemented.Id}");
                    }
                }

            }

            Console.WriteLine(output.ToString());
        }
    }
}