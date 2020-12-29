using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace SpeedrunComSharp
{
    public class Run : IElementWithID
    {
        public string ID { get; private set; }
        public Uri WebLink { get; private set; }
        public string GameID { get; private set; }
        public string LevelID { get; private set; }
        public string CategoryID { get; private set; }
        public RunVideos Videos { get; private set; }
        public string Comment { get; private set; }
        public RunStatus Status { get; private set; }
        public Player Player { get { return Players.FirstOrDefault(); } }
        public ReadOnlyCollection<Player> Players { get; internal set; }
        public DateTime? Date { get; private set; }
        public DateTime? DateSubmitted { get; private set; }
        public RunTimes Times { get; private set; }
        public RunSystem System { get; private set; }
        public Uri SplitsUri { get; private set; }
        public bool SplitsAvailable { get { return SplitsUri != null; } }
        public ReadOnlyCollection<VariableValue> VariableValues { get; private set; }

        #region Links

        internal Lazy<Game> game;
        internal Lazy<Category> category;
        private Lazy<Level> level;
        private Lazy<User> examiner;

        public Game Game { get { return game.Value; } }
        public Category Category { get { return category.Value; } }
        public Level Level { get { return level.Value; } }
        public Platform Platform { get { return System.Platform; } }
        public Region Region { get { return System.Region; } }
        public User Examiner { get { return examiner.Value; } }

        #endregion

        protected Run() { }

        internal static void Parse(Run run, SpeedrunComClient client, dynamic runElement)
        {
            //Parse Attributes
            IDictionary<string, dynamic> properties = runElement as IDictionary<string, dynamic>;

            run.ID = properties["id"] as string;
            run.WebLink = new Uri(properties["weblink"] as string);
            run.Videos = RunVideos.Parse(client, properties["videos"]) as RunVideos;
            run.Comment = properties["comment"] as string;
            run.Status = RunStatus.Parse(client, properties["status"]) as RunStatus;

            Func<dynamic, Player> parsePlayer = x => Player.Parse(client, x) as Player;

            if (properties["players"] is IEnumerable<dynamic>)
            {
                run.Players = client.ParseCollection(properties["players"], parsePlayer);
            }
            else if (properties["players"] is System.Collections.ArrayList && properties["players"].Count == 0)
            {
                run.Players = new List<Player>().AsReadOnly();
            }
            else
            {
                run.Players = client.ParseCollection(properties["players"].data, parsePlayer);
            }

            if (properties.ContainsKey("date"))
            {
                if (properties["date"] != null)
                    run.Date = DateTime.ParseExact(properties["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            if (properties.ContainsKey("submitted"))
            {
                if (runElement.submitted != null)
                    run.DateSubmitted = (DateTime)runElement.submitted;
            }

            run.Times = RunTimes.Parse(client, properties["times"]);
            run.System = RunSystem.Parse(client, properties["system"]);

            var splits = properties["splits"];

            if (splits != null)
            {
                run.SplitsUri = new Uri(splits.uri as string);
            }

            if (properties.ContainsKey("values"))
            {
                var valueProperties = properties["values"] as IDictionary<string, dynamic>;

                run.VariableValues = valueProperties.Select(x => VariableValue.ParseValueDescriptor(client, x) as VariableValue).ToList().AsReadOnly();
            }
            else
            {
                run.VariableValues = new List<VariableValue>().AsReadOnly();
            }

            //Parse Links

            if (properties["game"] is string)
            {
                run.GameID = properties["game"] as string;
                run.game = new Lazy<Game>(() => client.Games.GetGame(run.GameID));
            }
            else
            {
                var game = Game.Parse(client, properties["game"]["data"]) as Game;
                run.game = new Lazy<Game>(() => game);
                run.GameID = game.ID;
            }

            if (properties["category"] == null)
            {
                run.category = new Lazy<Category>(() => null);
            }
            else if (properties["category"] is string)
            {
                run.CategoryID = properties["category"] as string;
                run.category = new Lazy<Category>(() => client.Categories.GetCategory(run.CategoryID));
            }
            else
            {
                var category = Category.Parse(client, properties["category"]["data"]) as Category;
                run.category = new Lazy<Category>(() => category);

                if (category != null)
                {
                    run.CategoryID = category.ID;
                }
            }

            if (properties["level"] == null)
            {
                run.level = new Lazy<Level>(() => null);
            }
            else if (properties["level"] is string)
            {
                run.LevelID = properties["level"] as string;
                run.level = new Lazy<Level>(() => client.Levels.GetLevel(run.LevelID));
            }
            else
            {
                Level level = Level.Parse(client, properties["level"].data);

                run.level = new Lazy<Level>(() => level);

                if (level != null)
                {
                    run.LevelID = level.ID;
                }
            }

            if (properties.ContainsKey("platform"))
            {
                Platform platform = Platform.Parse(client, properties["platform"].data);

                run.System.platform = new Lazy<Platform>(() => platform);
            }

            if (properties.ContainsKey("region"))
            {
                Region region = Region.Parse(client, properties["region"].data);

                run.System.region = new Lazy<Region>(() => region);
            }

            if (!string.IsNullOrEmpty(run.Status.ExaminerUserID))
            {
                run.examiner = new Lazy<User>(() => client.Users.GetUser(run.Status.ExaminerUserID));
            }
            else
            {
                run.examiner = new Lazy<User>(() => null);
            }
        }

        public static Run Parse(SpeedrunComClient client, dynamic runElement)
        {
            var run = new Run();

            Parse(run, client, runElement);

            return run;
        }

        public override int GetHashCode()
        {
            return (ID ?? string.Empty).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Run))
            {
                return false;
            }

            return ID == (obj as Run).ID;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} in {2}", Game.Name, Category.Name, Times.Primary);
        }
    }
}
