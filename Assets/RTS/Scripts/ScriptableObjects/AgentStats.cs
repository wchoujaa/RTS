using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.RTS.Scripts.ScriptableObjects
{
	[CreateAssetMenu(menuName = "RTS/AgentStats")]

	public class AgentStats : ScriptableObject
	{
		public float maxSpeed;
		public float maxAccel;
		public float maxAngularSpeed;
		public float stoppingDistance; 
		public int leaderPriority = 30;
		public int priority = 50;
	}
}
