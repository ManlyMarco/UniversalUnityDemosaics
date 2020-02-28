using BepInEx;
using BepInEx.Logging;
using DemosaicCommon;
using UnityEngine;

namespace MaterialReplaceDemosaic
{
    /// <summary>
    /// Smarter version of DumbRendererDemosaic, uncensors some Live2D games where privates completely disappear with other demosaic
    /// </summary>
    [BepInPlugin("manlymarco.MaterialReplaceDemosaic", "Material Replace Demosaic", Metadata.Version)]
    public class MaterialReplaceDemosaic : BaseUnityPlugin
    {
        private Material _unlitMaterial;
        private void Update()
        {
            foreach (var renderer in FindObjectsOfType<MeshRenderer>())
            {
                if (MozaicTools.IsMozaicName(renderer.material.name))
                {
                    if (_unlitMaterial == null) continue;

                    if (renderer.sharedMaterial != null)
                    {
                        Logger.Log(LogLevel.Info, $"Replacing shared material {renderer.sharedMaterial.name} with {_unlitMaterial.name}");
                        renderer.sharedMaterial = _unlitMaterial;
                    }
                    else
                    {
                        Logger.Log(LogLevel.Info, $"Removing mozaic material from renderer {renderer.transform.name}");
                        renderer.material = _unlitMaterial;
                    }
                }
                else if (_unlitMaterial == null && renderer.material.name.StartsWith("Unlit"))
                {
                    _unlitMaterial = renderer.material;
                }
            }
        }
    }
}
