using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private float inputDirection_x;
	private float inputDirection_z;

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

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		anim = GetComponentInChildren<PlayerAnimator> ();
		playerState = new PlayerState ();
		facingRight = true;
	}
	
	// Update is called once per frame
	void Update () {

		moveVector = Vector3.zero;

		inputDirection_x = Input.GetAxis ("Horizontal") * speed;
		inputDirection_z = Input.GetAxis ("Depth") * speed;

		if (Input.GetKeyUp (KeyCode.L)) {
			anim.StopDefend ();
			playerState.SetState (PLAYERSTATE.IDLE);
		}


		if (controller.isGrounded) { // not reliable
//		if (IsControllerGrounded()) { // mine is bugged

			// When defending, you can't do anything else
			if (playerState.currentState != PLAYERSTATE.DEFENDING) {
				
				//			verticalVelocity = 0; // tutorial has this. But vertical velocity is reset in the else statement

				if (Input.GetKeyDown (KeyCode.K)) {
					verticalVelocity = 15;
					anim.Jump ();
					playerState.SetState (PLAYERSTATE.JUMPING);
				} else if (Input.GetKeyDown (KeyCode.J)) {
					print (playerState.currentState);

					if (playerState.currentState != PLAYERSTATE.PUNCH) {
						playerState.SetState (PLAYERSTATE.PUNCH);
						anim.Punch (0);
					} else {
						print ("print combo true");
						continuePunchCombo = true;
						if (attackNum == 2)
							continuePunchCombo = false;
					}

				} else if (Input.GetKeyDown (KeyCode.L)) {
					anim.StartDefend ();
					playerState.SetState (PLAYERSTATE.DEFENDING);
				} else if (inputDirection_x == 0 && inputDirection_z == 0) {
					if (playerState.currentState != PLAYERSTATE.PUNCH) {
						anim.Idle ();
						playerState.SetState (PLAYERSTATE.IDLE);
					}
				} else {
					anim.Walk ();
					playerState.SetState (PLAYERSTATE.MOVING);
				}

				moveVector.x = inputDirection_x;
				moveVector.z = inputDirection_z;
			} else {
				if (Input.GetKeyDown (KeyCode.J) && Input.GetKeyDown(KeyCode.W)) {
					print("Tornado");
				}
			}

		} else {
			verticalVelocity -= gravity;
			moveVector.x = lastMotion.x;
			moveVector.z = lastMotion.z;
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

	public void Ready() {
		print("ready");
		print(playerState.currentState);
		if (continuePunchCombo) {
			doPunchAttack ();
			continuePunchCombo = false;
				
		} else {
			print ("shouldnt be here");
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
		LastAttackTime = Time.time;
	}

}
