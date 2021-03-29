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
        float clicked = 0;
        float clicktime = 0;
        float clickdelay = 0.5f;
        private void Start()
		{
			doubleE = new DoubleClicker(KeyCode.E); 
		}

		private void Update()
		{
			doubleE.Update(); 
		}


        public  bool DoubleClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                clicked++;
                if (clicked == 1) clicktime = Time.time;
            }
            if (clicked > 1 && Time.time - clicktime < clickdelay)
            {
                clicked = 0;
                clicktime = 0;
                return true;
            }
            else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
            return false;
        }

    }
}
