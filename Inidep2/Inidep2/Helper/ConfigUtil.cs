using System.Configuration;

namespace Inidep2.Helper
{
    public sealed class ConfigUtil
    {
        public static string GetString(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }
    }
}
