using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour {

	public Transform lookAt;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 newPos = new Vector3 (lookAt.transform.position.x, transform.position.y, transform.position.z);
		transform.position = newPos;
	}
}
