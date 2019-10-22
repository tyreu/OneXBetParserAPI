using System.Collections.Generic;

namespace TestParser
{
    public class Team
    {
        public string Name { get; set; }
        public List<int> Score { get; set; } = new List<int>();
        public Team(string name)
        {
            Name = name;
        }
    }
}
