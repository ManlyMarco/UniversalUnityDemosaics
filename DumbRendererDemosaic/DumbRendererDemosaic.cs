using System.Linq;
using System.Reflection;
using System;
using BepInEx;
using DemosaicCommon;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace DumbRendererDemosaic
{
    /// <summary>
    /// Scans all renderers for materials that could be mozaics and removes the materials
    /// </summary>
    [BepInPlugin("manlymarco.DumbRendererDemosaic", "Dumb Renderer Demosaic", Metadata.Version)]
    internal class DumbRendererDemosaic : BaseUnityPlugin
    {
        internal static Version UnityVersion = new Version(GetCleanUnityVersion());
        private void Start()
        {
            MozaicTools.InitSetting(Config);
        }
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

            foreach (var renderer in array.Where(x => x.material != null && (MozaicTools.IsMozaicName(x.material.name) || MozaicTools.IsMozaicName(x.material.shader?.name))))
            {
                Logger.LogInfo($"Removing mozaic material {renderer.material.name} from renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                renderer.material = null;
                renderer.enabled = false;
                renderer.gameObject.SetActive(false);
            }
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
}
