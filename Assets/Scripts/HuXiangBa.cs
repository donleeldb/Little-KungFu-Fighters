using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuXiangBa : Attack {

    public float verticalVelocity;
    public float moveVectorX;
    public PLAYERSTATE state;

	// Use this for initialization
	void Start () {
        npcLayerMask = LayerMask.NameToLayer("NPC");
        playerLayerMask = LayerMask.NameToLayer("Player");
        attackLayerMask = LayerMask.NameToLayer("Attack");
        d = new DamageObject(20, AttackType.KnockDown, PowerType.Medium, this.gameObject.transform.parent.gameObject, 1f, Vector3.down, 0.005f, 10f);
        d.lag = 0f;

        verticalVelocity = 8;
        moveVectorX = 20;
        state = PLAYERSTATE.NOTIDLE;
	}

    public override void Execute(PlayerAnimator anim, int dir)
    {

        anim.HuXiangBa();
        transform.parent.GetComponent<Action>().playerState.SetState(state);
        StartCoroutine(waitBeforeChangeOfSpeed(1f, dir, 0.3f));
    }

    IEnumerator waitBeforeChangeOfSpeed(float ttl, int dir, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        transform.parent.GetComponent<Action>().verticalVelocity = verticalVelocity;
        transform.parent.GetComponent<Action>().moveVector.x = dir * moveVectorX;

        GetComponent<BoxCollider>().enabled = true;
        StartCoroutine(ColliderTimeToLive(GetComponent<BoxCollider>(), ttl));

    }

	private void OnTriggerEnter(Collider other)
	{
        int dir = -1;
        if (gameObject.transform.parent.GetComponent<Action>().facingRight)
        {
            dir = 1;
        }

        if (gameObject.transform.parent.GetComponent<PlayerState>().currentState == PLAYERSTATE.NOTIDLE && other.gameObject.transform != gameObject.transform.parent.transform) {
            DamageObject d1 = new DamageObject(20, this.gameObject.transform.parent.gameObject, 1f, Vector3.down, 0.1f, 6f);
            d1.attackType = AttackType.KnockDown;
            d1.lag = 0f;
            other.gameObject.GetComponent<Action>().getHit(d1, dir);
        }

	}

}
