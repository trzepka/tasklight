using System;
using System.Collections.Generic;

namespace jira_diagrams
{
    public class Milestone: Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<Entity> DependsOnMilestones { get; set; }
        public DateTime DueDate { get; set; }
    }
}