using System.Collections.Generic;

namespace jira_diagrams
{
    public class Task: Entity
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public override string NodeId => $"task-{Id}";
    }
}