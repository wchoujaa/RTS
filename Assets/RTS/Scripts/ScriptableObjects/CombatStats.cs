using UnityEngine;

namespace Assets.RTS.Scripts.Combat
{
	[CreateAssetMenu(menuName = "RTS/CombatStats")]

	public class CombatStats : ScriptableObject
	{
		public int damage;
		public int health;
		public float range;
		public float rate;
	}
}