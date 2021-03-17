using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.RTS.Scripts.ScriptableObjects
{
	[CreateAssetMenu(menuName = "RTS/UnitStats")]
	public class UnitStats : ScriptableObject
	{
		public Color color;
		public UnitType unitType;
		public float stoppingDistance = 1.5f;
		public float maxSpeed = 20f;
		public float radius = 1.5f;
	}
}