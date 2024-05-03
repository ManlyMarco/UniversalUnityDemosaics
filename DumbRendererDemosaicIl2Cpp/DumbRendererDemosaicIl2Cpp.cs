using System.Linq;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using DemosaicCommon;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace DumbRendererDemosaic
{
    [BepInPlugin(nameof(DumbRendererDemosaicPlugin), "DumbRendererDemosaic", Metadata.Version)]
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
                    DumbRendererDemosaicPlugin.Log.LogInfo($"Disabling mozaic renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                    renderer.enabled = false;
                    renderer.gameObject.SetActive(false);
                }
                else if (renderer.material != null && (MozaicTools.IsMozaicName(renderer.material.name) || MozaicTools.IsMozaicName(renderer.material.shader?.name)))
                {
                    DumbRendererDemosaicPlugin.Log.LogInfo($"Removing mozaic material {renderer.material.name} from renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                    renderer.material = null;
                    renderer.enabled = false;
                    renderer.gameObject.SetActive(false);
                }
            }
        }
    }
}
