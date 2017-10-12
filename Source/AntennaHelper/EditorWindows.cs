using System;
using UnityEngine;

namespace AntennaHelper
{
	public class EditorWindows
	{
		private static string antennaTypeStr = "Direct";
		private static bool antennaTypeIsDirect = true;

		public static void MainWindow (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (AntennaHelperEditor.rectMainWindow.size.x - 22, 2, 20, 20), "X")) {
				AntennaHelperEditor.CloseMainWindow ();
			}

			GUILayout.BeginVertical ();

			// Choose direct / relay antennas
			GUILayout.Label ("Selected type : " + antennaTypeStr);
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Direct (All Antennas)")) {
				antennaTypeStr = "Direct";
				antennaTypeIsDirect = true;
			}
			if (GUILayout.Button ("Relay")) {
				antennaTypeStr = "Relay";
				antennaTypeIsDirect = false;
			}
			GUILayout.EndHorizontal ();

			// Pick a target :
			GUILayout.Label ("Current target : " + AntennaHelperEditor.targetName);
			if (GUILayout.Button ("Pick A Target")) {
				if (AntennaHelperEditor.showTargetWindow) {
					AntennaHelperEditor.CloseTargetWindow ();
				} else {
					AntennaHelperEditor.showTargetWindow = true;
				}
			}

			// Number display :
			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();
			GUILayout.Label ("Status : ");
			GUILayout.Label ("Power : ");
			GUILayout.Label ("Max Range : ");
			GUILayout.Label ("Max Distance At 100% : ");
			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			if (antennaTypeIsDirect) {
				GUILayout.Label (AntennaHelperEditor.statusStringDirect);
				GUILayout.Label (AntennaHelperEditor.directBetterPower.ToString ("n"));
				GUILayout.Label (AntennaHelperEditor.directBetterRange.ToString ("n") + " m");
				GUILayout.Label (AntennaHelperEditor.directDistanceAt100.ToString ("n") + " m");
			} else {
				GUILayout.Label (AntennaHelperEditor.statusStringRelay);
				GUILayout.Label (AntennaHelperEditor.relayBetterPower.ToString ("n"));
				GUILayout.Label (AntennaHelperEditor.relayBetterRange.ToString ("n") + " m");
				GUILayout.Label (AntennaHelperEditor.relayDistanceAt100.ToString ("n") + " m");
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();

			GUILayout.Space (16f);
			GUIStyle guiStyleCenter = new GUIStyle (GUI.skin.GetStyle ("Label"));
			guiStyleCenter.alignment = TextAnchor.MiddleCenter;

			GUILayout.BeginHorizontal ();
			if (antennaTypeIsDirect) {
				GUILayout.Label (AntennaHelperEditor.directDistanceAt75.ToString ("n") + " m", guiStyleCenter);
				GUILayout.Label (AntennaHelperEditor.directDistanceAt25.ToString ("n") + " m", guiStyleCenter);
			} else {
				GUILayout.Label (AntennaHelperEditor.relayDistanceAt75.ToString ("n") + " m", guiStyleCenter);
				GUILayout.Label (AntennaHelperEditor.relayDistanceAt25.ToString ("n") + " m", guiStyleCenter);
			}
			GUILayout.EndHorizontal ();

			GUILayout.Label (AHUtil.signalPerDistanceTex);

			if (antennaTypeIsDirect) {
				GUILayout.Label (AntennaHelperEditor.directDistanceAt50.ToString ("n") + " m", guiStyleCenter);
			} else {
				GUILayout.Label (AntennaHelperEditor.relayDistanceAt50.ToString ("n") + " m", guiStyleCenter);
			}

			// Most powerfull antenna :
//			GUILayout.Label ("Most Powerfull Antenna");
//			GUILayout.BeginHorizontal ();
//			GUILayout.BeginVertical ();
//			GUILayout.Label ("Name of the antenna :");
//			GUILayout.Label ("Power :");
//			GUILayout.Label ("Range :");
//			GUILayout.EndVertical ();
//			GUILayout.BeginVertical ();
//			if (antennaTypeIsDirect) {
//				GUILayout.Label (AntennaHelperEditor.directAntennaName);
//				GUILayout.Label (AntennaHelperEditor.directPower.ToString ("n"));
//				GUILayout.Label (AntennaHelperEditor.directRange.ToString ("n") + " m");
//			} else {
//				GUILayout.Label (AntennaHelperEditor.relayAntennaName);
//				GUILayout.Label (AntennaHelperEditor.relayPower.ToString ("n"));
//				GUILayout.Label (AntennaHelperEditor.relayRange.ToString ("n") + " m");
//			}
//			GUILayout.EndVertical ();
//			GUILayout.EndHorizontal ();
//
//			GUILayout.Space (5f);
//			GUILayout.Box ("", new GUILayoutOption []{ GUILayout.ExpandWidth (true), GUILayout.Height (1) });
//			GUILayout.Space (5f);
//
//			// Combinable antennas :
//			GUILayout.Label ("Combinable Antennas");
//			GUILayout.BeginHorizontal ();
//			GUILayout.BeginVertical ();
//			GUILayout.Label ("Number of antennas :");
//			GUILayout.Label ("Number of combinable antennas :");
//			GUILayout.Label ("Power :");
//			GUILayout.Label ("Range :");
//			GUILayout.EndVertical ();
//			GUILayout.BeginVertical ();
//			if (antennaTypeIsDirect) {
//				GUILayout.Label (AntennaHelperEditor.nbDirectAntenna.ToString ());
//				GUILayout.Label (AntennaHelperEditor.nbDirectCombAntenna.ToString ());
//				GUILayout.Label (AntennaHelperEditor.directCombPower.ToString ("n"));
//				GUILayout.Label (AntennaHelperEditor.directCombRange.ToString ("n") + " m");
//			} else {
//				GUILayout.Label (AntennaHelperEditor.nbRelayAntenna.ToString ());
//				GUILayout.Label (AntennaHelperEditor.nbRelayCombAntenna.ToString ());
//				GUILayout.Label (AntennaHelperEditor.relayCombPower.ToString ("n"));
//				GUILayout.Label (AntennaHelperEditor.relayCombRange.ToString ("n") + " m");
//			}
//			GUILayout.EndVertical ();
//			GUILayout.EndHorizontal ();

			// Planet view button :
			if (GUILayout.Button ("Signal Strength / Distance")) {
				if (AntennaHelperEditor.showPlanetWindow) {
					AntennaHelperEditor.ClosePlanetWindow ();
				} else {
					AntennaHelperEditor.showPlanetWindow = true;
				}
			}

			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}

		public static void TargetWindow (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (AntennaHelperEditor.rectTargetWindow.size.x - 20, 2, 18, 18), "X")) {
				AntennaHelperEditor.CloseTargetWindow ();
			}

			GUIStyle guiStyleBold = new GUIStyle (GUI.skin.GetStyle ("Button"));
			guiStyleBold.fontStyle = FontStyle.Bold;

			GUILayout.BeginVertical ();

			for (int i = 0 ; i < AHUtil.DSNLevelList.Length ; i++) {
				if (i == AHUtil.DSNLevel) {
					if (GUILayout.Button ("DSN Level " + (i + 1) + "  (" + AHUtil.DSNLevelList[i] + ")", guiStyleBold)) {
						AntennaHelperEditor.SetTarget (i);
					}
				} else {
					if (GUILayout.Button ("DSN Level " + (i + 1) + "  (" + AHUtil.DSNLevelList[i] + ")")) {
						AntennaHelperEditor.SetTarget (i);
					}
				}
			}

			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}

		public static void PlanetWindow (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (AntennaHelperEditor.rectPlanetWindow.size.x - 22, 2, 20, 20), "X")) {
				AntennaHelperEditor.ClosePlanetWindow ();
			}

			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();
			// Planet name
			GUILayout.Label ("Planet / Moon");
			foreach (MyTuple planet in AHUtil.signalPlanetList) {
				GUILayout.Label (new GUIContent (planet.item1, "Min = " + planet.item2.ToString ("n") + "m | Max = " + planet.item3.ToString ("n") + "m"));
//				GUI.Label (new Rect (Mouse.screenPos.x, Mouse.screenPos.y, 50, 20), GUI.tooltip);
				GUILayout.BeginArea (new Rect 
					(Mouse.screenPos.x - AntennaHelperEditor.rectPlanetWindow.position.x, 
						Mouse.screenPos.y - AntennaHelperEditor.rectPlanetWindow.position.y - 15, 450, 30));
				GUILayout.Label (GUI.tooltip);
				GUILayout.EndArea ();
			}
			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			// Min distance
			GUILayout.Label ("Signal at Min Distance");
			if (antennaTypeIsDirect) {
				foreach (double signal in AntennaHelperEditor.signalMinDirect) {
					GUILayout.Label (signal.ToString ("0.0%"));
				}
			} else {
				foreach (double signal in AntennaHelperEditor.signalMinRelay) {
					GUILayout.Label (signal.ToString ("0.0%"));
				}
			}

			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			// Max distance
			GUILayout.Label ("Signal at Max Distance");
			if (antennaTypeIsDirect) {
				foreach (double signal in AntennaHelperEditor.signalMaxDirect) {
					GUILayout.Label (signal.ToString ("0.0%"));
				}
			} else {
				foreach (double signal in AntennaHelperEditor.signalMaxRelay) {
					GUILayout.Label (signal.ToString ("0.0%"));
				}
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();

			// Custom distance
			GUILayout.Label ("Check the Signal Strength at a given distance :");
			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();
			AntennaHelperEditor.customDistance = GUILayout.TextField (AntennaHelperEditor.customDistance);
			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			if (antennaTypeIsDirect) {
				GUILayout.Label (AntennaHelperEditor.signalCustomDistanceDirect.ToString ("0.0%"));
			} else {
				GUILayout.Label (AntennaHelperEditor.signalCustomDistanceRelay.ToString ("0.0%"));
			}
			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Math !")) {
				AntennaHelperEditor.CalcCustomDistance ();
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}
	}
}

