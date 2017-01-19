using Microsoft.ApplicationInsights.Extensibility;
using System.Configuration;

namespace Mood
{
    public class AppInsightsConfig
    {
        public static string Key { get; private set;}

        public static void Initialize()
        {
            Key = null;
            string key = ConfigurationManager.AppSettings["TelemetryInstrumentationKey"];
            if (string.IsNullOrWhiteSpace(key))
            {
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }
            else
            {
                TelemetryConfiguration.Active.InstrumentationKey = key;
                Key = key;
            }
        }
    }
}