using System;

namespace SpeedrunComSharp
{
    public class GameHeader : IElementWithID
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string JapaneseName { get; private set; }
        public string Abbreviation { get; private set; }
        public Uri WebLink { get; private set; }

        private GameHeader() { }

        public static GameHeader Parse(SpeedrunComClient client, dynamic gameHeaderElement)
        {
            var gameHeader = new GameHeader()
            {
                ID = gameHeaderElement.id as string,
                Name = gameHeaderElement.names.international as string,
                JapaneseName = gameHeaderElement.names.japanese as string,
                WebLink = new Uri(gameHeaderElement.weblink.ToString()),
                Abbreviation = gameHeaderElement.abbreviation as string,
            };

            return gameHeader;
        }

        public override int GetHashCode()
        {
            return (ID ?? string.Empty).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GameHeader))
            {
                return false;
            }

            return ID == (obj as GameHeader).ID;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
