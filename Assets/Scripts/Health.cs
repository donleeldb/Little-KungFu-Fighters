﻿using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public int MaxHp = 100;
	public int CurrentHp = 100;
	public bool invulnerable;
	public delegate void OnHealthChange(float percentage, GameObject GO);
	public static event OnHealthChange onHealthChange;

	//substract health
	public void SubstractHealth(int damage){
		if(!invulnerable){

			//reduce hp
			CurrentHp = Mathf.Clamp(CurrentHp -= damage, 0, MaxHp);

			//sendupdate Health Event
			SendUpdateEvent();
		}
	}

	//add health
	public void AddHealth(int amount){
		CurrentHp = Mathf.Clamp(CurrentHp += amount, 0, MaxHp);
		SendUpdateEvent();
	}


	//health update event
	void SendUpdateEvent(){
		float CurrentHealthPercentage = 1f/MaxHp * CurrentHp;
		if(onHealthChange != null) onHealthChange(CurrentHealthPercentage, gameObject);
	}
}
