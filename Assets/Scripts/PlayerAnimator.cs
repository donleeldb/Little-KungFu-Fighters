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
	}

	public void Punch(int id) {
		animator.SetTrigger ("Punch" + id);
		animator.ResetTrigger("Idle");
		StartCoroutine (WaitForAnimationFinish ("PlayerPunch" + id));
	}

	public void StartDefend() {		
		animator.SetBool("Walk", false);
		animator.SetBool ("Defend", true);
	}

	public void StopDefend() {		
		animator.SetBool ("Defend", false);
	}

	public void Jump() {
		animator.SetBool("Walk", false);
		animator.SetTrigger ("Jump");
		animator.ResetTrigger("Idle");
//		StartCoroutine (WaitForAnimationFinish ("Jump"));
	}

	//on animation finish
	IEnumerator WaitForAnimationFinish(string animName) {
		float time = GetAnimDuration(animName);
		yield return new WaitForSeconds(time);
		transform.parent.GetComponent<Player>().Ready();
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
}

