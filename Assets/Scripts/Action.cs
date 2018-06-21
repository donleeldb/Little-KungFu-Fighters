using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour {

	private float verticalVelocity;
	private float gravity = 1.0f;
	private float speed = 5.0f;

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
		CheckForHit ();
		continuePunchCombo = true;
		if (attackNum == 2)
			continuePunchCombo = false;
	}

	public void DoPunch() {
		playerState.SetState (PLAYERSTATE.PUNCH);
		anim.Punch (0);
		CheckForHit ();
	}
		
	public void getHit() {
		anim.Hit ();
		print("help");
		playerState.SetState (PLAYERSTATE.HIT);
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

	public void Ready() {
		print("ready");
		print(playerState.currentState);
		if (continuePunchCombo) {
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
			print (attackNum);

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
		if (facingRight) {
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
		print(hits.Length);
		for (int i = 0; i < hits.Length; i++) {

			LayerMask layermask = hits [i].collider.gameObject.layer;
			print("hit");
			//we have hit an enemy
			if (layermask == npcLayerMask || layermask == playerLayerMask) {
				GameObject enemy = hits [i].collider.gameObject;

				if (enemy.GetComponent<CharacterController>() != controller) {
					//				DealDamageToEnemy (hits [i].collider.gameObject);
					enemy.GetComponent<Action>().getHit();
					print("hit");
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

}
