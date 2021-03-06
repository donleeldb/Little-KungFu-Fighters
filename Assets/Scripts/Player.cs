﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private float inputDirection_x;
	private float inputDirection_z;

	private float gravity = 0.75f;
	private float speed = 5.0f;

	public float tapSpeed = 1f; //in seconds. seems weird tho. 1s should be very long. but still doesn't register sometimes, could be editor problems
	private bool flip;
	private float leftLastTapTime = 0;
	private float rightLastTapTime = 0;
	private float upLastTapTime = 0;
	private float downLastTapTime = 0;


	public bool facingRight;
	private bool continueAttack = false;
	private bool doAttack = false;
	private bool doJumpAttack = false;
	private bool doSprintAttack = false;
	private bool doStagger = false;
    private bool doParalyze = false;

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
	public KeyCode AttackKey;
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
		//print (playerState.currentState);

		stamina.AddStamina (0.1f);

        //waitforchangeofspeed might have changed it b4 update
        if (playerState.currentState != PLAYERSTATE.NOTIDLE && playerState.currentState != PLAYERSTATE.SHENGLONGBA) {
            action.moveVector = Vector3.zero;
        }

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
            // State determines what you can do. attacktype determines how you might react 
            if (playerState.currentState == PLAYERSTATE.KNOCKDOWN || 
                playerState.currentState == PLAYERSTATE.HIT || 
                playerState.currentState == PLAYERSTATE.STAGGERED || 
                playerState.currentState == PLAYERSTATE.STAGGER || 
                playerState.currentState == PLAYERSTATE.PARALYZED ||
                playerState.currentState == PLAYERSTATE.NOTIDLE ) { //dont want them to turn back to idle
				
			} else if (playerState.currentState == PLAYERSTATE.KNOCKBACK){
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
				} else if (Input.GetKeyDown (AttackKey)) {
					doSprintAttack = true;
				} else if (Input.GetKeyDown (JumpKey)) {
                    action.DoJump(inputDirection_x, inputDirection_z);
				} else {
					inputDirection_x = dir * speed * 2;
					action.moveVector.x = inputDirection_x;
					action.moveVector.z = inputDirection_z;
				}
					
			} else if (playerState.currentState == PLAYERSTATE.ATTACK) { // when attacking, can't do anything else except for keep attacking
				if (Input.GetKey (式Key)) {
                    listenForAttackComboInput(dir);
				} else if (Input.GetKeyDown (AttackKey)) {
					continueAttack = true;
				}

			} else if (playerState.currentState == PLAYERSTATE.SPRINTATTACK) {
				inputDirection_x = dir * speed;
				action.moveVector.x = inputDirection_x;
            }
            else
            { // right now is idle, walk

                IdleWalkInputListner(dir);
            }



		} else {
			action.verticalVelocity -= gravity;


			if (playerState.currentState == PLAYERSTATE.HIT || playerState.currentState == PLAYERSTATE.KNOCKBACK) {
            } else if (playerState.currentState == PLAYERSTATE.NOTIDLE) {
                playerState.SetState(PLAYERSTATE.SHENGLONGBA);
   
            }
            else {
				
                action.moveVector.x = lastMotion.x;
				action.moveVector.z = lastMotion.z;
				if (playerState.currentState != PLAYERSTATE.JUMPATTACK && Input.GetKeyDown (AttackKey)) { 
					doJumpAttack = true;
				}

			}
		}


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
		if (doAttack) {
			if (checkStamina (10)) {
				action.DoAttack ();
			}
		} else if (continueAttack) {
			if (checkStamina (10)) {
				action.ContinueAttack ();
			}
		} else if (doJumpAttack) {
			if (checkStamina (10)) {
				action.DoJumpAttack ();
			}
		} else if (doSprintAttack) {
			if (checkStamina (10)) {
				action.DoSprintAttack ();
			}
		} else if (doStagger) {
			if (checkStamina (10)) {
				action.DoStagger ();
			}
        } else if (doParalyze) {
            if (checkStamina(10))
            {
                action.DoParalyze();
            }
        }
		doAttack = false;
		continueAttack = false;
		doJumpAttack = false;
		doSprintAttack = false;
		doStagger = false;
        doParalyze = false;
	}


    private bool listenForMoveComboInput(int dir)
    {
        if (Input.GetKeyDown(Up))
        {

            if ((Time.time - upLastTapTime) < tapSpeed)
            {
                action.DoSideStep(speed, inputDirection_z);
            }
            else
            {
                upLastTapTime = Time.time;
            }
            return true;
        }
        else if (Input.GetKeyDown(Down))
        {
            if ((Time.time - downLastTapTime) < tapSpeed)
            {
                action.DoSideStep(speed, inputDirection_z);
            }
            else
            {
                downLastTapTime = Time.time;
            }
            return true;
        }
        else if (Input.GetKeyDown(Left))
        {
            if (playerState.currentState == PLAYERSTATE.WALKING || playerState.currentState == PLAYERSTATE.IDLE)
            {
                if ((Time.time - leftLastTapTime) < tapSpeed)
                {
                    action.DoVerticalStep(speed, inputDirection_x);
                }
                else
                {
                    leftLastTapTime = Time.time;
                }
            }

            if (dir == 1)
            {
                flip = false;
            }
            return true;
        }
        else if (Input.GetKeyDown(Right))
        {
            if ((Time.time - rightLastTapTime) < tapSpeed)
            {
                action.DoVerticalStep(speed, inputDirection_x);

            }
            else
            {
                rightLastTapTime = Time.time;
            }
            if (dir == -1)
            {
                flip = false;
            }
            return true;
        }

        return false;
    }

    private bool listenForAttackComboInput(int dir)
    {

        //if (Input.GetKey(AttackKey) && Input.GetKeyDown(Up))
        if (Input.GetKey(KeyCode.F))
        {
            if (checkStamina(10))
            {
                action.ShengLongBa(dir);
            }
            return true;
        }
        else if (Input.GetKey(Up) && Input.GetKeyDown(AttackKey))
        {
            if (checkStamina(10))
            {
                action.ShengLongBa(dir);
            }
            return true;
        //} else if (Input.GetKey(AttackKey) && Input.GetKeyDown(Down))
        } else if (Input.GetKey(KeyCode.G))
        {
            if (checkStamina(10))
            {
                action.HuXiangBa(dir);
            }
            return true;
        }
        else if (Input.GetKey(Down) && Input.GetKeyDown(AttackKey))
        {
            if (checkStamina(10))
            {
                action.HuXiangBa(dir);
            }
            return true;
        } else if (Input.GetKey (Up) && Input.GetKeyDown(DefendKey)) {
            if (checkStamina (10)) {
                doStagger = true;
            }
            return true;
        } else if (Input.GetKey(DefendKey) && Input.GetKeyDown(Up)) {
            if (checkStamina(10))
            {
                doStagger = true;
            }
            return true;
        } else if (Input.GetKey(Down) && Input.GetKeyDown(DefendKey))
        {
            if (checkStamina(10))
            {
                doParalyze = true;
            }
            return true;
        }
        else if (Input.GetKey(DefendKey) && Input.GetKeyDown(Down))
        {
            if (checkStamina(10))
            {
                doParalyze = true;
            }
            return true;
        }

        return false;

    }

    private void IdleWalkInputListner (int dir) {

        if (Input.GetKey(式Key))
        {
            if (listenForAttackComboInput(dir) || listenForMoveComboInput(dir)) {
            } else {
                anim.Idle();
                playerState.SetState(PLAYERSTATE.IDLE);
            }
        } else {

            if (Input.GetKeyDown(JumpKey))
            {
                action.DoJump(inputDirection_x, inputDirection_z);
            }
            else if (Input.GetKeyDown(AttackKey))
            { // remove combo here rn. This means that if you didnt press attack during attack animation you dont get to combo. might change
              // two method: 1) add lastAttackTime and allow combo if within time. 2) make attack animation include idle for a while (not recommended because you can't do anything else while punching)
                doAttack = true;
            }
            else if (Input.GetKeyDown(DefendKey))
            {
                anim.StartDefend();
                playerState.SetState(PLAYERSTATE.DEFENDING);
            }
            else if (Input.GetKeyDown(Left))
            {
                if ((Time.time - leftLastTapTime) < tapSpeed)
                {
                    action.SprintLeft(speed, dir, inputDirection_x, inputDirection_z);
                }
                else
                {
                    leftLastTapTime = Time.time;
                }
            }
            else if (Input.GetKeyDown(Right))
            {
                if ((Time.time - rightLastTapTime) < tapSpeed)
                {
                    action.SprintRight(speed, dir, inputDirection_x, inputDirection_z);
                }
                else
                {
                    rightLastTapTime = Time.time;
                }
            }
            else if (inputDirection_x == 0 && inputDirection_z == 0)
            {
                anim.Idle();
                playerState.SetState(PLAYERSTATE.IDLE);
            }
            else
            {
                anim.Walk();
                playerState.SetState(PLAYERSTATE.WALKING);
                action.moveVector.x = inputDirection_x;
                action.moveVector.z = inputDirection_z;
            }
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
//if (Input.GetKeyDown (AttackKey)) {
//	if (playerState.currentState == PLAYERSTATE.SPRINTING) {
//		if (checkStamina (20)) {
//			doSprintAttack = true;
//			playerState.SetState (PLAYERSTATE.SPRINTATTACK);
//		}
//	} else if (playerState.currentState == PLAYERSTATE.ATTACK) {
//		continueAttack = true;
//	} else if (playerState.currentState == PLAYERSTATE.WALKING || playerState.currentState == PLAYERSTATE.IDLE) {
//		doAttack = true;
//
//	}
//} else if (Input.GetKeyDown (JumpKey)) {
//	if (playerState.currentState == PLAYERSTATE.WALKING || playerState.currentState == PLAYERSTATE.IDLE || playerState.currentState == PLAYERSTATE.SPRINTING) {
//		action.Velocity = 10;
//		action.moveVector.x = inputDirection_x;
//		action.moveVector.z = inputDirectioverticaln_z;
//		anim.Jump ();
//		playerState.SetState (PLAYERSTATE.JUMPING);
//	} else if (playerState.currentState == PLAYERSTATE.SPRINTING) { // change later
//
//	} else if (playerState.currentState == PLAYERSTATE.KNOCKBACK) { // change later
//
//	}
//
//} else if (Input.GetKeyDown (DefendKey)) {
//	if (playerState.currentState == PLAYERSTATE.WALKING || playerState.currentState == PLAYERSTATE.IDLE) {
//		anim.StartDefend ();
//		playerState.SetState (PLAYERSTATE.DEFENDING);
//	}
//
//} else if (Input.GetKeyDown (Left)) {
//	if (playerState.currentState == PLAYERSTATE.WALKING || playerState.currentState == PLAYERSTATE.IDLE) {
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
//	if (playerState.currentState == PLAYERSTATE.WALKING || playerState.currentState == PLAYERSTATE.IDLE) {
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
//	if (playerState.currentState == PLAYERSTATE.WALKING || playerState.currentState == PLAYERSTATE.IDLE) {
//		if (playerState.currentState != PLAYERSTATE.ATTACK && playerState.currentState != PLAYERSTATE.SPRINTING) {
//			anim.Idle ();
//			playerState.SetState (PLAYERSTATE.IDLE);
//		}
//	}
//} else { //WALKING or SPRINTING
//	if (playerState.currentState == PLAYERSTATE.WALKING || playerState.currentState == PLAYERSTATE.IDLE) {
//		anim.Walk ();
//		playerState.SetState (PLAYERSTATE.WALKING);
//		action.moveVector.x = inputDirection_x;
//		action.moveVector.z = inputDirection_z;
//	} else if (playerState.currentState == PLAYERSTATE.SPRINTING) {
//		inputDirection_x = dir * speed * 2;
//		action.moveVector.x = inputDirection_x;
//		action.moveVector.z = inputDirection_z;
//	}
//}
