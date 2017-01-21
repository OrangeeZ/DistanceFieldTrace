using UnityEngine;
using System.Collections;

public class Occluder : MonoBehaviour {

	[SerializeField]
	private Bounds _bounds;

	[SerializeField]
	private float _additionalAngle;

	[SerializeField]
	private Matrix4x4 _finalTransform;

	[SerializeField]
	private Matrix4x4 _finalTransformInverse;

	private void Reset() {

		var renderer = GetComponentInChildren<UnityEngine.Renderer>( includeInactive: true );
		if ( renderer != null ) {

			_bounds = renderer.bounds;
		} else {

			var collider = GetComponentInChildren<Collider>( includeInactive: true );
			if ( collider != null ) {

				_bounds = collider.bounds;
			}
		}

		var inverseScale = transform.localScale;
		inverseScale.x = 1f / inverseScale.x;
		inverseScale.y = 1f / inverseScale.y;
		inverseScale.z = 1f / inverseScale.z;

		_bounds.center -= transform.position;
		_bounds.size = Vector3.Scale( _bounds.size, inverseScale );

		SetAdditionalAngle( _additionalAngle );
	}

	public bool IsAffectingPoint( Vector3 point ) {

		var localPoint = _finalTransform.MultiplyPoint3x4( point );

		return _bounds.Contains( localPoint );
	}

	public float GetSquareDistanceToPoint( Vector3 point ) {

		var localPoint = _finalTransform.MultiplyPoint3x4( point );
		var closestPointOnBounds = _finalTransformInverse.MultiplyPoint3x4( _bounds.ClosestPoint( localPoint ) );

		return ( point - closestPointOnBounds ).sqrMagnitude;
	}

	public void SetLocalBounds( Bounds warFogOccluderBounds ) {

		_bounds = warFogOccluderBounds;
	}

	public void SetAdditionalAngle( float additionalAngle ) {

		_additionalAngle = additionalAngle;

		_finalTransform = Matrix4x4.TRS( Vector3.zero, Quaternion.AngleAxis( _additionalAngle, Vector3.forward ), Vector3.one ) * transform.worldToLocalMatrix;
		_finalTransformInverse = _finalTransform.inverse;
	}

	private void OnDrawGizmos() {

		Gizmos.matrix = _finalTransform.inverse;
		Gizmos.DrawWireCube( _bounds.center, _bounds.size );
	}

}