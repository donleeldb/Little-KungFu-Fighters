using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

	private float gravity = 0.50f;
	private float speed = 5.0f;


	private PlayerState playerState;
	private Action action;

	private Vector3 lastMotion;
	private CharacterController controller;

	private PlayerAnimator anim;

	private int attackNum = 0; //the current attack number
	private bool continuePunchCombo; //true if a punch combo needs to continue
	private float LastAttackTime = 0;



	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		anim = GetComponentInChildren<PlayerAnimator> ();
		action = GetComponentInChildren<Action> ();
		playerState = GetComponent<PlayerState> ();
	}

	// Update is called once per frame
	void Update () {

		action.moveVector = Vector3.zero;
		action.verticalVelocity -= gravity;

		if (!controller.isGrounded) { // not reliable
			//		if (!IsControllerGrounded()) { // mine is bugged
			if (playerState.currentState != PLAYERSTATE.KNOCKBACK) {
				
				action.moveVector.x = lastMotion.x;
				action.moveVector.z = lastMotion.z;
			}
		} else {
			if (playerState.currentState == PLAYERSTATE.KNOCKBACK) {
				if (action.verticalVelocity < 1f) {
					playerState.currentState = PLAYERSTATE.KNOCKDOWN;
					anim.KnockDown ();
				}
			}
		}

		action.moveVector.y = action.verticalVelocity;
		action.move ();
		lastMotion = action.moveVector;

	}

	private bool IsControllerGrounded() {

		Vector3 leftRayStart;
		Vector3 rightRayStart;

		leftRayStart = controller.bounds.center;
		rightRayStart = controller.bounds.center;

		leftRayStart.x -= controller.bounds.extents.x;
		rightRayStart.x += controller.bounds.extents.x;

		Debug.DrawRay (leftRayStart, Vector3.down, Color.red);
		Debug.DrawRay (rightRayStart, Vector3.down, Color.green);
		Debug.Log (Physics.Raycast (leftRayStart, Vector3.down, (controller.height / 2) + 0.2f));
		if (Physics.Raycast (leftRayStart, Vector3.down, (controller.height / 2) + 0.2f)) {
			return true;

		}
		if (Physics.Raycast (rightRayStart, Vector3.down, (controller.height / 2) + 0.2f)) {
			return true;
		}
		return false;
	}

}
