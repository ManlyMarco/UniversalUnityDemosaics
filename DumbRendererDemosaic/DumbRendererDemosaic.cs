using System.Linq;
using BepInEx;
using DemosaicCommon;
using UnityEngine;

namespace DumbRendererDemosaic
{
    /// <summary>
    /// Scans all renderers for materials that could be mozaics and removes the materials
    /// </summary>
    [BepInPlugin("manlymarco.DumbRendererDemosaic", "Dumb Renderer Demosaic", Metadata.Version)]
    internal class DumbRendererDemosaic : BaseUnityPlugin
    {
        private void Start()
        {
            MozaicTools.InitSetting(Config);
        }

        private void Update()
        {
            foreach (var renderer in FindObjectsOfType<Renderer>().Where(x => x.material != null && (MozaicTools.IsMozaicName(x.material.name) || MozaicTools.IsMozaicName(x.material.shader?.name))))
            {
                Logger.LogInfo($"Removing mozaic material {renderer.material.name} from renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                renderer.material = null;
                renderer.enabled = false;
                renderer.gameObject.SetActive(false);
            }
        }
    }
}
