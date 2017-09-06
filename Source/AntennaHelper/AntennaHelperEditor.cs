using System;
using System.Collections.Generic;
using UnityEngine;

namespace AntennaHelper
{
	[KSPAddon (KSPAddon.Startup.EditorAny, false)]
	public class AntennaHelperEditor : MonoBehaviour
	{
		private static AntennaHelperEditor instance;

		public void Start ()
		{
			instance = this;

//			SetTarget (AntennaHelperUtil.targetDSNList [AntennaHelperUtil.DSNLevel]);

			GameEvents.onGUIApplicationLauncherReady.Add (AddToolbarButton);
			GameEvents.onGUIApplicationLauncherDestroyed.Add (RemoveToolbarButton);

			GameEvents.onEditorLoad.Add (VesselLoad);
			GameEvents.onEditorPartEvent.Add (PartEvent);
			GameEvents.onEditorPodPicked.Add (PodPicked);
			GameEvents.onEditorPodDeleted.Add (PodDeleted);
		}

		public void OnDestroy ()
		{
			GameEvents.onGUIApplicationLauncherReady.Remove (AddToolbarButton);
			GameEvents.onGUIApplicationLauncherDestroyed.Remove (RemoveToolbarButton);
			RemoveToolbarButton ();

			GameEvents.onEditorLoad.Remove (VesselLoad);
			GameEvents.onEditorPartEvent.Remove (PartEvent);
			GameEvents.onEditorPodPicked.Remove (PodPicked);
			GameEvents.onEditorPodDeleted.Remove (PodDeleted);
		}

		public void VesselLoad (ShipConstruct ship, KSP.UI.Screens.CraftBrowserDialog.LoadType screenType)
		{
			if (showMainWindow || showPlanetWindow || showTargetWindow) {
				CreateAntennaList ();
				DoTheMath ();
			}
		}

		public void PartEvent (ConstructionEventType eventType, Part part)
		{
			if (showMainWindow || showPlanetWindow || showTargetWindow) {
				if (eventType == ConstructionEventType.PartAttached) {
					AntennaListAddItem (part);

					// Symmetry counterparts
					foreach (Part symPart in part.symmetryCounterparts) {
						AntennaListAddItem (symPart);
					}

					// Child part
					foreach (Part childPart in part.children) {
						AntennaListAddItem (childPart);
					}

					DoTheMath ();

				} else if (eventType == ConstructionEventType.PartDetached) {
					AntennaListRemoveItem (part);
					List<ModuleDataTransmitter> remAntenna = new List<ModuleDataTransmitter> ();
					foreach (ModuleDataTransmitter antennaSym in directAntennaList) {
						if (antennaSym.part.isSymmetryCounterPart (part)) {
							remAntenna.Add (antennaSym);
						}
					}

					// Child part
					foreach (Part childPart in part.children) {
						AntennaListRemoveItem (childPart);
					}

					foreach (ModuleDataTransmitter remA in remAntenna) {
						AntennaListRemoveItem (remA);
					}
					DoTheMath ();
				}
			}
		}

		public void PodDeleted ()
		{
			CreateAntennaList ();
			DoTheMath ();
		}

		public void PodPicked (Part part = null)
		{
			CreateAntennaList ();
			DoTheMath ();
		}

		#region Logic
		public List<ModuleDataTransmitter> directAntennaList = new List<ModuleDataTransmitter> ();// Main list
		public List<ModuleDataTransmitter> relayAntennaList = new List<ModuleDataTransmitter> ();

		public List<ModuleDataTransmitter> directCombAntennaList = new List<ModuleDataTransmitter> ();
		public List<ModuleDataTransmitter> relayCombAntennaList = new List<ModuleDataTransmitter> ();

		public static int nbDirectAntenna = 0;
		public static int nbDirectCombAntenna = 0;

		public static int nbRelayAntenna = 0;
		public static int nbRelayCombAntenna = 0;

		public static double directPower;
		public static double directCombPower;

		public static double relayPower;
		public static double relayCombPower;

		public static double directRange;
		public static double directCombRange;

		public static double relayRange;
		public static double relayCombRange;

		public static string directAntennaName = "";
		public static string relayAntennaName = "";

		public static List<MyTuple> relaySignalPerPlanet;
		public static List<MyTuple> directSignalPerPlanet;

		public static double directDistanceAt100;
		public static double directDistanceAt75;
		public static double directDistanceAt50;
		public static double directDistanceAt25;

		public static double relayDistanceAt100;
		public static double relayDistanceAt75;
		public static double relayDistanceAt50;
		public static double relayDistanceAt25;

		public void DoTheMath ()
		{
			// Direct antenna (not-relay)
			nbDirectAntenna = directAntennaList.Count;
			nbDirectCombAntenna = directCombAntennaList.Count;

			// Direct combinable :
			if (nbDirectCombAntenna > 0) {
				directCombPower = AntennaHelperUtil.GetVesselPower (directCombAntennaList);
				directCombRange = AntennaHelperUtil.GetRange (directCombPower, targetPower);
			} else {
				directCombPower = 0;
				directCombRange = 0;
			}


			// Direct straight :
			if (nbDirectAntenna > 0) {
				ModuleDataTransmitter bigDirect = null;
				foreach (ModuleDataTransmitter antenna in directAntennaList) {
					if (bigDirect == null || bigDirect.antennaPower < antenna.antennaPower) {
						bigDirect = antenna;
					}
				}
				directPower = bigDirect.antennaPower;
				directRange = AntennaHelperUtil.GetRange (directPower, targetPower);
				directAntennaName = bigDirect.part.partInfo.title;
			} else {
				directPower = 0;
				directRange = 0;
				directAntennaName = "No Antenna";
			}


			// Relay antenna :
			nbRelayAntenna = relayAntennaList.Count;
			nbRelayCombAntenna = relayCombAntennaList.Count;

			// Relay combinable :
			if (nbRelayCombAntenna > 0) {
				relayCombPower = AntennaHelperUtil.GetVesselPower (relayCombAntennaList);
				relayCombRange = AntennaHelperUtil.GetRange (relayCombPower, targetPower);
			} else {
				relayCombPower = 0;
				relayCombRange = 0;
			}


			// Relay straight :
			if (nbRelayAntenna > 0) {
				ModuleDataTransmitter bigRelay = null;
				foreach (ModuleDataTransmitter antenna in relayAntennaList) {
					if (bigRelay == null || bigRelay.antennaPower < antenna.antennaPower) {
						bigRelay = antenna;
					}
				}
				relayPower = bigRelay.antennaPower;
				relayRange = AntennaHelperUtil.GetRange (relayPower, targetPower);
				relayAntennaName = bigRelay.part.partInfo.title;
			} else {
				relayPower = 0;
				relayRange = 0;
				relayAntennaName = "No Antenna";
			}

			FetchBetterAntennas ();
			FetchAntennaStatus ();
			SetPerPlanetList ();

			directDistanceAt100 = AntennaHelperUtil.GetDistanceAt100 (directBetterRange);
			directDistanceAt75 = AntennaHelperUtil.GetDistanceAt75 (directBetterRange);
			directDistanceAt50 = AntennaHelperUtil.GetDistanceAt50 (directBetterRange);
			directDistanceAt25 = AntennaHelperUtil.GetDistanceAt25 (directBetterRange);

			relayDistanceAt100 = AntennaHelperUtil.GetDistanceAt100 (relayBetterRange);
			relayDistanceAt75 = AntennaHelperUtil.GetDistanceAt75 (relayBetterRange);
			relayDistanceAt50 = AntennaHelperUtil.GetDistanceAt50 (relayBetterRange);
			relayDistanceAt25 = AntennaHelperUtil.GetDistanceAt25 (relayBetterRange);
		}

		public static double directBetterPower;
		public static double directBetterRange;

		public static double relayBetterPower;
		public static double relayBetterRange;

		private void FetchBetterAntennas ()
		{
			if (directRange > directCombRange || directPower > directCombPower) {
				directBetterPower = directPower;
				directBetterRange = directRange;
			} else {
				directBetterPower = directCombPower;
				directBetterRange = directCombRange;
			}

			if (relayRange > relayCombRange || relayPower > relayCombPower) {
				relayBetterPower = relayPower;
				relayBetterRange = relayRange;
			} else {
				relayBetterPower = relayCombPower;
				relayBetterRange = relayCombRange;
			}
		}

		public static string statusStringDirect;
		public static string statusStringRelay;

		private void FetchAntennaStatus ()
		{
			// DIRECT
			if (nbDirectAntenna == 0) {
				statusStringDirect = "No antenna";
			} else if (nbDirectAntenna == 1) {
				statusStringDirect = "One antenna : " + directAntennaName;
			} else {
				if (nbDirectCombAntenna < 2) {
					statusStringDirect = nbDirectAntenna + " antennas, not combinable, "
					+ directAntennaName + " is the most powerfull";
				} else {
					statusStringDirect = nbDirectCombAntenna + " of " + nbDirectAntenna 
					+ " antennas are combinable";
				}
			}

			// RELAY
			if (nbRelayAntenna == 0) {
				statusStringRelay = "No antenna";
			} else if (nbRelayAntenna == 1) {
				statusStringRelay = "One antenna : " + relayAntennaName;
			} else {
				if (nbRelayCombAntenna < 2) {
					statusStringRelay = nbRelayAntenna + " antennas, not combinable, "
						+ relayAntennaName + " is the most powerfull";
				} else {
					statusStringRelay = nbRelayCombAntenna + " of " + nbRelayAntenna 
					+ " antennas are combinable";
				}
			}
		}

		public static List<double> signalMinDirect;
		public static List<double> signalMaxDirect;
		public static List<double> signalMinRelay;
		public static List<double> signalMaxRelay;

		private void SetPerPlanetList ()
		{
			signalMinDirect = new List<double> ();
			signalMaxDirect = new List<double> ();
			signalMinRelay = new List<double> ();
			signalMaxRelay = new List<double> ();

			foreach (MyTuple planet in AntennaHelperUtil.signalPlanetList) {
				signalMinDirect.Add (AntennaHelperUtil.GetSignalStrength (directBetterRange, planet.item2));
				signalMaxDirect.Add (AntennaHelperUtil.GetSignalStrength (directBetterRange, planet.item3));
				signalMinRelay.Add (AntennaHelperUtil.GetSignalStrength (relayBetterRange, planet.item2));
				signalMaxRelay.Add (AntennaHelperUtil.GetSignalStrength (relayBetterRange, planet.item3));
			}
		}

		public static double signalCustomDistanceDirect = 0;
		public static double signalCustomDistanceRelay = 0;
		public static string customDistance = "";

		public static void CalcCustomDistance ()
		{
			signalCustomDistanceDirect = AntennaHelperUtil.GetSignalStrength (directBetterRange, Double.Parse (customDistance));
			signalCustomDistanceRelay = AntennaHelperUtil.GetSignalStrength (relayBetterRange, Double.Parse (customDistance));

		}


		private static double targetPower = 0;
		public static string targetName = "";

		public static void SetTarget (MyTuple tuple)
		{
			targetPower = tuple.item2;
			targetName = tuple.item1;
			instance.DoTheMath ();
		}

		public void CreateAntennaList ()
		{
			directAntennaList = new List<ModuleDataTransmitter> ();
			directCombAntennaList = new List<ModuleDataTransmitter> ();
			relayAntennaList = new List<ModuleDataTransmitter> ();
			relayCombAntennaList = new List<ModuleDataTransmitter> ();

			foreach (Part part in EditorLogic.fetch.ship.Parts) {
				foreach (ModuleDataTransmitter antenna in part.Modules.GetModules<ModuleDataTransmitter> ()) {
					directAntennaList.Add (antenna);
					if (antenna.antennaCombinable) {
						directCombAntennaList.Add (antenna);
					}
					if (antenna.antennaType == AntennaType.RELAY) {
						relayAntennaList.Add (antenna);
						if (antenna.antennaCombinable) {
							relayCombAntennaList.Add (antenna);
						}
					}
				}
			}
		}

		public void AntennaListAddItem (ModuleDataTransmitter antenna)
		{
			directAntennaList.Add (antenna);
			if (antenna.antennaCombinable) {
				directCombAntennaList.Add (antenna);
			}
			if (antenna.antennaType == AntennaType.RELAY) {
				relayAntennaList.Add (antenna);
				if (antenna.antennaCombinable) {
					relayCombAntennaList.Add (antenna);
				}
			}
		}

		public void AntennaListAddItem (Part part)
		{
			if (part.Modules.Contains<ModuleDataTransmitter> ()) {
				foreach (ModuleDataTransmitter antenna in part.Modules.GetModules<ModuleDataTransmitter> ()) {
					AntennaListAddItem (antenna);
				}
			}
		}

		public void AntennaListRemoveItem (ModuleDataTransmitter antenna)
		{
			if (directAntennaList.Contains (antenna)) {
				directAntennaList.Remove (antenna);
			}
			if (directCombAntennaList.Contains (antenna)) {
				directCombAntennaList.Remove (antenna);
			}
			if (relayAntennaList.Contains (antenna)) {
				relayAntennaList.Remove (antenna);
			}
			if (relayCombAntennaList.Contains (antenna)) {
				relayCombAntennaList.Remove (antenna);
			}
		}

		public void AntennaListRemoveItem (Part part)
		{
			if (part.Modules.Contains<ModuleDataTransmitter> ()) {
				foreach (ModuleDataTransmitter antenna in part.Modules.GetModules<ModuleDataTransmitter> ()) {
					AntennaListRemoveItem (antenna);
				}
			}
		}
		#endregion

		#region GUI
		public static bool showMainWindow = false;
		public static Rect rectMainWindow = new Rect (AntennaHelperUtil.centerScreen.x, 
			AntennaHelperUtil.centerScreen.y, 400, 200);

		public static bool showTargetWindow = false;
		public static Rect rectTargetWindow = new Rect (AntennaHelperUtil.centerScreen.x - 400, 
			AntennaHelperUtil.centerScreen.y, 400, 80);

		public static bool showPlanetWindow = false;
		public static Rect rectPlanetWindow = new Rect (AntennaHelperUtil.centerScreen.x + 400, 
			AntennaHelperUtil.centerScreen.y, 450, 240);

		public void OnGUI ()
		{
			if (showMainWindow) {
				GUILayout.BeginArea (rectMainWindow);
				rectMainWindow = GUILayout.Window (835298, rectMainWindow, EditorWindows.MainWindow, "Antenna Helper");
				GUILayout.EndArea ();
			}
			if (showTargetWindow) {
				GUILayout.BeginArea (rectTargetWindow);
				rectTargetWindow = GUILayout.Window (419256, rectTargetWindow, EditorWindows.TargetWindow, "Pick A Target");
				GUILayout.EndArea ();
			}
			if (showPlanetWindow) {
				GUILayout.BeginArea (rectPlanetWindow);
				rectPlanetWindow = GUILayout.Window (332980, rectPlanetWindow, EditorWindows.PlanetWindow, "Signal Strength / Distance");
				GUILayout.EndArea ();
			}
		}
		#endregion

		#region ToolbarButton
		private KSP.UI.Screens.ApplicationLauncherButton toolbarButton;

		private void AddToolbarButton ()
		{
			toolbarButton = KSP.UI.Screens.ApplicationLauncher.Instance.AddModApplication (
				ToolbarButtonOnTrue, 
				ToolbarButtonOnFalse, 
				AntennaHelperUtil.DummyVoid, 
				AntennaHelperUtil.DummyVoid, 
				AntennaHelperUtil.DummyVoid, 
				AntennaHelperUtil.DummyVoid,
				KSP.UI.Screens.ApplicationLauncher.AppScenes.VAB | KSP.UI.Screens.ApplicationLauncher.AppScenes.SPH,
				AntennaHelperUtil.toolbarButtonTex);
		}

		private void RemoveToolbarButton ()
		{
			ToolbarButtonOnFalse ();
			KSP.UI.Screens.ApplicationLauncher.Instance.RemoveModApplication (toolbarButton);
		}

		private void ToolbarButtonOnTrue ()
		{
			CreateAntennaList ();
			DoTheMath ();
			showMainWindow = true;
		}

		private void ToolbarButtonOnFalse ()
		{
			showMainWindow = false;
			showTargetWindow = false;
			showPlanetWindow = false;
		}
		#endregion
	}
}

