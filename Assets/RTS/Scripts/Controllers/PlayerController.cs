using Assets.RTS.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.RTS.Scripts.Controllers
{


	public class PlayerController : MonoBehaviour
	{

		private UnitController unitController;
		private InputManager inputManager;

		private void Start()
		{
			inputManager = FindObjectOfType<InputManager>();
			unitController = GetComponent<UnitController>();
		}


		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.S))
			{
				CancelOrder();
			}

			handleInput();
		}

		private void handleInput()
		{


			if (unitController.IsSelected && unitController.isGroupLeader)
			{
				if (inputManager.doubleE.DoubleClickLongPressedCheak())
				{
					//flocking.SpreadUnit(-flocking.flockingStats.spreadAmount);

				}
				else if (inputManager.doubleE.SingleClickLongPressedCheck())
				{
					//flocking.SpreadUnit(flocking.flockingStats.spreadAmount);
				}
			}
		}

		public void CancelOrder()
		{
			//combatBehaviour.Cancel();
			unitController.MoveUnit(transform.position, transform.position);
		}
	}
}
