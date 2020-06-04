using System.Collections.Generic;

namespace SpeedrunComSharp
{
    public class UserNameStyle
    {
        public bool IsGradient { get; private set; }
        public string LightSolidColorCode { get; private set; }
        public string LightGradientStartColorCode
        {
            get { return LightSolidColorCode; }
            private set { LightSolidColorCode = value; }
        }
        public string LightGradientEndColorCode { get; private set; }
        public string DarkSolidColorCode { get; private set; }
        public string DarkGradientStartColorCode
        {
            get { return DarkSolidColorCode; }
            private set { DarkSolidColorCode = value; }
        }
        public string DarkGradientEndColorCode { get; private set; }

        private UserNameStyle() { }

        public static UserNameStyle Parse(SpeedrunComClient client, dynamic styleElement)
        {
            UserNameStyle style = new UserNameStyle();
            IDictionary<string, dynamic> properties = styleElement as IDictionary<string, dynamic>;

            style.IsGradient = styleElement.style == "gradient";

            if (style.IsGradient)
            {
                IDictionary<string, dynamic> colorFromProperties = properties["color-from"] as IDictionary<string, dynamic>;
                IDictionary<string, dynamic> colorToProperties = properties["color-to"] as IDictionary<string, dynamic>;

                style.LightGradientStartColorCode = colorFromProperties["light"] as string;
                style.LightGradientEndColorCode = colorToProperties["light"] as string;
                style.DarkGradientStartColorCode = colorFromProperties["dark"] as string;
                style.DarkGradientEndColorCode = colorToProperties["dark"] as string;
            }
            else
            {
                IDictionary<string, dynamic> colorProperties = properties["color"] as IDictionary<string, dynamic>;

                style.LightSolidColorCode = colorProperties["light"] as string;
                style.DarkSolidColorCode = colorProperties["dark"] as string;
            }

            return style;
        }
    }
}
