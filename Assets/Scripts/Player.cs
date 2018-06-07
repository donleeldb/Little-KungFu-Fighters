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
		// TODO: change Vertical to sth else
		inputDirection_z = Input.GetAxis ("Depth") * speed;

		if (Input.GetKeyUp (KeyCode.L)) {
			anim.StopDefend ();
		}

		print (verticalVelocity);
		if (controller.isGrounded) { // not reliable
//		if (IsControllerGrounded()) { // mine is bugged

			if (inputDirection_x == 0 && inputDirection_z == 0) {
				anim.Idle ();
				playerState.SetState (PLAYERSTATE.IDLE);
			} else {
				anim.Walk();
				playerState.SetState(PLAYERSTATE.MOVING);
			}
				


//			verticalVelocity = 0; // tutorial has this. But vertical velocity is reset in the else statement
			if (Input.GetKeyDown (KeyCode.K)) {
				verticalVelocity = 15;
				anim.Jump ();
				playerState.SetState (PLAYERSTATE.JUMPING);
			} else if (Input.GetKeyDown (KeyCode.J)) {
				anim.Punch (0);
				playerState.SetState (PLAYERSTATE.PUNCH);
			} else if (Input.GetKeyDown (KeyCode.L)) {
				anim.StartDefend ();
				playerState.SetState (PLAYERSTATE.DEFENDING);
			} else {
				moveVector.x = inputDirection_x;
				moveVector.z = inputDirection_z;
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
		anim.Idle ();
		playerState.SetState (PLAYERSTATE.IDLE);

	}

}
