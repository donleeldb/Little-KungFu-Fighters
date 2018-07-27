using UnityEngine;
using System.Collections;

public class Stamina : MonoBehaviour {

	public int MaxStamina = 100;
	public float CurrentStamina = 100;
	public bool invulnerable;
	public delegate void OnStaminaChange(float percentage, GameObject GO);
	public static event OnStaminaChange onHealthChange;

	//substract health
	public void SubstractStamina(float damage){
		if(!invulnerable){

			//reduce hp
			CurrentStamina = Mathf.Clamp(CurrentStamina -= damage, 0, MaxStamina);

			//sendupdate Health Event
			SendUpdateEvent();
		}
	}

	//add health
	public void AddStamina(float amount){
		CurrentStamina = Mathf.Clamp(CurrentStamina += amount, 0, MaxStamina);
		SendUpdateEvent();
	}


	//health update event
	void SendUpdateEvent(){
		float CurrentHealthPercentage = 1f/MaxStamina * CurrentStamina;
		if(onHealthChange != null) onHealthChange(CurrentHealthPercentage, gameObject);
	}
}
