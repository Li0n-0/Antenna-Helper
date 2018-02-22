using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace AntennaHelper
{
	public class AHGameSettings : GameParameters.CustomParameterNode
	{
		public override string Title {
			get {
				return "Antenna Helper";
			}
		}
		public override string Section {
			get {
				return "Antenna Helper";
			}
		}
		public override string DisplaySection {
			get {
				return "Antenna Helper";
			}
		}
		public override int SectionOrder {
			get {
				return 1;
			}
		}
		public override GameParameters.GameMode GameMode {
			get {
				return GameParameters.GameMode.ANY;
			}
		}
		public override bool HasPresets {
			get {
				return false;
			}
		}

		[GameParameters.CustomParameterUI ("Use Blizzy's Toolbar")]
		public bool useBlizzy = false;

		[GameParameters.CustomParameterUI ("Enable in the Editor")]
		public bool enableInEditor = true;

		[GameParameters.CustomParameterUI ("Enable in the Tracking Station")]
		public bool enableInTrackingStation = true;

		[GameParameters.CustomParameterUI ("Enable in Flight")]
		public bool enableInFlight = true;

		[GameParameters.CustomParameterUI ("Enable in the MapView")]
		public bool enableInMapView = true;

		[GameParameters.CustomParameterUI ("\nFlight and MapView \nDelay Between GUI Update ")]
		public DelayEnum delayFlightUI = DelayEnum.Half_Second;

		public override bool Enabled (MemberInfo member, GameParameters parameters)
		{
			if (member.Name == "useBlizzy") {
				if (!ToolbarControl_NS.ToolbarManager.ToolbarAvailable) {
					useBlizzy = false;
					return false;
				}
			}
			return true;
		}

		public override bool Interactible (MemberInfo member, GameParameters parameters)
		{
			if (member.Name == "delayFlightUI") {
				if (!enableInFlight && !enableInMapView) {
					return false;
				}
			}
			return true;
		}

		public enum DelayEnum
		{
			Tenth_Second,
			Half_Second,
			One_Second,
			Two_Seconds,
			Window_Open
		}
	}
}

