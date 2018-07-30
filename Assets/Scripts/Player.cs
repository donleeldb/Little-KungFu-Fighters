using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private float inputDirection_x;
	private float inputDirection_z;

	private float gravity = 0.50f;
	private float speed = 5.0f;

	public float tapSpeed = 1f; //in seconds. seems weird tho. 1s should be very long. but still doesn't register sometimes, could be editor problems
	private bool flip;
	private float leftLastTapTime = 0;
	private float rightLastTapTime = 0;
	private float upLastTapTime = 0;
	private float downLastTapTime = 0;


	public bool facingRight;
	private bool continuePunch = false;
	private bool doPunch = false;
	private bool doJumpKick = false;
	private bool doSprintPunch = false;

	private Vector3 lastMotion;
	private CharacterController controller;

	private PlayerState playerState;
	private PlayerAnimator anim;
	private Action action;
	private Health health;
	private Stamina stamina;
	public GameObject staminaBar;

	public KeyCode Left;
	public KeyCode Right;
	public KeyCode Up;
	public KeyCode Down;
	public KeyCode PunchKey;
	public KeyCode JumpKey;
	public KeyCode DefendKey;
	public KeyCode 式Key;

	public string horizontal = "Horizontal";
	public string depth = "Depth";


	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		anim = GetComponentInChildren<PlayerAnimator> ();
		action = GetComponent<Action> ();
		playerState = GetComponent<PlayerState> ();
		health = GetComponent<Health> ();
		stamina = GetComponent<Stamina> ();

	}
	
	// Update is called once per frame
	void Update () {
		print (playerState.currentState);

		stamina.AddStamina (0.1f);

		action.moveVector = Vector3.zero;

		inputDirection_x = Input.GetAxis (horizontal) * speed;
		inputDirection_z = Input.GetAxis (depth) * speed;

		int dir = -1;
		if (action.facingRight) {
			dir = 1;
		}

		//STOPDEFENDING is a character action
		// second condition because if not might break the current combo system by stopdefending twice
		if (Input.GetKeyUp (DefendKey) && playerState.currentState == PLAYERSTATE.DEFENDING) {
			action.StopDefend ();
		}
			
		if (controller.isGrounded) { // not reliable
			flip = true;

//		if (IsControllerGrounded()) { // mine is bugged

			//took away hit because you can now move when 轻伤
			if (playerState.currentState == PLAYERSTATE.KNOCKDOWN) {
				
			} else if (playerState.currentState == PLAYERSTATE.KNOCKBACK){
				print (action.verticalVelocity);
				if (action.verticalVelocity < 1f) {
					playerState.currentState = PLAYERSTATE.KNOCKDOWN;
					anim.KnockDown ();
				}

			} else if (playerState.currentState == PLAYERSTATE.DEFENDING) { // When defending, you can't do anything else
				action.StartDefend ();

			} else if (playerState.currentState == PLAYERSTATE.SPRINTING) {
				
				if ((Input.GetKeyDown (Left) && action.facingRight) || Input.GetKeyDown (Right) && !action.facingRight) {
					anim.Idle ();
					playerState.SetState (PLAYERSTATE.IDLE);
				} else if (Input.GetKeyDown (PunchKey)) {
					if (checkStamina (20)) {
						doSprintPunch = true;
						playerState.SetState (PLAYERSTATE.SPRINTPUNCH);
					}
				} if (Input.GetKeyDown (JumpKey)) {
					action.verticalVelocity = 10;
					action.moveVector.x = inputDirection_x;
					action.moveVector.z = inputDirection_z;
					anim.Jump ();
					playerState.SetState (PLAYERSTATE.JUMPING);
				} else {
					inputDirection_x = dir * speed * 2;
					action.moveVector.x = inputDirection_x;
					action.moveVector.z = inputDirection_z;
				}
					
			} else if (playerState.currentState == PLAYERSTATE.PUNCH) { // when attacking, can't do anything else except for keep attacking
				if (Input.GetKey (式Key)) {
					if (Input.GetKey (PunchKey) && Input.GetKeyDown (Up)) {
						if (checkStamina (10)) {
							action.verticalVelocity = 10;
							action.moveVector.x = dir * 2;
							action.ShengLongBa ();
							playerState.SetState (PLAYERSTATE.JUMPING);
						}
					} else if (Input.GetKey (Up) && Input.GetKeyDown (PunchKey)) {
						if (checkStamina (10)) {
							action.verticalVelocity = 10;
							action.moveVector.x = dir * 2;
							action.ShengLongBa ();
							playerState.SetState (PLAYERSTATE.JUMPING);
						}
					} else if (Input.GetKeyDown (Up)) {

						if ((Time.time - upLastTapTime) < tapSpeed) {
							action.verticalVelocity = 2;
							action.moveVector.x = 0;
							action.moveVector.z = inputDirection_z * speed;
							anim.Jump ();
							playerState.SetState (PLAYERSTATE.JUMPING);
						} else {
							upLastTapTime = Time.time;
						}

					} else if (Input.GetKeyDown (Down)) {
						if ((Time.time - downLastTapTime) < tapSpeed) {
							action.verticalVelocity = 2;
							action.moveVector.x = 0;
							action.moveVector.z = inputDirection_z * speed;
							anim.Jump ();
							playerState.SetState (PLAYERSTATE.JUMPING);
						} else {
							downLastTapTime = Time.time;
						}
					} else { 
						// should probs change to 式 animation
						anim.Idle ();
						playerState.SetState (PLAYERSTATE.IDLE);

					}
				} else if (Input.GetKeyDown (PunchKey)) {
					continuePunch = true;
				}
			} else if (playerState.currentState == PLAYERSTATE.SPRINTPUNCH) {
				inputDirection_x = dir * speed;
				action.moveVector.x = inputDirection_x;
			} else { // right now is idle, walk


				if (Input.GetKey (式Key)) {
					if (Input.GetKey (PunchKey) && Input.GetKeyDown (Up)) {
						if (checkStamina (10)) {
							action.verticalVelocity = 10;
							action.moveVector.x = dir * 2;
							action.ShengLongBa ();
							playerState.SetState (PLAYERSTATE.JUMPING);
						}
					} else if (Input.GetKey (Up) && Input.GetKeyDown (PunchKey)) {
						if (checkStamina (10)) {
							action.verticalVelocity = 10;
							action.moveVector.x = dir * 2;
							action.ShengLongBa ();
							playerState.SetState (PLAYERSTATE.JUMPING);
						}
					} else if (Input.GetKeyDown (Up)) {

						if ((Time.time - upLastTapTime) < tapSpeed) {
							action.verticalVelocity = 2;
							action.moveVector.x = 0;
							action.moveVector.z = inputDirection_z * speed;
							anim.Jump ();
							playerState.SetState (PLAYERSTATE.JUMPING);
						} else {
							upLastTapTime = Time.time;
						}

					} else if (Input.GetKeyDown (Down)) {
						if ((Time.time - downLastTapTime) < tapSpeed) {
							action.verticalVelocity = 2;
							action.moveVector.x = 0;
							action.moveVector.z = inputDirection_z * speed;
							anim.Jump ();
							playerState.SetState (PLAYERSTATE.JUMPING);
						} else {
							downLastTapTime = Time.time;
						}
					
					} else if (Input.GetKeyDown (Left)) {
						if (playerState.currentState == PLAYERSTATE.MOVING || playerState.currentState == PLAYERSTATE.IDLE) {
							if ((Time.time - leftLastTapTime) < tapSpeed) {
								action.verticalVelocity = 2;
								action.moveVector.x = inputDirection_x * speed;
								action.moveVector.z = 0;
								anim.Jump ();
								playerState.SetState (PLAYERSTATE.JUMPING);
							} else {
								leftLastTapTime = Time.time;
							}
						}

						if (dir == 1) {
							flip = false;
						}
					
					} else if (Input.GetKeyDown (Right)) {
						if ((Time.time - rightLastTapTime) < tapSpeed) {
							action.verticalVelocity = 2;
							action.moveVector.x = inputDirection_x * speed;
							action.moveVector.z = 0;
							anim.Jump ();
						} else {
							rightLastTapTime = Time.time;
						}
						if (dir == -1) {
							flip = false;
						}

					} else { 
						// should probs change to 式 animation
						anim.Idle ();
						playerState.SetState (PLAYERSTATE.IDLE);

					}
				} else {
					IdleWalkInputManagement (dir);
				}

			}



		} else {
			action.verticalVelocity -= gravity;


			if (playerState.currentState == PLAYERSTATE.HIT || playerState.currentState == PLAYERSTATE.KNOCKBACK) {
			} else {
				action.moveVector.x = lastMotion.x;
				action.moveVector.z = lastMotion.z;
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

		action.moveVector.y = action.verticalVelocity;
		action.move (flip);
		lastMotion = action.moveVector;

		staminaBar.GetComponentsInChildren<StaminaBar> ()[0].UpdateBar (stamina.CurrentStamina, stamina.MaxStamina);

	}

	bool checkStamina (int staminaRequired) {
		if (stamina.CurrentStamina > staminaRequired) {
			stamina.SubstractStamina (staminaRequired);
			return true;
		}
		return false;
	}

	void FixedUpdate () {
		if (doPunch) {
			if (checkStamina (10)) {
				action.DoPunch ();
			}
		} else if (continuePunch) {
			if (checkStamina (10)) {
				action.ContinuePunch ();
			}
		} else if (doJumpKick) {
			if (checkStamina (10)) {
				action.DoJumpKick ();
			}
		} else if (doSprintPunch) {
			action.DoSprintPunch ();
		}
		doPunch = false;
		continuePunch = false;
		doJumpKick = false;
		doSprintPunch = false;
	}

	private void IdleWalkInputManagement(int dir) {
		if (Input.GetKeyDown (JumpKey)) {
			action.verticalVelocity = 10;
			action.moveVector.x = inputDirection_x;
			action.moveVector.z = inputDirection_z;
			anim.Jump ();
			playerState.SetState (PLAYERSTATE.JUMPING);
		} else if (Input.GetKeyDown (PunchKey)) { // remove combo here rn. This means that if you didnt press attack during attack animation you dont get to combo. might change
			// two method: 1) add lastAttackTime and allow combo if within time. 2) make attack animation include idle for a while (not recommended because you can't do anything else while punching)
			doPunch = true;
		} else if (Input.GetKeyDown (DefendKey)) {
			anim.StartDefend ();
			playerState.SetState (PLAYERSTATE.DEFENDING);
		} else if (Input.GetKeyDown (Left)) {

			if ((Time.time - leftLastTapTime) < tapSpeed) {
				anim.Sprint ();
				playerState.SetState (PLAYERSTATE.SPRINTING);
				inputDirection_x = dir * speed * 2;
				action.moveVector.x = inputDirection_x;
				action.moveVector.z = inputDirection_z;
			} else {
				leftLastTapTime = Time.time;
			}

		} else if (Input.GetKeyDown (Right)) {
			if ((Time.time - rightLastTapTime) < tapSpeed) {
				anim.Sprint ();
				playerState.SetState (PLAYERSTATE.SPRINTING);
				inputDirection_x = dir * speed * 2;
				action.moveVector.x = inputDirection_x;
				action.moveVector.z = inputDirection_z;
			} else {
				rightLastTapTime = Time.time;
			}
		} else if (inputDirection_x == 0 && inputDirection_z == 0) {
			anim.Idle ();
			playerState.SetState (PLAYERSTATE.IDLE);
		} else {
			anim.Walk ();
			playerState.SetState (PLAYERSTATE.MOVING);
			action.moveVector.x = inputDirection_x;
			action.moveVector.z = inputDirection_z;
		}
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





//Different input managment style
//if (Input.GetKeyDown (PunchKey)) {
//	if (playerState.currentState == PLAYERSTATE.SPRINTING) {
//		if (checkStamina (20)) {
//			doSprintPunch = true;
//			playerState.SetState (PLAYERSTATE.SPRINTPUNCH);
//		}
//	} else if (playerState.currentState == PLAYERSTATE.PUNCH) {
//		continuePunch = true;
//	} else if (playerState.currentState == PLAYERSTATE.MOVING || playerState.currentState == PLAYERSTATE.IDLE) {
//		doPunch = true;
//
//	}
//} else if (Input.GetKeyDown (JumpKey)) {
//	if (playerState.currentState == PLAYERSTATE.MOVING || playerState.currentState == PLAYERSTATE.IDLE || playerState.currentState == PLAYERSTATE.SPRINTING) {
//		action.verticalVelocity = 10;
//		action.moveVector.x = inputDirection_x;
//		action.moveVector.z = inputDirection_z;
//		anim.Jump ();
//		playerState.SetState (PLAYERSTATE.JUMPING);
//	} else if (playerState.currentState == PLAYERSTATE.SPRINTING) { // change later
//
//	} else if (playerState.currentState == PLAYERSTATE.KNOCKBACK) { // change later
//
//	}
//
//} else if (Input.GetKeyDown (DefendKey)) {
//	if (playerState.currentState == PLAYERSTATE.MOVING || playerState.currentState == PLAYERSTATE.IDLE) {
//		anim.StartDefend ();
//		playerState.SetState (PLAYERSTATE.DEFENDING);
//	}
//
//} else if (Input.GetKeyDown (Left)) {
//	if (playerState.currentState == PLAYERSTATE.MOVING || playerState.currentState == PLAYERSTATE.IDLE) {
//		if ((Time.time - leftLastTapTime) < tapSpeed) {
//			anim.Sprint ();
//			playerState.SetState (PLAYERSTATE.SPRINTING);
//			inputDirection_x = dir * speed * 2;
//			action.moveVector.x = inputDirection_x;
//			action.moveVector.z = inputDirection_z;
//		} else {
//			leftLastTapTime = Time.time;
//		}
//	}
//
//} else if (Input.GetKeyDown (Right)) {
//	if (playerState.currentState == PLAYERSTATE.MOVING || playerState.currentState == PLAYERSTATE.IDLE) {
//		if ((Time.time - rightLastTapTime) < tapSpeed) {
//			anim.Sprint ();
//			playerState.SetState (PLAYERSTATE.SPRINTING);
//			inputDirection_x = dir * speed * 2;
//			action.moveVector.x = inputDirection_x;
//			action.moveVector.z = inputDirection_z;
//		} else {
//			rightLastTapTime = Time.time;
//		}
//	}
//} else if (inputDirection_x == 0 && inputDirection_z == 0) {
//	if (playerState.currentState == PLAYERSTATE.MOVING || playerState.currentState == PLAYERSTATE.IDLE) {
//		if (playerState.currentState != PLAYERSTATE.PUNCH && playerState.currentState != PLAYERSTATE.SPRINTING) {
//			anim.Idle ();
//			playerState.SetState (PLAYERSTATE.IDLE);
//		}
//	}
//} else { //MOVING or SPRINTING
//	if (playerState.currentState == PLAYERSTATE.MOVING || playerState.currentState == PLAYERSTATE.IDLE) {
//		anim.Walk ();
//		playerState.SetState (PLAYERSTATE.MOVING);
//		action.moveVector.x = inputDirection_x;
//		action.moveVector.z = inputDirection_z;
//	} else if (playerState.currentState == PLAYERSTATE.SPRINTING) {
//		inputDirection_x = dir * speed * 2;
//		action.moveVector.x = inputDirection_x;
//		action.moveVector.z = inputDirection_z;
//	}
//}
