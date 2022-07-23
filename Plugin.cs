using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using BestHTTP;
using Il2CppSystem;
using EventArgs = System.EventArgs;

namespace HttpProxy
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        static ManualLogSource _log;
        static ConfigEntry<string> _configHttpProxyURL;

        public override void Load()
        {
            _log = Log;
            _log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            _configHttpProxyURL = Config.Bind(
                "Proxy",
                "HTTP Proxy URL",
                "http://localhost:8888",
                "A HTTP Proxy URL. If empty, no proxy will be used."
            );
            _configHttpProxyURL.SettingChanged   += OnConfigChanged;
            InitProxy();
        }

        static void InitProxy()
        {
            if (string.IsNullOrEmpty(_configHttpProxyURL.Value))
            {
                _log.LogInfo("No proxy will be used.");
                HTTPManager.Proxy = null;
                return;
            }

            _log.LogInfo($"Using proxy {_configHttpProxyURL.Value}.");
            HTTPManager.Proxy = new HTTPProxy(new Uri(_configHttpProxyURL.Value));
        }

        static void OnConfigChanged(object sender, EventArgs e)
        {
            if (e is not SettingChangedEventArgs)
            {
                return;
            }

            InitProxy();
        }
    }
}
