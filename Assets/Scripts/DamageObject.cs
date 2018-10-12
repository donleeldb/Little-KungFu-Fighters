using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageObject {

	public int damage;
	public float range;
	public Vector3 centerOffset;
	public AttackType attackType;
    public PowerType powerType;
	public GameObject inflictor;
	public float force = 0.01f;
	public float verticalForce = 0f;
	public float lag = 0.1f; // time before shooting raycast

	public DamageObject(int _damage, GameObject _inflictor, float _range, Vector3 _centerOffset, float _force, float _verticalForce=0f){
		damage =  _damage;
		inflictor = _inflictor;
		range = _range;
		centerOffset = _centerOffset;
		force = _force;
		verticalForce = _verticalForce;
        powerType = PowerType.Low;
	}

    public DamageObject(int _damage, AttackType _attackType, PowerType _powerType, GameObject _inflictor, float _range, Vector3 _centerOffset, float _force, float _verticalForce=0f){
		damage =  _damage;
		attackType = _attackType;
		inflictor = _inflictor;
		range = _range;
		centerOffset = _centerOffset;
		force = _force;
		verticalForce = _verticalForce;
        powerType = _powerType;
	}
}

//for hit
public enum AttackType {
	RegularAttack = 10,
	KnockDown = 30,
	Stagger = 10,
    Paralyze = 10,
};

//for parry
public enum PowerType
{
    Low = 1,
    Medium = 2,
    High = 3,
    Ultimate = 4,
};


