using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour {

	private bool facingRight;

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


	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		anim = GetComponentInChildren<PlayerAnimator> ();
		playerState = GetComponent<PlayerState> ();
		facingRight = true;
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
		CheckForHit ();
	}

	public void DoJumpKick() {
		playerState.SetState (PLAYERSTATE.JUMPKICK);
		anim.JumpKick ();
		CheckForHit ();
	}
		
	public void getHit(DamageObject d) {

		bool wasHit = true;
		UpdateHitCounter ();
		//knockdown hit
		if (HitKnockDownCount >= HitKnockDownThreshold) { 
			d.attackType = AttackType.KnockDown; 
			HitKnockDownCount = 0;
		}

		if (playerState.currentState == PLAYERSTATE.KNOCKDOWN) {
			wasHit = false;
		}

		//defend
		if(playerState.currentState == PLAYERSTATE.DEFENDING){
//			if(BlockAttacksFromBehind || isFacingTarget (d.inflictor)) wasHit = false;
//			if(!wasHit){
////				GlobalAudioPlayer.PlaySFX ("Defend");
////				anim.ShowDefendEffect();
////				anim.CamShakeSmall();
//
//				if(isFacingTarget(d.inflictor)){ 
//					anim.AddForce(-1.5f);
//				} else {
//					anim.AddForce(1.5f);
//				}
//			}
		}

//		//getting hit while being in the air also causes a knockdown
//		if(!GetComponent<PlayerMovement>().playerIsGrounded()){
//			d.attackType = AttackType.KnockDown; 
//			HitKnockDownCount = 0;
//		}

		//start knockDown sequence
		if (wasHit && playerState.currentState != PLAYERSTATE.KNOCKDOWN) {
//			GetComponent<HealthSystem> ().SubstractHealth (d.damage);
//			anim.ShowHitEffect ();

			if (d.attackType == AttackType.KnockDown) {
				playerState.SetState (PLAYERSTATE.KNOCKDOWN);
				KnockDown (d.inflictor);

			} else {
				playerState.SetState (PLAYERSTATE.HIT);
				anim.Hit ();
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
				controller.enabled = true;
			}
		}
		if (continuePunchCombo) {
			CheckForHit ();
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
	private void CheckForHit() {
		int dir = -1;
		if (GetComponent<Player> ().facingRight) {
			dir = 1;
		}
		Vector3 playerPos = transform.position + Vector3.up * 1.5f;
		LayerMask npcLayerMask = LayerMask.NameToLayer ("NPC");
		LayerMask playerLayerMask = LayerMask.NameToLayer ("Player");

		//do a raycast to see which enemies/objects are in attack range
		//		RaycastHit2D[] hits = Physics2D.RaycastAll (playerPos, Vector3.right * dir, getAttackRange(), 1 << fighterLayerMask | 1 << itemLayerMask);
		//		Gizmos.DrawSphere (playerPos + new Vector3 (0f,getAttackRange(),0f), 0.5f);
		RaycastHit[] hits = Physics.SphereCastAll (controller.bounds.center, 0.5f, Vector3.right * dir, getAttackRange(), 1 << npcLayerMask | 1 << playerLayerMask);
		Debug.DrawRay (controller.bounds.center, Vector3.right * dir, Color.red, getAttackRange());

		//we have hit something
		for (int i = 0; i < hits.Length; i++) {

			LayerMask layermask = hits [i].collider.gameObject.layer;
			//we have hit an enemy
			if (layermask == npcLayerMask || layermask == playerLayerMask) {
				GameObject enemy = hits [i].collider.gameObject;

				if (enemy.GetComponent<CharacterController>() != controller) {
					//				DealDamageToEnemy (hits [i].collider.gameObject);
					enemy.GetComponent<Action>().getHit(new DamageObject(20, this.gameObject));
					targetHit = true;
				}

			}

			//			//we have hit an item
			//			if (layermask == itemLayerMask) {
			//				GameObject item = hits [i].collider.gameObject;
			//				if (ObjInYRange (item)) {
			//					item.GetComponent<ItemInteractable> ().ActivateItem (gameObject);
			//					ShowHitEffectAtPosition (hits [i].point);
			//				}
			//			}
		}

		//we havent hit anything
		if(hits.Length == 0){ 
			targetHit = false;
		}
	}

	//returns the attack range of the current attack
	private float getAttackRange() {
		//		if (playerState.currentState == PLAYERSTATE.PUNCH && attackNum <= PunchAttackData.Length) {
		//			return PunchAttackData [attackNum].range;
		//		} else if (playerState.currentState == PLAYERSTATE.KICK && attackNum <= KickAttackData.Length) {
		//			return KickAttackData [attackNum].range;
		//		} else if(jumpKickActive){
		//			return JumpKickData.range;
		//		} else {
		//			return 0f;
		//		}
		return 0.5f;
	}

	//returns true is the player is facing the enemy
	public bool isFacingTarget(GameObject g) {
		int dir = -1;
		if (facingRight) {
			dir = 1;
		}		if ((g.transform.position.x > transform.position.x && dir == 1) || (g.transform.position.x < transform.position.x && dir == -1))
			return true;
		else
			return false;
	}

	public void KnockDown(GameObject inflictor) {
		controller.enabled = false;
		anim.KnockDown ();
		float t = 0;
		float travelSpeed = 2f;
		Rigidbody2D rb = GetComponent<Rigidbody2D> ();

		//get the direction of the attack
		int dir = inflictor.transform.position.x > transform.position.x ? 1 : -1;

		//look towards the direction of the incoming attack (should I?)
//		GetComponent<Player>().facingRight = false;
//		if (dir == 1) {
//			GetComponent<Player>().facingRight = true;
//		}

//		while (t < 1) {
//			controller.Move (moveVector * dir * travelSpeed);
//
//			rb.velocity = Vector2.left * dir * travelSpeed;
//			t += Time.deltaTime;
//			yield return 0;
//		}
//
//		//stop traveling
//		rb.velocity = Vector2.zero;


//		do i need this if not doing force?
//		yield return new WaitForSeconds (1);
		//reset
//		playerState.currentState = PLAYERSTATE.IDLE;
//		anim.Idle ();
	}

}
