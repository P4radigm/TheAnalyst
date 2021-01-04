using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    public GameObject UISlot;
    public GameObject NewPrefab;
    public LayerMask LayerToDropAt;


    private Ray GenerateMouseRay()
    {
        Vector3 mousePositionFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);

        Vector3 mousePositionNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

        Vector3 mousePositionF = Camera.main.ScreenToWorldPoint(mousePositionFar);
        Vector3 mousePositionN = Camera.main.ScreenToWorldPoint(mousePositionNear);

        Ray r = new Ray(mousePositionN, mousePositionF - mousePositionN);
        return r;
    }

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = transform as RectTransform;

        if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            Debug.Log("Drop item out of inventory");
            // Ray rayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Generate a ray from MousePosition(touchPosition 0 )
            Ray mouseRay = GenerateMouseRay();

            //Create a raycasthit
            RaycastHit hit;
            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, Mathf.Infinity, LayerToDropAt))
            {
                GameObject.Instantiate(NewPrefab, hit.transform.position - new Vector3(0,0,0.07f), hit.transform.rotation);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Currently using transform.position as raycast origin, could easily be something else if desired.
        var rayCastOrigin = transform.position;

        // Save the current layer the dropped object is in,
        // and then temporarily place the object in the IgnoreRaycast layer to avoid hitting self with Raycast.
        int oldLayer = gameObject.layer;
        gameObject.layer = 2;

        var raycastOriginInScreenSpace = Camera.main.WorldToScreenPoint(rayCastOrigin);
        var screenRay = Camera.main.ScreenPointToRay(new Vector3(raycastOriginInScreenSpace.x, raycastOriginInScreenSpace.y, 0.0f));

        // Perform Physics.Raycast from transform and see if any 3D object was under transform.position on drop.
        RaycastHit hit3D;
        if (Physics.Raycast(screenRay, out hit3D))
        {
            var dropComponent = hit3D.transform.gameObject.GetComponent<IDropHandler>();
            if (dropComponent != null)
                dropComponent.OnDrop(eventData);
        }

        // Perform Physics2D.GetRayIntersection from transform and see if any 2D object was under transform.position on drop.
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(screenRay);
        if (hit2D)
        {
            var dropComponent = hit2D.transform.gameObject.GetComponent<IDropHandler>();
            if (dropComponent != null)
                dropComponent.OnDrop(eventData);
        }

        // Reset the object's layer to the layer it was in before the drop.
        gameObject.layer = oldLayer;
    }
}
