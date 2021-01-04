using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public class Touch3DMovementManager : MonoBehaviour
    {
        GameObject SelectedElement = null;
        public List<TouchLocation> Touches = new List<TouchLocation>();
        public LayerMask Layers;
         float RotateSpeed = 1f;
        private float startingPosition;

        private Ray GenerateMouseRay()
        {
            Vector3 mousePositionFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);

            Vector3 mousePositionNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

            Vector3 mousePositionF = Camera.main.ScreenToWorldPoint(mousePositionFar);
            Vector3 mousePositionN = Camera.main.ScreenToWorldPoint(mousePositionNear);

            Ray r = new Ray(mousePositionN, mousePositionF - mousePositionN);
            return r;
        }

        private void Update()
        {
            int i = 0;
            while (i < Input.touchCount)
            {
                Touch t = Input.GetTouch(i);

                if (Input.touchCount == 1)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        Ray mouseRay = GenerateMouseRay();
                        RaycastHit hit;

                        if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit))
                        {
                            SelectedElement = hit.transform.gameObject;
                            Touches.Add(new TouchLocation(t.fingerId, SelectedElement));
                        }
                    }
                    else if (t.phase == TouchPhase.Ended)
                    {
                        TouchLocation thisTouchLocation = Touches.Find(tl => tl.TouchId == t.fingerId);
                        Touches.RemoveAt(Touches.IndexOf(thisTouchLocation));
                        SelectedElement = null;

                    }
                    else if (t.phase == TouchPhase.Moved)
                    {
                        if (SelectedElement.GetComponent<IMovable>() != null)
                        {
                            Vector3 newPosition = GetTouchPosition(t.position);

                            SelectedElement.transform.position = new Vector3(newPosition.x, newPosition.y, SelectedElement.transform.position.z);
                        }
                        else if (SelectedElement.GetComponent<Iinspectable>() != null)
                        {
                            SelectedElement.transform.Rotate(t.deltaPosition.y * RotateSpeed, t.deltaPosition.x * RotateSpeed, 0, Space.World);
                        }
                    }
                    
                }
                ++i;
            }
            /*
            if (Input.GetMouseButtonDown(0))
            {
                Ray mouseRay = GenerateMouseRay();
                RaycastHit hit;

                if(Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit))
                {
                    SelectedElement = hit.transform.gameObject;
                }
            }
            else if(Input.GetMouseButton(0) && SelectedElement)
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                SelectedElement.transform.position = new Vector3(newPosition.x, newPosition.y, SelectedElement.transform.position.z);
            }
            else if(Input.GetMouseButtonUp(0)&& SelectedElement)
            {
                SelectedElement = null;
            }*/
        }

        private Vector2 GetTouchPosition(Vector2 touchPosition)
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z));
        }
    }
}
