using System.Linq;
using System.Reflection;
using System;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using DemosaicCommon;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace DumbRendererDemosaic
{
    [BepInPlugin(nameof(DumbRendererDemosaicPlugin), "DumbRendererDemosaic", Metadata.Version)]
    public class DumbRendererDemosaicPlugin : BasePlugin
    {
        internal static new ManualLogSource Log;
        internal static Version UnityVersion = new Version(GetCleanUnityVersion());
        public override void Load()
        {
            Log = base.Log;

            MozaicTools.InitSetting(Config);

            ClassInjector.RegisterTypeInIl2Cpp<DumbRendererDemosaic>();
            AddComponent<DumbRendererDemosaic>();

            Log.LogInfo($"UnityVersion : {UnityVersion.ToString()}");
        }
        private static string GetCleanUnityVersion()
            {
                string version = Application.unityVersion;
                int end = 0;
                for (end = 0; end < version.Length - 1; end++)
                {
                     if (char.IsLetter(version[end]))
                     {
                         break;
                     }
                }
                return version.Substring(0,end);
            }
    }

    public class DumbRendererDemosaic : MonoBehaviour
    {
        public DumbRendererDemosaic(System.IntPtr handle) : base(handle) { }
	private static bool IsUp2022_2 = DumbRendererDemosaicPlugin.UnityVersion >= new Version("2022.2");
	private static MethodInfo FindObject = typeof(UnityEngine.Object)
	                                        .GetMethod(IsUp2022_2 ? "FindObjectsByType" : "FindObjectsOfType"
	                                                   ,BindingFlags.Public | BindingFlags.Static
	                                                   ,IsUp2022_2 ? new Type[] {typeof(Il2CppSystem.Type),typeof(FindObjectsSortMode)} : new Type[] {typeof(Il2CppSystem.Type),typeof(bool)});
        private void Update()
        {
            Il2CppReferenceArray<UnityEngine.Object> array =
            				(Il2CppReferenceArray<UnityEngine.Object>)(IsUp2022_2 ?
            				FindObject.Invoke(null,new object[] {Il2CppType.Of<Renderer>(),FindObjectsSortMode.None}) :
            				FindObject.Invoke(null,new object[] {Il2CppType.Of<Renderer>(),false}));
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
