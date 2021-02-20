using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/UnitStats")]
public class UnitStats : ScriptableObject {
    [Header("Combat")]
    public float attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float health;
	public float rotationSpeed;
    [Header("Agent")]
    public float maxSpeed;
    public float maxAccel;
    public float maxAngularSpeed;
    public float stoppingDistance;

    [Header("Unit")]
    public Color color;
}
