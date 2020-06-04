﻿using System;

namespace SpeedrunComSharp
{
    public class ImageAsset
    {
        public Uri Uri { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private ImageAsset() { }

        public static ImageAsset Parse(SpeedrunComClient client, dynamic imageElement)
        {
            if (imageElement == null || imageElement.uri == null || imageElement.width == null || imageElement.height == null)
                return null;

            var image = new ImageAsset();

            var uri = imageElement.uri?.ToString();
            image.Uri = new Uri(uri);
            image.Width = (int)imageElement.width;
            image.Height = (int)imageElement.height;

            return image;
        }
    }
}
