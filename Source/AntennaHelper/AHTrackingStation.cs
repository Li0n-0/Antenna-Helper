using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AntennaHelper
{
	[KSPAddon (KSPAddon.Startup.TrackingStation, false)]
	public class AHTrackingStation : MonoBehaviour
	{
		// UI stuff
		private bool isGUIOn, isGUIShipListOn;
		private KSP.UI.Screens.ApplicationLauncherButton toolbarButton;
		private Rect windowRect, windowShipRect;
		private GUICircleSelection circleType;


		private bool aHIsReady;
		private float trackingStationLvl;
		private double dsnRange;
		private Dictionary<string, Dictionary <string, string>> shipList;

		public void Start ()
		{
			isGUIOn = false;
			isGUIShipListOn = false;
			windowRect = new Rect (0, 0, 150, 220);
			windowRect.position = new Vector2 (Screen.width - windowRect.width, Screen.height - windowRect.height - 40);
			windowShipRect = new Rect (0, 0, 150, 200);
			windowShipRect.position = new Vector2 (windowRect.position.x - windowShipRect.width, windowRect.position.y);
			circleType = GUICircleSelection.ACTIVE;

			aHIsReady = false;
			trackingStationLvl = ScenarioUpgradeableFacilities.GetFacilityLevel (SpaceCenterFacility.TrackingStation);
			dsnRange = GameVariables.Instance.GetDSNRange (trackingStationLvl);
			shipList = AHShipList.GetShipList ();

			GameEvents.onGUIApplicationLauncherReady.Add (AddToolbarButton);
			GameEvents.onGUIApplicationLauncherDestroyed.Add (RemoveToolbarButton);

			GameEvents.onPlanetariumTargetChanged.Add (TargetChanged);

			StartCoroutine ("WaitAtStart");
		}

		public void OnDestroy ()
		{
			DestroyMarkers ();
			RemoveToolbarButton ();
			GameEvents.onGUIApplicationLauncherReady.Remove (AddToolbarButton);
			GameEvents.onGUIEngineersReportDestroy.Remove (RemoveToolbarButton);
			GameEvents.onPlanetariumTargetChanged.Remove (TargetChanged);
		}

		private void DestroyMarkers ()
		{
			foreach (KeyValuePair<Vessel, Dictionary<string, List<GameObject>>> kvpVessel in listVessels) {
				foreach (KeyValuePair<string, List<GameObject>> kvpObject in kvpVessel.Value) {
					foreach (GameObject gO in kvpObject.Value) {
						Destroy (gO);
					}
				}
			}
		}

//		private void Update ()
//		{
//			if (Input.GetKeyDown (KeyCode.Keypad4)) {
//				foreach (KeyValuePair<Vessel, Dictionary<string, List<GameObject>>> kvpVessel in listVessels) {
//					foreach (KeyValuePair<string, List<GameObject>> kvpObject in kvpVessel.Value) {
//						foreach (GameObject gO in kvpObject.Value) {
//							gO.SetLayerRecursive (gO.layer - 1);
//						}
//					}
//				}
//				Debug.Log ("[AH] circles on layer " + listVessels [lastTarget] ["ACTIVE"] [0].layer);
//			}
//			if (Input.GetKeyDown (KeyCode.Keypad6)) {
//				foreach (KeyValuePair<Vessel, Dictionary<string, List<GameObject>>> kvpVessel in listVessels) {
//					foreach (KeyValuePair<string, List<GameObject>> kvpObject in kvpVessel.Value) {
//						foreach (GameObject gO in kvpObject.Value) {
//							gO.SetLayerRecursive (gO.layer + 1);
//						}
//					}
//				}
//				Debug.Log ("[AH] circles on layer " + listVessels [lastTarget] ["ACTIVE"] [0].layer);
//			}
//		}

		private IEnumerator WaitAtStart ()
		{
			// just to be sure that commnet is ready
			// .5s don't seem to be enough...
			yield return new WaitForSeconds (1f);
			FetchRelays ();
			FetchVessels ();
			aHIsReady = true;
		}

		private void TargetChanged (MapObject mapObject)
		{
			if (mapObject.vessel != null) {
				lastTarget = mapObject.vessel;
				if (isGUIOn) {
					HideFlightCircles ();
					ShowCircles ();
				}
			}
		}

		private Dictionary<Vessel, double> listRelays;
		private void FetchRelays ()
		{
			listRelays = new Dictionary<Vessel, double> ();

			foreach (Vessel v in FlightGlobals.Vessels) {
				if (v.vesselType == VesselType.Relay) {
					if (v.Connection.IsConnected) {
						double realSignal = AHUtil.GetRealSignal (v.Connection.ControlPath, v);
						listRelays.Add (v, realSignal);
					}
				}
			}
		}

		private Dictionary<Vessel, Dictionary<string, List<GameObject>>> listVessels;
		private Dictionary<string, Dictionary<string, List<GameObject>>> listVesselsPreFlight;
		private void FetchVessels ()
		{
			listVessels = new Dictionary<Vessel, Dictionary<string, List<GameObject>>> ();
			foreach (Vessel v in FlightGlobals.Vessels) {
				if ((v.vesselType != VesselType.EVA) && 
					(v.vesselType != VesselType.Flag) && 
					(v.vesselType != VesselType.SpaceObject) && 
					(v.vesselType != VesselType.Unknown) &&
					(v.vesselType != VesselType.Debris)) {

					listVessels.Add (v, MarkersForVessel (v));
				}
			}

			listVesselsPreFlight = new Dictionary<string, Dictionary<string, List<GameObject>>> ();
			foreach (KeyValuePair<string, Dictionary<string, string>> kvp in shipList) {
				listVesselsPreFlight.Add (kvp.Key, MarkersForPreFlightVessel (Double.Parse (kvp.Value ["powerTotal"])));
			}
		}

		private Dictionary<string, List<GameObject>> MarkersForPreFlightVessel (double vesselPower)
		{
			Dictionary<string, List<GameObject>> listMarker = new Dictionary<string, List<GameObject>> ();

			double range, realSignal;
			Transform relay;
			bool isHome;

			AHMapMarker marker;

			vesselPower = AHUtil.TruePower (vesselPower);

			// Active and DSN marker
			range = AHUtil.GetDistanceAt0 (AHUtil.GetRange (vesselPower, dsnRange));
			relay = FlightGlobals.GetHomeBody ().MapObject.trf;

			listMarker.Add ("ACTIVE", new List<GameObject> ());
			listMarker["ACTIVE"].Add (new GameObject ());
			marker = listMarker["ACTIVE"][0].AddComponent<AHMapMarker> ();
			marker.SetUp (range, null, relay, true, 1d, true);

			listMarker.Add ("DSN", new List<GameObject> ());
			listMarker["DSN"].Add (new GameObject ());
			marker = listMarker["DSN"][0].AddComponent<AHMapMarker> ();
			marker.SetUp (range, null, relay, true, 1d, true);

			// Relay marker
			List<GameObject> relaysMarker = new List<GameObject> ();
			foreach (KeyValuePair<string, Dictionary<string, string>> kvp in AHShipList.GetShipList (false, true)) {
				range = range = AHUtil.GetRange (vesselPower, AHUtil.TruePower (Double.Parse (kvp.Value ["powerRelay"])));
				range = AHUtil.GetDistanceAt0 (range);

				realSignal = Double.Parse (kvp.Value ["realSignal"]);
				relay = FlightGlobals.Vessels.Find (x => x.id.ToString () == kvp.Key).mapObject.trf;
				isHome = false;
				relaysMarker.Add (new GameObject ());
				marker = relaysMarker [relaysMarker.Count - 1].AddComponent<AHMapMarker> ();
				marker.SetUp (range, null, relay, isHome, realSignal, true);
			}
			listMarker.Add ("RELAY", relaysMarker);

			return listMarker;
		}

		private Dictionary<string, List<GameObject>> MarkersForVessel (Vessel v)
		{
			if (v.Connection == null) {
				Debug.Log ("[AH] Connection is null on vessel : " + v.GetName ());
				return new Dictionary<string, List<GameObject>> ();
			}

			Dictionary<string, List<GameObject>> listMarker = new Dictionary<string, List<GameObject>> ();

			double vesselPower, range, realSignal;
			Transform relay;
			bool isHome;

			AHMapMarker marker;

			vesselPower = AHUtil.GetActualVesselPower (v);
			// Active Marker
			// Check if connected to the DSN (home)
			if (!v.Connection.IsConnected || v.Connection.ControlPath[0].b.isHome) {
				range = AHUtil.GetRange (vesselPower, dsnRange);
				realSignal = 1d;
				relay = FlightGlobals.GetHomeBody ().MapObject.trf;
				isHome = true;
			} else {
				range = AHUtil.GetRange (vesselPower, v.Connection.ControlPath[0].b.antennaRelay.power);//is the range mod applied ?
				realSignal = AHUtil.GetRealSignal (v.Connection.ControlPath, v);
				relay = v.Connection.ControlPath [0].b.transform.GetComponent<Vessel> ().mapObject.trf;
				isHome = false;
			}
			// Get real max range (should be done by GetRange ?)
			range = AHUtil.GetDistanceAt0 (range);
			// Create the marker
			listMarker.Add ("ACTIVE", new List<GameObject> ());
			listMarker["ACTIVE"].Add (new GameObject ());
			marker = listMarker["ACTIVE"][0].AddComponent<AHMapMarker> ();
			marker.SetUp (range, v, relay, isHome, realSignal);

			// DSN marker
			range = AHUtil.GetDistanceAt0 (AHUtil.GetRange (vesselPower, dsnRange));
			realSignal = 1d;
			relay = FlightGlobals.GetHomeBody ().MapObject.trf;
			isHome = true;
			// Create the marker
			listMarker.Add ("DSN", new List<GameObject> ());
			listMarker["DSN"].Add (new GameObject ());
			marker = listMarker["DSN"][0].AddComponent<AHMapMarker> ();
			marker.SetUp (range, v, relay, isHome, realSignal);

			// Relays markers
			List<GameObject> relaysMarker = new List<GameObject> ();
			foreach (KeyValuePair<string, Dictionary<string, string>> kvp in AHShipList.GetShipList (false, true)) {
				if (kvp.Key == v.id.ToString ()) {
					continue;
				}
				Debug.Log ("[AH] powerRelay : " + kvp.Value ["powerRelay"]);
				range = AHUtil.GetRange (vesselPower, AHUtil.TruePower (Double.Parse (kvp.Value ["powerRelay"])));
				range = AHUtil.GetDistanceAt0 (range);
				Debug.Log ("[AH] realSignal : " + kvp.Value ["realSignal"]);
				realSignal = Double.Parse (kvp.Value ["realSignal"]);
				relay = FlightGlobals.Vessels.Find (x => x.id.ToString () == kvp.Key).mapObject.trf;
				isHome = false;
				relaysMarker.Add (new GameObject ());
				marker = relaysMarker [relaysMarker.Count - 1].AddComponent<AHMapMarker> ();
				marker.SetUp (range, v, relay, isHome, realSignal);
			}

			listMarker.Add ("RELAY", relaysMarker);

			return listMarker;
		}

		private Vessel lastTarget;

		private void ShowPreFlightCircles (string vesselPid)
		{
			HideCircles ();

			switch (circleType) {
			case GUICircleSelection.ACTIVE:
				listVesselsPreFlight [vesselPid] ["ACTIVE"] [0].GetComponent<AHMapMarker> ().Show ();
				listVesselsPreFlight [vesselPid] ["DSN"] [0].GetComponent<AHMapMarker> ().Hide ();
				foreach (GameObject gO in listVesselsPreFlight [vesselPid] ["RELAY"]) {
					gO.GetComponent<AHMapMarker> ().Hide ();
				}
				break;
			case GUICircleSelection.DSN:
				listVesselsPreFlight [vesselPid] ["ACTIVE"] [0].GetComponent<AHMapMarker> ().Hide ();
				listVesselsPreFlight [vesselPid] ["DSN"] [0].GetComponent<AHMapMarker> ().Show ();
				foreach (GameObject gO in listVesselsPreFlight [vesselPid] ["RELAY"]) {
					gO.GetComponent<AHMapMarker> ().Hide ();
				}
				break;
			case GUICircleSelection.RELAY:
				listVesselsPreFlight [vesselPid] ["ACTIVE"] [0].GetComponent<AHMapMarker> ().Hide ();
				listVesselsPreFlight [vesselPid] ["DSN"] [0].GetComponent<AHMapMarker> ().Hide ();
				foreach (GameObject gO in listVesselsPreFlight [vesselPid] ["RELAY"]) {
					gO.GetComponent<AHMapMarker> ().Show ();
				}
				break;
			case GUICircleSelection.DSN_AND_RELAY:
			default:
				listVesselsPreFlight [vesselPid] ["ACTIVE"] [0].GetComponent<AHMapMarker> ().Hide ();
				listVesselsPreFlight [vesselPid] ["DSN"] [0].GetComponent<AHMapMarker> ().Show ();
				foreach (GameObject gO in listVesselsPreFlight [vesselPid] ["RELAY"]) {
					gO.GetComponent<AHMapMarker> ().Show ();
				}
				break;
			}
		}

		private void ShowCircles ()
		{
			HideCircles ();

			if (PlanetariumCamera.fetch.target.vessel != null) {
				lastTarget = PlanetariumCamera.fetch.target.vessel;
			}
			if (lastTarget == null) {
				return;
			}

			switch (circleType) {
			case GUICircleSelection.ACTIVE:
				listVessels [lastTarget] ["ACTIVE"] [0].GetComponent<AHMapMarker> ().Show ();
				listVessels [lastTarget] ["DSN"] [0].GetComponent<AHMapMarker> ().Hide ();
				foreach (GameObject gO in listVessels [lastTarget] ["RELAY"]) {
					gO.GetComponent<AHMapMarker> ().Hide ();
				}
				break;
			case GUICircleSelection.DSN:
				listVessels [lastTarget] ["ACTIVE"] [0].GetComponent<AHMapMarker> ().Hide ();
				listVessels [lastTarget] ["DSN"] [0].GetComponent<AHMapMarker> ().Show ();
				foreach (GameObject gO in listVessels [lastTarget] ["RELAY"]) {
					gO.GetComponent<AHMapMarker> ().Hide ();
				}
				break;
			case GUICircleSelection.RELAY:
				listVessels [lastTarget] ["ACTIVE"] [0].GetComponent<AHMapMarker> ().Hide ();
				listVessels [lastTarget] ["DSN"] [0].GetComponent<AHMapMarker> ().Hide ();
				foreach (GameObject gO in listVessels [lastTarget] ["RELAY"]) {
					gO.GetComponent<AHMapMarker> ().Show ();
				}
				break;
			case GUICircleSelection.DSN_AND_RELAY:
			default:
				listVessels [lastTarget] ["ACTIVE"] [0].GetComponent<AHMapMarker> ().Hide ();
				listVessels [lastTarget] ["DSN"] [0].GetComponent<AHMapMarker> ().Show ();
				foreach (GameObject gO in listVessels [lastTarget] ["RELAY"]) {
					gO.GetComponent<AHMapMarker> ().Show ();
				}
				break;
			}
		}

		private void HideCircles ()
		{
			HideFlightCircles ();
			HideEditorCircles ();
		}

		private void HideFlightCircles ()
		{
			foreach (KeyValuePair<Vessel, Dictionary<string, List<GameObject>>> kvpVessel in listVessels) {
				foreach (KeyValuePair<string, List<GameObject>> kvpObject in kvpVessel.Value) {
					foreach (GameObject gO in kvpObject.Value) {
						gO.GetComponent<AHMapMarker> ().Hide ();
					}
				}
			}
		}

		private void HideEditorCircles ()
		{
			foreach (KeyValuePair<string, Dictionary<string, List<GameObject>>> kvpVessel in listVesselsPreFlight) {
				foreach (KeyValuePair<string, List<GameObject>> kvpObject in kvpVessel.Value) {
					foreach (GameObject gO in kvpObject.Value) {
						gO.GetComponent<AHMapMarker> ().Hide ();
					}
				}
			}
		}

		#region GUI
		public void OnGUI ()
		{
			if (isGUIOn) {
				windowRect = GUI.Window (889204, windowRect, WindowTrackingStation, "Antenna Helper");
			}

			if (isGUIShipListOn) {
				windowShipRect = GUI.Window (524258, windowShipRect, WindowTrackingStationShipList, "Ship List");
			}
		}

		private void WindowTrackingStation (int id)
		{
			GUILayout.BeginVertical ();
			string vesselName;
			if (lastTarget != null) {
				vesselName = lastTarget.GetName ();
			} else {
				vesselName = "None";
			}
			GUILayout.Label ("Transmitter : " + vesselName);
			GUILayout.Label ("Display Type : " + circleType.ToString ());
			GUILayout.Space (3f);
			if (GUILayout.Button ("Active Connection")) {
				circleType = GUICircleSelection.ACTIVE;
				ShowCircles ();
			}
			if (GUILayout.Button ("DSN")) {
				circleType = GUICircleSelection.DSN;
				ShowCircles ();
			}
			if (GUILayout.Button ("Relay")) {
				circleType = GUICircleSelection.RELAY;
				ShowCircles ();
			}
			if (GUILayout.Button ("DSN + Relay")) {
				circleType = GUICircleSelection.DSN_AND_RELAY;
				ShowCircles ();
			}

			GUILayout.Space (10f);
			if (GUILayout.Button ("Show Ship List")) {
				isGUIShipListOn = true;
			}

			GUILayout.EndVertical ();

			GUI.DragWindow ();
		}

		private void WindowTrackingStationShipList (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (windowShipRect.size.x - 22, 2, 20, 20), "X")) {
				isGUIShipListOn = false;
			}

			GUILayout.BeginVertical ();
			foreach (KeyValuePair<string, Dictionary <string, string>> vesselPairInfo in shipList) {
				if (GUILayout.Button (vesselPairInfo.Value ["name"] + "  (" + AHUtil.TruePower (Double.Parse (vesselPairInfo.Value ["powerTotal"])).ToString () + ")")) {
					ShowPreFlightCircles (vesselPairInfo.Key);
				}
			}
			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}
		#endregion

		#region AppLauncher
		private void AddToolbarButton ()
		{
			toolbarButton = KSP.UI.Screens.ApplicationLauncher.Instance.AddModApplication (
				ToolbarButtonOnTrue, 
				ToolbarButtonOnFalse, 
				null, 
				null, 
				null, 
				null,
				KSP.UI.Screens.ApplicationLauncher.AppScenes.TRACKSTATION,
				AHUtil.toolbarButtonTexOff);
		}

		private void RemoveToolbarButton ()
		{
			KSP.UI.Screens.ApplicationLauncher.Instance.RemoveModApplication (toolbarButton);
		}

		private void ToolbarButtonOnTrue ()
		{
			if (aHIsReady) {
				isGUIOn = true;
				ShowCircles ();

				// Change the button texture :
				if (UnityEngine.Random.Range (0, 2) == 1) {
					toolbarButton.SetTexture (AHUtil.toolbarButtonTexSatOn);
				} else {
					toolbarButton.SetTexture (AHUtil.toolbarButtonTexDishOn);
				}
			}
		}

		private void ToolbarButtonOnFalse ()
		{
			if (aHIsReady) {
				isGUIOn = false;
				isGUIShipListOn = false;
				HideCircles ();
				// Change the button texture :
				toolbarButton.SetTexture (AHUtil.toolbarButtonTexOff);
			}
		}
		#endregion
	}
}

