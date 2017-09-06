using System;
using UnityEngine;

namespace AntennaHelper
{
	public static class AHSettings
	{
		private static ConfigNode settingsNode;
//		private static ConfigNode nodeEditor;
		private static ConfigNode nodePositionEditor;

		public static Vector2 posMainWindow = new Vector2 (Screen.width / 2f, Screen.height / 2f);
		public static Vector2 posTargetWindow = new Vector2 (Screen.width / 2f - 400f, Screen.height / 2f);
		public static Vector2 posPlanetWindow = new Vector2 (Screen.width / 2f + 400f, Screen.height / 2f);

		static AHSettings ()
		{
			Debug.Log ("Check");
			// Check for the settings file
			settingsNode = ConfigNode.Load (KSPUtil.ApplicationRootPath + "GameData/AntennaHelper/PluginData/Settings.cfg");
			if (settingsNode == null) {
				settingsNode = new ConfigNode ();
			}
		
			// Check for nodes in settings file
			if (! settingsNode.HasNode("Editor_Window_Position")) {
				settingsNode.AddNode ("Editor_Window_Position");
			}
			nodePositionEditor = settingsNode.GetNode ("Editor_Window_Position");

			// Check for value in nodes
			if (nodePositionEditor.HasValue ("main_window_position")) {
				posMainWindow = ConfigNode.ParseVector2 (nodePositionEditor.GetValue ("main_window_position"));
			}
			nodePositionEditor.SetValue ("main_window_position", posMainWindow, true);

			if (nodePositionEditor.HasValue ("target_window_position")) {
				posTargetWindow = ConfigNode.ParseVector2 (nodePositionEditor.GetValue ("target_window_position"));
			}
			nodePositionEditor.SetValue ("target_window_position", posTargetWindow, true);

			if (nodePositionEditor.HasValue ("signal_strenght_per_planet_window_position")) {
				posPlanetWindow = ConfigNode.ParseVector2 (nodePositionEditor.GetValue ("signal_strenght_per_planet_window_position"));
			}
			nodePositionEditor.SetValue ("signal_strenght_per_planet_window_position", posPlanetWindow, true);

			WriteSave ();
		}

		public static void SavePosition (string windowName, Vector2 position)
		{
			nodePositionEditor.SetValue (windowName, position, true);
		}

		public static void WriteSave ()
		{
			settingsNode.Save (KSPUtil.ApplicationRootPath + "GameData/AntennaHelper/PluginData/Settings.cfg");
		}
	}
}

