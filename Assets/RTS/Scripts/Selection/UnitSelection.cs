using Assets.RTS.Scripts.Controllers;
using Assets.RTS.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.RTS.Scripts.Selection
{
    class UnitSelection : MonoBehaviour
    {

        // GUI Rect Source code found at:
        // http://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/

        // public GameObject DestinationPrefab;

        // Currently selected objects
        private List<GameObject> _selectedUnits;
        // Friendly (selectable) vehicles
        private GameObject[] _selectableUnits;
        [Tooltip("Mouse Input Setting")]

        public LayerMask ground;
        public LayerMask unit;
        public string playerUnitTag;
        public FollowObject cameraFollow;

        #region Selection Utility Rectangles
        static Texture2D _whiteTexture;
        private static Texture2D WhiteTexture
        {
            get
            {
                if (_whiteTexture == null)
                {
                    _whiteTexture = new Texture2D(1, 1);
                    _whiteTexture.SetPixel(0, 0, Color.white);
                    _whiteTexture.Apply();
                }

                return _whiteTexture;
            }
        }


        private static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        private static void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, WhiteTexture);
            GUI.color = Color.white;
        }

        private static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            // Top
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Left
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            // Bottom
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }

        private static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
        {
            var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
            var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
            var min = Vector3.Min(v1, v2);
            var max = Vector3.Max(v1, v2);
            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        bool isSelecting = false;
        Vector3 mousePosition1;

        private bool IsWithinSelectionBounds(GameObject gameObject)
        {
            var camera = Camera.main;
            var viewportBounds =
                GetViewportBounds(camera, mousePosition1, Input.mousePosition);

            return viewportBounds.Contains(
                camera.WorldToViewportPoint(gameObject.transform.position));
        }

        void OnGUI()
        {
            if (dragSelect)
            {
                // Create a rect from both mouse positions
                var rect = GetScreenRect(mousePosition1, Input.mousePosition);
                // Draw transparent rectangle
                DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                // Draw rectangle border
                DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
        }
        #endregion Selection Utility Rectangles


        public GameObject pointer;
        private RaycastHit hit;
        [Tooltip("Mouse Input Setting")]

        [Range(0, 2)]
        public int selection;
        [Range(0, 2)]
        public int destination;
        private Vector3 target;
        private bool dragSelect; //marquee selection flag
        private InputManager inputManager;
        [Tooltip("Drag Setting")]
        private float timer = 0f;
        public float drag_delay = 0.1f;
        public float drag_magnitude = 30;
        void Start()
        {
            inputManager = FindObjectOfType<InputManager>();

            _selectedUnits = new List<GameObject>();
            pointer.transform.parent = null;
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (selection == 0 && inputManager.doubleMouse0.DoubleClickLongPressedCheck())
            {
                ClearSelectedUnits();
            }
            else if (selection == 1 && inputManager.doubleMouse0.DoubleClickLongPressedCheck())
            {
                ClearSelectedUnits();
            }


            //0. when right mouse button clicked
            if (Input.GetMouseButtonDown(destination))
            {
                timer = 0;
            }

            // 1. when selection button clicked
            if (Input.GetMouseButtonDown(selection))
            {
                mousePosition1 = Input.mousePosition;
            }

            // 2. while selection button held
            if (Input.GetMouseButton(selection))
            {
                if ((mousePosition1 - Input.mousePosition).magnitude > drag_magnitude) //if the mouse has moved a lot, then we enter marquee mode
                {
                    dragSelect = true;
                } else if(timer > drag_delay)
				{
                    MoveToTarget();
				}
            } 

            // 3. when selection button up
            if (Input.GetMouseButtonUp(selection))
            {

                if (dragSelect == false) // mouse click event
                {
                    Ray ray = Camera.main.ScreenPointToRay(mousePosition1); //raycast from previous mouse pointer position

                    if (Physics.Raycast(ray, out hit, 50000.0f, unit)) ///if we hit a unit
					{
                        UnitController unitController = hit.transform.gameObject.GetComponent<UnitController>();
                        if (unitController != null && unitController.tag == playerUnitTag)
                        {
                            if (!Input.GetKey(KeyCode.LeftShift))
                            {
                                ClearSelectedUnits();
                            }
                            _selectedUnits.Add(hit.transform.gameObject);
                        }
                    }
                }
                else if (_selectableUnits != null) // drag event
                {
                    foreach (GameObject go in _selectableUnits)
                        if (IsWithinSelectionBounds(go))
                        {
                            go.GetComponent<UnitController>().SetSelected(true);
                            _selectedUnits.Add(go);
                        }
                        else
                        {
                            go.GetComponent<UnitController>().SetSelected(false);
                            _selectedUnits.Remove(go);
                        }
                }
                dragSelect = false;
            }

        }

        private void FixedUpdate()
        {
            //HandleMousePointer();

        }

		private void HandleMousePointer()
		{
            //4. Is Mouse hovering
            if (_selectedUnits.Count != 0)
            {
                pointer.SetActive(true);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raycast from   mouse pointer position

                if (Physics.Raycast(ray, out hit, 50000.0f, ground)) //if we hit ground
                {
                    Vector3 point = hit.point;

                    pointer.transform.position = point;
                }
            }
            else
            {
                pointer.SetActive(false);
            }
        }

		private void MoveToTarget()
		{
            _selectableUnits = GameObject.FindGameObjectsWithTag(playerUnitTag);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raycast from previous mouse pointer position

            if (Physics.Raycast(ray, out hit, 50000.0f, unit)) //if we hit a unit
            {
                //Debug.Log("clicked on a unit");
                target = hit.transform.position;
            }
            else if (Physics.Raycast(ray, out hit, 50000.0f, ground)) //if we hit ground
            {

                target = hit.point;
            }
            else
            {
                target = Vector3.zero;
            }
            if (target != Vector3.zero)
            {
                //Debug.Log(target.transform.position);
                BroadcastNewTarget(hit.point, Input.GetKey(KeyCode.LeftShift));
            }
        }



        /// <summary>
        /// Mark all selected units as unselected
        /// </summary>
        void ClearSelectedUnits()
        {
            foreach (GameObject go in _selectedUnits)
            {
                go.GetComponent<UnitController>().SetSelected(false);
            }
            _selectedUnits = new List<GameObject>();
        }


        /// <summary>
        /// Broadcast to all selected units that a new destination has been given.
        /// </summary>
        public void BroadcastNewTarget(Vector3 position, bool isWaypoint)
        {
            foreach (GameObject go in _selectedUnits)
            {
                go.GetComponent<UnitController>().MoveUnit(position, position, isWaypoint);
            }
        }
    }
}
