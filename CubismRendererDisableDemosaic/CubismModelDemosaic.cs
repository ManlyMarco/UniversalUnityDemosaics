using BepInEx;
using BepInEx.Harmony;
using BepInEx.Logging;
using DemosaicCommon;
using HarmonyLib;
using Live2D.Cubism.Core;
using UnityEngine;

namespace CubismModelDemosaic
{
    /// <summary>
    /// Smarter version of DumbRendererDemosaic, faster on some games using Live2D
    /// </summary>
    [BepInPlugin("manlymarco.CubismModelDemosaic", "CubismModel Demosaic", Metadata.Version)]
    public class CubismModelDemosaic : BaseUnityPlugin
    {
        private static CubismModelDemosaic _instance;

        private void Awake()
        {
            _instance = this;
            // Test if type is accessible
            var _ = typeof(CubismModel);
            HarmonyWrapper.PatchAll(typeof(CubismModelDemosaic));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CubismModel), "OnEnable")]
        public static void DemosaicHook(CubismModel __instance)
        {
            foreach (var renderer in __instance.gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer != null && renderer.material != null && MozaicTools.IsMozaicName(renderer.material.name))
                {
                    _instance.Logger.LogInfo($"Removing mozaic material {renderer.material.name} from renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                    renderer.material = null;
                    renderer.enabled = false;
                }
            }
        }
    }
}
