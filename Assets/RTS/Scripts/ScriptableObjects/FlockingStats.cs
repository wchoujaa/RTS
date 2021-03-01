using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.RTS.Scripts.ScriptableObjects
{
	[CreateAssetMenu(menuName = "RTS/FlockingStats")]
	public class FlockingStats : ScriptableObject
	{
		public float leaderRadius = 20f;

		[Header("Cohesion")]

		public float neighbordist = 20; 
		[Header("Separation")]
		public float desiredseparation = 15.0f; 

		[Header("weights")]
		public float cohesion = 1f;
		public float separation = 10f;   
		public float lerpSpeed = 1.1f; 
		public float multiplier = 1.1f;
		[Range(0.1f, 1f)]
		public float updateRate = 1f; 
		public float spreadAmount = 1f; 
		[Range(0.1f, 100f)]
		public float leaderSpeedFactor = 80f; 
	}
}