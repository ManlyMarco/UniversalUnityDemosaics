using System.Linq;

namespace DemosaicCommon
{
    public static class MozaicTools
    {
        public static readonly string[] MozaicNameParts = { "mozaic", "mosaic", "mozaik", "mosaik", "pixelate", "censor", "cenzor", "masaco" };

        public static bool IsMozaicName(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            str = str.ToLower();
            return MozaicNameParts.Any(x => str.Contains(x));
        }
    }
}
