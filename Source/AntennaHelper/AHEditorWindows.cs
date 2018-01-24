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

			GUIStyle guiStyle;
			GUIStyle guiStyleNorm = new GUIStyle (GUI.skin.GetStyle ("Button"));
			GUIStyle guiStyleBold = new GUIStyle (GUI.skin.GetStyle ("Button"));
			guiStyleBold.fontStyle = FontStyle.Bold;

			GUILayout.BeginVertical ();

			for (int i = 0 ; i < 3 ; i++) {
				
				string dsnStr;
				if (i / 2f == AHEditor.trackingStationLevel) {
					dsnStr = "** DSN Level " + (i + 1) + "  (" + GameVariables.Instance.GetDSNRange (i / 2f) + ") **";
				} else {
					dsnStr = "DSN Level " + (i + 1) + "  (" + GameVariables.Instance.GetDSNRange (i / 2f) + ")";
				}

				if ((AHEditor.targetType == AHEditorTargetType.DSN) 
				    && (AHEditor.targetName == "DSN Level " + (i + 1).ToString ())) {

					guiStyle = guiStyleBold;
				} else {
					guiStyle = guiStyleNorm;
				}

				if (GUILayout.Button (dsnStr, guiStyle)) {
					AHEditor.SetTarget (i / 2f);
				}
			}

			GUILayout.BeginHorizontal ();
			if (AHEditor.targetType == AHEditorTargetType.FLIGHT) {
				guiStyle = guiStyleBold;
			} else {
				guiStyle = guiStyleNorm;
			}
			if (GUILayout.Button ("In-Flight Ships", guiStyle)) {
				AHEditor.CloseTargetShipEditorWindow ();
				AHEditor.CloseTargetPartWindow ();
				AHEditor.showTargetShipFlightWindow = true;
			}

			if (AHEditor.targetType == AHEditorTargetType.EDITOR) {
				guiStyle = guiStyleBold;
			} else {
				guiStyle = guiStyleNorm;
			}
			if (GUILayout.Button ("Editor Ships", guiStyle)) {
				AHEditor.CloseTargetShipFlightWindow ();
				AHEditor.CloseTargetPartWindow ();
				AHEditor.showTargetShipEditorWindow = true;
			}

			if (AHEditor.targetType == AHEditorTargetType.PART) {
				guiStyle = guiStyleBold;
			} else {
				guiStyle = guiStyleNorm;
			}
			if (GUILayout.Button ("Antenna Parts", guiStyle)) {
				AHEditor.CloseTargetShipEditorWindow ();
				AHEditor.CloseTargetShipFlightWindow ();
				AHEditor.showTargetPartWindow = true;
			}
			GUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			GUI.DragWindow ();
		}

		private static bool vab = true;
		private static bool relay = false;
		private static Vector2 scrollVectorEditor;
		private static List<Dictionary<string, string>> displayList;
		public static void TargetWindowShipEditor (int id)
		{
			GUIStyle guiStyleLabel;
			GUIStyle guiStyleLabelNorm = new GUIStyle (GUI.skin.GetStyle ("Label"));
			GUIStyle guiStyleLabelBold = new GUIStyle (GUI.skin.GetStyle ("Label"));
			guiStyleLabelBold.fontStyle = FontStyle.Bold;

			GUIStyle guiStyleButton;
			GUIStyle guiStyleButtonNorm = new GUIStyle (GUI.skin.GetStyle ("Button"));
			GUIStyle guiStyleButtonBold = new GUIStyle (GUI.skin.GetStyle ("Button"));
			guiStyleButtonBold.fontStyle = FontStyle.Bold;

			GUIStyle guiStyleButtonRed = new GUIStyle (GUI.skin.GetStyle ("Button"));
			guiStyleButtonRed.fontStyle = FontStyle.Bold;
			guiStyleButtonRed.normal.textColor = Color.red;
			guiStyleButtonRed.hover.textColor = Color.red;

			// Close Button
			if (GUI.Button (new Rect (AHEditor.rectTargetShipEditorWindow.size.x - 20, 2, 18, 18), "X")) {
				AHEditor.CloseTargetShipEditorWindow ();
			}

			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			if (vab) {
				guiStyleButton = guiStyleButtonBold;
			} else {
				guiStyleButton = guiStyleButtonNorm;
			}
			if (GUILayout.Button ("VAB", guiStyleButton)) {
				vab = true;
			}

			if (vab) {
				guiStyleButton = guiStyleButtonNorm;
			} else {
				guiStyleButton = guiStyleButtonBold;
			}
			if (GUILayout.Button ("SPH", guiStyleButton)) {
				vab = false;
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (35f);
			if (relay) {
				guiStyleButton = guiStyleButtonNorm;
			} else {
				guiStyleButton = guiStyleButtonBold;
			}
			if (GUILayout.Button ("All", guiStyleButton)) {
				relay = false;
			}

			if (relay) {
				guiStyleButton = guiStyleButtonBold;
			} else {
				guiStyleButton = guiStyleButtonNorm;
			}
			if (GUILayout.Button ("Relay", guiStyleButton)) {
				relay = true;
			}
			GUILayout.Space (35f);
			GUILayout.EndHorizontal ();

			scrollVectorEditor = GUILayout.BeginScrollView (scrollVectorEditor);
			if (vab) {
				if (relay) {
					displayList = AHEditor.guiExternListShipEditorVabRelay;
				} else {
					displayList = AHEditor.guiExternListShipEditorVabAll;
				}
			} else {
				if (relay) {
					displayList = AHEditor.guiExternListShipEditorSphRelay;
				} else {
					displayList = AHEditor.guiExternListShipEditorSphAll;
				}
			}

			foreach (Dictionary <string, string> vesselInfo in displayList) {
				if ((vab && (vesselInfo ["type"] != "VAB")) || (!vab && (vesselInfo ["type"] != "SPH"))) {
					continue;
				}

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Select", GUILayout.Width (60f))) {
					AHEditor.SetTarget (vesselInfo ["pid"]);
				}

				if (AHEditor.targetPid == vesselInfo ["pid"]) {
					guiStyleLabel = guiStyleLabelBold;
				} else {
					guiStyleLabel = guiStyleLabelNorm;
				}
				GUILayout.Label (
					"("
					+ AHUtil.TruePower (Double.Parse (vesselInfo ["powerRelay"])).ToString ()
					+ ")  "
					+ vesselInfo ["name"], guiStyleLabel);
				if (GUILayout.Button ("X", guiStyleButtonRed, GUILayout.Width (22f))) {
					AHEditor.RemoveShipFromShipList (vesselInfo ["pid"]);
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndScrollView ();

			GUILayout.EndVertical ();
		}

		private static Vector2 scrollVectorFlight;
		public static void TargetWindowShipFlight (int id)
		{
			GUIStyle guiStyleLabel;
			GUIStyle guiStyleLabelNorm = new GUIStyle (GUI.skin.GetStyle ("Label"));
			GUIStyle guiStyleLabelBold = new GUIStyle (GUI.skin.GetStyle ("Label"));
			guiStyleLabelBold.fontStyle = FontStyle.Bold;

			// Close Button
			if (GUI.Button (new Rect (AHEditor.rectTargetShipFlightWindow.size.x - 20, 2, 18, 18), "X")) {
				AHEditor.CloseTargetShipFlightWindow ();
			}

			GUILayout.BeginVertical ();

			scrollVectorFlight = GUILayout.BeginScrollView (scrollVectorFlight);
			foreach (Dictionary <string, string> vesselInfo in AHEditor.guiExternListShipFlight) {
				if (vesselInfo ["type"] != "Relay") {
					continue;
				}

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Select", GUILayout.Width (60f))) {
					AHEditor.SetTarget (vesselInfo ["pid"]);
				}

				if (AHEditor.targetPid == vesselInfo ["pid"]) {
					guiStyleLabel = guiStyleLabelBold;
				} else {
					guiStyleLabel = guiStyleLabelNorm;
				}
				GUILayout.Label (
					"("
					+ AHUtil.TruePower (Double.Parse (vesselInfo ["powerRelay"])).ToString ()
					+ ")  "
					+ vesselInfo ["name"], guiStyleLabel);
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndScrollView ();

			GUILayout.EndVertical ();
		}

		private static Vector2 scrollVectorPart;
		public static void TargetWindowPart (int id)
		{
			GUIStyle guiStyleLabel;
			GUIStyle guiStyleLabelNorm = new GUIStyle (GUI.skin.GetStyle ("Label"));
			GUIStyle guiStyleLabelBold = new GUIStyle (GUI.skin.GetStyle ("Label"));
			guiStyleLabelBold.fontStyle = FontStyle.Bold;

			GUIStyle guiStyleButtonBold = new GUIStyle (GUI.skin.GetStyle ("Button"));
			guiStyleButtonBold.fontStyle = FontStyle.Bold;

			// Close Button
			if (GUI.Button (new Rect (AHEditor.rectTargetPartWindow.size.x - 20, 2, 18, 18), "X")) {
				AHEditor.CloseTargetPartWindow ();
			}

			GUILayout.BeginVertical ();
			scrollVectorPart = GUILayout.BeginScrollView (scrollVectorPart);

			foreach (ModuleDataTransmitter antenna in AHShipList.listAntennaPart) {
				
				if (antenna.antennaType != AntennaType.RELAY) {
					continue;
				}

				if (AHEditor.listAntennaPart [antenna] > 0) {
					guiStyleLabel = guiStyleLabelBold;
				} else {
					guiStyleLabel = guiStyleLabelNorm;
				}

				GUILayout.BeginHorizontal ();

				GUILayout.Label (AHEditor.listAntennaPart [antenna].ToString (), guiStyleLabel, GUILayout.Width (15f));

				if (GUILayout.Button ("+", guiStyleButtonBold, GUILayout.Width (20f))) {
					AHEditor.listAntennaPart [antenna]++;
					AHEditor.UpdateTargetPartPower ();
				}
				if (GUILayout.Button ("-", guiStyleButtonBold, GUILayout.Width (20f))) {
					AHEditor.listAntennaPart [antenna]--;
					AHEditor.UpdateTargetPartPower ();
				}

				GUILayout.Label (
					"(" + AHUtil.TruePower (antenna.antennaPower).ToString () + ")  " 
					+ antenna.part.partInfo.title, guiStyleLabel);

				GUILayout.EndHorizontal ();
			}
			GUILayout.EndScrollView ();

			GUILayout.Space (10f);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Power : " + AHEditor.targetPartPower.ToString ());
			if (GUILayout.Button ("Set As Target")) {
				AHEditor.SetTargetAsPart ();
			}
			GUILayout.EndHorizontal ();

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

