using System;
using System.Xml;

namespace SpeedrunComSharp
{
    public class RunTimes
    {
        public TimeSpan? Primary { get; private set; }
        public TimeSpan? PrimaryISO { get; private set; }
        public TimeSpan? RealTime { get; private set; }
        public TimeSpan? RealTimeISO { get; private set; }
        public TimeSpan? RealTimeWithoutLoads { get; private set; }
        public TimeSpan? RealTimeWithoutLoadsISO { get; private set; }
        public TimeSpan? GameTime { get; private set; }
        public TimeSpan? GameTimeISO { get; private set; }

        private RunTimes() { }

        public static RunTimes Parse(SpeedrunComClient client, dynamic timesElement)
        {
            var times = new RunTimes();

            if (timesElement.primary != null)
            {
                times.Primary = TimeSpan.FromSeconds((double)timesElement.primary_t);
                times.PrimaryISO = XmlConvert.ToTimeSpan((string)timesElement.primary);
            }

            if (timesElement.realtime != null)
            {
                times.RealTime = TimeSpan.FromSeconds((double)timesElement.realtime_t);
                times.RealTimeISO = XmlConvert.ToTimeSpan((string)timesElement.realtime);
            }

            if (timesElement.realtime_noloads != null)
            {
                times.RealTimeWithoutLoads = TimeSpan.FromSeconds((double)timesElement.realtime_noloads_t);
                times.RealTimeWithoutLoadsISO = XmlConvert.ToTimeSpan((string)timesElement.realtime_noloads);
            }

            if (timesElement.ingame != null)
            {
                times.GameTime = TimeSpan.FromSeconds((double)timesElement.ingame_t);
                times.GameTimeISO = XmlConvert.ToTimeSpan((string)timesElement.ingame);
            }

            return times;
        
        }

        public override string ToString()
        {
            if (Primary.HasValue)
                return Primary.Value.ToString();
            else
                return "-";
        }
    }
}
