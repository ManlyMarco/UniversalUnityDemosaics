using System.Linq;
using BepInEx;
using BepInEx.Logging;
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
        private void Update()
        {
            foreach (var renderer in FindObjectsOfType<Renderer>().Where(x => MozaicTools.IsMozaicName(x.material.name) || MozaicTools.IsMozaicName(x.material.shader?.name)))
            {
                Logger.Log(LogLevel.Info, $"Removing mozaic material from renderer {renderer.transform.name}");
                renderer.material = null;
                renderer.enabled = false;
                renderer.gameObject.SetActive(false);
            }
        }
    }
}
