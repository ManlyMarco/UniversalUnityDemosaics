using System.Collections;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using DemosaicCommon;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace DumbRendererDemosaic;

[BepInPlugin(nameof(DumbRendererDemosaicPlugin), "DumbRendererDemosaic", Metadata.Version)]
public class DumbRendererDemosaicPlugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;

        MozaicTools.InitSetting(Config);

        ClassInjector.RegisterTypeInIl2Cpp<DumbRendererDemosaic>();
        AddComponent<DumbRendererDemosaic>();
    }
}
public class DumbRendererDemosaic : MonoBehaviour
{
    public DumbRendererDemosaic(System.IntPtr handle) : base(handle) { }

    private void Start()
    {
        StartCoroutine(CoroutineUpdate().WrapToIl2Cpp());
    }

    private static IEnumerator CoroutineUpdate()
    {
        while (true)
        {
            var array = FindObjectsOfType(Il2CppType.Of<Renderer>());
            yield return null;
            var ofType = array.Select(x => x.Cast<Renderer>());
            var count = 0;
            foreach (var renderer in ofType)
            {
                count++;
                if (count % 100 == 0) yield return null;
                if (renderer == null) break;

                if (!renderer.enabled || !renderer.gameObject.activeSelf) continue;

                if (MozaicTools.IsMozaicName(renderer.name))
                {
                    DumbRendererDemosaicPlugin.Log.LogInfo(
                        $"Disabling mozaic renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                    renderer.enabled = false;
                    renderer.gameObject.SetActive(false);
                }
                else if (renderer.material != null && (MozaicTools.IsMozaicName(renderer.material.name) ||
                                                       MozaicTools.IsMozaicName(renderer.material.shader?.name)))
                {
                    DumbRendererDemosaicPlugin.Log.LogInfo(
                        $"Removing mozaic material {renderer.material.name} from renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                    renderer.material = null;
                    renderer.enabled = false;
                    renderer.gameObject.SetActive(false);
                }
            }

            yield return null;
        }
    }
}