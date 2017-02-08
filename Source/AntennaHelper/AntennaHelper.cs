using System;
using System.Collections.Generic;
using UnityEngine;

namespace AntennaHelper
{
	[KSPAddon (KSPAddon.Startup.EditorAny, false)]
	public class AntennaHelper : MonoBehaviour
	{
		private bool showMainWindow = false;
		private bool showTargetWindow = false;

		private KSP.UI.Screens.ApplicationLauncherButton button;

		private Rect mainWindowRect = new Rect(400f, 400f, 400f, 350f);
		private Rect targetWindowRect = new Rect (0, 0, 400, 500);

		//		private GUIStyle centeredStyle;
		// Toolbars :
		private int toolbarTypeIndex = 0;
		//		private int toolbarTypeIndex {
		//			get {return _toolbarTypeIndex;}
		//			set {
		//				_toolbarTypeIndex = value;
		//				CalcAntennas ();
		//			}
		//		}
		private string[] toolbarTypeStrings = {"Direct", "Relay"};

		private string targetStr = "DSN lvl 3";
		private int nbAntenna = 0;
		private int nbCombAntenna = 0;
		private double maxRangeFull = 0;
		private double maxRange = 0;
		private double maxRangeVessel = 0;

		private double targetPower = 250000000000;

		private List<ModuleDataTransmitter> activeAntennas;
		private List<ModuleDataTransmitter> targetAntennas;


		public void Start ()
		{
			GetFlightSat ();

			GameEvents.onEditorLoad.Add (NewShip);

			GameEvents.onEditorPartPlaced.Add (PartChange);
			GameEvents.onEditorPartDeleted.Add (PartChange);

			GameEvents.onGUIApplicationLauncherReady.Add (ToolbarButton);
			GameEvents.onGUIApplicationLauncherDestroyed.Add (DestroyButton);
		}

		public void OnDestroy ()
		{
			GameEvents.onEditorLoad.Remove (NewShip);

			GameEvents.onEditorPartPlaced.Remove (PartChange);
			GameEvents.onEditorPartDeleted.Remove (PartChange);

			GameEvents.onGUIApplicationLauncherReady.Remove (ToolbarButton);
			GameEvents.onGUIApplicationLauncherDestroyed.Remove (DestroyButton);

			DestroyButton ();
		}

		#region ToolbarButton

		private void DestroyButton ()
		{
			showMainWindow = false;
			KSP.UI.Screens.ApplicationLauncher.Instance.RemoveModApplication (button);
		}

		private void ToolbarButton ()
		{
			KSP.UI.Screens.ApplicationLauncher.AppScenes visibleScene = 
				KSP.UI.Screens.ApplicationLauncher.AppScenes.VAB | KSP.UI.Screens.ApplicationLauncher.AppScenes.SPH;

			//			string path = @"file://" + KSPUtil.ApplicationRootPath + "GameData/AntennaHelper/icon.png";
			//			WWW www = new WWW (path);
			Texture tex = new Texture ();
			tex = (Texture)GameDatabase.Instance.GetTexture ("AntennaHelper/icon", false);

			button = KSP.UI.Screens.ApplicationLauncher.Instance.AddModApplication (
				ToolbarButtonOnTrue, 
				ToolbarButtonOnFalse, 
				DummyVoid, 
				DummyVoid, 
				DummyVoid, 
				DummyVoid,
				visibleScene, tex);
		}

		private void ToolbarButtonOnTrue ()
		{
			showMainWindow = true;
		}

		private void ToolbarButtonOnFalse ()
		{
			showMainWindow = false;
		}

		private void DummyVoid () {}

		#endregion

		#region ParseVessel

		private void NewShip (ShipConstruct ship, KSP.UI.Screens.CraftBrowserDialog.LoadType screenType)
		{
			activeAntennas = ParseVessel (ship.Parts);
			targetAntennas = activeAntennas;

			CalcAntennas ();
		}

		private List<ModuleDataTransmitter> ParseVessel (List<Part> vessel)
		{
			List<ModuleDataTransmitter> antList = new List<ModuleDataTransmitter> ();

			foreach (Part part in vessel) {
				if (part.Modules.Contains<ModuleDataTransmitter> ()) {
					foreach (ModuleDataTransmitter antenna in part.Modules.GetModules<ModuleDataTransmitter> ()) {
						antList.Add (antenna);
					}
				}
			}
			return antList;
		}

		private void PartChange (Part part)
		{
			if (part.Modules.Contains<ModuleDataTransmitter> ()) {
				activeAntennas = ParseVessel (EditorLogic.fetch.ship.Parts);
				CalcAntennas ();
			}
		}

		#endregion

		#region Math

		private void CalcAntennas ()
		{
			List <ModuleDataTransmitter> workAntennas = new List<ModuleDataTransmitter> ();
			List <ModuleDataTransmitter> workCombAntennas = new List<ModuleDataTransmitter> ();

			if (toolbarTypeIndex == 0) {
				// Direct calculation :
				foreach (ModuleDataTransmitter ant in activeAntennas) {
					if (ant.CommCombinable) {
						workCombAntennas.Add (ant);
					}
				}
				workAntennas = activeAntennas;

			} else {
				// Relay calculation :
				foreach (ModuleDataTransmitter antenna in activeAntennas) {
					if (antenna.antennaType == AntennaType.RELAY) {
						workAntennas.Add (antenna);
						if (antenna.CommCombinable) {
							workCombAntennas.Add (antenna);
						}
					}
				}
			}

			nbAntenna = workAntennas.Count;
			nbCombAntenna = workCombAntennas.Count;
			if (nbCombAntenna > 0) {
				maxRangeVessel = GetVesselPower (workCombAntennas);


				maxRange = GetRange (GetVesselPower (workCombAntennas), targetPower);
			}

		}

		private double GetAWCE (List<ModuleDataTransmitter> antennas)
		{
			// Get the Average Weighted Combinability Exponent for this set of antennas
			// From the wiki : SUM (( Antenna 'n' Power * Antenna 'n' Exponent ) : ( Antenna 'n+1' Power * Antenna 'n+1' Exponent )) / SUM ( Antenna 'n' Power ) : ( Antenna 'n+1' Power )
			// (( 100e9 * 0.75 ) + ( 500e3 * 1.00 )) / ( 100e9 + 500e3 ) = 0.75000125
			// x / y = z

			double x = 0;
			double y = 0;
			double z;

			foreach (ModuleDataTransmitter ant in antennas) {
				x += ant.antennaPower * ant.antennaCombinableExponent;
				y += ant.antennaPower;
			}
			z = x / y;
			return z;
		}

		private double GetVesselPower (List<ModuleDataTransmitter> antennas)
		{
			// Get the total antenna power for the vessel

			double strongestAnt = 0;
			double allAnt = 0;
			float awce = Convert.ToSingle (GetAWCE (antennas));
			foreach (ModuleDataTransmitter ant in antennas) {
				allAnt += ant.antennaPower;
				if (ant.antennaPower > strongestAnt) {
					strongestAnt = ant.antennaPower;
				}
			}
			double vesselPower = strongestAnt * Math.Pow (allAnt / strongestAnt, awce);
			return vesselPower;
		}

		private double GetRange (double activeAntPower, double targetAntPower)
		{
			return Math.Sqrt (activeAntPower * targetAntPower);
		}

		#endregion

		#region GUI

		private void OnGUI ()
		{
			//			centeredStyle = GUI.skin.GetStyle("Label");
			//			centeredStyle.alignment = TextAnchor.UpperLeft;
			if (showMainWindow) {
				mainWindowRect = GUILayout.Window (0, mainWindowRect, MainWindow, "Antenna Helper");
			}

			if (showTargetWindow) {
				targetWindowRect = GUILayout.Window (1, targetWindowRect, TargetWindow, "Choose a target");
			}
		}

		private void MainWindow (int id)
		{
			GUILayout.BeginVertical ();

			GUILayout.Label ("Type of antennas :");
			toolbarTypeIndex = GUILayout.Toolbar (toolbarTypeIndex, toolbarTypeStrings);
			GUILayout.Label ("Target : " + targetStr);
			if (GUILayout.Button ("Pick another target")) {
				// open a new window with the list of vessel
				showTargetWindow = ! showTargetWindow;
			}

			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();

			GUILayout.Label ("Number of antennas :");
			GUILayout.Label ("Max range with 100% signal :");
			GUILayout.Label ("Max range from the target :");
			GUILayout.Label ("Max power :");

			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();

			GUILayout.Label (nbAntenna.ToString () + " (" + nbCombAntenna.ToString () + ")");
			GUILayout.Label (maxRangeFull.ToString ("N1", System.Globalization.CultureInfo.InvariantCulture) + " m");
			GUILayout.Label (maxRange.ToString ("N1", System.Globalization.CultureInfo.InvariantCulture) + " m");
			GUILayout.Label (maxRangeVessel.ToString ("N1", System.Globalization.CultureInfo.InvariantCulture));

			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			//			GUILayout.EndArea ();

			GUI.DragWindow ();
		}

		private int toolbarTargetIndex = 0;
		private string[] toolbarTargetStrings = { "Editor", "Flight" };

		private Vector2 scrollPos;

		private void TargetWindow (int id)
		{
			GUILayout.BeginVertical ();

			toolbarTargetIndex = GUILayout.Toolbar (toolbarTargetIndex, toolbarTargetStrings);

			scrollPos = GUILayout.BeginScrollView (scrollPos, GUILayout.Width (380), GUILayout.Height (400));

			//			if (flightVesselList.Count == 0) {
			//				GUILayout.Label ("No vessel in FlightGlobals.vessels");
			//			}
			//
			//			foreach (List<ModuleDataTransmitter> listModule in flightVesselList) {
			//				if (GUILayout.Button (listModule[0].vessel.GetName ())) {
			//					
			//				}
			//			}
			if (GUILayout.Button ("DSN lvl 1")) {
				targetPower = 2000000000;
				targetStr = "DSN lvl 1";
				CalcAntennas ();
			}
			if (GUILayout.Button ("DSN lvl 2")) {
				targetPower = 50000000000;
				targetStr = "DSN lvl 2";
				CalcAntennas ();
			}
			if (GUILayout.Button ("DSN lvl 3")) {
				targetPower = 250000000000;
				targetStr = "DSN lvl 3";
				CalcAntennas ();
			}
			//			if (GUILayout.Button ("Current Vessel")) {
			//				targetPower = maxRangeVessel;
			//				CalcAntennas ();
			//			}

			GUILayout.EndScrollView ();

			GUILayout.EndVertical ();

			GUI.DragWindow ();
		}

		#endregion

		#region VesselList

		private List<List<ModuleDataTransmitter>> flightVesselList;

		private void GetFlightSat ()
		{

			//			flightVesselList = new List<List<ModuleDataTransmitter>> ();
			//			int vesselId = 0;
			//			if (HighLogic.fetch == null) {
			//				Debug.Log ("No Highlogic.fetch at this time");
			//			}
			//			if (HighLogic.CurrentGame == null) {
			//				Debug.Log ("No CurrentGame at this time");
			//			}
			//			if (HighLogic.CurrentGame.flightState == null) {
			//				Debug.Log ("No flightState at this time");
			//			}
			//			foreach(ProtoVessel v in HighLogic.CurrentGame.flightState.protoVessels) {
			//				flightVesselList.Add (new List<ModuleDataTransmitter> ());
			//				foreach (ProtoPartSnapshot part in v.protoPartSnapshots) {
			//					foreach (ProtoPartModuleSnapshot module in part.modules) {
			//						if (module.moduleName == "ModuleDataTransmitter") {
			//							flightVesselList [vesselId].Add (module.moduleRef.GetComponent<ModuleDataTransmitter> ());
			//						}
			//					}
			//				}
			//				vesselId++;
			//			}
			//
			//			// Clean the list :
			//			List<List<ModuleDataTransmitter>> workList = new List<List<ModuleDataTransmitter>> (flightVesselList);
			//			int listId = 0;
			//			foreach (List<ModuleDataTransmitter> list in workList) {
			//				if (list.Count == 0) {
			//					flightVesselList.RemoveAt (listId);
			//				}
			//				listId++;
			//			}
		}

		#endregion
	}
}

