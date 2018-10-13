using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : Attack {


	// Use this for initialization
	void Start () {
	}

    public override void Execute(PlayerAnimator anim, int dir)
    {
        return;
    }

    public IEnumerator WaitBeforeCollide(DamageObject _d, int dir)
    {   npcLayerMask = LayerMask.NameToLayer("NPC");
        playerLayerMask = LayerMask.NameToLayer("Player");
        attackLayerMask = LayerMask.NameToLayer("Attack");


        d = _d;
        bool parried = false;
        BoxCollider col = GetComponent<BoxCollider>();
        col.enabled = true;
        gameObject.SetActive(true);

        yield return new WaitForSeconds(d.lag);

        //      Vector3 playerPos = transform.position + Vector3.up * 1.5f;

        Vector3 originalScale = transform.localScale;
        Vector3 scaleFactor = new Vector3(d.range, 1f, 1f);
        transform.localScale = Vector3.Scale(originalScale, scaleFactor);


        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation);

        //we have hit something

        print(cols.Length);

        foreach (Collider c in cols)
        {
            GameObject target = c.gameObject;

            if (target.GetComponent<CharacterController>() == gameObject.transform.parent.GetComponent<CharacterController>() || target.transform == gameObject.transform)
            {
                continue;
            }

            if (Parry(c, col))
            {
                parried = true;
                anim.ShowParryEffect();
            }


        }
        print(parried);
        if (!parried)
        {
            foreach (Collider c in cols)
            {
                GameObject target = c.gameObject;

                if (target.GetComponent<CharacterController>() == gameObject.transform.parent.GetComponent<CharacterController>())
                {
                    continue;
                }

                LayerMask layermask = c.gameObject.layer;
                //we have hit an enemy
                if (layermask == npcLayerMask || layermask == playerLayerMask)
                {
                    print("hi");


                    target.GetComponent<Action>().getHit(d);

                }

            }
        }
       transform.localScale = originalScale;
        yield return new WaitForSeconds(d.lag);
        col.enabled = false;
        gameObject.SetActive(false);

    }

    //IEnumerator doShengLongBa(DamageObject d)
    //{
    //    yield return new WaitForSeconds(0.7f); 

    //}
}
