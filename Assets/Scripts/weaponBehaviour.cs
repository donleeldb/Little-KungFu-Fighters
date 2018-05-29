using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class weaponBehaviour : usableObjectScript {

	[Tooltip("ammo prefab")] public GameObject ammo;
	[Tooltip("number of ammo")] public int ammoNumber;
	[Tooltip("shoot power")] public float shootPower;
	[Tooltip("animator state")] public int fightState;

	[HideInInspector] public List<GameObject> ammunition; // list of an ammo

	// Use this for initialization
	void Start () {
		prepareAmmo ();
		prepareObject ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	}

	void prepareAmmo() 
	{
		ammunition = new List<GameObject>(); // creates ammo list
		for (int i = 0; i < ammoNumber - 1; i++) 
		{
			GameObject newAmmo = Instantiate(ammo, Vector3.zero, Quaternion.identity) as GameObject; // creates new ammo
			ammunition.Add(newAmmo); // puts new ammo in array
			newAmmo.SetActive(false); // disables ammo
		}
	}

	public override void checkFighter (GameObject fighter) // checks for fighter
	{
		if (fighter.gameObject.layer == 8 || fighter.gameObject.layer == 9) 
		{
			fighter.gameObject.GetComponent<fighterScript>().use(gameObject); // fighter takes weapon
			gameObject.SetActive (false); // disable gameObject
		}
	}

}
