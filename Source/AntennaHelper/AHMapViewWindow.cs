using System;
using UnityEngine;

namespace AntennaHelper
{
	public class AHMapViewWindow
	{
		public static void AntennaSelectWindow (int id)
		{
			GUILayout.BeginVertical ();
			GUILayout.Label ("Curently : " + AHFlight.guiCircle.ToString ());
			GUILayout.Space (3f);
			if (GUILayout.Button ("Active Connection")) {
				AHFlight.guiCircle = GUICircleSelection.ACTIVE;
				AHFlight.GUISelectCircle ();
			}
			if (GUILayout.Button ("DSN")) {
				AHFlight.guiCircle = GUICircleSelection.DSN;
				AHFlight.GUISelectCircle ();
			}
			if (GUILayout.Button ("Relay")) {
				AHFlight.guiCircle = GUICircleSelection.RELAY;
				AHFlight.GUISelectCircle ();
			}
			if (GUILayout.Button ("DSN + Relay")) {
				AHFlight.guiCircle = GUICircleSelection.DSN_AND_RELAY;
				AHFlight.GUISelectCircle ();
			}
			GUILayout.EndVertical ();

			GUI.DragWindow ();
		}
	}
}

