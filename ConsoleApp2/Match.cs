namespace TestParser
{
    public class Match
    {
        public int Id { get; set; }
        public SportCategory Category { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; } = "<No URI>";
        public string Time { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }

        public string Coef1 { get; set; }
        public string CoefDraw { get; set; }
        public string Coef2 { get; set; }

        public override string ToString()
        {
            string matchInfo = string.Empty;

            matchInfo += $"{Uri}\n";
            matchInfo += $"{Id} — {Team1.Name}-{Team2.Name} — {Time}\n";

            if (Team1.Score != null)
                for (int i = 0; i < Team1.Score.Count; i++)
                    matchInfo += $"{Team1.Score[i]}:{Team2.Score[i]}|";

            return matchInfo;
        }
    }
}
