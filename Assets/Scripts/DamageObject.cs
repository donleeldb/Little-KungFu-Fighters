using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageObject {

	public int damage;
	public float range;
	public Vector3 centerOffset;
	public AttackType attackType;
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
	}

	public DamageObject(int _damage, AttackType _attackType, GameObject _inflictor, float _range, Vector3 _centerOffset, float _force, float _verticalForce=0f){
		damage =  _damage;
		attackType = _attackType;
		inflictor = _inflictor;
		range = _range;
		centerOffset = _centerOffset;
		force = _force;
		verticalForce = _verticalForce;
	}
}

public enum AttackType {
	Default = 0,
	SoftPunch = 10,
	MediumPunch = 20,
	KnockDown = 30,
	Stagger = 10,
    Paralyze = 10,
};


