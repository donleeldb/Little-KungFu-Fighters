using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour {

	public bool facingRight;

	private Vector3 moveVector;
	private Vector3 lastMotion;
	private CharacterController controller;

	private PlayerState playerState;
	private PlayerAnimator anim;


	private int attackNum = 0; //the current attack number
	private bool continuePunchCombo; //true if a punch combo needs to continue
	private float LastAttackTime = 0;
	private bool targetHit; //true if the last hit has hit a target

	private int HitKnockDownThreshold = 3; //the number of times the player can be hit before being knocked down
	private int HitKnockDownCount = 0; //the number of times the player is hit in a row
	private int HitKnockDownResetTime = 1; //the time before the hitknockdown counter resets
	private float LastHitTime = 0; // the time when we were hit 

	private int DefenseThreshold = 3; 
	private int DefenseCount = 0; 
	private int DefenseResetTime = 1; 
	private float LastDefenseTime = 0;

	public float verticalVelocity;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		anim = GetComponentInChildren<PlayerAnimator> ();
		playerState = GetComponent<PlayerState> ();
		facingRight = true;
	}

	public void move(Vector3 moveVector) {
		moveVector.y = verticalVelocity;
		controller.Move (moveVector * Time.deltaTime);
		Flip (moveVector.x);
	}

	public void StopDefend() {
		anim.StopDefend ();
		playerState.SetState (PLAYERSTATE.IDLE);
	}

	public void StartDefend() {
		anim.StartDefend ();
		playerState.SetState (PLAYERSTATE.DEFENDING);
	}

	public void ContinuePunch() {
		
		continuePunchCombo = true;
		if (attackNum == 2)
			continuePunchCombo = false;
	}

	public void DoPunch() {
		playerState.SetState (PLAYERSTATE.PUNCH);
		anim.Punch (0);
		DamageObject d = new DamageObject (20, this.gameObject, 0.5f, Vector3.zero, 0.005f);
		CheckForHit (d);
	}

	public void DoJumpKick() {
		playerState.SetState (PLAYERSTATE.JUMPKICK);
		anim.JumpKick ();
		DamageObject d = new DamageObject (20, this.gameObject, 0.5f, Vector3.zero, 0.01f);
		d.attackType = AttackType.KnockDown;
		CheckForHit (d);
	}

	public void DoSprintPunch() {
		playerState.SetState (PLAYERSTATE.SPRINTPUNCH);
		anim.SprintPunch ();
		DamageObject d = new DamageObject (20, this.gameObject, 1.5f, Vector3.zero, 0.01f);
		d.attackType = AttackType.KnockDown;
		CheckForHit (d);
	}

	public void ShengLongBa() {
		anim.ShengLongBa ();
		DamageObject d1 = new DamageObject (20, this.gameObject, 0.5f, Vector3.down, 0.01f, 3f);
		d1.attackType = AttackType.KnockDown;
		d1.lag = 0f;
		CheckForHit (d1);
		DamageObject d2 = new DamageObject (20, this.gameObject, 0.5f, Vector3.down, 0.01f, 0.5f);
		d2.lag = 0.1f;
		CheckForHit (d2);
	}
		
	public void getHit(DamageObject d) {

		bool wasHit = true;

		if (playerState.currentState == PLAYERSTATE.KNOCKDOWN) {
			wasHit = false;
		}

		// HOTFIX for addForce making character not grounded.
		if (controller.isGrounded == false && playerState.currentState != PLAYERSTATE.HIT && playerState.currentState != PLAYERSTATE.DEFENDING) {
			d.attackType = AttackType.KnockDown; 
			HitKnockDownCount = 0;
		}

		//defend
		if(playerState.currentState == PLAYERSTATE.DEFENDING){
			wasHit = false;
			UpdateDefenseCounter ();
			if (DefenseCount >= DefenseThreshold) { 
				wasHit = true;
				anim.StopDefend();
				DefenseCount = 0;
			} else {
//			if(BlockAttacksFromBehind || isFacingTarget (d.inflictor)) wasHit = false;
//			if(!wasHit){
////				GlobalAudioPlayer.PlaySFX ("Defend");
////				anim.ShowDefendEffect();
//
				if(isFacingTarget(d.inflictor)){ 
					anim.AddForce(-0.005f, facingRight);
				} else {
					anim.AddForce(0.005f, facingRight);
				}
//			}
			}

		}

		//parry //need to add math for different types of attack
		if (playerState.currentState == PLAYERSTATE.PUNCH) {
			wasHit = false;
		}

		if (wasHit) {
			UpdateHitCounter ();
		}

		if (HitKnockDownCount >= HitKnockDownThreshold) { 
			d.attackType = AttackType.KnockDown; 
			HitKnockDownCount = 0;
		}
			

		//start knockDown sequence
		if (wasHit && playerState.currentState != PLAYERSTATE.KNOCKDOWN) {
//			GetComponent<HealthSystem> ().SubstractHealth (d.damage);
//			anim.ShowHitEffect ();

			if (d.attackType == AttackType.KnockDown) {
				playerState.SetState (PLAYERSTATE.KNOCKDOWN);
				KnockDown (d.inflictor);

				if(isFacingTarget(d.inflictor)){ 
					anim.AddForce(-0.05f, facingRight);
				} else {
					anim.AddForce(0.05f, facingRight);
				}

				if (d.verticalForce != 0f) {
//					anim.AddVerticalForce (d.verticalForce, facingRight);
					verticalVelocity = 15f;
				}

			} else {
				playerState.SetState (PLAYERSTATE.HIT);
				anim.Hit ();
				if(isFacingTarget(d.inflictor)){ 
					anim.AddForce(-d.force, facingRight);
				} else {
					anim.AddForce(d.force, facingRight);
				}
			}
		}
	}

	//updates the hit counter
	private void UpdateHitCounter() {
		if (Time.time - LastHitTime < HitKnockDownResetTime) { 
			HitKnockDownCount += 1;
		} else {
			HitKnockDownCount = 1;
		}
		LastHitTime = Time.time;
	}

	//updates the Defense counter
	private void UpdateDefenseCounter() {
		if (Time.time - LastDefenseTime < DefenseResetTime) { 
			DefenseCount += 1;
		} else {
			DefenseCount = 1;
		}
		LastDefenseTime = Time.time;
	}

	private void Flip(float speed) {
		if (speed > 0 && !facingRight || speed < 0 && facingRight) {
			facingRight = !facingRight;
			Vector3 temp = transform.localScale;
			temp.x *= -1;
			transform.localScale = temp;
		}

	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.tag == "fighter") {
			Physics.IgnoreCollision (hit.gameObject.GetComponent<CharacterController> (), controller);
		}
	}

	public void Ready(string animName) {
		if (playerState.currentState == PLAYERSTATE.KNOCKDOWN) {
			if (animName == "PlayerKnockDown") {
//				controller.enabled = true;
			} else { //Hit's ready call, do nothing if knocked down.
				return;
			}

		}
		if (continuePunchCombo) {
			DamageObject d = new DamageObject (20, this.gameObject, 0.3f, Vector3.zero, 0.005f);
			CheckForHit (d);
			doPunchAttack ();
			continuePunchCombo = false;

		} else {
			attackNum = 0;
			anim.Idle ();
			playerState.SetState (PLAYERSTATE.IDLE);

		}

	}

	//returns the next attack number in the combo chain
	private int GetNextAttackNum() {
		if (playerState.currentState == PLAYERSTATE.PUNCH) {
			//			attackNum = Mathf.Clamp (attackNum += 1, 0, PunchAttackData.Length - 1);
			//			if (Time.time - LastAttackTime > KickAttackData [attackNum].comboResetTime || !targetHit)

			attackNum = Mathf.Clamp (attackNum + 1, 0, 2); // it's not clamping but if num greater than 2 animator fails.

			return attackNum;

		} 
		return 0;
	}

	//do a punch attack
	private void doPunchAttack() {
		playerState.SetState (PLAYERSTATE.PUNCH);
		anim.Punch (GetNextAttackNum ());
		if (attackNum == 2)
			continuePunchCombo = false;
		//		LastAttackTime = Time.time; // not using this rn
	}

	//checks if we have hit something (animation event)
	private void CheckForHit(DamageObject d) {
		int dir = -1;
		if (facingRight) {
			dir = 1;
		}

		float moveTime = 0.5f;

		StartCoroutine (WaitBeforeRaycast (d, dir));

	}

	//returns true is the player is facing the enemy
	public bool isFacingTarget(GameObject g) {
		int dir = -1;
		if (facingRight) {
			dir = 1;
		}		
		if ((g.transform.position.x > transform.position.x && dir == 1) || (g.transform.position.x < transform.position.x && dir == -1))
			return true;
		else
			return false;
	}

	public void KnockDown(GameObject inflictor) {
//		controller.enabled = false;
		anim.KnockDown ();
		float t = 0;
		float travelSpeed = 2f;
		Rigidbody rb = GetComponent<Rigidbody> ();

		//get the direction of the attack
		int dir = inflictor.transform.position.x > transform.position.x ? 1 : -1;

		//look towards the direction of the incoming attack (should I?)
//		GetComponent<Player>().facingRight = false;
//		if (dir == 1) {
//			GetComponent<Player>().facingRight = true;
//		}

	}

	//on animation finish
	IEnumerator WaitBeforeRaycast(DamageObject d, int dir) {
		
		LayerMask npcLayerMask = LayerMask.NameToLayer ("NPC");
		LayerMask playerLayerMask = LayerMask.NameToLayer ("Player"); 

		yield return new WaitForSeconds(d.lag);

//		Vector3 playerPos = transform.position + Vector3.up * 1.5f;

		Vector3 rayLength = Vector3.right * dir * d.range; // dictates the attack range
		Vector3 center = controller.bounds.center + d.centerOffset;


		RaycastHit[] hits = Physics.SphereCastAll (center + rayLength * 0.7f, d.range * 0.3f, Vector3.right * dir, 0, 1 << npcLayerMask | 1 << playerLayerMask);
		Debug.DrawRay (center + rayLength * 0.4f, Vector3.right * dir * d.range * 0.6f, Color.red,1); // first param is the source, second is length

		//we have hit something
		for (int i = 0; i < hits.Length; i++) {

			LayerMask layermask = hits [i].collider.gameObject.layer;
			//we have hit an enemy
			if (layermask == npcLayerMask || layermask == playerLayerMask) {
				GameObject enemy = hits [i].collider.gameObject;

				if (enemy.GetComponent<CharacterController>() != controller && !(enemy.GetComponent<PlayerState>().currentState == PLAYERSTATE.KNOCKDOWN && enemy.GetComponent<CharacterController>().isGrounded)) {

					enemy.GetComponent<Action>().getHit(d);
					targetHit = true;
				}

			}
		}

		//we havent hit anything
		if(hits.Length == 0){ 
			targetHit = false;
		}
	}



}
