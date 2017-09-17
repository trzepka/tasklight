using System.Collections.Generic;

namespace jira_diagrams
{
    public class Task: Entity
    {
        public string Title { get; set; }
        public IEnumerable<Entity> Implements { get; set; }
    }
}