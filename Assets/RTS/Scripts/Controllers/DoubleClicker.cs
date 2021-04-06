using System.Collections;
using UnityEngine;

namespace Assets.RTS.Scripts.Controllers
{

	public class DoubleClicker 
	{


		/// <summary>
		/// Construcor with defult deltatime 
		/// </summary>
		public DoubleClicker(KeyCode key)
		{

			//set key
			_key = key;
		}

		private KeyCode _key;
		private float _deltaTime = defultDeltaTime;

		//defult deltaTime
		public const float defultDeltaTime = 0.21f;
		public bool doubleTapped = false;
		/// <summary>
		/// Current key property
		/// </summary>
		public KeyCode key
		{
			get { return _key; }
		}

		/// <summary>
		/// Current deltaTime property
		/// </summary>
		public float deltaTime
		{
			get { return _deltaTime; }
		}

		public void Reset()
		{
			doubleTapped = false;
		}

		//time pass
		private float timePass = 0;
		/// <summary>
		/// Cheak for double press
		/// </summary>
		/// 
		//public bool DoubleClickCheak()
		//{
		//	if (timePass > 0) { timePass -= Time.deltaTime; }

		//	if (Input.GetKeyDown(_key))
		//	{
		//		if (timePass > 0) { timePass = 0; return true; }

		//		timePass = _deltaTime;
		//	}

		//	return false;
		//}



		public void Update()
		{ 
			if (timePass > 0)
			{
				timePass -= Time.deltaTime;
			}
			if (Input.GetKeyDown(_key))
			{
				if (timePass > 0)
				{
					doubleTapped = true;
					timePass = 0;
				}
				else
				{
					timePass = _deltaTime;
				}

			}
			if (doubleTapped && Input.GetKeyUp(_key))
			{
				doubleTapped = false;
			}
		}

		public bool SingleClickLongPressedCheck()
		{
		 
			return Input.GetKey(_key) && timePass < 0 && !doubleTapped;
		}


		public bool DoubleClickLongPressedCheck()
		{
			  
			return doubleTapped;
		}
	}


}