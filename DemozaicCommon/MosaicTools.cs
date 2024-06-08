using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace DemosaicCommon
{
    public static class MozaicTools
    {
        private static string[] _mozaicNameParts = { "mozaic", "mosaic", "mozaik", "mosaik", "pixelate", "censor", "cenzor", "masaco" };
        private static string MozaicNamePartsString
        {
            get => string.Join("/", _mozaicNameParts);
            set => _mozaicNameParts = value.ToLower().Split('/');
        }

        internal static void InitSetting(ConfigFile config)
        {
            var setting = config.Bind("General", "Mozaic search strings", MozaicNamePartsString, "Shaders, materials and GameObjects with names that contain any of these strings are considered to be mozaics and will become targets of this plugin.\nCase insensitive. Separate with /");
            MozaicNamePartsString = setting.Value;
            setting.SettingChanged += (sender, args) => MozaicNamePartsString = setting.Value;
        }

        public static bool IsMozaicName(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            str = str.ToLower();
            return _mozaicNameParts.Any(x => str.Contains(x));
        }

        public static string GetTransformPath(Transform tr)
        {
            var parent = tr.parent;
            return parent != null ? string.Concat(GetTransformPath(parent), "/", tr.name) : tr.name;
        }
    }
}
