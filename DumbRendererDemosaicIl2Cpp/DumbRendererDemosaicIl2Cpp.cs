using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using DemosaicCommon;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace DumbRendererDemosaicIl2Cpp
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, Metadata.Version)]
    public class DumbRendererDemosaicPlugin : BasePlugin
    {
        internal static new ManualLogSource Log;

        public override void Load()
        {
            Log = base.Log;

            ClassInjector.RegisterTypeInIl2Cpp<DumbRendererDemosaic>();
            AddComponent<DumbRendererDemosaic>();
        }
    }

    public class DumbRendererDemosaic : MonoBehaviour
    {
        public DumbRendererDemosaic(System.IntPtr handle) : base(handle) { }

        private void Update()
        {
            var array = FindObjectsOfType(UnhollowerRuntimeLib.Il2CppType.Of<Renderer>());
            var ofType = array.Select(x => x.Cast<Renderer>());
            foreach (var renderer in ofType)
            {
                if (!renderer.enabled || !renderer.gameObject.activeSelf) continue;

                if (MozaicTools.IsMozaicName(renderer.name))
                {
                    DumbRendererDemosaicPlugin.Log.LogInfo($"Disabling mozaic renderer {renderer.name}");
                    renderer.enabled = false;
                    renderer.gameObject.SetActive(false);
                }
                else if (MozaicTools.IsMozaicName(renderer.material.name) || MozaicTools.IsMozaicName(renderer.material.shader?.name))
                {
                    DumbRendererDemosaicPlugin.Log.LogInfo($"Removing mozaic material from renderer {renderer.name}");
                    renderer.material = null;
                    renderer.enabled = false;
                    renderer.gameObject.SetActive(false);
                }
            }
        }
    }
}
