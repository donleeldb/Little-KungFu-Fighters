using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

	private Transform lookAt;
	// Use this for initialization
	void Start () {
		lookAt = GetComponentsInParent<Transform> ()[0];
	}

	// Update is called once per frame
	void LateUpdate () {
		Vector3 newPos = new Vector3 (lookAt.transform.position.x, 0.6f, lookAt.transform.position.z);
		transform.position = newPos;
	}
}