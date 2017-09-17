using System.Collections.Generic;

namespace jira_diagrams
{
    internal class MilestoneData
    {
        public IEnumerable<Milestone> Milestones { get; set; }
        public IEnumerable<Task> Tasks { get; set; }
    }
}