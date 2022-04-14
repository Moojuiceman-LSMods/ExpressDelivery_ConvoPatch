using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace ExpressDelivery_ConvoPatch
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource logger;

        private void Awake()
        {
            // Plugin startup logic
            logger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded");
            Logger.LogInfo($"Patching...");
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Patched");
        }

        [HarmonyPatch(typeof(DeliveryTruck), "StuckChecker")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> StuckChecker_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions).MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(DeliveryTruck), "_savey")),
                new CodeMatch(OpCodes.Callvirt/*, AccessTools.Method(typeof(Component), "GetComponent")*/),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Pause), "_npc")),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Callvirt/*, AccessTools.Method(typeof(GameObject), "GetComponent")*/),
                new CodeMatch(OpCodes.Ldc_I4_1),
                new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(NPCbehaviour), "convoID")),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(DeliveryTruck), "_savey")),
                new CodeMatch(OpCodes.Callvirt/*, AccessTools.Method(typeof(Component), "GetComponent")*/),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Pause), "_npc")),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Callvirt/*, AccessTools.Method(typeof(GameObject), "GetComponent")*/),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(NPCbehaviour), "convoIDStage"))
            ).RemoveInstructions(16);

            return matcher.InstructionEnumeration();
        }
    }
}
