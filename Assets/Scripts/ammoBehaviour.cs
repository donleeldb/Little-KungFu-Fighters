using UnityEngine;
using System.Collections;

public class ammoBehaviour : MonoBehaviour {

	[Tooltip("how much health would be decreased")] public float damage;
	[Tooltip("direction, where hitted character would fly (not normilized)")] public Vector2 hitDirection;

	[HideInInspector] public int k; // factor of a line of sight

	void OnTriggerEnter2D (Collider2D character) // checks for an collision with fighters
	{
		if (character.gameObject.layer == 8 || character.gameObject.layer == 9) // checks if it is a fighter
		{
			character.gameObject.GetComponent<fighterScript>().getAttacked(true, damage, true, k, hitDirection); // calls getAttacked function
		}
		gameObject.SetActive (false); // disables ammo;
	}

}
