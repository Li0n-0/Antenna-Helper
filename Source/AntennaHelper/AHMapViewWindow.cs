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
				AHFlight.guiCircle = AHFlight.GUICircleSelection.active;
				AHFlight.GUISelectCircle ();
			}
			if (GUILayout.Button ("DSN")) {
				AHFlight.guiCircle = AHFlight.GUICircleSelection.dsn;
				AHFlight.GUISelectCircle ();
			}
			if (GUILayout.Button ("Relay")) {
				AHFlight.guiCircle = AHFlight.GUICircleSelection.relay;
				AHFlight.GUISelectCircle ();
			}
			if (GUILayout.Button ("DSN + Relay")) {
				AHFlight.guiCircle = AHFlight.GUICircleSelection.dsnAndRelay;
				AHFlight.GUISelectCircle ();
			}
			GUILayout.EndVertical ();
		}
	}
}

