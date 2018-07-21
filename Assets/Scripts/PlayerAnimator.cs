using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class PlayerAnimator : MonoBehaviour {

	private Animator animator;

	void Awake() {
		animator = GetComponent<Animator> ();
	}

	public void Idle() {
		animator.SetTrigger ("Idle");
		animator.SetBool("Walk", false);
	}

	public void Walk() {
		animator.SetBool("Walk", true);
		animator.ResetTrigger("Punch1");
		animator.ResetTrigger("Punch2");
	}

	public void Sprint() {
		animator.SetTrigger ("Sprint");
		animator.ResetTrigger("Idle");
	}

	public void Punch(int id) {
		animator.SetBool("Walk", false);
		animator.SetTrigger ("Punch" + id);
		animator.ResetTrigger("Idle");
		StartCoroutine (WaitForAnimationFinish ("PlayerPunch" + id));
	}

	public void JumpKick() {
		animator.SetTrigger ("JumpKick");
	}

	public void SprintPunch() {
		animator.SetTrigger ("SprintAttack");
		StartCoroutine (WaitForAnimationFinish ("PlayerSprintAttack"));
	}

	public void StartDefend() {		
		animator.SetBool("Walk", false);
		animator.SetBool ("Defend", true);
	}

	public void StopDefend() {		
		animator.SetBool ("Defend", false);
	}

	public void Jump() {
		animator.ResetTrigger("JumpKick");
		animator.SetBool("Walk", false);
		animator.SetTrigger ("Jump");
		animator.ResetTrigger("Idle");
//		StartCoroutine (WaitForAnimationFinish ("Jump"));
	}

	public void Hit() {
		animator.SetTrigger ("Hit");
		StartCoroutine (WaitForAnimationFinish ("PlayerHit"));
	}

	public void ShengLongBa() {
		animator.SetTrigger ("ShengLongBa");
		animator.ResetTrigger("Idle");
		animator.ResetTrigger("JumpKick");
	}

	public void KnockBack() {
		animator.SetTrigger ("KnockBack");
		animator.ResetTrigger("Idle");
		animator.ResetTrigger("KnockDown");
		animator.SetBool ("Defend", false);
	}

	public void KnockDown() {
		animator.SetTrigger ("KnockDown");
		StartCoroutine (WaitForAnimationFinish ("PlayerKnockDown"));
	}

	//on animation finish
	IEnumerator WaitForAnimationFinish(string animName) {
		float time = GetAnimDuration(animName);
		yield return new WaitForSeconds(time);
		transform.parent.GetComponent<Action>().Ready(animName);
	}

	//returns the duration of an animation
	float GetAnimDuration(string animName) {
		RuntimeAnimatorController ac = animator.runtimeAnimatorController;
		for (int i = 0; i < ac.animationClips.Length; i++) {
			if (ac.animationClips [i].name == animName) {
				return ac.animationClips [i].length;
			}
		}
		print ("no animation found with name: " + animName);
		return 0f;
	}

	//adds a small forward force
	public void AddForce(float force, bool facingRight) {
		StartCoroutine (AddForceCoroutine(force, facingRight, false));
	}


	public void AddVerticalForce(float force, bool facingRight) {
		StartCoroutine (AddForceCoroutine(force, facingRight, true));
	}
	//adds small force over time
	IEnumerator AddForceCoroutine(float force, bool facingRight, bool vertical) {
		CharacterController controller = transform.parent.GetComponent<CharacterController> ();
		int dir = -1;
		if (facingRight) {
			dir = 1;
		}
		float speed = 2f;
		float t = 0;

		Vector3 direction = Vector3.right * dir;

		if (vertical) {
			direction = Vector3.up;
		}

		while (t < 1) {

			controller.Move (direction * Mathf.Lerp (force, 0, MathUtilities.Sinerp (0, 1, t)));
			t += Time.deltaTime * speed;
			yield return null;
		}
	}
}

