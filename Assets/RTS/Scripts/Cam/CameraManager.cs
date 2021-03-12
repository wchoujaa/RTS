using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Handles Camera movement
/// </summary>
public class CameraManager : Singleton<CameraManager>
{
	private Camera mainCamera;
	public Rect panLimitBorder = new Rect(-100f, -100f, 200f, 200f);
	public Range scrollLimitBorder = new Range(5f, 100f);

	public static Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
	private static Vector3 screenMiddlePos = new Vector3(0.5f, 0.5f, 0f);

	public bool isFramingPlatoon { get; private set; } //from the outside it's read-only

	private Vector3 newPosition;
	private Quaternion newRotation;
	private float newZoom;


	public float translateSpeed = 2f;
	public float keyboardMovementSpeed = 5f; //speed with keyboard movement
	public float screenEdgeMovementSpeed = 3f; //spee with screen edge movement
	public float panningSpeed = 10f;

	public Vector3 velocity = Vector3.one;
	public Vector3 desiredMove = Vector3.zero;
	public float smoothTime = 2f;
	private bool useKeyboardInput = true;
	public float maxHeight = 10f; //maximal height
	public float minHeight = 15f; //minimnal height
	public float heightDampening = 5f;
	public string horizontalAxis = "Horizontal";
	public string verticalAxis = "Vertical";
	public bool usePanning = true;
	public KeyCode panningKey = KeyCode.Mouse2;
	private bool useScreenEdgeInput = false;
	public float screenEdgeBorder = 25f;

	public float zoomSpeed = 2f;
	public Vector3 velocityZoom = Vector3.one;


	public float angularSpeed = 2f;


	public LayerMask ground;





	public bool useKeyboardZooming = true;
	public KeyCode zoomInKey = KeyCode.E;
	public KeyCode zoomOutKey = KeyCode.Q;

	public bool useScrollwheelZooming = true;
	public string zoomingAxis = "Mouse ScrollWheel";
 

	public float keyboardZoomingSensitivity = 2f;
	public float scrollWheelZoomingSensitivity = 25f;



	public float followingSpeed = 5f; //speed when following a target

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


	public Camera gameplayCamera
	{
		get
		{
			return mainCamera;
		}
	}

	void Awake()
	{
		if (mainCamera == null)
		{
			mainCamera = GetComponentInChildren<Camera>();
		}

		if (mainCamera == null)
		{
			mainCamera = Camera.main;//GameObject.FindObjectOfType<Camera>();
		}
	}

	void Start()
	{
 
	}

	void Update()
	{
		Move();

	}

	/// <summary>
	/// move camera with keyboard or with screen edge
	/// </summary>
	private void Move()
	{

		//var desiredMove = is2d ? new Vector3(KeyboardInput.x, KeyboardInput.y, 0) : new Vector3(KeyboardInput.x, 0, KeyboardInput.y);
		desiredMove = mainCamera.transform.right * KeyboardInput.x + transform.forward * KeyboardInput.y;
		desiredMove *= keyboardMovementSpeed;


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
			desiredMove = mainCamera.transform.right * -MouseAxis.x + transform.transform.forward * -MouseAxis.y;
			desiredMove *= panningSpeed;
		}

		desiredMove *= translateSpeed;
		desiredMove *= Time.deltaTime;
		desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
		desiredMove = transform.InverseTransformDirection(desiredMove);
		transform.position = Vector3.SmoothDamp(transform.position, transform.position + desiredMove, ref velocity, smoothTime);
		Vector3 borderPosition = transform.position;
		borderPosition.x = Mathf.Clamp(borderPosition.x, panLimitBorder.xMin, panLimitBorder.xMax);
		borderPosition.z = Mathf.Clamp(borderPosition.z, panLimitBorder.yMin, panLimitBorder.yMax);
		transform.position = borderPosition;
		// m_Transform.Translate(desiredMove, Space.Self);

	}

	void OnDrawGizmos()
	{
		/*
        Vector3 middlePoint;
        if (GetCameraViewPointOnGroundPlane(mainCamera, screenMiddlePos, out middlePoint, InputManager.Instance.groundLayerMask))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(middlePoint, 0.5f);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(mainCamera.transform.parent.position, 0.5f);
        */
	}

	public void MoveGameplayCamera(Vector3 positionChange)
	{
		newPosition += mainCamera.transform.parent.rotation * positionChange;
	}

	public void RotateGameplayCamera(Quaternion rotationChange)
	{
		newRotation *= rotationChange;
	}

	public void ZoomGameplayCamera(float zoomAmount)
	{
		newZoom = scrollLimitBorder.Clamp(newZoom + zoomAmount);
	}

	public void MoveGameplayCameraTo(Vector3 point)
	{
		Vector3 middlePoint;
		if (GetCameraViewPointOnGroundPlane(mainCamera, screenMiddlePos, out middlePoint, ground))
		{
			point -= middlePoint - mainCamera.transform.parent.position;
			point.y = mainCamera.transform.parent.position.y;
			mainCamera.transform.parent.position = point;
		}
	}

	public static bool GetCameraScreenPointOnGroundPlane(Camera camera, Vector3 screenPoint, out Vector3 hitPoint)
	{
		hitPoint = Vector3.one * float.NaN;
		Ray ray = camera.ScreenPointToRay(screenPoint);
		float rayDistance;
		if (groundPlane.Raycast(ray, out rayDistance))
		{
			hitPoint = ray.GetPoint(rayDistance);
			return true;
		}
		return false;
	}

	public static bool GetCameraScreenPointOnGround(Camera camera, Vector3 screenPoint, out Vector3 hitPoint, LayerMask groundMask)
	{
		hitPoint = Vector3.one * float.NaN;
		Ray ray = camera.ScreenPointToRay(screenPoint);

		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, groundMask))
		{
			hitPoint = hitInfo.point;
			return true;
		}
		return false;
	}

	public static bool GetCameraViewPointOnGroundPlane(Camera camera, Vector3 viewportPoint, out Vector3 hitPoint, LayerMask groundMask)
	{
		hitPoint = Vector3.one * float.NaN;
		Ray ray = camera.ViewportPointToRay(viewportPoint);

		float hitInfo;
		if (groundPlane.Raycast(ray, out hitInfo))
		{
			hitPoint = ray.origin + ray.direction.normalized * hitInfo;
			return true;
		}
		return false;
	}
}