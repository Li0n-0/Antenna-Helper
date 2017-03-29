using System;
using System.Collections.Generic;
using UnityEngine;

namespace AntennaHelper
{
	[KSPAddon (KSPAddon.Startup.SpaceCentre, true)]
	public class AntennaHelperUtil : MonoBehaviour
	{
		public static Texture toolbarButtonTex;

		public static float DSNMod;
		public static float rangeMod;
		public static int DSNLevel;
//		public static double DSNLvl1 = 2000000000d;
//		public static double DSNLvl2 = 50000000000d;
//		public static double DSNLvl3 = 250000000000d;
		public static List<MyTuple> targetDSNList;

		public static List<MyTuple> signalPlanetList;

//		public static List<CelestialBody> planetsList;
//		public static List<CelestialBody> moonsList;
		public static CelestialBody homePlanet;

		public static Vector2 centerScreen;


//		public static List<List<ModuleDataTransmitter>> inFlightRelay;

		public void Start ()
		{
			centerScreen = new Vector2 (Screen.width / 2f, Screen.height / 2f);

			toolbarButtonTex = (Texture)GameDatabase.Instance.GetTexture ("AntennaHelper/icon", false);

			DSNMod = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams> ().DSNModifier;
			rangeMod = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams> ().rangeModifier;
			FetchDSNLevel ();
			GameEvents.OnKSCFacilityUpgraded.Add (DSNUpgrade);

			// Target List (only DSN for now)
			targetDSNList = new List<MyTuple> ();
			targetDSNList.Add (new MyTuple ("DSN Level 1", 2000000000d));
			targetDSNList.Add (new MyTuple ("DSN Level 2", 50000000000d));
			targetDSNList.Add (new MyTuple ("DSN Level 3", 250000000000d));

			homePlanet = FlightGlobals.GetHomeBody ();
//			planetsList = FlightGlobals.Bodies [0].orbitingBodies;
//			moonsList = homePlanet.orbitingBodies;

			signalPlanetList = new List<MyTuple> ();
			foreach (CelestialBody moon in homePlanet.orbitingBodies) {
				signalPlanetList.Add (new MyTuple (moon.bodyName, moon.orbit.PeR, moon.orbit.ApR));
			}
			foreach (CelestialBody planet in FlightGlobals.Bodies[0].orbitingBodies) {
				if (planet != homePlanet) {
					signalPlanetList.Add (GetDistancePlanet (homePlanet, planet));
				}
			}

//			inFlightRelay = new List<List<ModuleDataTransmitter>> ();
//			foreach (Vessel vessel in FlightGlobals.VesselsUnloaded) {
//				if (vessel.FindPartModulesImplementing<ModuleDataTransmitter> ().Count != 0) {
//					inFlightRelay.Add (new List<ModuleDataTransmitter> 
//						(vessel.FindPartModulesImplementing<ModuleDataTransmitter> ()));
//				}
//			}




//			Debug.Log ("[AntennaHelper] : DSN modificateur : " + DSNMod);
//			Debug.Log ("[AntennaHelper] : range mod : " + rangeMod);
//			Debug.Log ("[AntennaHelper] : home planet : " + homePlanet.GetName ());
//			Debug.Log ("[AntennaHelper] : List of planets : ");
//			foreach (CelestialBody body in planetsList) {
//				Debug.Log ("[AntennaHelper] : " + body.GetName ());
//			}
//			Debug.Log ("[AntennaHelper] : List of moons : ");
//			foreach (CelestialBody body in moonsList) {
//				Debug.Log ("[AntennaHelper] : " + body.GetName ());
//			}
//			Debug.Log ("[AntennaHelper] : List of in-fligh relay : ");
//			foreach (List<ModuleDataTransmitter> v in inFlightRelay) {
//				Debug.Log ("[AntennaHelper] : " + v[0].vessel.GetName ());
//			}
		}

		public void OnDestroy ()
		{
			GameEvents.OnKSCFacilityUpgraded.Remove (DSNUpgrade);
		}

		public void DSNUpgrade (Upgradeables.UpgradeableFacility facility, int level)
		{
			FetchDSNLevel ();
		}

		private void FetchDSNLevel ()
		{
			float dsnLevelF = ScenarioUpgradeableFacilities.GetFacilityLevel (SpaceCenterFacility.TrackingStation);
			if (dsnLevelF == 0) {
				DSNLevel = 0;
			} else if (dsnLevelF == 1) {
				DSNLevel = 2;
			} else {
				DSNLevel = 1;
			}
		}

		public static void DummyVoid () {}// For the toolbar button

		#region Math
		public static double GetAWCE (List<ModuleDataTransmitter> antennas)
		{
			// Get the Average Weighted Combinability Exponent for this set of antennas
			// From the wiki : SUM (( Antenna 'n' Power * Antenna 'n' Exponent ) : ( Antenna 'n+1' Power * Antenna 'n+1' Exponent )) / SUM ( Antenna 'n' Power ) : ( Antenna 'n+1' Power )
			// (( 100e9 * 0.75 ) + ( 500e3 * 1.00 )) / ( 100e9 + 500e3 ) = 0.75000125
			// x / y = z

			double x = 0;
			double y = 0;
			double z;

			if (antennas.Count == 1) {
				return antennas [0].antennaCombinableExponent;
			}

			foreach (ModuleDataTransmitter ant in antennas) {
				x += ant.antennaPower * ant.antennaCombinableExponent;
				y += ant.antennaPower;
			}
			z = x / y;
			return z;
		}

		public static double GetVesselPower (List<ModuleDataTransmitter> antennas)
		{
			// Get the total antenna power for the vessel

			double strongestAnt = 0;
			double allAnt = 0;
			double awce = GetAWCE (antennas);
			foreach (ModuleDataTransmitter ant in antennas) {
				allAnt += ant.antennaPower;
				if (ant.antennaPower > strongestAnt) {
					strongestAnt = ant.antennaPower;
				}
			}
			double vesselPower = strongestAnt * Math.Pow (allAnt / strongestAnt, awce);
			return vesselPower;
		}

		public static double GetRange (double activeAntPower, double targetAntPower)
		{
			return Math.Sqrt (activeAntPower * targetAntPower);
		}

		public static MyTuple GetDistancePlanet (CelestialBody home, CelestialBody target)
		{
			double max = home.orbit.ApR + target.orbit.ApR;
			double min;
			if (home.orbit.PeR > target.orbit.PeR) {
				min = home.orbit.PeR - target.orbit.PeR;
			} else {
				min = target.orbit.PeR - home.orbit.PeR;
			}
			return new MyTuple (target.bodyName, min, max);
		}

//		public static MyTuple GetDistanceMoon (CelestialBody home, CelestialBody moon)
//		{
//			return new MyTuple (moon.bodyName, moon.orbit.PeR, moon.orbit.ApR);
//		}

		public static double GetSignalStrength (double maxRange, double distance)
		{
			if (distance > maxRange) { return 0; }

			double relativeDistance = 1 - (distance / maxRange);//Math.Abs (1 - (distance / maxRange));
//			Debug.Log ("Relative distance = " + relativeDistance);
			double strength = (3 - (2 * relativeDistance)) * (relativeDistance * relativeDistance);

			if (strength < 0) { return 0; }

			return strength;
		}
		#endregion
	}

	public class MyTuple
	{
		public string item1 = null;
		public double item2 = Double.NaN;
		public double item3 = Double.NaN;

		public MyTuple (string itemStr, double itemDouble)
		{
			item1 = itemStr;
			item2 = itemDouble;
		}

		public MyTuple (string itemStr, double itemDouble, double itemDouble2)
		{
			item1 = itemStr;
			item2 = itemDouble;
			item3 = itemDouble2;
		}
	}
}

