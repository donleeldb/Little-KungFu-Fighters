using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class PlayerAnimator : MonoBehaviour {

	private Animator animator;
	private Action action;

	void Awake() {
		animator = GetComponent<Animator> ();
		action = GetComponentsInParent<Action> ()[0];
	}

	public void Idle() {
		animator.SetTrigger ("Idle");
		animator.SetBool("Walk", false);
	}

	public void Walk() {
		animator.SetBool("Walk", true);
		animator.ResetTrigger("Attack1");
		animator.ResetTrigger("Attack2");
	}

	public void Sprint() {
		animator.SetTrigger ("Sprint");
		animator.ResetTrigger("Idle");
	}

	public void Attack(int id) {
		animator.SetBool("Walk", false);
		animator.SetTrigger ("Attack" + id);
		animator.ResetTrigger("Idle");
		StartCoroutine (WaitForAnimationFinish ("PlayerAttack" + id));
	}

	public void Stagger() {
		animator.SetBool("Walk", false);
		animator.SetTrigger ("Stagger");
		animator.ResetTrigger("Idle");
		StartCoroutine (WaitForAnimationFinish ("PlayerStagger"));
	}

	public void JumpAttack() {
		animator.SetTrigger ("JumpAttack");
	}

	public void SprintAttack() {
		animator.SetTrigger ("SprintAttack");
		StartCoroutine (WaitForAnimationFinish ("PlayerSprintAttack"));
	}

	public void StartDefend() {		
		animator.SetBool("Walk", false);
		animator.SetBool ("Defend", true);
	}

	public void StopDefend() {		
		animator.SetBool ("Defend", false);
		animator.SetTrigger ("Idle");
	}

	public void Jump() {
		animator.ResetTrigger("JumpAttack");
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
		animator.ResetTrigger("JumpAttack");
	}

    public void HuXiangBa()
    {
        animator.SetTrigger("HuXiangBa");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("JumpAttack");
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

    public void Paralyzed()
    {
        animator.SetTrigger("Paralyzed");
        animator.ResetTrigger("Idle");
        StartCoroutine(WaitForAnimationFinish("Paralyzed"));
    }

	public void Staggered() {
		animator.ResetTrigger ("Idle");

		animator.SetBool ("Defend", false);
		animator.SetTrigger ("Staggered");
		StartCoroutine (WaitForAnimationFinish ("PlayerStaggered"));
	}

	//show hit effect
	public void ShowHitEffect() {
		GameObject.Instantiate (Resources.Load ("HitEffect"), transform.position, Quaternion.identity);
	}

    //show hit effect
    public void ShowStaggerEffect()
    {
        GameObject.Instantiate(Resources.Load("StaggerEffect"), transform.position, Quaternion.identity);
    }

    //show hit effect
    public void ShowParryEffect()
    {
        GameObject.Instantiate(Resources.Load("ParryEffect"), transform.position, Quaternion.identity);
    }

	//show defend effect
	public void ShowDefendEffect() {
//		Vector3 offset = Vector3.up * 1.7f + Vector3.right * (int)transform.parent.GetComponent<PlayerMovement> ().getCurrentDirection () * .2f;
		Vector3 offset = Vector3.zero;
		GameObject.Instantiate (Resources.Load ("DefendEffect"), transform.position + offset, Quaternion.identity);
	}

	//Show dust effect
	public void ShowDustEffect() {
		GameObject.Instantiate (Resources.Load ("SmokePuffEffect"), transform.position, Quaternion.identity);
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

		while (t < 1 || !controller.isGrounded) {
//			controller.Move (direction * Mathf.Lerp (force, 0, MathUtilities.Sinerp (0, 1, t)));
			controller.Move (direction * Mathf.Lerp (force, 0, 0.5f));

			t += Time.deltaTime * speed;
			yield return null;
		}

	}
}

