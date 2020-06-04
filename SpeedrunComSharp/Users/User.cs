using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace SpeedrunComSharp
{
    public class User : IElementWithID
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string JapaneseName { get; private set; }
        public Uri WebLink { get; private set; }
        public UserNameStyle NameStyle { get; private set; }
        public UserRole Role { get; private set; }
        public DateTime? SignUpDate { get; private set; }
        public Location Location { get; private set; }

        public Uri TwitchProfile { get; private set; }
        public Uri HitboxProfile { get; private set; }
        public Uri YoutubeProfile { get; private set; }
        public Uri TwitterProfile { get; private set; }
        public Uri SpeedRunsLiveProfile { get; private set; }

        #region Links

        private Lazy<ReadOnlyCollection<Record>> personalBests;

        public IEnumerable<Run> Runs { get; private set; }
        public IEnumerable<Game> ModeratedGames { get; private set; }
        public ReadOnlyCollection<Record> PersonalBests { get { return personalBests.Value; } }

        #endregion

        private User() { }

        private static UserRole parseUserRole(string role)
        {
            switch (role)
            {
                case "banned":
                    return UserRole.Banned;
                case "user":
                    return UserRole.User;
                case "trusted":
                    return UserRole.Trusted;
                case "moderator":
                    return UserRole.Moderator;
                case "admin":
                    return UserRole.Admin;
                case "programmer":
                    return UserRole.Programmer;
                case "contentmoderator":
                    return UserRole.ContentModerator;
            }

            throw new ArgumentException("role");
        }

        public static User Parse(SpeedrunComClient client, dynamic userElement)
        {
            var user = new User();

            var properties = userElement as IDictionary<string, dynamic>;

            //Parse Attributes

            user.ID = properties["id"] as string;
            user.WebLink = new Uri(properties["weblink"] as string);
            user.NameStyle = UserNameStyle.Parse(client, properties["name-style"]) as UserNameStyle;
            user.Role = parseUserRole(properties["role"] as string);

            var nameProperties = properties["names"] as IDictionary<string, dynamic>;

            user.Name = nameProperties["international"] as string;
            user.JapaneseName = nameProperties["japanese"] as string;

            if (properties.ContainsKey("signup"))
            {
                user.SignUpDate = DateTime.Parse(properties["signup"].ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            }

            user.Location = Location.Parse(client, properties["location"]) as Location;

            var twitchLink = properties["twitch"];
            if (twitchLink != null)
                user.TwitchProfile = new Uri(twitchLink["uri"] as string);

            var hitboxLink = properties["hitbox"];
            if (hitboxLink != null)
                user.HitboxProfile = new Uri(hitboxLink["uri"] as string);

            var youtubeLink = properties["youtube"];
            if (youtubeLink != null)
                user.YoutubeProfile = new Uri(youtubeLink["uri"] as string);

            var twitterLink = properties["twitter"];
            if (twitterLink != null)
                user.TwitterProfile = new Uri(twitterLink["uri"] as string);

            var speedRunsLiveLink = properties["speedrunslive"];
            if (speedRunsLiveLink != null)
                user.SpeedRunsLiveProfile = new Uri(speedRunsLiveLink["uri"] as string);

            //Parse Links

            user.Runs = client.Runs.GetRuns(userId: user.ID);
            user.ModeratedGames = client.Games.GetGames(moderatorId: user.ID);
            user.personalBests = new Lazy<ReadOnlyCollection<Record>>(() =>
                {
                    var records = client.Users.GetPersonalBests(userId: user.ID);
                    var lazy = new Lazy<User>(() => user);

                    foreach (var record in records)
                    {
                        var player = record.Players.FirstOrDefault(x => x.UserID == user.ID);
                        if (player != null)
                        {
                            player.user = lazy;
                        }
                    }

                    return records;
                });

            return user;
        }

        public override int GetHashCode()
        {
            return (ID ?? string.Empty).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as User;

            if (other == null)
                return false;

            return ID == other.ID;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
