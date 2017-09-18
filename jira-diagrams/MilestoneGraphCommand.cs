using System;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace jira_diagrams
{
    internal class MilestoneGraphCommand
    {
        private string format;
        private readonly MilestoneData milestoneData;

        public MilestoneGraphCommand(string format, MilestoneData milestoneData)
        {
            this.format = format;
            this.milestoneData = milestoneData;
        }

        internal void Execute()
        {
            var output = new StringBuilder();
            output.AppendLine("graph LR");
            var existingTasks = milestoneData.Tasks.ToDictionary(t => t.Id);
            var existingNodes = milestoneData.Tasks.Cast<Entity>().Union(milestoneData.Milestones.Cast<Entity>()).ToLookup(x => x.Id);
            var statuses = milestoneData.Tasks.Select(t => t.Status).Distinct().Select((s, i) => new { Name = s, NodeId = $"status-{i}" });
            var milestones = milestoneData.Milestones.Select(m => new { Milestone = m, ShownStatuses = statuses.ToDictionary(s => s.Name, x => new { NodeId = x.NodeId, Name = x.Name }) });
            var shownStatuses = new HashSet<string>();
            foreach (var milestone in milestones)
            {
                var due = milestone.Milestone.DueDate.HasValue ? $" - Due: {milestone.Milestone.DueDate.Value.ToString("yyyy-MM-dd")}" : String.Empty;
                output.AppendLine($"{milestone.Milestone.NodeId}(\"Milestone {milestone.Milestone.Id} - {milestone.Milestone.Title}{due}\")");
            }
            foreach (var link in milestones.SelectMany(m => m.Milestone.DependsOn.Select(x => new { Milestone = m.Milestone, DependsOn = x, ShownStatuses = m.ShownStatuses }).Where(d => existingNodes.Contains(d.DependsOn.Id))))
            {
                if (existingTasks.ContainsKey(link.DependsOn.Id))
                {
                    var task = existingTasks[link.DependsOn.Id];
                    output.AppendLine($"{task.NodeId}(\"{task.Id} - {task.Title}\")");
                }
                var node = existingNodes[link.DependsOn.Id].First();
                if (existingTasks.ContainsKey(node.Id))
                {
                    var status = link.ShownStatuses[existingTasks[node.Id].Status];
                    var statusMilestoneId = $"{status.NodeId}-{link.Milestone.NodeId}";
                    if (!shownStatuses.Contains(statusMilestoneId))
                    {
                        output.AppendLine($"{status.NodeId}-{link.Milestone.NodeId}(\"{status.Name}\") --> {link.Milestone.NodeId}");
                        shownStatuses.Add(statusMilestoneId);
                    }
                    output.AppendLine($"{node.NodeId} --> {status.NodeId}-{link.Milestone.NodeId}");

                }
                else
                {
                    output.AppendLine($"{node.NodeId} --> {link.Milestone.NodeId}");
                }
            }

            Console.WriteLine(output.ToString());
        }
    }
}