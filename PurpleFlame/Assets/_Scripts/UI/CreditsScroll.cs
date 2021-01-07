using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    private Vector3 beginPos;

    private Vector2 beginTouchPos;
    private Vector2 currentTouchPos;

    private bool beginPhaseMouse;

    [SerializeField] private float scrollSpeed;
    //[SerializeField] private float minY;
    //[SerializeField] private float maxY;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;

    private float t;
    private float tPlus;

    void Start()
    {
        t = 0;
        tPlus = t;
        beginPhaseMouse = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (beginPhaseMouse)
            {
                beginPos = transform.localPosition;
                //beginTouchPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.nearClipPlane));
                beginTouchPos = Input.mousePosition;
                beginPhaseMouse = false;
                //Debug.Log($"beginPos = {beginPos}, beginTouchPos = {beginTouchPos}");
            }

            //currentTouchPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            currentTouchPos = Input.mousePosition;
            float _yDifference = scrollSpeed*(currentTouchPos.y - beginTouchPos.y);
            //float _newY = Mathf.Clamp(beginPos.y + _yDifference, minY, maxY);
            //Vector3 _newPos = beginPos + _yDifference*transform.up;
            tPlus = t + _yDifference;
            t = Mathf.Clamp(t, 0, 1);
            tPlus = Mathf.Clamp(tPlus, 0, 1);
            Vector3 _newPos = Vector3.Lerp(startPos, endPos, tPlus);

            transform.localPosition = _newPos;
            Debug.DrawLine(beginPos, _newPos, Color.cyan);
            Debug.DrawLine(beginTouchPos, currentTouchPos, Color.green);

        }
        else
        {
            t = tPlus;
            //Debug.Log($"t = {t}");
            beginPhaseMouse = true;
        }
    }

    public void ResetScroll()
    {
        t = 0;
        tPlus = t;
    }
}
