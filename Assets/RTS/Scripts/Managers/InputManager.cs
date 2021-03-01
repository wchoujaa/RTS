using Assets.RTS.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.RTS.Scripts.Managers
{
	public class InputManager : MonoBehaviour
	{
		public DoubleClicker doubleE;

		private void Start()
		{
			doubleE = new DoubleClicker(KeyCode.E); 
		}

		private void Update()
		{
			doubleE.Update(); 
		}

	}
}
