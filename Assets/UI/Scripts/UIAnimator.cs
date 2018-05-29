using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIAnimator : MonoBehaviour {

	RectTransform t;
	public float speed;
	public List<Vector2> path;

	// Use this for initialization
	void Start () {
		t = gameObject.GetComponent<RectTransform> ();
		List<Vector2> newPath = new List<Vector2> ();
		newPath.Add (t.localPosition);
		for (int i = 0; i < path.Count; i++) {
			newPath.Add (path[i] + newPath [0]);
		}
		path = newPath;
	}
	
	// Update is called once per frame
	void Update () {
		
		t.localPosition = Vector2.MoveTowards (t.localPosition, path[0], speed);

		if (Vector2.Distance (t.localPosition, path[0]) < 3) {
			Vector2 point = path [0];
			path.Remove (point);
			path.Add (point);
		}

	}
}
