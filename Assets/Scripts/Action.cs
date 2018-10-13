using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour {

	public bool facingRight;

	public Vector3 moveVector;
	private Vector3 lastMotion;
	private CharacterController controller;

	public PlayerState playerState;
	private PlayerAnimator anim;
	private Health health;
	private Stamina stamina;

	private int attackNum = 0; //the current attack number
	private bool continueAttackCombo; //true if a punch combo needs to continue
	private float LastAttackTime = 0;
	private bool targetHit; //true if the last hit has hit a target

	private int HitKnockDownThreshold = 3; //the number of times the player can be hit before being knocked down
	private int HitKnockDownCount = 0; //the number of times the player is hit in a row
	private int HitKnockDownResetTime = 1; //the time before the hitknockdown counter resets
	private float LastHitTime = 0; // the time when we were hit 

	private int DefenseThreshold = 3; 
	private int DefenseCount = 0; 
	private int DefenseResetTime = 1; 
	private float LastDefenseTime = 0;

    public GameObject attackHitBox;
    public GameObject specialHitBox;
    public GameObject specialHitBox1;
	public float verticalVelocity;

    LayerMask npcLayerMask;
    LayerMask playerLayerMask;
    LayerMask attackLayerMask;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		anim = GetComponentInChildren<PlayerAnimator> ();
		playerState = GetComponent<PlayerState> ();
		health = GetComponent<Health> ();
		stamina = GetComponent<Stamina> ();
		facingRight = true;


        npcLayerMask = LayerMask.NameToLayer("NPC");
        playerLayerMask = LayerMask.NameToLayer("Player");
        attackLayerMask = LayerMask.NameToLayer("Attack");
	}

	public void move(bool flip = true) {
		moveVector.y = verticalVelocity;
		controller.Move (moveVector * Time.deltaTime);
		if (flip) {
			Flip (moveVector.x);

		}
	}

	public void StopDefend() {
		anim.StopDefend ();
		playerState.SetState (PLAYERSTATE.IDLE);
	}

	public void StartDefend() {
		anim.StartDefend ();
		playerState.SetState (PLAYERSTATE.DEFENDING);
	}

	public void ContinueAttack() {
		
		continueAttackCombo = true;
		if (attackNum == 2)
			continueAttackCombo = false;
	}

	public void DoAttack() {
		playerState.SetState (PLAYERSTATE.ATTACK);
		anim.Attack (0);
		DamageObject d = new DamageObject (20, this.gameObject, 0.5f, Vector3.zero, 0.005f);
		CheckForHit (d);
	}

	public void DoStagger() {
		playerState.SetState (PLAYERSTATE.STAGGER);
		anim.Stagger ();
		DamageObject d = new DamageObject (20, this.gameObject, 0.5f, Vector3.zero, 0.005f);
		d.attackType = AttackType.Stagger;
		CheckForHit (d);
	}

    public void DoParalyze()
    {
        playerState.SetState(PLAYERSTATE.STAGGER);
        anim.Stagger();
        DamageObject d = new DamageObject(20, this.gameObject, 0.5f, Vector3.zero, 0.005f);
        d.attackType = AttackType.Paralyze;
        CheckForHit(d);
    }

	public void DoJumpAttack() {
		playerState.SetState (PLAYERSTATE.JUMPATTACK);
		anim.JumpAttack ();
		DamageObject d = new DamageObject (20, this.gameObject, 0.5f, Vector3.zero, 0.01f);
		d.attackType = AttackType.KnockDown;
		CheckForHit (d);
	}

    public void DoJump(float inputDirection_x, float inputDirection_z)
    {
        verticalVelocity = 10;
        moveVector.x = inputDirection_x;
        moveVector.z = inputDirection_z;
        anim.Jump();
        playerState.SetState(PLAYERSTATE.JUMPING);
    }

    public void DoSideStep(float speed, float inputDirection_z)
    {
        verticalVelocity = 2;
        moveVector.x = 0;
        moveVector.z = inputDirection_z * speed;
        anim.Jump();
        playerState.SetState(PLAYERSTATE.JUMPING);
    }

    public void DoVerticalStep(float speed, float inputDirection_x)
    {
        verticalVelocity = 2;
        moveVector.x = inputDirection_x * speed;
        moveVector.z = 0;
        anim.Jump();
        playerState.SetState(PLAYERSTATE.JUMPING);
    }

	public void DoSprintAttack() {
		playerState.SetState (PLAYERSTATE.SPRINTATTACK);
		anim.SprintAttack ();
		DamageObject d = new DamageObject (20, this.gameObject, 1.5f, Vector3.zero, 0.01f);
		d.attackType = AttackType.KnockDown;
		CheckForHit (d);
	}


	public void ShengLongBa(int dir) {

        GameObject slb = Instantiate(Resources.Load("Moves/ShengLongBa"), transform.position, Quaternion.identity) as GameObject;
        slb.transform.parent = transform;

        // need to somehow find attackObject without using the name
        Attack attackObject = slb.GetComponent<ShengLongBa>();

        attackObject.Execute(anim, dir);

	}

    public void HuXiangBa(int dir)
    {
        GameObject hxb = Instantiate(Resources.Load("Moves/HuXiangBa"), transform.position, Quaternion.identity) as GameObject;
        hxb.transform.parent = transform;

        Attack attackObject = hxb.GetComponent<HuXiangBa>();

        attackObject.Execute(anim, dir);

    }

    public void SprintLeft(float speed, int dir, float inputDirection_x, float inputDirection_z)
    {
        anim.Sprint();
        playerState.SetState(PLAYERSTATE.SPRINTING);
        inputDirection_x = dir * speed * 2;
        moveVector.x = inputDirection_x;
        moveVector.z = inputDirection_z;
    }

    public void SprintRight(float speed, int dir, float inputDirection_x, float inputDirection_z)
    {
        anim.Sprint();
        playerState.SetState(PLAYERSTATE.SPRINTING);
        inputDirection_x = dir * speed * 2;
        moveVector.x = inputDirection_x;
        moveVector.z = inputDirection_z;
    }
		
	public void getHit(DamageObject d, int fixedDir = 0) {

		bool wasHit = true;

		if (playerState.currentState == PLAYERSTATE.KNOCKDOWN) {
			wasHit = false;
		}

		// HOTFIX for addForce making character not grounded.
		if (controller.isGrounded == false && playerState.currentState != PLAYERSTATE.HIT && playerState.currentState != PLAYERSTATE.DEFENDING) {
			d.attackType = AttackType.KnockDown; 
			HitKnockDownCount = 0;
		}

		//defend
		if(playerState.currentState == PLAYERSTATE.DEFENDING){
			wasHit = false;
			UpdateDefenseCounter ();

            if (d.attackType == AttackType.Stagger || d.attackType == AttackType.KnockDown) {
				wasHit = true;
				anim.Staggered ();
				playerState.SetState (PLAYERSTATE.STAGGERED);
                anim.ShowStaggerEffect();
                anim.AddForce(0.005f, d.inflictor.GetComponent<Action>().facingRight);
				DefenseCount = 0;
				return;

			} else if (DefenseCount >= DefenseThreshold) { 
                anim.Staggered();
                playerState.SetState(PLAYERSTATE.STAGGERED);
                anim.ShowStaggerEffect();
                anim.AddForce(0.005f, d.inflictor.GetComponent<Action>().facingRight);

                DefenseCount = 0;
                return;
			} else {
//			if(BlockAttacksFromBehind || isFacingTarget (d.inflictor)) wasHit = false;
//			if(!wasHit){
////				GlobalAudioPlayer.PlaySFX ("Defend");
////				anim.ShowDefendEffect();
//
				anim.ShowDefendEffect();
                anim.AddForce(0.005f, d.inflictor.GetComponent<Action>().facingRight);

//			}
			}

		}

		//parry //need to add math for different types of attack
		//if (playerState.currentState == PLAYERSTATE.ATTACK) {
  //          anim.ShowParryEffect();
		//	wasHit = false;
		//}

		if (wasHit) {
			UpdateHitCounter ();
		}

		if (HitKnockDownCount >= HitKnockDownThreshold) { 
			d.attackType = AttackType.KnockDown; 
			HitKnockDownCount = 0;
		}
			

		//start knockDown sequence
		if (wasHit && playerState.currentState != PLAYERSTATE.KNOCKDOWN) {
//			GetComponent<HealthSystem> ().SubstractHealth (d.damage);
			anim.ShowHitEffect ();

			moveVector.x = 0;
			moveVector.z = 0;
			verticalVelocity = 0;

            if (d.attackType == AttackType.Paralyze)
            {
                playerState.SetState(PLAYERSTATE.PARALYZED);
                anim.Paralyzed();

            } else if (d.attackType == AttackType.KnockDown) {
				playerState.SetState (PLAYERSTATE.KNOCKBACK);
				KnockBack (d.inflictor);

                verticalVelocity = 5f;
                if (d.verticalForce != 0f)
                    verticalVelocity = d.verticalForce;

                anim.AddForce(d.force * 20, d.inflictor.GetComponent<Action>().facingRight);

			} else {
				playerState.SetState (PLAYERSTATE.HIT);
				anim.Hit ();
                anim.AddForce(d.force, d.inflictor.GetComponent<Action>().facingRight);
			}
		}
	}

	//updates the hit counter
	private void UpdateHitCounter() {
		if (Time.time - LastHitTime < HitKnockDownResetTime) { 
			HitKnockDownCount += 1;
		} else {
			HitKnockDownCount = 1;
		}
		LastHitTime = Time.time;
	}

	//updates the Defense counter
	private void UpdateDefenseCounter() {
		if (Time.time - LastDefenseTime < DefenseResetTime) { 
			DefenseCount += 1;
		} else {
			DefenseCount = 1;
		}
		LastDefenseTime = Time.time;
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

	public void Ready(string animName) {

        if (playerState.currentState == PLAYERSTATE.KNOCKDOWN || playerState.currentState == PLAYERSTATE.KNOCKBACK || playerState.currentState == PLAYERSTATE.PARALYZED) {
			if (animName == "PlayerKnockDown" || animName == "PlayerParalyzed") {
//				controller.enabled = true;
				anim.Idle ();
				playerState.SetState (PLAYERSTATE.IDLE);
			} else { //Hit's ready call, do nothing if knocked down.
				return;
			}

		} else if (playerState.currentState == PLAYERSTATE.ATTACK && (animName == "PlayerAttack0" || animName == "PlayerAttack1" || animName == "PlayerAttack2")) {
			if (continueAttackCombo) {
				DamageObject d = new DamageObject (20, this.gameObject, 0.3f, Vector3.zero, 0.005f);
				CheckForHit (d);
				doAttack ();
				continueAttackCombo = false;

			} else {
				attackNum = 0;
				anim.Idle ();
				playerState.SetState (PLAYERSTATE.IDLE);
			}
		} else if ((playerState.currentState == PLAYERSTATE.STAGGERED && animName == "PlayerStaggered") || 
					(playerState.currentState == PLAYERSTATE.HIT && animName == "PlayerHit") ||
					(playerState.currentState == PLAYERSTATE.STAGGER && animName == "PlayerStagger" ) ||
                   (playerState.currentState == PLAYERSTATE.SPRINTATTACK && animName == "PlayerSprintAttack")) {
			anim.Idle ();
			playerState.SetState (PLAYERSTATE.IDLE);
		}


	

	}

	//returns the next attack number in the combo chain
	private int GetNextAttackNum() {
		if (playerState.currentState == PLAYERSTATE.ATTACK) {

			attackNum = Mathf.Clamp (attackNum + 1, 0, 2); // it's not clamping but if num greater than 2 animator fails.

			return attackNum;

		} 
		return 0;
	}

	//do a punch attack
	private void doAttack() {
		playerState.SetState (PLAYERSTATE.ATTACK);
		anim.Attack (GetNextAttackNum ());
		if (attackNum == 2)
			continueAttackCombo = false;
		//		LastAttackTime = Time.time; // not using this rn
	}

	//checks if we have hit something (animation event)
	private void CheckForHit(DamageObject d) {
		int dir = -1;
		if (facingRight) {
			dir = 1;
		}

		float moveTime = 0.5f;

        //StartCoroutine (WaitBeforeRaycast (d, dir));
        transform.GetChild(3).gameObject.SetActive(true);
        //StartCoroutine(GetComponentInChildren<NormalAttack>().WaitBeforeCollide(d, dir));
        StartCoroutine(GetComponentInChildren<NormalAttack>().WaitBeforeCollide(d, dir));

	}

	//returns true is the player is facing the enemy
	public bool isFacingTarget(GameObject g) {
		int dir = -1;
		if (facingRight) {
			dir = 1;
		}		
		if ((g.transform.position.x > transform.position.x && dir == 1) || (g.transform.position.x < transform.position.x && dir == -1))
			return true;
		else
			return false;
	}

	public void KnockBack(GameObject inflictor) {
//		controller.enabled = false;
		anim.KnockBack ();
		float t = 0;
		float travelSpeed = 2f;
		Rigidbody rb = GetComponent<Rigidbody> ();

		//get the direction of the attack
		int dir = inflictor.transform.position.x > transform.position.x ? 1 : -1;

		//look towards the direction of the incoming attack (should I?)
//		GetComponent<Player>().facingRight = false;
//		if (dir == 1) {
//			GetComponent<Player>().facingRight = true;
//		}

	}

   

}
