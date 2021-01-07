using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinch : MonoBehaviour
{
    [SerializeField] private float minimumSingleDistance;
    [SerializeField] private float minimumTotalDistance;
    [SerializeField] private float dotBorder;

    private bool firstPhasePinch;
    private Vector2 oneFirstPos;
    private Vector2 twoFirstPos;
    private Vector2 oneCurrentPos;
    private Vector2 twoCurrentPos;
    private float oneDistance;
    private float twoDistance;
    private float dot;
    private MainMenuController mmC;
    private MainMenuEventManager mmEm;

    void Start()
    {
        firstPhasePinch = true;
        mmC = FindObjectOfType<MainMenuController>();
        mmEm = MainMenuEventManager.instance;
    }


    void Update()
    {
        if(Input.touchCount >= 2)
        {
            mmC.UpdateButtonList();

            for (int i = 0; i < mmC.buttonList.Count; i++)
            {
                mmC.buttonList[i].enabled = false;
            }

            Touch _touchOne = Input.GetTouch(0);
            Touch _touchTwo = Input.GetTouch(1);

            if (firstPhasePinch)
            {
                oneFirstPos = _touchOne.position;
                twoFirstPos = _touchTwo.position;

                //Debug.Log($"oneFirstPos = {oneFirstPos}");
                //Debug.Log($"twoFirstPos = {twoFirstPos}");
                firstPhasePinch = false;
            }

            oneCurrentPos = _touchOne.position;
            twoCurrentPos = _touchTwo.position;

            oneDistance = Vector2.Distance(oneFirstPos, oneCurrentPos);
            twoDistance = Vector2.Distance(twoFirstPos, twoCurrentPos);

            //Debug.Log(_touchOne.deltaPosition);
            //Debug.Log(_touchTwo.deltaPosition);

            Vector2 _oneVector = new Vector2(oneFirstPos.x - oneCurrentPos.x, oneFirstPos.y - oneCurrentPos.y);
            Vector2 _twoVector = new Vector2(twoFirstPos.x - twoCurrentPos.x, twoFirstPos.y - twoCurrentPos.y);

            dot = Vector2.Dot(_oneVector.normalized, _twoVector.normalized);

        }
        else
        {
            float _distanceFirst = Vector2.Distance(oneFirstPos, twoFirstPos);
            float _distanceCurrent = Vector2.Distance(oneCurrentPos, twoCurrentPos);

            for (int i = 0; i < mmC.buttonList.Count; i++)
            {
                mmC.buttonList[i].enabled = true;
            }

            firstPhasePinch = true;

            if (oneDistance > minimumSingleDistance && twoDistance > minimumSingleDistance && oneDistance + twoDistance >= minimumTotalDistance)
            {
                if (_distanceFirst > _distanceCurrent)
                {
                    if(dot <= dotBorder)
                    {
                        Debug.LogWarning("Pinched Inwards");
                        mmEm.PinchedInwards();
                    }
                    else
                    {
                        Debug.Log($"no dot, {dot}");
                    }
                }
                else
                {
                    Debug.Log($"no distance, first = {_distanceFirst} & current = {_distanceCurrent}");
                }
            }
            else if(oneDistance != 0 && twoDistance != 0)
            {
                Debug.Log($"no single or total distance");
            }

            oneDistance = 0;
            twoDistance = 0;
            oneFirstPos = Vector2.zero;
            oneCurrentPos = Vector2.zero;
            twoFirstPos = Vector2.zero;
            twoCurrentPos = Vector2.zero;

        }

        Debug.DrawLine(oneFirstPos, oneCurrentPos, Color.green);
        Debug.DrawLine(twoFirstPos, twoCurrentPos, Color.red);
    }
}
