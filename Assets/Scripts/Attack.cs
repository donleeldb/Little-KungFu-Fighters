using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour {

    protected PlayerAnimator anim;
    protected LayerMask npcLayerMask;
    protected LayerMask playerLayerMask;
    protected LayerMask attackLayerMask;
    protected DamageObject d;



	// Use this for initialization
	void Start () {
        anim = transform.parent.GetComponentInChildren<PlayerAnimator>();
        npcLayerMask = LayerMask.NameToLayer("NPC");
        playerLayerMask = LayerMask.NameToLayer("Player");
        attackLayerMask = LayerMask.NameToLayer("Attack");
	}

    //public abstract float getVerticalVelocity();
    //public abstract float getMoveVectorX();
    //public abstract PLAYERSTATE getPlayerState();
    public abstract void Execute(PlayerAnimator anim, int dir);

	private void OnTriggerEnter(Collider other)
    {

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

                    target.GetComponent<Action>().getHit(d, dir);

                }
            }
        }

	}

    protected bool Parry(Collider targetCollider, Collider thisCollider)
    {
        GameObject target = targetCollider.gameObject;

        LayerMask layermask = target.layer;
        //we have hit an enemy
        LayerMask attackLayerMask = LayerMask.NameToLayer("Attack");
        return (layermask == attackLayerMask);

    }

    protected IEnumerator ColliderTimeToLive(BoxCollider collider, float ttl)
    {
        yield return new WaitForSeconds(ttl);
        collider.enabled = false;
        Destroy(gameObject);
    }

    //IEnumerator doShengLongBa(DamageObject d)
    //{
    //    yield return new WaitForSeconds(0.7f); 

    //}
}
