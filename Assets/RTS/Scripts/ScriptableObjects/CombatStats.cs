using UnityEngine;

namespace Assets.RTS.Scripts.Combat
{
	[CreateAssetMenu(menuName = "RTS/CombatStats")]

	public class CombatStats : ScriptableObject
	{
		public int attackDamage;
		public int health;
		public float attackRange;
		public float attackRate;
	}
}