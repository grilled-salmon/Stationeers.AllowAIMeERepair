using AllowAIMeERepair.Scripts;
using Stationeers.Addons;
using UnityEngine;

namespace AllowAIMeERepair.Scripts
{
    public class AllowAIMeERepair : IPlugin
    {
        public void OnLoad()
        {
            Debug.Log(ModReference.Name + ": Loaded");
        }

        public void OnUnload()
        {
            Debug.Log(ModReference.Name + ": Unloaded");
        }
    }
}
