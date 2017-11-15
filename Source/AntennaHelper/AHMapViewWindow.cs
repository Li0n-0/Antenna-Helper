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
			if (GUILayout.Button ("Active Connection")) {
				AHFlight.guiCircle = AHFlight.GUICircleSelection.ACTIVE;
				AHFlight.GUISelectCircle ();
			}
			if (GUILayout.Button ("DSN")) {
				AHFlight.guiCircle = AHFlight.GUICircleSelection.DSN;
				AHFlight.GUISelectCircle ();
			}
			if (GUILayout.Button ("Relay")) {
				AHFlight.guiCircle = AHFlight.GUICircleSelection.RELAY;
				AHFlight.GUISelectCircle ();
			}
			if (GUILayout.Button ("DSN + Relay")) {
				AHFlight.guiCircle = AHFlight.GUICircleSelection.DSN_AND_RELAY;
				AHFlight.GUISelectCircle ();
			}
			GUILayout.EndVertical ();
		}
	}
}

