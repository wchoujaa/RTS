using Assets.RTS.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.RTS.Scripts.Selection
{
	class UnitSelection2 : MonoBehaviour
	{

        // GUI Rect Source code found at:
        // http://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/

       // public GameObject DestinationPrefab;

        // Currently selected objects
        private List<GameObject> _selectedUnits;
        // Friendly (selectable) vehicles
        private GameObject[] _selectableUnits;

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
            if (isSelecting)
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
        [Range(0,2)]
        public int selection;
        [Range(0, 2)] 
        public int destination;
        private Vector3 target; 
        private bool dragSelect; //marquee selection flag

        void Start()
        {
            _selectedUnits = new List<GameObject>();
            pointer.transform.parent = null;
 
        }

        void Update()
        {

            //         if (Input.GetMouseButtonDown(selection) && Input.touchCount > 1 && !EventSystem.current.IsPointerOverGameObject())
            //{
            //             Debug.Log("Deselect");
            //             ClearSelectedUnits();
            //             isSelecting = false;

            //         }
            //0. when right mouse button clicked
            if (Input.GetMouseButtonDown(destination))
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

            // 1. when selection button clicked
            if (Input.GetMouseButtonDown(selection))
            {
                mousePosition1 = Input.mousePosition;
            }

            // 2. while selection button held
            if (Input.GetMouseButton(selection))
            {
                if ((mousePosition1 - Input.mousePosition).magnitude > 40) //if the mouse has moved a lot, then we enter marquee mode
                {
                    dragSelect = true;
                    isSelecting = true;
                }
            }




            // 3. when selection button up
            if (Input.GetMouseButtonUp(selection))
            {

                if (dragSelect == false) //differentiating between a rapid click cycle vs a drag-select
                {
                    Ray ray = Camera.main.ScreenPointToRay(mousePosition1); //raycast from previous mouse pointer position

                    if (Physics.Raycast(ray, out hit, 50000.0f, unit)) ///if we hit something that isn't ground
					{
                        UnitController unitController = hit.transform.gameObject.GetComponent<UnitController>();
                        if (unitController.tag == playerUnitTag)
                        {
                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                _selectedUnits.Add(hit.transform.gameObject);

                                //Debug.Log("Inclusive Select");
                            }
                            else
                            {
                                ClearSelectedUnits();
                                _selectedUnits.Add(hit.transform.gameObject);


                            }
                        }

                    }
                    else //if we didn't hit something
                    {

                            ClearSelectedUnits();
                    
                    }

                }
                else if (_selectableUnits != null) //marquee select
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
















































   //         if (Input.GetMouseButtonDown(selection) && Input.touchCount > 1 && !EventSystem.current.IsPointerOverGameObject())
			//{
   //             Debug.Log("Deselect");
   //             ClearSelectedUnits();
   //             isSelecting = false;

   //         }
   //         // If we press the left mouse button, save mouse location and begin selection
   //         else if (Input.GetMouseButtonDown(selection) && !EventSystem.current.IsPointerOverGameObject())
   //         {
   //             _selectableUnits = GameObject.FindGameObjectsWithTag(playerUnitTag);

   //             // If player clicked on a selectable unit
   //             if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, unit))
   //             {
   //                 if (hit.transform.gameObject.tag == playerUnitTag)
   //                 {
   //                     ClearSelectedUnits();
   //                     hit.transform.gameObject.GetComponent<UnitController>().SetSelected(true);
   //                     _selectedUnits.Add(hit.transform.gameObject);
   //                     isSelecting = false;

   //                     return;
   //                 }
   //             } 
         

   //             isSelecting = true;
   //             mousePosition1 = Input.mousePosition;
   //         }
   //         // If we let go of the left mouse button, end selection
   //         if (isSelecting &&  Input.GetMouseButtonUp(selection) && !EventSystem.current.IsPointerOverGameObject())
   //         {

   //             isSelecting = false;
   //             // Find selectable gameObjects in rectangle
   //             foreach (GameObject go in _selectableUnits)
   //                 if (IsWithinSelectionBounds(go))
   //                 {
   //                     go.GetComponent<UnitController>().SetSelected(true);
   //                     _selectedUnits.Add(go);
   //                 }
   //                 else
   //                 {
   //                     go.GetComponent<UnitController>().SetSelected(false);
   //                     _selectedUnits.Remove(go);
   //                 }
   //         }

   //         // Right mouse button : Check for new destination
   //         if ( Input.GetMouseButtonDown(destination) && !EventSystem.current.IsPointerOverGameObject())
   //         {
   //             if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, ground))
   //             {
   //                 //GameObject.Instantiate(DestinationPrefab, hit.point, Quaternion.identity).transform.eulerAngles = Vector3.left * 90;
   //                 BroadcastNewTarget(hit.point, Input.GetKey(KeyCode.LeftShift));
   //             }
   //         } 
        }

        private void FixedUpdate()
        {
            //4. Is Mouse hovering
            if (_selectedUnits.Count != 0)
            {
                pointer.SetActive(true);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raycast from   mouse pointer position

                if (Physics.Raycast(ray, out hit, 50000.0f, ground)) //if we hit ground
                {
                    pointer.transform.position = hit.point;
                }
            } else
			{
                pointer.SetActive(false);

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
