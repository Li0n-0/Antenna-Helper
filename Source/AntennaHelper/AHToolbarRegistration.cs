using UnityEngine;
using ToolbarControl_NS;
using KSP.Localization;

namespace AntennaHelper
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(AHEditor.MODID, Localizer.Format(AHEditor.MODNAME) + " Editor");
            ToolbarControl.RegisterMod(AHFlight.MODID, Localizer.Format(AHFlight.MODNAME) + " Flight");
            ToolbarControl.RegisterMod(AHTrackingStation.MODID, Localizer.Format(AHTrackingStation.MODNAME) + " Tracking Station");
        }
    }
}