using UnityEngine;
using System.Collections;

public class OccluderDistanceTester : MonoBehaviour {

	[SerializeField]
	private Occluder _targetOccluder;

	void OnDrawGizmos() {

		Debug.Log( Mathf.Sqrt( _targetOccluder.GetSquareDistanceToPoint( transform.position ) ) );
		Debug.Log( _targetOccluder.IsAffectingPoint( transform.position ) );
	}

}