using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private float inputDirection_x;
	private float inputDirection_z;

	private float gravity = 0.50f;
	private float speed = 5.0f;

	public float tapSpeed = 0.2f; //in seconds

	private float leftLastTapTime = 0;
	private float rightLastTapTime = 0;

	public bool facingRight;
	private bool continuePunch = false;
	private bool doPunch = false;
	private bool doJumpKick = false;
	private bool doSprintPunch = false;

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
	}
	
	// Update is called once per frame
	void Update () {
		print (playerState.currentState);

		moveVector = Vector3.zero;

		inputDirection_x = Input.GetAxis (horizontal) * speed;
		inputDirection_z = Input.GetAxis (depth) * speed;

		int dir = -1;
		if (action.facingRight) {
			dir = 1;
		}	

		//STOPDEFENDING is a character action
		if (Input.GetKeyUp (DefendKey)) {
			action.StopDefend ();
		}
			
		if (controller.isGrounded) { // not reliable
//		if (IsControllerGrounded()) { // mine is bugged

			if (playerState.currentState == PLAYERSTATE.HIT) {
				
			} else if (playerState.currentState == PLAYERSTATE.KNOCKBACK){
				print (action.verticalVelocity);
				if (action.verticalVelocity < 1f) {
					playerState.currentState = PLAYERSTATE.KNOCKDOWN;
					anim.KnockDown ();
				}

			} else {
				
				// When defending, you can't do anything else
				if (playerState.currentState == PLAYERSTATE.DEFENDING) {
					action.StartDefend ();
					if (Input.GetKeyDown (PunchKey) && Input.GetKeyDown (Up)) {
						action.verticalVelocity = 15;
						moveVector.x = dir*2;
						action.StopDefend ();
						action.ShengLongBa ();
						playerState.SetState (PLAYERSTATE.JUMPING);
					}

				} else if (playerState.currentState == PLAYERSTATE.SPRINTING) {
					if ((Input.GetKeyDown (Left) && action.facingRight) || Input.GetKeyDown (Right) && !action.facingRight) {
						anim.Idle ();
						playerState.SetState (PLAYERSTATE.IDLE);
					} else if (Input.GetKeyDown (PunchKey)) {
						doSprintPunch = true;
						playerState.SetState (PLAYERSTATE.SPRINTPUNCH);
					} if (Input.GetKeyDown (JumpKey)) {
						action.verticalVelocity = 15;
						moveVector.x = inputDirection_x;
						moveVector.z = inputDirection_z;
						anim.Jump ();
						playerState.SetState (PLAYERSTATE.JUMPING);
					} else {
						inputDirection_x = dir * speed * 2;
						moveVector.x = inputDirection_x;
						moveVector.z = inputDirection_z;
					}
						
				} else if (playerState.currentState == PLAYERSTATE.PUNCH) { // when attacking, can't do anything else except for keep attacking
					if (Input.GetKeyDown (PunchKey)) {
						continuePunch = true;
					}
				} else if (playerState.currentState == PLAYERSTATE.SPRINTPUNCH) {
					inputDirection_x = dir * speed;
					moveVector.x = inputDirection_x;
				} else { // right now is idle, walk
					
					if (Input.GetKeyDown (JumpKey)) {
						action.verticalVelocity = 15;
						moveVector.x = inputDirection_x;
						moveVector.z = inputDirection_z;
						anim.Jump ();
						playerState.SetState (PLAYERSTATE.JUMPING);
					} else if (Input.GetKeyDown (PunchKey)) { // remove combo here rn. This means that if you didnt press attack during attack animation you dont get to combo. might change
						// two method: 1) add lastAttackTime and allow combo if within time. 2) make attack animation include idle for a while (not recommended because you can't do anything else while punching)
						doPunch = true;
					} else if (Input.GetKeyDown (DefendKey)) {
						anim.StartDefend ();
						playerState.SetState (PLAYERSTATE.DEFENDING);
					} else if (playerState.currentState == PLAYERSTATE.KNOCKBACK) {
						// do not set idle trigger
					} else if (Input.GetKeyDown (Left)) {
						if ((Time.time - leftLastTapTime) < tapSpeed) {
							anim.Sprint ();
							playerState.SetState (PLAYERSTATE.SPRINTING);
							inputDirection_x = dir * speed * 2;
							moveVector.x = inputDirection_x;
							moveVector.z = inputDirection_z;
						} else {
							leftLastTapTime = Time.time;
						}

					} else if (Input.GetKeyDown (Right)) {
						if ((Time.time - rightLastTapTime) < tapSpeed) {
							anim.Sprint ();
							playerState.SetState (PLAYERSTATE.SPRINTING);
							inputDirection_x = dir * speed * 2;
							moveVector.x = inputDirection_x;
							moveVector.z = inputDirection_z;
						} else {
							rightLastTapTime = Time.time;
						}
					} else if (inputDirection_x == 0 && inputDirection_z == 0) {
						if (playerState.currentState != PLAYERSTATE.PUNCH && playerState.currentState != PLAYERSTATE.SPRINTING) {
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
			}

		} else {
			action.verticalVelocity -= gravity;
			moveVector.x = lastMotion.x;
			moveVector.z = lastMotion.z;

			if (playerState.currentState == PLAYERSTATE.HIT || playerState.currentState == PLAYERSTATE.KNOCKBACK) {
			} else {
				if (playerState.currentState != PLAYERSTATE.JUMPKICK && Input.GetKeyDown (PunchKey)) { 
					doJumpKick = true;
				}

//				if (Input.GetKeyDown (DefendKey)) {
//					playerState.SetState (PLAYERSTATE.DEFENDING);
//				}
			}
		}

		// cant remember why i put this here
//		if (playerState.currentState == PLAYERSTATE.DEFENDING) {
//			return;
//		}
//

		moveVector.y = action.verticalVelocity;
		action.move (moveVector);
		lastMotion = moveVector;

	}

	void FixedUpdate () {
		if (doPunch) {
			action.DoPunch ();

		} else if (continuePunch) {
			action.ContinuePunch ();
		} else if (doJumpKick) {
			action.DoJumpKick ();
		} else if (doSprintPunch) {
			action.DoSprintPunch ();
		}
		doPunch = false;
		continuePunch = false;
		doJumpKick = false;
		doSprintPunch = false;
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



	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.tag == "fighter") {
			Physics.IgnoreCollision (hit.gameObject.GetComponent<CharacterController> (), controller);
		}
	}
		
}
