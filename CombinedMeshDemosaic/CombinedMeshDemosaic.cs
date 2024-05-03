using System.Linq;
using BepInEx;
using DemosaicCommon;
using UnityEngine;

namespace CombinedMeshDemosaic
{
    /// <summary>
    /// Scans individual materials on all renderers for materials that could be mozaics and changes their shaders to be invisible.
    /// Useful when meshes are combined and not represented by transforms.
    /// Use together with DumbRendererDemosaic if there are any dedicated mozaic renderers.
    /// </summary>
    [BepInPlugin("manlymarco.CombinedMeshDemosaic", "Combined Mesh Demosaic", Metadata.Version)]
    internal class CombinedMeshDemosaic : BaseUnityPlugin
    {
        private Shader _additiveShader;
        private Shader _standardShader;

        private void Start()
        {
            _additiveShader = Shader.Find("Mobile/Particles/Additive");
            if (_additiveShader == null)
            {
                _standardShader = Shader.Find("Standard");
                if(_standardShader != null)
                {
                    Logger.LogWarning("Could not find the Additive shader, falling back to the Standard shader");
                }
                else
                {
                    Logger.LogWarning("Could not find any replacement shaders, deactivating");
                    enabled = false;
                }
            }
        }

        private void Update()
        {
            foreach (var renderer in FindObjectsOfType<Renderer>())
            {
                if (renderer.materials.Length < 2) continue;

                foreach (var material in renderer.materials.Where(x => x != null && (MozaicTools.IsMozaicName(x.name) || MozaicTools.IsMozaicName(x.shader?.name))))
                {
                    Logger.LogInfo($"Removing mozaic material {renderer.material.name} from renderer {MozaicTools.GetTransformPath(renderer.transform)}");

                    if (_additiveShader != null)
                    {
                        material.shader = _additiveShader;
                    }
                    else
                    {
                        material.shader = _standardShader;
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    }

                    material.color = Color.clear;
                    material.mainTexture = null;
                    material.name = "[Replaced]";
                }
            }
        }
    }
}
