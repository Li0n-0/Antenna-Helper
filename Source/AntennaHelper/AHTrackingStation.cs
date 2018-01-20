using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AntennaHelper
{
	[KSPAddon (KSPAddon.Startup.TrackingStation, false)]
	public class AHTrackingStation : MonoBehaviour
	{
		private string targetPid;
		private Dictionary<string, Dictionary<string, string>> listShipTransmitter;
		private Dictionary<string, Dictionary<string, string>> listShipRelay;
		private Dictionary<string, Dictionary<string, List<GameObject>>> listMarkers;

		private float trackingStationLvl;
		private double dsnPower;

		// GUI
		private KSP.UI.Screens.ApplicationLauncherButton toolbarButton;
		private Rect rectMainWindow, rectEditorShipWindow;
		private Vector2 scrollerEditorShipWindow;
		private GUICircleSelection circleTypeSelected;
		private bool mainWindowOn, editorShipWindowOn;

		public void Start ()
		{
			targetPid = "";

			trackingStationLvl = ScenarioUpgradeableFacilities.GetFacilityLevel (SpaceCenterFacility.TrackingStation);
			dsnPower = GameVariables.Instance.GetDSNRange (trackingStationLvl);

			GameEvents.onPlanetariumTargetChanged.Add (NewTarget);
			GameEvents.OnMapFocusChange.Add (NewTarget);
			GameEvents.CommNet.OnCommStatusChange.Add (CommNetUpdate);

			GetListsShip ();
			CreateMarkers ();

			// GUI
			rectMainWindow = new Rect (0, 0, 150, 250);
			rectMainWindow.position = new Vector2 (Screen.width - rectMainWindow.width, Screen.height - rectMainWindow.height - 40);
			rectEditorShipWindow = new Rect (0, 0, 150, 200);
			rectEditorShipWindow.position = new Vector2 (rectMainWindow.position.x - rectEditorShipWindow.width, rectMainWindow.position.y);
			circleTypeSelected = GUICircleSelection.ACTIVE;
			mainWindowOn = false;
			editorShipWindowOn = false;

			GameEvents.onGUIApplicationLauncherReady.Add (AddToolbarButton);
			GameEvents.onGUIApplicationLauncherDestroyed.Add (RemoveToolbarButton);
		}

		public void OnDestroy ()
		{
			DestroyMarkers ();

			GameEvents.onPlanetariumTargetChanged.Remove (NewTarget);
			GameEvents.OnMapFocusChange.Remove (NewTarget);
			GameEvents.CommNet.OnCommStatusChange.Remove (CommNetUpdate);

			// GUI
			RemoveToolbarButton ();

			GameEvents.onGUIApplicationLauncherReady.Remove (AddToolbarButton);
			GameEvents.onGUIApplicationLauncherDestroyed.Remove (RemoveToolbarButton);
		}

		private void NewTarget (MapObject targetMapObject = null)
		{
			if (targetMapObject != null && targetMapObject.vessel != null) {
				targetPid = targetMapObject.vessel.id.ToString ();
				if (mainWindowOn) {
					ShowCircles ();
				}
			}
		}

		private void CommNetUpdate (Vessel v, bool b)
		{
			DestroyMarkers ();
			GetListsShip ();
			CreateMarkers ();
		}

		private void GetListShipTransmitter ()
		{
			listShipTransmitter = AHShipList.GetShipList (true, true);
		}

		private void GetListShipRelay ()
		{
			listShipRelay = new Dictionary<string, Dictionary<string, string>> ();
			foreach (KeyValuePair<string, Dictionary<string, string>> vesselPairInfo in listShipTransmitter) {
				if (vesselPairInfo.Value ["type"] != "VAB" && vesselPairInfo.Value ["type"] != "SPH") {
					if (vesselPairInfo.Value ["powerRelay"] != "0") {
						listShipRelay.Add (vesselPairInfo.Key, vesselPairInfo.Value);
					}
				}
			}
		}

		private void GetListsShip ()
		{
			GetListShipTransmitter ();
			GetListShipRelay ();
		}

		private void CreateMarkers ()
		{
			listMarkers = new Dictionary<string, Dictionary<string, List<GameObject>>> ();

			foreach (KeyValuePair<string, Dictionary<string, string>> vesselPairInfo in listShipTransmitter)
			{
//				Debug.Log ("[AH] creating marker for vessel : " + vesselPairInfo.Value ["name"]);

				listMarkers.Add (vesselPairInfo.Key, new Dictionary<string, List<GameObject>> ());

				double vesselPower, maxRange, realSignal;
				Vessel transmiter;
				bool editorShip;
				bool isHome;
				Transform relay;
				AHMapMarker marker;

				vesselPower = AHUtil.TruePower (Double.Parse (vesselPairInfo.Value ["powerTotal"]));
				if ((vesselPairInfo.Value ["type"] == "VAB") || (vesselPairInfo.Value ["type"] == "SPH")) {
					transmiter = null;
					editorShip = true;
				} else {
					transmiter = FlightGlobals.Vessels.Find (v => v.id.ToString () == vesselPairInfo.Key);
					editorShip = false;
				}
//				Debug.Log ("[AH] vessel power computed");

				// Active Connection :
				if (vesselPairInfo.Value ["connectedTo"] == "") {
					// the active connection is to the DSN or isn't set
					maxRange = AHUtil.GetRange (vesselPower, dsnPower);
					realSignal = 1d;
					isHome = true;
					relay = Planetarium.fetch.Home.MapObject.trf;
				} else {
					// active connection going trough a relay
					maxRange = AHUtil.GetRange (vesselPower, AHUtil.TruePower (Double.Parse (listShipRelay [vesselPairInfo.Value ["connectedTo"]] ["powerRelay"])));
					realSignal = Double.Parse (listShipRelay [vesselPairInfo.Value ["connectedTo"]] ["realSignal"]);
					isHome = false;
					relay = FlightGlobals.Vessels.Find (v => v.id.ToString () == vesselPairInfo.Value ["connectedTo"]).mapObject.trf;
				}
				maxRange = AHUtil.GetDistanceAt0 (maxRange);
				listMarkers [vesselPairInfo.Key].Add ("ACTIVE", new List<GameObject> ());
				listMarkers [vesselPairInfo.Key] ["ACTIVE"].Add (new GameObject ());
				marker = listMarkers [vesselPairInfo.Key] ["ACTIVE"] [0].AddComponent<AHMapMarker> ();
				marker.SetUp (maxRange, transmiter, relay, isHome, realSignal, editorShip);
//				Debug.Log ("[AH] active connection done");

				// DSN Connection :
				maxRange = AHUtil.GetRange (vesselPower, dsnPower);
				maxRange = AHUtil.GetDistanceAt0 (maxRange);
				realSignal = 1d;
				isHome = true;
				relay = Planetarium.fetch.Home.MapObject.trf;

				listMarkers [vesselPairInfo.Key].Add ("DSN", new List<GameObject> ());
				listMarkers [vesselPairInfo.Key] ["DSN"].Add (new GameObject ());
				marker = listMarkers [vesselPairInfo.Key] ["DSN"] [0].AddComponent<AHMapMarker> ();
				marker.SetUp (maxRange, transmiter, relay, isHome, realSignal, editorShip);
//				Debug.Log ("[AH] dsn connection done");

				// Relay(s) Connection :
				List<GameObject> listRelayMarkers = new List<GameObject> ();
				foreach (KeyValuePair<string, Dictionary<string, string>> relayPairInfo in listShipRelay) {
					if (relayPairInfo.Key == vesselPairInfo.Key) {
						continue;
					}
					maxRange = AHUtil.GetRange (vesselPower, AHUtil.TruePower (Double.Parse (relayPairInfo.Value ["powerRelay"])));
					maxRange = AHUtil.GetDistanceAt0 (maxRange);
					realSignal = Double.Parse (relayPairInfo.Value ["realSignal"]);
					isHome = false;
					relay = FlightGlobals.Vessels.Find (v => v.id.ToString () == relayPairInfo.Key).mapObject.trf;

					listRelayMarkers.Add (new GameObject ());
					marker = listRelayMarkers [listRelayMarkers.Count - 1].AddComponent<AHMapMarker> ();
					marker.SetUp (maxRange, transmiter, relay, isHome, realSignal, editorShip);
				}
				listMarkers [vesselPairInfo.Key].Add ("RELAY", listRelayMarkers);
//				Debug.Log ("[AH] relays connections done");
			}
		}

		private void UpdateMarkersScale ()
		{
			foreach (KeyValuePair<string, Dictionary<string, List<GameObject>>> vesselPairMarker in listMarkers) {
				foreach (KeyValuePair<string, List<GameObject>> connectPairMarker in vesselPairMarker.Value) {
					foreach (GameObject marker in connectPairMarker.Value) {
						double newRealSignal = 1d;
						if (listShipTransmitter [vesselPairMarker.Key] ["connectedTo"] != "") {
							newRealSignal = Double.Parse (listShipRelay [listShipTransmitter [vesselPairMarker.Key] ["connectedTo"]] ["realSignal"]);
						}
						marker.GetComponent<AHMapMarker> ().SetScale (newRealSignal);
					}
				}
			}
		}

		private void DestroyMarkers ()
		{
			foreach (KeyValuePair<string, Dictionary<string, List<GameObject>>> vesselPairMarker in listMarkers) {
				foreach (KeyValuePair<string, List<GameObject>> connectPairMarker in vesselPairMarker.Value) {
					foreach (GameObject marker in connectPairMarker.Value) {
						Destroy (marker);
					}
				}
			}
		}

		private void ShowCircles ()
		{
			HideCircles ();

			if (targetPid == "") {
				return;
			}

			switch (circleTypeSelected) {
			case GUICircleSelection.ACTIVE:
				listMarkers [targetPid] ["ACTIVE"] [0].GetComponent<AHMapMarker> ().Show ();
				break;
			case GUICircleSelection.DSN:
				listMarkers [targetPid] ["DSN"] [0].GetComponent<AHMapMarker> ().Show ();
				break;
			case GUICircleSelection.RELAY:
				foreach (GameObject marker in listMarkers [targetPid] ["RELAY"]) {
					marker.GetComponent<AHMapMarker> ().Show ();
				}
				break;
			case GUICircleSelection.DSN_AND_RELAY:
			default:
				listMarkers [targetPid] ["DSN"] [0].GetComponent<AHMapMarker> ().Show ();
				foreach (GameObject marker in listMarkers [targetPid] ["RELAY"]) {
					marker.GetComponent<AHMapMarker> ().Show ();
				}
				break;
			}
		}

		private void HideCircles ()
		{
			foreach (KeyValuePair<string, Dictionary<string, List<GameObject>>> vesselPairMarker in listMarkers) {
				foreach (KeyValuePair<string, List<GameObject>> connectPairMarker in vesselPairMarker.Value) {
					foreach (GameObject marker in connectPairMarker.Value) {
						marker.GetComponent<AHMapMarker> ().Hide ();
					}
				}
			}
		}

		#region GUI
		public void OnGUI ()
		{
			if (mainWindowOn) {
				rectMainWindow = GUI.Window (889204, rectMainWindow, MainWindow, "Antenna Helper");
			}

			if (editorShipWindowOn) {
				rectEditorShipWindow = GUI.Window (524258, rectEditorShipWindow, EditorShipListWindow, "Editor Ship List");
			}
		}

		private void MainWindow (int id)
		{
			GUILayout.BeginVertical ();

			string transmitterName = "";
			if (targetPid != "") {
				transmitterName = listShipTransmitter [targetPid] ["name"];
			}

			GUILayout.Label ("Transmitter : " + transmitterName);
			GUILayout.Label ("Display Type : " + circleTypeSelected.ToString ());
			GUILayout.Space (3f);
			if (GUILayout.Button ("Active Connection")) {
				circleTypeSelected = GUICircleSelection.ACTIVE;
				ShowCircles ();
			}
			if (GUILayout.Button ("DSN")) {
				circleTypeSelected = GUICircleSelection.DSN;
				ShowCircles ();
			}
			if (GUILayout.Button ("Relay")) {
				circleTypeSelected = GUICircleSelection.RELAY;
				ShowCircles ();
			}
			if (GUILayout.Button ("DSN + Relay")) {
				circleTypeSelected = GUICircleSelection.DSN_AND_RELAY;
				ShowCircles ();
			}

			GUILayout.Space (10f);
			if (GUILayout.Button ("Editor Ship List")) {
				editorShipWindowOn = !editorShipWindowOn;
			}

			GUILayout.EndVertical ();

			GUI.DragWindow ();
		}

		private void EditorShipListWindow (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (rectEditorShipWindow.size.x - 22, 2, 20, 20), "X")) {
				editorShipWindowOn = false;
			}

			GUILayout.BeginVertical ();
			scrollerEditorShipWindow = GUILayout.BeginScrollView (scrollerEditorShipWindow);
			foreach (KeyValuePair<string, Dictionary <string, string>> vesselPairInfo in listShipTransmitter) {
				if ((vesselPairInfo.Value ["type"] == "VAB") || (vesselPairInfo.Value ["type"] == "SPH")) {
					if (GUILayout.Button (vesselPairInfo.Value ["name"] + "  (" + AHUtil.TruePower (Double.Parse (vesselPairInfo.Value ["powerTotal"])).ToString () + ")")) {
						targetPid = vesselPairInfo.Key;
						ShowCircles ();
					}
				}
			}
			GUILayout.EndScrollView ();
			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}

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

		public void Update ()
		{
			if (Input.GetKeyDown (KeyCode.Keypad5)) {
				Debug.Log ("[AH] parsing ship list :");
				foreach (KeyValuePair<string, Dictionary<string, string>> kvp in listShipTransmitter) {
					Debug.Log ("[AH] info on ship : " + kvp.Key);
					foreach (KeyValuePair<string, string> infos in kvp.Value) {
						Debug.Log ("[AH] " + infos.Key + " : " + infos.Value);
					}
				}
				return;
			}
		}

		private void ToolbarButtonOnTrue ()
		{
			
			mainWindowOn = true;
			ShowCircles ();

			// Change the button texture :
			if (UnityEngine.Random.Range (0, 2) == 1) {
				toolbarButton.SetTexture (AHUtil.toolbarButtonTexSatOn);
			} else {
				toolbarButton.SetTexture (AHUtil.toolbarButtonTexDishOn);
			}
		}

		private void ToolbarButtonOnFalse ()
		{
			mainWindowOn = false;
			editorShipWindowOn = false;
			HideCircles ();
			// Change the button texture :
			toolbarButton.SetTexture (AHUtil.toolbarButtonTexOff);
		}
		#endregion
	}
}

