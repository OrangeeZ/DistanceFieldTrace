using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WarFog {

	public class WarFogController : MonoBehaviour {

		public static List<Tracer> Tracers = new List<Tracer>();

		[SerializeField]
		private DistanceField _distanceField;

		private void Start() {

			_distanceField.GenerateTracingData();
			//_distanceField.SubmitTexture();
		}

		private void Update() {

			foreach ( var each in Tracers ) {

				WarFogRenderer.Instance.SetTracerPosition( each.transform.position );

//				each.Trace( _distanceField );
			}

			_distanceField.SubmitTexture();

//			GetComponentInChildren<MeshRenderer>().material.mainTexture = _distanceField.GetTexture();
		}

	}

}