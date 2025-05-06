using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomTraderOrder;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

public class CustomTraderOrderPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(TraderScreensGroup), "method_4");
    }

    public static readonly Dictionary<string, string> TraderNameToId = new()
    {
        { "Prapor", "54cb50c76803fa8b248b4571" },
        { "Therapist", "54cb57776803fa99248b456e" },
        { "Fence", "579dc571d53a0658a154fbec" },
        { "Skier", "58330581ace78e27b8b10cee" },
        { "Peacekeeper", "5935c25fb3acc3127c3d8cd9" },
        { "Mechanic", "5a7c2eca46aef81a7ca2145d" },
        { "Ragman", "5ac3b934156ae10c4430e83c" },
        { "Jaeger", "5c0647fdd443bc2504c2d371" },
        { "Ref", "6617beeaa9cfa777ca915b7c" },
        { "Survivor", "6758f03d55bf9943a7b14e47" },
        { "Scorpion", "6688d464bc40c867f60e7d7e" },
        { "Priscilu Origins", "6748adca5c70634464b214a8" },
        { "Lotus", "6747208ef022cbbfc65c41bf" },
        { "Croupier", "1337bb0dd843363fcd1be869" },
        { "Anastasia", "678937229797bd983dca2137" },
        { "Evelyn", "6789373b0f5ecebf0bc152d5" },
        { "Svetlana", "6789374aebcb3ad4c6e073bd" },
        { "EpaKim1", "67b67103b32c5563e508998a" }
    };

    [PatchPrefix]
    public static bool Prefix(TraderScreensGroup.GClass3599 ___gclass3599_0)
    {
        TraderClass[] reordered = ReOrderTraders(___gclass3599_0.TradersList.ToArray());

       
        EnsureTraderAssortments(reordered);

        var tradersListField = AccessTools.Field(___gclass3599_0.GetType(), "TradersList");
        if (tradersListField != null)
        {
            tradersListField.SetValue(___gclass3599_0, reordered);
        }
        else
        {
            CustomTraderOrderPlugin.LoggerInstance.LogWarning("Could not find TradersList field to update.");
        }

        return true;
    }

    private static void EnsureTraderAssortments(TraderClass[] traders)
    {
        foreach (var trader in traders)
        {
            
            CustomTraderOrderPlugin.LoggerInstance.LogInfo($"Ensuring assortment for trader: {trader.Id}");
            
        }
    }

    private static TraderClass[] ReOrderTraders(TraderClass[] traders)
    {
        var traderById = traders.ToDictionary(t => t.Id);
        var traderList = new List<TraderClass>();

        if (CustomTraderOrderPlugin.TraderOrderNames is { Count: > 0 })
        {
            var orderedIds = CustomTraderOrderPlugin.TraderOrderNames
                .Where(name => TraderNameToId.ContainsKey(name))
                .Select(name => TraderNameToId[name])
                .ToList();

            foreach (string id in orderedIds)
            {
                if (traderById.TryGetValue(id, out var trader))
                {
                    traderList.Add(trader);
                }
            }

            traderList.AddRange(traders.Where(t => !orderedIds.Contains(t.Id)));
        }
        else
        {
            CustomTraderOrderPlugin.LoggerInstance.LogWarning("Trader order list is empty or null, using default order.");
            return traders;
        }

        return traderList.ToArray();
    }
}


