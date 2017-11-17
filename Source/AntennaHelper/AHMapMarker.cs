using System;
using UnityEngine;

namespace AntennaHelper
{
	public class AHMapMarker : MonoBehaviour
	{
		private Transform parent, target;
		private GameObject marker, markerMirror, circleGreen, circleYellow, circleOrange, circleRed;
		private float scaleGreen, scaleYellow, scaleOrange, scaleRed;
		private bool isEnabled;

		public void Start ()
		{
//			TimingManager.LateUpdateAdd (TimingManager.TimingStage.BetterLateThanNever, DoUpdate);

			isEnabled = false;
			GameEvents.OnMapEntered.Add (MapEnter);
			GameEvents.OnMapExited.Add (MapExit);
		}

		public void SetUp (double maxRange, Transform mapObjectTransmitter, Transform mapObjectRelay, bool relayIsHome = false, CommNet.CommLink link = null, double sS = Double.NaN)
		{
			if (mapObjectRelay != null && mapObjectTransmitter != null) {
				parent = mapObjectRelay;
				target = mapObjectTransmitter;
			}


			if (relayIsHome) {
				maxRange += Planetarium.fetch.Home.Radius;
			}


			//
			if (link != null) {
				double rangeOffset = 0;
				if (link.b.isHome) {
					Debug.Log ("[AH] MapMarker : link.b is home");
					parent = FlightGlobals.GetHomeBody ().MapObject.trf;
					rangeOffset = FlightGlobals.GetHomeBody ().Radius;
				} else {
					Debug.Log ("[AH] MapMarker : link.b is a vessel");
					parent = link.b.transform.GetComponent<Vessel> ().mapObject.trf;
				}
				target = link.a.transform.GetComponent<Vessel> ().mapObject.trf;
			}

			//


//			scaleGreen = AHUtil.GetMapScale (AHUtil.GetDistanceAt75 (maxRange));
//			scaleYellow = AHUtil.GetMapScale (AHUtil.GetDistanceAt50 (maxRange));
//			scaleOrange = AHUtil.GetMapScale (AHUtil.GetDistanceAt25 (maxRange));
//			scaleRed = AHUtil.GetMapScale (maxRange);
//
//			//
//			if (sS <= .25d) {
//				scaleGreen = 0;
//				scaleYellow = 0;
//				scaleOrange = 0;
//				scaleRed = AHUtil.GetMapScale (maxRange);
//			} else if (sS <= .5d) {
//				scaleGreen = 0;
//				scaleYellow = 0;
//				scaleOrange = AHUtil.GetMapScale (AHUtil.GetDistanceAt25 (maxRange));
//				scaleRed = AHUtil.GetMapScale (maxRange);
//			} else if (sS <= .75d) {
//				scaleGreen = 0;
//				scaleYellow = AHUtil.GetMapScale (AHUtil.GetDistanceAt50 (maxRange));
//				scaleOrange = AHUtil.GetMapScale (AHUtil.GetDistanceAt25 (maxRange));
//				scaleRed = AHUtil.GetMapScale (maxRange);
//			} else {
//				scaleGreen = AHUtil.GetMapScale (AHUtil.GetDistanceAt75 (maxRange));
//				scaleYellow = AHUtil.GetMapScale (AHUtil.GetDistanceAt50 (maxRange));
//				scaleOrange = AHUtil.GetMapScale (AHUtil.GetDistanceAt25 (maxRange));
//				scaleRed = AHUtil.GetMapScale (maxRange);
//			}
			//

			scaleGreen = 0;
			scaleYellow = 0;
			scaleOrange = 0;
			scaleRed = AHUtil.GetMapScale (maxRange);
			if (sS >= .25d) {
				// draw orange circle
				scaleOrange = AHUtil.GetMapScale (AHUtil.GetDistanceForOrange (sS, maxRange));
			}
			if (sS >= .5d) {
				// draw yellow circle
				scaleYellow = AHUtil.GetMapScale (AHUtil.GetDistanceForYellow (sS, maxRange));
			}
			if (sS >= .75d) {
				// draw green circle
				scaleGreen = AHUtil.GetMapScale (AHUtil.GetDistanceForGreen (sS, maxRange));
			}
			if (sS == 1d) {
				scaleGreen = AHUtil.GetMapScale (AHUtil.GetDistanceAt75 (maxRange));
				scaleYellow = AHUtil.GetMapScale (AHUtil.GetDistanceAt50 (maxRange));
				scaleOrange = AHUtil.GetMapScale (AHUtil.GetDistanceAt25 (maxRange));
			}


			// Creating circles :
			circleGreen = GameObject.CreatePrimitive (PrimitiveType.Quad);
			circleGreen.layer = 10;
			Destroy (circleGreen.GetComponent<MeshCollider> ());
			circleGreen.GetComponent<MeshRenderer> ().material = new Material (Shader.Find ("Unlit/Transparent"));
			circleGreen.GetComponent<MeshRenderer> ().material.mainTexture = AHUtil.circleGreenTex;
			circleGreen.transform.localScale = new Vector3 (scaleGreen, scaleGreen, 1f);

			circleYellow = GameObject.CreatePrimitive (PrimitiveType.Quad);
			circleYellow.layer = 10;
			Destroy (circleYellow.GetComponent<MeshCollider> ());
			circleYellow.GetComponent<MeshRenderer> ().material = new Material (Shader.Find ("Unlit/Transparent"));
			circleYellow.GetComponent<MeshRenderer> ().material.mainTexture = AHUtil.circleYellowTex;
			circleYellow.transform.localScale = new Vector3 (scaleYellow, scaleYellow, 1f);

			circleOrange = GameObject.CreatePrimitive (PrimitiveType.Quad);
			circleOrange.layer = 10;
			Destroy (circleOrange.GetComponent<MeshCollider> ());
			circleOrange.GetComponent<MeshRenderer> ().material = new Material (Shader.Find ("Unlit/Transparent"));
			circleOrange.GetComponent<MeshRenderer> ().material.mainTexture = AHUtil.circleOrangeTex;
			circleOrange.transform.localScale = new Vector3 (scaleOrange, scaleOrange, 1f);

			circleRed = GameObject.CreatePrimitive (PrimitiveType.Quad);
			circleRed.layer = 10;
			Destroy (circleRed.GetComponent<MeshCollider> ());
			circleRed.GetComponent<MeshRenderer> ().material = new Material (Shader.Find ("Unlit/Transparent"));
			circleRed.GetComponent<MeshRenderer> ().material.mainTexture = AHUtil.circleRedTex;
			circleRed.transform.localScale = new Vector3 (scaleRed, scaleRed, 1f);

			// set position and parenting :
			marker = this.gameObject;
			marker.layer = 10;
			marker.transform.localPosition = Vector3.zero;
			marker.transform.SetParent (parent);
			marker.transform.position = parent.position;

			circleGreen.transform.localPosition = Vector3.zero;
			circleGreen.transform.SetParent (marker.transform);
			circleGreen.transform.position = marker.transform.position;
			circleGreen.transform.localRotation = Quaternion.Euler (Vector3.zero);

			circleYellow.transform.localPosition = Vector3.zero;
			circleYellow.transform.SetParent (marker.transform);
			circleYellow.transform.position = marker.transform.position;
			circleYellow.transform.localRotation = Quaternion.Euler (Vector3.zero);

			circleOrange.transform.localPosition = Vector3.zero;
			circleOrange.transform.SetParent (marker.transform);
			circleOrange.transform.position = marker.transform.position;
			circleOrange.transform.localRotation = Quaternion.Euler (Vector3.zero);

			circleRed.transform.localPosition = Vector3.zero;
			circleRed.transform.SetParent (marker.transform);
			circleRed.transform.position = marker.transform.position;
			circleRed.transform.localRotation = Quaternion.Euler (Vector3.zero);

			//Set a mirror of the circles
			markerMirror = (GameObject)Instantiate (marker, marker.transform);
			Destroy (markerMirror.GetComponent<AHMapMarker> ());
			markerMirror.transform.eulerAngles = new Vector3 (180, 0, 0);
			foreach (MeshCollider collider in markerMirror.GetComponentsInChildren<MeshCollider> ()) {
				Destroy (collider);
				////
				/// For whatever reason when duplicating a gameObject the collider destroyed on
				/// the original gets added back to the clone...
				//
			}
			marker.SetActive (false);
		}

		public void OnDestroy ()
		{
//			TimingManager.LateUpdateRemove (TimingManager.TimingStage.BetterLateThanNever, DoUpdate);
			GameEvents.OnMapEntered.Remove (MapEnter);
			GameEvents.OnMapExited.Remove (MapExit);
		}
		#region Hide/Show
		private void MapEnter ()
		{
			if (isEnabled) {
				marker.SetActive (true);
			}
		}

		private void MapExit ()
		{
			if (marker != null) {
				marker.SetActive (false);
			}
		}

		public void Hide ()
		{
			marker.SetActive (false);
			isEnabled = false;
		}

		public void Show ()
		{
			marker.SetActive (true);
			isEnabled = true;
		}
		#endregion
		public void Update ()//DoUpdate ()
		{
			if (! this.isActiveAndEnabled) { return; }

			marker.transform.LookAt (target);
			marker.transform.Rotate (Vector3.right, 90f);
		}
	}
}

