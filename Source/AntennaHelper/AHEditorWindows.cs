using System;
using System.Collections.Generic;
using UnityEngine;

namespace AntennaHelper
{
	public class AHEditorWindows
	{
		private static string antennaTypeStr = "Direct";
		private static bool antennaTypeIsDirect = true;

		public static void MainWindow (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (AHEditor.rectMainWindow.size.x - 22, 2, 20, 20), "X")) {
				AHEditor.CloseMainWindow ();
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
			GUILayout.Label ("Current target : " + AHEditor.targetName + "  (" + AHEditor.targetPower + ")");
			if (GUILayout.Button ("Pick A Target")) {
				if (AHEditor.showTargetWindow) {
					AHEditor.CloseTargetWindow ();
				} else {
					AHEditor.showTargetWindow = true;
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
				GUILayout.Label (AHEditor.statusStringDirect);
				GUILayout.Label (AHEditor.directBetterPower.ToString ("n"));
				GUILayout.Label (AHEditor.directBetterRange.ToString ("n") + " m");
				GUILayout.Label (AHEditor.directDistanceAt100.ToString ("n") + " m");
			} else {
				GUILayout.Label (AHEditor.statusStringRelay);
				GUILayout.Label (AHEditor.relayBetterPower.ToString ("n"));
				GUILayout.Label (AHEditor.relayBetterRange.ToString ("n") + " m");
				GUILayout.Label (AHEditor.relayDistanceAt100.ToString ("n") + " m");
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();

			GUILayout.Space (16f);
			GUIStyle guiStyleCenter = new GUIStyle (GUI.skin.GetStyle ("Label"));
			guiStyleCenter.alignment = TextAnchor.MiddleCenter;

			GUILayout.BeginHorizontal ();
			if (antennaTypeIsDirect) {
				GUILayout.Label (AHEditor.directDistanceAt75.ToString ("n") + " m", guiStyleCenter);
				GUILayout.Label (AHEditor.directDistanceAt25.ToString ("n") + " m", guiStyleCenter);
			} else {
				GUILayout.Label (AHEditor.relayDistanceAt75.ToString ("n") + " m", guiStyleCenter);
				GUILayout.Label (AHEditor.relayDistanceAt25.ToString ("n") + " m", guiStyleCenter);
			}
			GUILayout.EndHorizontal ();

			GUILayout.Label (AHUtil.signalPerDistanceTex);

			if (antennaTypeIsDirect) {
				GUILayout.Label (AHEditor.directDistanceAt50.ToString ("n") + " m", guiStyleCenter);
			} else {
				GUILayout.Label (AHEditor.relayDistanceAt50.ToString ("n") + " m", guiStyleCenter);
			}

			// Planet view button :
			if (GUILayout.Button ("Signal Strength / Distance")) {
				if (AHEditor.showPlanetWindow) {
					AHEditor.ClosePlanetWindow ();
				} else {
					AHEditor.showPlanetWindow = true;
				}
			}

			if (GUILayout.Button ("Add Ship to the Target List")) {
				AHEditor.AddShipToShipList ();
			}

			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}

		public static void TargetWindow (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (AHEditor.rectTargetWindow.size.x - 20, 2, 18, 18), "X")) {
				AHEditor.CloseTargetWindow ();
			}

			GUIStyle guiStyleBold = new GUIStyle (GUI.skin.GetStyle ("Button"));
			guiStyleBold.fontStyle = FontStyle.Bold;

			GUILayout.BeginVertical ();

			for (int i = 0 ; i < 3 ; i++) {
				if (i / 2f == AHEditor.trackingStationLevel) {
					if (GUILayout.Button ("DSN Level " + (i + 1) + "  (" + GameVariables.Instance.GetDSNRange (i / 2f) + ")", guiStyleBold)) {
						AHEditor.SetTarget (i / 2f);
					}
				} else {
					if (GUILayout.Button ("DSN Level " + (i + 1) + "  (" + GameVariables.Instance.GetDSNRange (i / 2f) + ")")) {
						AHEditor.SetTarget (i / 2f);
					}
				}
			}

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("In-Flight Ships")) {
				AHEditor.CloseTargetShipEditorWindow ();
				AHEditor.showTargetShipFlightWindow = true;
			}
			if (GUILayout.Button ("Editor Ships")) {
				AHEditor.CloseTargetShipFlightWindow ();
				AHEditor.showTargetShipEditorWindow = true;
			}
			GUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}

		private static bool vab = true;
		private static Vector2 scrollVectorEditor;
		public static void TargetWindowShipEditor (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (AHEditor.rectTargetShipEditorWindow.size.x - 20, 2, 18, 18), "X")) {
				AHEditor.CloseTargetShipEditorWindow ();
			}

			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("VAB")) {
				vab = true;
			}
			if (GUILayout.Button ("SPH")) {
				vab = false;
			}
			GUILayout.EndHorizontal ();

			scrollVectorEditor = GUILayout.BeginScrollView (scrollVectorEditor/*, GUILayout.Width (AHEditor.rectTargetWindow.width), GUILayout.Height (AHEditor.rectTargetWindow.height)*/);
			foreach (KeyValuePair<string, Dictionary <string, string>> vesselPairInfo in AHEditor.externListShipEditor) {
				if ((vab && (vesselPairInfo.Value ["type"] != "VAB")) || (!vab && (vesselPairInfo.Value ["type"] != "SPH"))) {
					continue;
				}
				if (GUILayout.Button (vesselPairInfo.Value ["name"] + "  (" + AHUtil.TruePower (Double.Parse (vesselPairInfo.Value ["powerRelay"])).ToString () + ")")) {
					AHEditor.SetTarget (vesselPairInfo);
				}
			}
			GUILayout.EndScrollView ();

			GUILayout.EndVertical ();
		}

		private static Vector2 scrollVectorFlight;
		public static void TargetWindowShipFlight (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (AHEditor.rectTargetShipFlightWindow.size.x - 20, 2, 18, 18), "X")) {
				AHEditor.CloseTargetShipFlightWindow ();
			}

			GUILayout.BeginVertical ();

			scrollVectorFlight = GUILayout.BeginScrollView (scrollVectorFlight/*, GUILayout.Width (AHEditor.rectTargetWindow.width), GUILayout.Height (AHEditor.rectTargetWindow.height)*/);
			foreach (KeyValuePair<string, Dictionary <string, string>> vesselPairInfo in AHEditor.externListShipFlight) {
				if (GUILayout.Button (vesselPairInfo.Value["name"] + "  (" + vesselPairInfo.Value["powerRelay"] + ")")) {
					AHEditor.SetTarget (vesselPairInfo);
				}
			}
			GUILayout.EndScrollView ();

			GUILayout.EndVertical ();
		}

		public static void PlanetWindow (int id)
		{
			// Close Button
			if (GUI.Button (new Rect (AHEditor.rectPlanetWindow.size.x - 22, 2, 20, 20), "X")) {
				AHEditor.ClosePlanetWindow ();
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
					(Mouse.screenPos.x - AHEditor.rectPlanetWindow.position.x, 
						Mouse.screenPos.y - AHEditor.rectPlanetWindow.position.y - 15, 450, 30));
				GUILayout.Label (GUI.tooltip);
				GUILayout.EndArea ();
			}
			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			// Min distance
			GUILayout.Label ("Signal at Min Distance");
			if (antennaTypeIsDirect) {
				foreach (double signal in AHEditor.signalMinDirect) {
					GUILayout.Label (signal.ToString ("0.0%"));
				}
			} else {
				foreach (double signal in AHEditor.signalMinRelay) {
					GUILayout.Label (signal.ToString ("0.0%"));
				}
			}

			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			// Max distance
			GUILayout.Label ("Signal at Max Distance");
			if (antennaTypeIsDirect) {
				foreach (double signal in AHEditor.signalMaxDirect) {
					GUILayout.Label (signal.ToString ("0.0%"));
				}
			} else {
				foreach (double signal in AHEditor.signalMaxRelay) {
					GUILayout.Label (signal.ToString ("0.0%"));
				}
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();

			// Custom distance
			GUILayout.Label ("Check the Signal Strength at a given distance :");
			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();
			AHEditor.customDistance = GUILayout.TextField (AHEditor.customDistance);
			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			if (antennaTypeIsDirect) {
				GUILayout.Label (AHEditor.signalCustomDistanceDirect.ToString ("0.0%"));
			} else {
				GUILayout.Label (AHEditor.signalCustomDistanceRelay.ToString ("0.0%"));
			}
			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Math !")) {
				AHEditor.CalcCustomDistance ();
			}
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}
	}
}

