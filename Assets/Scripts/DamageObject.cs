using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageObject {

	public int damage;
	public float range;
	public Vector3 center;
	public AttackType attackType;
	public GameObject inflictor;
	public float comboResetTime = .5f;

	public DamageObject(int _damage, GameObject _inflictor, float _range, Vector3 _center){
		damage =  _damage;
		inflictor = _inflictor;
		range = _range;
		center = _center;
	}

	public DamageObject(int _damage, AttackType _attackType, GameObject _inflictor, float _range, Vector3 _center){
		damage =  _damage;
		attackType = _attackType;
		inflictor = _inflictor;
		range = _range;
		center = _center;
	}
}

public enum AttackType {
	Default = 0,
	SoftPunch = 10,
	MediumPunch = 20,
	KnockDown = 30,
	SoftKick = 40,
	HardKick = 50,
	SpecialMove = 60,
	DeathBlow = 70,
};


