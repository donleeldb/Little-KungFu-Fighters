using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShengLongBa : MonoBehaviour {

    private PlayerAnimator anim;
    LayerMask npcLayerMask;
    LayerMask playerLayerMask;
    LayerMask attackLayerMask;

	// Use this for initialization
	void Start () {
        anim = transform.parent.GetComponentInChildren<PlayerAnimator>();
        npcLayerMask = LayerMask.NameToLayer("NPC");
        playerLayerMask = LayerMask.NameToLayer("Player");
        attackLayerMask = LayerMask.NameToLayer("Attack");
	}

	private void OnTriggerEnter(Collider other)
	{
        DamageObject d1 = new DamageObject(20, this.gameObject.transform.parent.gameObject, 1f, Vector3.down, 0.005f, 10f);
        d1.attackType = AttackType.KnockDown;
        d1.lag = 0f;

        HashSet<Transform> parriedTargets = new HashSet<Transform>();

        int dir = -1;
        if (gameObject.transform.parent.GetComponent<Action>().facingRight)
        {
            dir = 1;
        }

        BoxCollider col = gameObject.GetComponent<BoxCollider>();

        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation);

        //we have hit something

        print(gameObject + " overlapping");


        foreach (Collider c in cols)
        {
            GameObject target = c.gameObject;


            if (target.transform == gameObject.transform.parent.transform)    
            {
                print(target + " is parent");
                continue;
            }

            if (target.transform == gameObject.transform)
            {
                print(target + " is self");
                continue;
            }

            // second arg probs wrong
            if (Parry(c, gameObject.GetComponent<BoxCollider>()))
            {
                //print("Parry " + gameObject.transform.parent.ToString());
                anim.ShowParryEffect();
                parriedTargets.Add(target.transform.parent.transform);
            }
            print(c.gameObject);

        }

        foreach (Collider c in cols)
        {
            GameObject target = c.gameObject;


            if (gameObject.transform.parent.GetComponent<PlayerState>().currentState == PLAYERSTATE.SHENGLONGBA && target.transform != gameObject.transform.parent.transform && !parriedTargets.Contains(target.transform))
            {
                LayerMask layermask = c.gameObject.layer;
                //we have hit an enemy
                if (layermask == npcLayerMask || layermask == playerLayerMask)
                {

                    target.GetComponent<Action>().getHit(d1, dir);

                }
            }
        }

	}

    private bool Parry(Collider targetCollider, Collider thisCollider)
    {
        GameObject target = targetCollider.gameObject;

        LayerMask layermask = target.layer;
        //we have hit an enemy
        LayerMask attackLayerMask = LayerMask.NameToLayer("Attack");
        return (layermask == attackLayerMask);

    }

    //IEnumerator doShengLongBa(DamageObject d)
    //{
    //    yield return new WaitForSeconds(0.7f); 

    //}
}
