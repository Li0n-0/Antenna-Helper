using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AntennaHelper
{
	[KSPAddon (KSPAddon.Startup.Flight, false)]
	public class AHFlight : MonoBehaviour
	{
		private KSP.UI.Screens.ApplicationLauncherButton toolbarButton;
		private float timeAtStart;
		private AHMapMarker mapMarker;

		public void Start ()
		{
			timeAtStart = Time.time;

			GameEvents.onGUIApplicationLauncherReady.Add (AddToolbarButton);
			GameEvents.onGUIApplicationLauncherDestroyed.Add (RemoveToolbarButton);

			GameEvents.onVesselSwitching.Add (VesselSwitch);

			StartCoroutine ("WaitAtStart");
		}

		public void OnDestroy ()
		{
			Destroy (mapMarker.gameObject);
			// Toolbar button
			RemoveToolbarButton ();
			GameEvents.onGUIApplicationLauncherReady.Remove (AddToolbarButton);
			GameEvents.onGUIApplicationLauncherDestroyed.Remove (RemoveToolbarButton);

			GameEvents.onVesselSwitching.Remove (VesselSwitch);
		}

		private void VesselSwitch (Vessel fromVessel, Vessel toVessel)
		{
			StopAllCoroutines ();
			Destroy (mapMarker.gameObject);
			timeAtStart = Time.time;
			StartCoroutine ("WaitAtStart");
			SetMapMarker ();
		}

		private IEnumerator WaitAtStart ()
		{
			while (Time.time < timeAtStart + .5f) {
				yield return new WaitForSeconds (.1f);
			}
			SetMapMarker ();
		}

		private void SetMapMarker ()
		{
			double maxRange = AHUtil.GetRange (FlightGlobals.ActiveVessel.Connection.Comm.antennaTransmit.power, AHUtil.DSNLevelList [AHUtil.DSNLevel]);
			GameObject mapMarkerObj = new GameObject ();
			mapMarker = mapMarkerObj.AddComponent<AHMapMarker> ();
			mapMarker.Start ();
			mapMarker.SetUp (maxRange, FlightGlobals.ActiveVessel.mapObject.trf, FlightGlobals.GetHomeBody ().MapObject.trf, true);

//			Debug.Log ("[AH] DSN level = " + ScenarioUpgradeableFacilities.GetFacilityLevel (SpaceCenterFacility.TrackingStation));
//			Debug.Log ("[AH] DSN level int = " + AHUtil.DSNLevel);
//			Debug.Log ("[AH] DSN power = " + AHUtil.DSNLevelList [AHUtil.DSNLevel]);
//			Debug.Log ("[AH] vessel power = " + FlightGlobals.ActiveVessel.Connection.Comm.antennaTransmit.power);
//			Debug.Log ("[AH] max range = " + maxRange);
//
//			CommNet.CommLink firstLink = null;
//
//			if (FlightGlobals.ActiveVessel.Connection != null) {
//				Debug.Log ("[AH] active vessel CommnetVessel found");
//				if (FlightGlobals.ActiveVessel.Connection.ControlPath != null) {
//					Debug.Log ("[AH] active vessel ControlPath found");
//					if (FlightGlobals.ActiveVessel.Connection.ControlPath.First != null) {
//						Debug.Log ("[AH] active vessel ControlPath.First found");
//						Debug.Log ("[AH] ControlPath.First : " + FlightGlobals.ActiveVessel.Connection.ControlPath.First.ToString ());
//						firstLink = FlightGlobals.ActiveVessel.Connection.ControlPath.First;
//					} else {
//						Debug.Log ("[AH] active vessel ControlPath.First not found");
//					}
//				} else {
//					Debug.Log ("[AH] active vessel ControlPath not found");
//				}
//			} else {
//				Debug.Log ("[AH] active vessel CommnetVessel not found");
//			}
//
//			Debug.Log ("[AH] rangeModifier = " + HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams> ().rangeModifier);
//
//			double maxRange;
//			if (firstLink != null) {
//				// a is the vessel, b the relay
////				Debug.Log ("[AH] a name = " + firstLink.a.name);
////				Debug.Log ("[AH] b name = " + firstLink.b.name);
////				Debug.Log ("[AH] a power = " + firstLink.a.antennaTransmit.power);
////				Debug.Log ("[AH] b power = " + firstLink.b.antennaRelay.power);
//
//				maxRange = AntennaHelperUtil.GetRange (firstLink.a.antennaTransmit.power, firstLink.b.antennaRelay.power);
//
//				Transform firstRelay;
//				bool relayIsHome = false;
//				if (firstLink.b.isHome) {
//					Debug.Log ("[AH] : b is home");
//					firstRelay = FlightGlobals.GetHomeBody ().MapObject.trf;
//					relayIsHome = true;
//				} else {
//					Debug.Log ("[AntennaHelper] Connection to another vessel is not supported, yet");
//					return;
////					Debug.Log ("[AH] : b is a vessel, " + firstLink.b.name);
////					firstRelay = GetRelayByName (firstLink.b.name);
//				}
//
//				GameObject mapMarkerObj = new GameObject ();
//				mapMarker = mapMarkerObj.AddComponent<AHMapMarker> ();
//				mapMarker.Start ();
//				mapMarker.SetUp (maxRange, FlightGlobals.ActiveVessel.mapObject.trf, firstRelay, relayIsHome);
//			} else {
////				maxRange = AntennaHelperUtil.GetRange (FlightGlobals.ActiveVessel.Connection.Comm.antennaTransmit.power, ;
//			}

		}

		private Transform GetRelayByName (string name)
		{
			string[] realName = Regex.Split (name, @"\s\(unloaded\)$");
			foreach (Vessel vessel in FlightGlobals.Vessels) {
				if (vessel.protoVessel.vesselName == realName[0]) {
					return vessel.mapObject.trf;
				}
			}
			return null;
		}

		#region ToolbarButton
		private void AddToolbarButton ()
		{
			toolbarButton = KSP.UI.Screens.ApplicationLauncher.Instance.AddModApplication (
				ToolbarButtonOnTrue, 
				ToolbarButtonOnFalse, 
				AHUtil.DummyVoid, 
				AHUtil.DummyVoid, 
				AHUtil.DummyVoid, 
				AHUtil.DummyVoid,
				KSP.UI.Screens.ApplicationLauncher.AppScenes.MAPVIEW,
				AHUtil.toolbarButtonTex);
		}

		private void RemoveToolbarButton ()
		{
			ToolbarButtonOnFalse ();
			KSP.UI.Screens.ApplicationLauncher.Instance.RemoveModApplication (toolbarButton);
		}

		private void ToolbarButtonOnTrue ()
		{
			if (mapMarker) {
				mapMarker.Show ();
			}
		}

		private void ToolbarButtonOnFalse ()
		{
			if (mapMarker) {
				mapMarker.Hide ();
			}
		}
		#endregion
	}
}

