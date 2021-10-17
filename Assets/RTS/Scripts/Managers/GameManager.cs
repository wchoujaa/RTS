using System.Collections;
using UnityEngine;

namespace Assets.RTS.Scripts.Managers
{
	public class GameManager : MonoBehaviour
	{

		public static GameManager instance;



		private void Awake()
		{
			instance = this;
		}
		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}