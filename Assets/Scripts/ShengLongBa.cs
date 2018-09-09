using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShengLongBa : MonoBehaviour {


	// Use this for initialization
	void Start () {
	}

	private void OnTriggerEnter(Collider other)
	{

        int dir = -1;
        if (gameObject.transform.parent.GetComponent<Action>().facingRight)
        {
            dir = 1;
        }

        if (gameObject.transform.parent.GetComponent<PlayerState>().currentState == PLAYERSTATE.SHENGLONGBA && other.gameObject.transform != gameObject.transform.parent.transform) {
            DamageObject d1 = new DamageObject(20, this.gameObject, 1f, Vector3.down, 0.005f, 10f);
            d1.attackType = AttackType.KnockDown;
            d1.lag = 0f;
            other.gameObject.GetComponent<Action>().getHit(d1, dir);
        }

	}

    //IEnumerator doShengLongBa(DamageObject d)
    //{
    //    yield return new WaitForSeconds(0.7f); 

    //}
}
