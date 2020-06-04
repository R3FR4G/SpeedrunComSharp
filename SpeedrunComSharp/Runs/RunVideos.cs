using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpeedrunComSharp
{
    public class RunVideos
    {
        public string Text { get; private set; }
        public ReadOnlyCollection<Uri> Links { get; private set; }

        private RunVideos() { }

        private static Uri parseVideoLink(dynamic element)
        {
            var videoUri = element.uri as string;
            if (!string.IsNullOrEmpty(videoUri))
            {
                if (!videoUri.StartsWith("http"))
                    videoUri = "http://" + videoUri;

                if (Uri.IsWellFormedUriString(videoUri, UriKind.Absolute))
                    return new Uri(videoUri);
            }

            return null;
        }

        public static RunVideos Parse(SpeedrunComClient client, dynamic videosElement)
        {
            if (videosElement == null)
            {
                return null;
            }

            IDictionary<string, dynamic> properties = videosElement as IDictionary<string, dynamic>;

            RunVideos videos = new RunVideos();

            if (properties.ContainsKey("text"))
            {
                videos.Text = properties["text"];
            }

            if (properties.ContainsKey("links"))
            {
                videos.Links = client.ParseCollection(properties["links"], new Func<dynamic, Uri>(parseVideoLink));
            }


            return videos;
        }
    }
}
