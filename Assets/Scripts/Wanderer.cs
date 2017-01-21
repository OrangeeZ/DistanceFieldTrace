using UnityEngine;
using System.Collections;

public class Wanderer : MonoBehaviour {

	private Vector3 _initialPosition;

	// Use this for initialization
	void Start () {

		_initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = _initialPosition + Vector3.forward * Mathf.Sin( Time.time * 10 ) * 10 + Vector3.right * Mathf.Cos( 10 + Time.time * 10 ) * 10;

	}
}
