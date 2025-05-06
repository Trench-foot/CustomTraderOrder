using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CustomTraderOrder
{
    [BepInPlugin("CustomTraderOrder.d34db33f", "Custom Trader Order", "1.0.0")]
    public class CustomTraderOrderPlugin : BaseUnityPlugin
    {
        public static ManualLogSource LoggerInstance;
        public static List<string> TraderOrderNames = new();

        private void Awake()
        {
            LoggerInstance = Logger;
            LoggerInstance.LogInfo("Custom Trader Order plugin loaded.");

            
            string pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configPath = Path.Combine(pluginFolder, "config", "traderorder.json");

            if (File.Exists(configPath))
            {
                try
                {
                    string json = File.ReadAllText(configPath);
                    TraderOrderNames = JsonConvert.DeserializeObject<List<string>>(json);
                    LoggerInstance.LogInfo($"Loaded {TraderOrderNames.Count} trader order entries.");
                }
                catch (System.Exception ex)
                {
                    LoggerInstance.LogError($"Failed to load trader order config: {ex.Message}");
                }
            }
            else
            {
                LoggerInstance.LogWarning($"Config file not found at {configPath}");
            }


            new CustomTraderOrderPatch().Enable();
        }
    }
}

  