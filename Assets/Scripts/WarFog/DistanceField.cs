using System.Linq;
using UnityEngine;

namespace WarFog {

	public class DistanceField : MonoBehaviour {

		[SerializeField]
		[HideInInspector]
		private float[] _field;

		[SerializeField]
		private float _maxFieldDistance;

		[SerializeField]
		[Range( 0.2f, 10 )]
		private float _cellSize;

		[SerializeField]
		private Bounds _bounds;

		private Occluder[] _occluders;

		[SerializeField]
		private int _cellsX;

		[SerializeField]
		private int _cellsZ;

		[SerializeField]
		private Texture2D _distanceFieldTexture;

		private Color[] _distanceFieldTextureColors;

		public Bounds GetBounds() {

			return _bounds;
		}

		public float GetMaxFieldDistance() {

			return _maxFieldDistance;
		}

		public void SubmitTexture() {

			//UpdateDistanceTextureData();

			if ( WarFogRenderer.Instance != null ) {

				WarFogRenderer.Instance.SetTexture( this, _distanceFieldTexture );
			}
		}

		[ContextMenu( "Generate" )]
		private void Generate() {

			var cellSizeRatio = Mathf.ClosestPowerOfTwo( Mathf.RoundToInt( _bounds.size.x / _cellSize ) ) / ( _bounds.size.x / _cellSize );
			_cellSize /= cellSizeRatio;

			_occluders = FindObjectsOfType<Occluder>();

			var startingPoint = _bounds.center - _bounds.extents;
			_cellsX = ( Mathf.RoundToInt( _bounds.size.x / _cellSize ) );
			_cellsZ = ( Mathf.RoundToInt( _bounds.size.z / _cellSize ) );

			_field = new float[_cellsX * _cellsZ];

			for ( var x = 0; x < _cellsX; ++x ) {

				for ( var z = 0; z < _cellsZ; ++z ) {

					var currentPoint = startingPoint + Vector3.forward * z * _cellSize + Vector3.right * x * _cellSize;
					_field[z * _cellsX + x] = GetDistanceAtPoint( currentPoint );
				}
			}

			GenerateTracingData();
		}

		public void GenerateTracingData() {

			if ( _distanceFieldTexture != null ) {

				DestroyImmediate( _distanceFieldTexture );
			}

			_distanceFieldTexture = new Texture2D( _cellsX, _cellsZ, TextureFormat.RGBAFloat, mipmap: false, linear: true );
			_distanceFieldTextureColors = _distanceFieldTexture.GetPixels();

			UpdateDistanceTextureData();
		}

		private void UpdateDistanceTextureData() {

			for ( var i = 0; i < _distanceFieldTextureColors.Length; i++ ) {

				_distanceFieldTextureColors[i].r = (_field[i] / _maxFieldDistance);//new Color( _field[i] / _maxFieldDistance, 0, 0, 1 ).r;//.linear.r;
			}

			_distanceFieldTexture.SetPixels( _distanceFieldTextureColors );
			_distanceFieldTexture.Apply();
		}

		private float GetDistanceAtPoint( Vector3 point ) {

			var sqrResult = _occluders.Min( _ => _.GetSquareDistanceToPoint( point ) );

			var result = Mathf.Sqrt( sqrResult );

			_maxFieldDistance = Mathf.Max( _maxFieldDistance, result );

			return result;
		}

		private void OnDrawGizmosSelected() {

			Gizmos.color = new Color( 0, 1, 0, 0.3f );
			Gizmos.DrawCube( _bounds.center, _bounds.size );

			if ( _field == null ) {

				return;
			}

//			var startingPoint = _bounds.center - _bounds.extents;
//			var cellsX = Mathf.RoundToInt( _bounds.size.x / _cellSize );
//			var cellsZ = Mathf.RoundToInt( _bounds.size.z / _cellSize );
//
//			for ( var x = 0; x < cellsX; ++x ) {
//
//				for ( var z = 0; z < cellsZ; ++z ) {
//
//					var currentPoint = startingPoint + Vector3.forward * z * _cellSize + Vector3.right * x * _cellSize;
//
//					Gizmos.color = _field[z * cellsX + x] * Color.white;
//
//					Gizmos.DrawCube( currentPoint, Vector3.one * _cellSize );
//				}
//			}
		}

	}

}