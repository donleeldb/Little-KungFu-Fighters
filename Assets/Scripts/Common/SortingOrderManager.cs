using UnityEngine;
using System.Collections;

public class SortingOrderManager : MonoBehaviour {

	SpriteRenderer sr;
	public int offset;

	public void refreshSortingOrder () {
		sr = gameObject.GetComponent <SpriteRenderer> ();
		sr.sortingOrder = - (int) (transform.position.y + offset) * 10;
	}

}
