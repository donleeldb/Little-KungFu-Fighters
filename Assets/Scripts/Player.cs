﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private float inputDirection_x;
	private float inputDirection_z;

	private float verticalVelocity;
	private float gravity = 1.0f;
	private float speed = 5.0f;

	public bool facingRight;

	private Vector3 moveVector;
	private Vector3 lastMotion;
	private CharacterController controller;

	private PlayerState playerState;
	private PlayerAnimator anim;
	private Action action;

	public KeyCode Left;
	public KeyCode Right;
	public KeyCode Up;
	public KeyCode Down;
	public KeyCode PunchKey;
	public KeyCode JumpKey;
	public KeyCode DefendKey;

	public string horizontal = "Horizontal";
	public string depth = "Depth";


	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		anim = GetComponentInChildren<PlayerAnimator> ();
		action = GetComponent<Action> ();
		playerState = GetComponent<PlayerState> ();
		facingRight = true;
	}
	
	// Update is called once per frame
	void Update () {

		moveVector = Vector3.zero;

		inputDirection_x = Input.GetAxis (horizontal) * speed;
		inputDirection_z = Input.GetAxis (depth) * speed;

		//STOPDEFENDING is a character action
		if (Input.GetKeyUp (DefendKey)) {
			action.StopDefend ();
		}

		if (playerState.currentState == PLAYERSTATE.HIT ||  playerState.currentState == PLAYERSTATE.KNOCKDOWN) {
			return;
		}

		if (controller.isGrounded) { // not reliable
//		if (IsControllerGrounded()) { // mine is bugged

			// When defending, you can't do anything else
			if (playerState.currentState == PLAYERSTATE.DEFENDING) {
				action.StartDefend ();
				if (Input.GetKeyDown (PunchKey) && Input.GetKeyDown(Up)) {
					print("Tornado");
				}

			} else if (playerState.currentState == PLAYERSTATE.PUNCH) { // when attacking, can't do anything else except for keep attacking
				if (Input.GetKeyDown (PunchKey)) {
					action.ContinuePunch ();
				}
			} else { // right now is idle, walk
				
				if (Input.GetKeyDown (JumpKey)) {
					verticalVelocity = 15;
					moveVector.x = inputDirection_x;
					moveVector.z = inputDirection_z;
					anim.Jump ();
					playerState.SetState (PLAYERSTATE.JUMPING);
				} else if (Input.GetKeyDown (PunchKey)) { // remove combo here rn. This means that if you didnt press attack during attack animation you dont get to combo. might change
					// two method: 1) add lastAttackTime and allow combo if within time. 2) make attack animation include idle for a while (not recommended because you can't do anything else while punching)

					action.DoPunch ();
				} else if (Input.GetKeyDown (DefendKey)) {
					anim.StartDefend ();
					playerState.SetState (PLAYERSTATE.DEFENDING);
				} else if (playerState.currentState == PLAYERSTATE.KNOCKDOWN) {
					// do not set idle trigger
				} else if (inputDirection_x == 0 && inputDirection_z == 0) {
					if (playerState.currentState != PLAYERSTATE.PUNCH) {
						anim.Idle ();
						playerState.SetState (PLAYERSTATE.IDLE);
					}
				} else {
					anim.Walk ();
					playerState.SetState (PLAYERSTATE.MOVING);
					moveVector.x = inputDirection_x;
					moveVector.z = inputDirection_z;
				}
			}

		} else {
			verticalVelocity -= gravity;
			moveVector.x = lastMotion.x;
			moveVector.z = lastMotion.z;

			if (Input.GetKeyDown (DefendKey)) {
				playerState.SetState (PLAYERSTATE.DEFENDING);
			}
		}
			
		moveVector.y = verticalVelocity;
		controller.Move (moveVector * Time.deltaTime);
		Flip (moveVector.x);
		lastMotion = moveVector;

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
		
}
