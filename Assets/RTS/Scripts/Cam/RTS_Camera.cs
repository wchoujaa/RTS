using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.RTS.Scripts.Cam

{

	[AddComponentMenu("RTS Camera")]
	public class RTS_Camera : MonoBehaviour
	{

		#region Foldouts

#if UNITY_EDITOR

		public int lastTab = 0;
		public bool presetSettingsFoldout;
		public bool movementSettingsFoldout;
		public bool zoomingSettingsFoldout;
		public bool rotationSettingsFoldout;
		public bool heightSettingsFoldout;
		public bool mapLimitSettingsFoldout;
		public bool targetingSettingsFoldout;
		public bool inputSettingsFoldout;

#endif

		#endregion

		private Transform m_Transform; //camera tranform
		private UnityEngine.Camera m_Camera; //camera for 2d zoom 

		public bool useFixedUpdate = false; //use FixedUpdate() or Update()        

		#region Presets
		public bool RTSCameraDefault = true;
		public bool UnityEditorStyle = false;
		public bool ClassicRTSStyle = false;
		#endregion

		#region Movement

		public bool is2d = false;
		public bool isMobile = false;
		public float keyboardMovementSpeed = 5f; //speed with keyboard movement
		public float screenEdgeMovementSpeed = 3f; //spee with screen edge movement
		public float followingSpeed = 5f; //speed when following a target
		public float rotationSped = 3f;
		public float panningSpeed = 10f;
		public float mouseRotationSpeed = 10f;

		#endregion

		#region Height

		public bool autoHeight = true;
		public LayerMask groundMask = 1; //layermask of ground or other objects that affect height

		public float maxHeight = 10f; //maximal height
		public float minHeight = 15f; //minimnal height
		public float heightDampening = 5f;
		public float keyboardZoomingSensitivity = 2f;
		public float scrollWheelZoomingSensitivity = 25f;

		private float zoomPos = 1; //value in range (0, 1) used as t in Matf.Lerp

		#endregion

		#region MapLimits

		public bool limitMap = true;
		public float limitX = 50f; //x limit of map
		public float limitY = 50f; //z limit of map

		#endregion

		#region Targeting

		public Transform targetFollow; //target to follow
		public Vector3 targetOffset;

		/// <summary>
		/// are we following target
		/// </summary>
		public bool FollowingTarget
		{
			get
			{
				return targetFollow != null;
			}
		}

		#endregion

		#region Input

		public bool useScreenEdgeInput = true;
		public float screenEdgeBorder = 25f;

		public bool useKeyboardInput = true;
		public string horizontalAxis = "Horizontal";
		public string verticalAxis = "Vertical";

		public bool usePanning = true;
		public KeyCode panningKey = KeyCode.Mouse2;

		public bool useKeyboardZooming = true;
		public KeyCode zoomInKey = KeyCode.E;
		public KeyCode zoomOutKey = KeyCode.Q;

		public bool useScrollwheelZooming = true;
		public string zoomingAxis = "Mouse ScrollWheel";

		public bool useKeyboardRotation = true;
		public KeyCode rotateRightKey = KeyCode.X;
		public KeyCode rotateLeftKey = KeyCode.Z;

		public bool useMouseRotation = true;
		public KeyCode mouseRotationKey = KeyCode.Mouse1;
		private Quaternion rotationVector;
		private Vector3 zoomDirection;
		public Vector3 zoomAmount;
		public float zoomSpeed;
		public float translateSpeed;
		public Vector3 velocityZoom = Vector3.one;
		public Vector3 velocity = Vector3.one;
		public Vector3 desiredMove = Vector3.zero;
		public float smoothTime = 2f;
		private Vector2 KeyboardInput
		{
			get { return useKeyboardInput ? new Vector2(CrossPlatformInputManager.GetAxis(horizontalAxis), CrossPlatformInputManager.GetAxis(verticalAxis)) : Vector2.zero; }
		}

		private Vector2 MouseInput
		{
			get { return CrossPlatformInputManager.mousePosition; }
		}

		private float ScrollWheel
		{
			get { return CrossPlatformInputManager.GetAxis(zoomingAxis); }
		}

		private Vector2 MouseAxis
		{
			get { return new Vector2(CrossPlatformInputManager.GetAxis("Mouse X"), CrossPlatformInputManager.GetAxis("Mouse Y")); }
		}

		private int ZoomDirection
		{
			get
			{
				bool zoomIn = Input.GetKey(zoomInKey);
				bool zoomOut = Input.GetKey(zoomOutKey);
				if (zoomIn && zoomOut)
					return 0;
				else if (!zoomIn && zoomOut)
					return 1;
				else if (zoomIn && !zoomOut)
					return -1;
				else
					return 0;
			}
		}

		private int RotationDirection
		{
			get
			{
				bool rotateRight = Input.GetKey(rotateRightKey);
				bool rotateLeft = Input.GetKey(rotateLeftKey);
				if (rotateLeft && rotateRight)
					return 0;
				else if (rotateLeft && !rotateRight)
					return -1;
				else if (!rotateLeft && rotateRight)
					return 1;
				else
					return 0;
			}
		}

		#endregion

		#region Unity_Methods

		private void Start()
		{
			rotationVector = transform.rotation;
			m_Transform = transform;
			m_Camera = GetComponentInChildren<UnityEngine.Camera>();
			zoomDirection = m_Camera.transform.localPosition;
		}

		private void Update()
		{
			if (!useFixedUpdate)
				CameraUpdate();
		}

		private void FixedUpdate()
		{

		}


		private void LateUpdate()
		{
			if (useFixedUpdate)
				CameraUpdate();
		}

		#endregion

		#region RTSCamera_Methods

		/// <summary>
		/// update camera movement and rotation
		/// </summary>
		private void CameraUpdate()
		{
			if (FollowingTarget)
				FollowTarget();
			else
				Move();

			HeightCalculation();
			Rotation();
			LimitPosition();
		}

		/// <summary>
		/// move camera with keyboard or with screen edge
		/// </summary>
		private void Move()
		{
			if (useKeyboardInput)
			{
				//var desiredMove = is2d ? new Vector3(KeyboardInput.x, KeyboardInput.y, 0) : new Vector3(KeyboardInput.x, 0, KeyboardInput.y);
				desiredMove = m_Camera.transform.right * KeyboardInput.x + m_Transform.transform.forward * KeyboardInput.y;
				desiredMove *= keyboardMovementSpeed;
			}

			if (useScreenEdgeInput)
			{

				Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
				Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
				Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
				Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

				//desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
				//desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

				//desiredMove *= screenEdgeMovementSpeed;

			}

			if (usePanning && Input.GetKey(panningKey) && MouseAxis != Vector2.zero)
			{
				desiredMove = m_Camera.transform.right * -MouseAxis.x + m_Transform.transform.forward * -MouseAxis.y;
				desiredMove *= panningSpeed;
			}

			desiredMove *= translateSpeed;
			desiredMove *= Time.deltaTime;
			desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
			desiredMove = m_Transform.InverseTransformDirection(desiredMove);
			m_Transform.position = Vector3.SmoothDamp(m_Transform.position, m_Transform.position + desiredMove, ref velocity, smoothTime);
			// m_Transform.Translate(desiredMove, Space.Self);

		}

		/// <summary>
		/// calcualte height
		/// </summary>
		private void HeightCalculation()
		{
			float distanceToGround = DistanceToGround();
			if (useScrollwheelZooming)
				zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
			if (useKeyboardZooming)
				zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;

			zoomPos = Mathf.Clamp01(zoomPos);

			float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomPos);
			float difference = 0;



			if (distanceToGround != targetHeight)
				difference = targetHeight - distanceToGround;

			if (is2d)
			{
				m_Camera.orthographicSize = Mathf.Lerp(m_Camera.orthographicSize, targetHeight + difference, Time.deltaTime * heightDampening);
			}
			else
			{
				if(distanceToGround > minHeight && distanceToGround < maxHeight)
				{
 
 

					//Vector3 direction = m_Camera.transform.forward *  (targetHeight + difference);// new Vector3(m_Transform.position.x, targetHeight + difference, m_Transform.position.z);
					zoomDirection += zoomAmount   * ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;

					//zoomDirection *= distanceToGround;

					//m_Camera.transform.localPosition = Vector3.SmoothDamp(m_Camera.transform.localPosition, zoomDirection, ref velocityZoom, Time.deltaTime * zoomSpeed);
					m_Camera.transform.localPosition = Vector3.Lerp(m_Camera.transform.localPosition, zoomDirection, Time.deltaTime * heightDampening);
				} 
			}
		}

		/// <summary>
		/// rotate camera
		/// </summary>
		private void Rotation()
		{
			var rotationVector = is2d ? Vector3.forward : Vector3.up;
			if (useKeyboardRotation)
				transform.Rotate(rotationVector, RotationDirection * Time.deltaTime * rotationSped, Space.World);

			//rotationVector *= Quaternion.Euler(Vector3.up * -MouseAxis.x * 1000f);
			//Debug.Log(rotationVector);
			if (useMouseRotation && Input.GetKey(mouseRotationKey))
			{
				//m_Transform.rotation = Quaternion.Lerp(transform.rotation, rotationVector, Time.deltaTime * mouseRotationSpeed);
				m_Transform.Rotate(rotationVector, -MouseAxis.x * Time.deltaTime * mouseRotationSpeed, Space.World);
			}
		}

		/// <summary>
		/// follow targetif target != null
		/// </summary>
		private void FollowTarget()
		{
			Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z) + targetOffset;
			m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
		}

		/// <summary>
		/// limit camera position
		/// </summary>
		private void LimitPosition()
		{
			if (!limitMap)
				return;

			m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, -limitX, limitX),
				m_Transform.position.y,
				Mathf.Clamp(m_Transform.position.z, -limitY, limitY));
		}

		/// <summary>
		/// set the target
		/// </summary>
		/// <param name="target"></param>
		public void SetTarget(Transform target)
		{
			targetFollow = target;
		}

		/// <summary>
		/// reset the target (target is set to null)
		/// </summary>
		public void ResetTarget()
		{
			targetFollow = null;
		}

		/// <summary>
		/// calculate distance to ground
		/// </summary>
		/// <returns></returns>
		private float DistanceToGround()
		{
			if (is2d)
			{
				return m_Camera.orthographicSize;
			}
			else
			{
				Ray ray = new Ray(m_Camera.transform.position, Vector3.down);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, groundMask.value))
					return (hit.point - m_Camera.transform.position).magnitude;
			}
			return 0f;
		}

		#endregion

		#region PresetsMethods
		void SetPresetRTSCameraDefault()
		{

		}
		void SetPresetUnityEditorStyle()
		{

		}
		void SetPresetClassicRTSStyle()
		{

		}
		#endregion
	}
}