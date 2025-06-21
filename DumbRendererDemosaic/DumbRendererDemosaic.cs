using System.Collections;
using System.Collections.Generic;
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
            StartCoroutine(CoroutineUpdate());
        }

        private IEnumerator CoroutineUpdate()
        {
            while (true)
            {
                var count = 0;
                foreach (var renderer in FindObjectsOfType<Renderer>().Where(x => x.material != null && (MozaicTools.IsMozaicName(x.material.name) || MozaicTools.IsMozaicName(x.material.shader?.name))))
                {
                    count++;
                    if (count % 100 == 0) yield return null;
                    if (renderer == null) break;
                    
                    Logger.LogInfo($"Removing mozaic material {renderer.material.name} from renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                    renderer.material = null;
                    renderer.enabled = false;
                    renderer.gameObject.SetActive(false);
                }

                yield return null;
            }
        }
    }
}
