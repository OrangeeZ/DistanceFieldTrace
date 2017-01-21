using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WarFog {

	public class Tracer : MonoBehaviour {

		public float RadiusScale { get; set; }

		[SerializeField]
		private float _radius = 5f;

		private WarFogSpaceMap _warFogSpaceMap;

		private void OnDrawGizmos() {

			if ( !Application.isPlaying ) {

				Trace( _warFogSpaceMap = _warFogSpaceMap ?? FindObjectOfType<WarFogSpaceMap>() );
			}
		}

		private void Start() {

			RadiusScale = 1f;

			WarFogController.Tracers.Add( this );
		}

		private void OnDestroy() {

			WarFogController.Tracers.Remove( this );
		}

		public void SetRadiusScale( float scale ) {

			RadiusScale = scale;

			_warFogSpaceMap.ClearVisible();
		}

		public void Trace( WarFogSpaceMap warFogSpaceMap ) {
			return;
			_warFogSpaceMap = warFogSpaceMap;
			warFogSpaceMap.Trace( transform.position, Mathf.RoundToInt( _radius * RadiusScale ) );
		}

	}

}