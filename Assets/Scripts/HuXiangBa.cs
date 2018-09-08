using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuXiangBa : MonoBehaviour {


    private BoxCollider collider;

	// Use this for initialization
	void Start () {
        collider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider other)
	{
        if (gameObject.transform.parent.GetComponent<PlayerState>().currentState == PLAYERSTATE.SHENGLONGBA && other.gameObject.transform != gameObject.transform.parent.transform) {
            DamageObject d1 = new DamageObject(20, this.gameObject, 1f, Vector3.down, 0.1f, 6f);
            d1.attackType = AttackType.KnockDown;
            d1.lag = 0f;
            other.gameObject.GetComponent<Action>().getHit(d1);
        }

	}

}
