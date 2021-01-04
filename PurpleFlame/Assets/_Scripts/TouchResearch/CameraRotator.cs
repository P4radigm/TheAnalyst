using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasHelpers;
using TouchBehaviours;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject target;
    [SerializeField] private float cameraMovementSpeed = 5f;
    [SerializeField] private float clampRotationAt = 180f;
    [SerializeField] private CameraTarget cameraFollowTarget;
 
    private Vector3 previousPosition;
    private int requiredTabs = 1;
    private int countedTabs = 0;
    private bool dubbelTap = false;

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
        if(Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = GenerateMouseRay();
            RaycastHit hit;

            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit))
            {
                if (hit.transform.gameObject != null && hit.transform.gameObject.HasComponent<Iinspectable>())
                {
                    if (target == null)
                        target = new GameObject();
                    if (target != hit.transform.gameObject)
                    {
                        target = hit.transform.gameObject;
                        dubbelTap = false;
                    }
                    else
                    {
                        countedTabs++;
                        dubbelTap = true;
                        target = null;
                    }

                    if (dubbelTap)
                    {
                        target = hit.transform.gameObject;

                        //cameraFollowTarget.MoveToNewTarget(target.transform, 1, );
                    }
                }
                else
                    return;
            }
            //cam.transform.LerpTransform(this, target.transform.position, cameraMovementSpeed);

            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        /*
        int i = 0;
        while (i < Input.touchCount)
        {
            Touch t = Input.GetTouch(i);

            if (Input.touchCount == 1)
            {
                if (t.phase == TouchPhase.Began)
                {
                    
                }
                else if (t.phase == TouchPhase.Ended)
                {
                    

                }
                else if (t.phase == TouchPhase.Moved)
                {
                    if (target == null) return;

                    Vector3 dir = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);
                    //cam.transform.position = target.transform.position;


                    cam.transform.Rotate(axis: new Vector3(1, 0, 0), angle: dir.y * 180);
                    cam.transform.Rotate(axis: new Vector3(0, 1, 0), angle: -dir.x * 180, relativeTo: Space.World);
                    //cam.transform.Translate(0, 0, -10);
                    previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
                }

            }
            ++i;
        }*/
    }
}
 