using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AntennaHelper
{
	public static class AHShipList
	{
		private static Dictionary<string, Dictionary <string, string>> listEditorVessel;
		private static Dictionary<string, Dictionary <string, string>> listFlyingVessel;
		private static string savePath;
		private static string loadedGame;
		private static bool loadedOnce;

		static AHShipList ()
		{
			savePath = KSPUtil.ApplicationRootPath + "GameData/AntennaHelper/PluginData/VesselList.cfg";
			loadedOnce = false;
		}

		private static void DoStart ()
		{
			loadedGame = HighLogic.CurrentGame.Title;
			if (!LoadFromFile (loadedGame)) {
				SaveToFile ();
			}
			loadedOnce = true;
		}

		private static bool LoadFromFile (string saveTitle)
		{
			listEditorVessel = new Dictionary<string, Dictionary<string, string>> ();

			ConfigNode confNode = ConfigNode.Load (savePath);
			if (confNode == null) {
				Debug.Log ("[AH] no vessel list can be found");
				return false;
			}

			ConfigNode saveNode = confNode.GetNode ("SAVE", "title", saveTitle);
			if (saveNode == null) {
				Debug.Log ("[AH] no vessel list for this save");
				return false;
			}

			if (!saveNode.HasNode ("VESSEL")) {
				Debug.Log ("[AH] no vessel in the vessel list for this save");
				return true;
			}

			foreach (ConfigNode vesselNode in saveNode.GetNodes ("VESSEL")) {
				string vesselPid = vesselNode.GetValue ("pid");
				listEditorVessel.Add (vesselPid, new Dictionary<string, string> ());
				listEditorVessel [vesselPid].Add ("name", vesselNode.GetValue ("name"));
				listEditorVessel [vesselPid].Add ("type", vesselNode.GetValue ("type"));
				listEditorVessel [vesselPid].Add ("powerTotal", vesselNode.GetValue ("powerTotal"));
				listEditorVessel [vesselPid].Add ("powerRelay", vesselNode.GetValue ("powerRelay"));
				listEditorVessel [vesselPid].Add ("saveDate", vesselNode.GetValue ("saveDate"));


			}
			return true;
		}

		private static void SaveToFile ()
		{
			ConfigNode mainNode = ConfigNode.Load (savePath);
			if (mainNode == null) {
				mainNode = new ConfigNode ("AH_VESSEL_LIST");
			}

			ConfigNode saveNode = mainNode.GetNode ("SAVE", "title", loadedGame);
			if (saveNode == null) {
				saveNode = mainNode.AddNode ("SAVE");
				saveNode.AddValue ("title", loadedGame);
			}

			foreach (KeyValuePair<string, Dictionary <string, string>> vesselPairInfo in listEditorVessel) {
				
				ConfigNode vesselNode = saveNode.GetNode ("VESSEL", "pid", vesselPairInfo.Key);
				if (vesselNode == null) {
					vesselNode = saveNode.AddNode ("VESSEL");
					vesselNode.AddValue ("pid", vesselPairInfo.Key);
				}

				foreach (KeyValuePair<string, string> kvp in vesselPairInfo.Value) {
					vesselNode.SetValue (kvp.Key, kvp.Value, true);
				}
			}

			mainNode.Save (savePath);
		}

		public static void ParseFlyingVessel ()
		{
			listFlyingVessel = new Dictionary<string, Dictionary<string, string>> ();

			foreach (Vessel v in FlightGlobals.Vessels) {
				if ((v.vesselType != VesselType.EVA) && 
					(v.vesselType != VesselType.Flag) && 
					(v.vesselType != VesselType.SpaceObject) && 
					(v.vesselType != VesselType.Unknown) &&
					(v.vesselType != VesselType.Debris)) {

					string pid = v.id.ToString ();
					double vesselPower = AHUtil.GetActualVesselPower (v, false, true, false);
					double vesselRelayPower = AHUtil.GetActualVesselPower (v, true, true, false);

					listFlyingVessel.Add (pid, new Dictionary<string, string> ());
					listFlyingVessel [pid].Add ("name", v.GetName ());
					listFlyingVessel [pid].Add ("type", v.vesselType.ToString ());
					listFlyingVessel [pid].Add ("powerTotal", vesselPower.ToString ());
					listFlyingVessel [pid].Add ("powerRelay", vesselRelayPower.ToString ());
					listFlyingVessel [pid].Add ("saveDate", "");
					listFlyingVessel [pid].Add ("realSignal", "0");
				}
			}
		}

		public static void ComputeRealSignal (Vessel v)
		{
//			Debug.Log ("[AH] Computing real signal for vessel : " + v.GetName ());
			if (listFlyingVessel [v.id.ToString ()] != null) {
//				Debug.Log ("[AH] " + v.GetName () + " is in the list");
				if (v.Connection.IsConnected) {
					listFlyingVessel [v.id.ToString ()] ["realSignal"] = AHUtil.GetRealSignal (v.Connection.ControlPath).ToString ();
				} else {
					listFlyingVessel [v.id.ToString ()] ["realSignal"] = "0";
				}

//				Debug.Log ("[AH] its real signal = " + listFlyingVessel [v.id.ToString ()] ["realSignal"]);
			} else {
//				Debug.Log ("[AH] " + v.GetName () + " is not in the list");
			}
		}

		public static void ParseCraftFiles ()
		{
			// This run per save
			// Overwrite the existing data for this save
			listEditorVessel = new Dictionary<string, Dictionary<string, string>> ();
			Debug.Log ("[AH] Starting to parse .craft files for save : " + loadedGame);

			string loadedGameStr = loadedGame;
			if (loadedGame.Contains (" (SANDBOX)")) {
				loadedGameStr.Remove (loadedGame.IndexOf (" (SANDBOX)"));
			} else if (loadedGame.Contains (" (CAREER)")) {
				loadedGameStr.Remove (loadedGame.IndexOf (" (CAREER)"));
			} else if (loadedGame.Contains (" (SCIENCE)")) {
				loadedGameStr.Remove (loadedGame.IndexOf (" (SCIENCE)"));
			} else {
				Debug.Log ("[AH] the name of the save can't be parsed");
			}

			DirectoryInfo shipsDir = new DirectoryInfo (KSPUtil.ApplicationRootPath + "saves/" + loadedGameStr + "/Ships");
			FileInfo[] craftFiles = shipsDir.GetFiles ();

			foreach (FileInfo craft in craftFiles) {
				if (craft.Extension != "craft") {
					continue;
				}

				ConfigNode craftConf = ConfigNode.Load (craft.FullName);
				string craftName = craftConf.GetValue ("ship");
				listEditorVessel.Add (craftName, new Dictionary<string, string> ());
				listEditorVessel [craftName].Add ("type", craftConf.GetValue ("type"));


			}
		}

		public static void UpdateLoadedGame ()
		{
			if (!loadedOnce) {
				DoStart ();
				return;
			}

			if (HighLogic.CurrentGame.Title != loadedGame) {
				SaveToFile ();
				loadedGame = HighLogic.CurrentGame.Title;
				LoadFromFile (loadedGame);
			}
		}

		public static void SaveShip (string shipName, string type, string totalPower, string relayPower)
		{
			string pid = UnityEngine.Random.Range (1, 100000).ToString ();
			// this need to be more safe

			listEditorVessel.Add (pid, new Dictionary<string, string> ());
			listEditorVessel [pid].Add ("name", shipName);
			listEditorVessel [pid].Add ("type", type);
			listEditorVessel [pid].Add ("powerTotal", totalPower);
			listEditorVessel [pid].Add ("powerRelay", relayPower);
			listEditorVessel [pid].Add ("saveDate", System.DateTime.Now.ToString ());


			SaveToFile ();
		}

		public static Dictionary<string, Dictionary <string, string>> GetShipList (bool editorShip = true, bool onlyRelay = false)
		{
			Dictionary<string, Dictionary <string, string>> returnList;

			if (editorShip) {
				returnList = new Dictionary<string, Dictionary<string, string>> (listEditorVessel);
			} else {
				returnList = new Dictionary<string, Dictionary<string, string>> (listFlyingVessel);
			}

			if (onlyRelay) {
				foreach (KeyValuePair<string, Dictionary <string, string>> kvp in listEditorVessel) {
					if (kvp.Value["powerRelay"] == "0") {
						returnList.Remove (kvp.Key);
					}
				}
			}

			return returnList;
		}
	}

	[KSPAddon (KSPAddon.Startup.SpaceCentre, false)]
	public class AHShipListListener : MonoBehaviour
	{
		public void Start ()
		{
			AHShipList.UpdateLoadedGame ();

			GameEvents.CommNet.OnNetworkInitialized.Add (CommNetInit);
			GameEvents.CommNet.OnCommStatusChange.Add (CommNetChange);
		}

		public void OnDestroy ()
		{
			GameEvents.CommNet.OnNetworkInitialized.Remove (CommNetInit);
			GameEvents.CommNet.OnCommStatusChange.Remove (CommNetChange);
		}

		private void CommNetInit ()
		{
			// create the vessel list now but wait for the real signal

//			Debug.Log ("[AH] Commnet is initialized");
			AHShipList.ParseFlyingVessel ();
		}

		private void CommNetChange (Vessel v, bool b)
		{
			// add the real signal now
			if (v.Connection.ControlPath.Count > 0) {
				AHShipList.ComputeRealSignal (v);
			}
		}
	}

	[KSPAddon (KSPAddon.Startup.TrackingStation, false)]
	public class AHShipListListenerTR : MonoBehaviour
	{
		public void Start ()
		{
			AHShipList.UpdateLoadedGame ();

			GameEvents.CommNet.OnCommStatusChange.Add (CommNetChange);
		}

		public void OnDestroy ()
		{
			GameEvents.CommNet.OnCommStatusChange.Remove (CommNetChange);
		}

		private void CommNetChange (Vessel v, bool b)
		{
//			Debug.Log ("[AH] CommNet change for vessel : " + v.GetName ());
			AHShipList.ComputeRealSignal (v);
		}
	}
}

