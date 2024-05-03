using BepInEx;
using BepInEx.Configuration;
using DemosaicCommon;
using UnityEngine;

namespace ShaderReplaceDemosaic
{
    /// <summary>
    /// Scans individual shaders of materials on all renderers for ones that could be mozaics and replaces them to the specified shader.
    /// Useful whem the mozaic effect is done by using a custom shader on a fully modelled mesh. Make sure to set the Replacement shader name setting!
    /// Use together with DumbRendererDemosaic if there are any dedicated mozaic renderers.
    /// </summary>
    [BepInPlugin("manlymarco.ShaderReplaceDemosaic", "Shader Replace Demosaic", Metadata.Version)]
    public class ShaderReplaceDemosaicPlugin : BaseUnityPlugin
    {
        private Shader _goodShader;

        private ConfigEntry<string> _nameSetting;
        private ConfigEntry<string> _targetSetting;

        private void Awake()
        {
            _nameSetting = Config.Bind("Shader replace", "Replacement shader name", "Body", "Part or whole name of the shader that should be used to replace mozaic shaders. Case sensitive.");
            _nameSetting.SettingChanged += (sender, args) => _goodShader = null;

            _targetSetting = Config.Bind("Shader replace", "Target shader name", "", "Part or whole name of the shader (or material it is on) to be replaced. Case sensitive. If empty, common names of mozaic shaders/materials are searched.");
        }

        private void Update()
        {
            foreach (var renderer in FindObjectsOfType<SkinnedMeshRenderer>())
            {
                if (renderer == null)
                    continue;

                foreach (var material in renderer.sharedMaterials.Length == 0 ? renderer.materials : renderer.sharedMaterials)
                {
                    if (material == null)
                        continue;

                    if (_targetSetting.Value.Length == 0 ? (MozaicTools.IsMozaicName(material.name) || MozaicTools.IsMozaicName(material.shader.name)) : (material.name.Contains(_targetSetting.Value) || material.shader.name.Contains(_targetSetting.Value)))
                    {
                        if (_goodShader == null) continue;

                        if (material.shader != _goodShader)
                        {
                            Logger.LogInfo($"Replacing shader {material.shader.name} on material {material.name} on renderer {MozaicTools.GetTransformPath(renderer.transform)} with shader {_goodShader.name}");
                            material.shader = _goodShader;
                        }
                    }
                    else if (_goodShader == null && material.shader.name.Contains(_nameSetting.Value))
                    {
                        Logger.LogInfo($"Found replacement shader {material.shader.name} on material {material.name} on renderer {MozaicTools.GetTransformPath(renderer.transform)}");
                        _goodShader = material.shader;
                    }
                }
            }
        }
    }
}
