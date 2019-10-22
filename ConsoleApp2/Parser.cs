using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestParser
{
    public class Parser : IPrinter
    {
        public const string URL = "https://ua1xbet.com/ru/live/Football";
        public const string MobURL = "https://1xbetua.mobi";
        private HttpClient client;
        public IList<Match> Matches { get; set; } = new List<Match>();
        public ParserOptions Options { get; set; }
        public Parser(ParserOptions options)
        {
            try
            {
                Options = options;
                client = new HttpClient
                {
                    BaseAddress = new Uri(Options.Uri)
                };
            }
            catch (UriFormatException uriEx)
            {
                Console.WriteLine(uriEx.Message);
            }
        }

        public Match GetMatchById(int id) => Matches.FirstOrDefault(match => match.Id == id);

        public void ParseMatchesByAPI(int count)
        {
            WebClient webClient = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            webClient.QueryString.Add("sports", $"{(int)SportCategory.Football}");
            webClient.QueryString.Add("count", $"{count}");
            string result = webClient.DownloadString("https://ua1xbet.com/LiveFeed/BestGamesExtVZip");
            dynamic obj = JObject.Parse(result);
            dynamic matches = obj.Value;
            foreach (var match in matches)
            {
                var game = new Match()
                {
                    Name = match.L,
                    Id = match.I,
                    Team1 = new Team($"{match.O1}"),
                    Team2 = new Team($"{match.O2}"),
                    Category = (SportCategory)((int)match.SI),
                    Time = match.SC.CPS,
                    Uri=""
                };

                game.Coef1 = match.E.Count > 0 ? match.E[0].C : "-";//победа первой команды
                game.Coef2 = match.E.Count > 1 ? match.E[2].C : "-";//победа второй команды
                game.CoefDraw = match.E.Count > 2 ? match.E[1].C : "-";//ничья

                game.Team1.Score.Add(int.TryParse($"{match.SC.FS.S1}", out int parsedScore) ? parsedScore : 0);//счет текущего тайма первой команды
                game.Team2.Score.Add(int.TryParse($"{match.SC.FS.S2}", out parsedScore) ? parsedScore : 0);//счет текущего тайма второй команды
                foreach (var score in match.SC.PS)
                {
                    game.Team1.Score.Add(int.TryParse($"{score.Value.S1}", out parsedScore) ? parsedScore : 0);//счет первой команды
                    game.Team2.Score.Add(int.TryParse($"{score.Value.S2}", out parsedScore) ? parsedScore : 0);//счет второй команды
                }

                Matches.Add(game);
            }
            Print();
        }

        public void Print()
        {
            foreach (var match in Matches)
                Console.WriteLine(match);
        }

        public async Task<string> SignIn()
        {
            List<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("uLogin", Convert.ToBase64String(Encoding.UTF8.GetBytes(Options.Login))),
                new KeyValuePair<string, string>("uPassword", Convert.ToBase64String(Encoding.UTF8.GetBytes(Options.Password)))
            };
            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync("user/auth", content);
            //Console.WriteLine(response.RequestMessage);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return responseString;
            return null;
        }
    }
}
