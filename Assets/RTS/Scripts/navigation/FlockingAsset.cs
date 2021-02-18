using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Flocking")]
public class FlockingAsset : ScriptableObject
{


	[Header("Cohesion")]

	public float neighbordist = 20;



	[Header("Separation")]
	public float desiredseparation = 15.0f;

 

	[Header("Arrive")]
	public float targetRadius = 5.0f;
	public float slowRadius = 15.0f;
	public float timeToTarget = 0.1f;

	[Header("weights")]
	public float cohesion = 1f;
	public float separation = 1f;
	public float alignement = 1f;
	public float arrive = 1f;
	public float targetReachedNotifyTime = 1f;
}
