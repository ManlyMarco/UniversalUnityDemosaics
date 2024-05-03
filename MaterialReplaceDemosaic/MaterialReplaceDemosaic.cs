using BepInEx;
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
                if (renderer.material == null)
                {
                    continue;
                }
                else if (MozaicTools.IsMozaicName(renderer.material.name))
                {
                    if (_unlitMaterial == null) continue;

                    if (renderer.sharedMaterial != null)
                    {
                        Logger.LogInfo($"Replacing shared material {renderer.sharedMaterial.name} with {_unlitMaterial.name} on renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                        renderer.sharedMaterial = _unlitMaterial;
                    }
                    else
                    {
                        Logger.LogInfo($"Replacing material {renderer.material.name} with {_unlitMaterial.name} on renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                        renderer.material = _unlitMaterial;
                    }
                }
                else if (_unlitMaterial == null && renderer.material.name.StartsWith("Unlit"))
                {
                    Logger.LogInfo($"Found Unlit replacement material {renderer.material.name} on renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                    _unlitMaterial = renderer.material;
                }
            }
        }
    }
}
