using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShengLongBa : Attack {


	// Use this for initialization
	void Start () {
        anim = transform.parent.GetComponentInChildren<PlayerAnimator>();
        npcLayerMask = LayerMask.NameToLayer("NPC");
        playerLayerMask = LayerMask.NameToLayer("Player");
        attackLayerMask = LayerMask.NameToLayer("Attack");
        d = new DamageObject(20, AttackType.KnockDown, PowerType.Medium, this.gameObject.transform.parent.gameObject, 1f, Vector3.down, 0.005f, 10f);
        d.lag = 0f;

	}

    private void OnTriggerEnter(Collider other)
    {
        int dir = -1;
        if (gameObject.transform.parent.GetComponent<Action>().facingRight)
        {
            dir = 1;
        }

        bool parried = false;

        HashSet<Transform> parriedTargets = new HashSet<Transform>();

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
                parried = true;
            }
            print(c.gameObject);

        }

        if (!parried)
        {
            foreach (Collider c in cols)
            {
                GameObject target = c.gameObject;


                if (gameObject.transform.parent.GetComponent<PlayerState>().currentState == PLAYERSTATE.SHENGLONGBA && target.transform != gameObject.transform.parent.transform)
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
    }


	//private void OnTriggerEnter(Collider other)
	//{

 //       HashSet<Transform> parriedTargets = new HashSet<Transform>();

 //       int dir = -1;
 //       if (gameObject.transform.parent.GetComponent<Action>().facingRight)
 //       {
 //           dir = 1;
 //       }

 //       BoxCollider col = gameObject.GetComponent<BoxCollider>();

 //       Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation);

 //       //we have hit something

 //       print(gameObject + " overlapping");


 //       foreach (Collider c in cols)
 //       {
 //           GameObject target = c.gameObject;


 //           if (target.transform == gameObject.transform.parent.transform)    
 //           {
 //               print(target + " is parent");
 //               continue;
 //           }

 //           if (target.transform == gameObject.transform)
 //           {
 //               print(target + " is self");
 //               continue;
 //           }

 //           // second arg probs wrong
 //           if (Parry(c, gameObject.GetComponent<BoxCollider>()))
 //           {
 //               //print("Parry " + gameObject.transform.parent.ToString());
 //               anim.ShowParryEffect();
 //               parriedTargets.Add(target.transform.parent.transform);
 //           }
 //           print(c.gameObject);

 //       }

 //       foreach (Collider c in cols)
 //       {
 //           GameObject target = c.gameObject;


 //           if (gameObject.transform.parent.GetComponent<PlayerState>().currentState == PLAYERSTATE.SHENGLONGBA && target.transform != gameObject.transform.parent.transform && !parriedTargets.Contains(target.transform))
 //           {
 //               LayerMask layermask = c.gameObject.layer;
 //               //we have hit an enemy
 //               if (layermask == npcLayerMask || layermask == playerLayerMask)
 //               {

 //                   target.GetComponent<Action>().getHit(d, dir);

 //               }
 //           }
 //       }

	//}

    //IEnumerator doShengLongBa(DamageObject d)
    //{
    //    yield return new WaitForSeconds(0.7f); 

    //}
}
