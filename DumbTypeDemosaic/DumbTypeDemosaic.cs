using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using DemosaicCommon;
using UnityEngine;

namespace DumbTypeDemosaic
{
    /// <summary>
    /// Scans for all types that could be applying mozaics and disables them
    /// </summary>
    [BepInPlugin("manlymarco.DumbTypeDemosaic", "Dumb Type Demosaic", Metadata.Version)]
    internal class DumbTypeDemosaic : BaseUnityPlugin
    {
        private void Awake()
        {
            var compType = typeof(Behaviour);
            var matType = typeof(Material);
            _mozaicTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(
                    x =>
                    {
                        try
                        {
                            return x.GetTypes();
                        }
                        catch
                        {
                            return new Type[0];
                        }
                    })
                .Where(x => compType.IsAssignableFrom(x) && MozaicTools.IsMozaicName(x.Name))
                .ToDictionary(x => x, x => x.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(y => matType.IsAssignableFrom(y.FieldType)).ToList());

            if (_mozaicTypes.Count == 0)
            {
                Logger.Log(LogLevel.Warning, "No potential mozaic types found! Shutting down");
                enabled = false;
            }
            else
            {
                Logger.Log(LogLevel.Info, $"Found {_mozaicTypes.Count} potential mozaic types");
            }
        }

        private Dictionary<Type, List<FieldInfo>> _mozaicTypes;

        private void Update()
        {
            foreach (var mozaicType in _mozaicTypes)
            {
                foreach (var instance in FindObjectsOfType(mozaicType.Key))
                {
                    var mozObject = (Behaviour) instance;
                    if (mozObject.enabled)
                    {
                        Logger.Log(LogLevel.Info, $"Disabling mozaic behaviour {mozObject.transform.name}");
                        mozObject.enabled = false;

                        foreach (var fieldInfo in mozaicType.Value)
                        {
                            try
                            {
                                var mat = (Material) fieldInfo.GetValue(instance);
                                mat.mainTexture = null;
                            }
                            catch
                            {
                                // If can't set main tex, ignore
                            }
                        }
                    }
                }
            }
        }
    }
}
