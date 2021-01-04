using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    [HideInInspector] public bool resetBook;
    [HideInInspector] public bool done;

    [SerializeField] private bool correctBook;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float maxRotateAngle;
    [Range(0, 0.5f)]
    [SerializeField] private float resetSpeed;

    private Vector3 v3;

    private void Start()
    {
        v3 = transform.localEulerAngles;
    }

    private void Update()
    {
        if (!resetBook) { return; }

        if(Mathf.Abs(v3.z - 0) > 2)
        {
            v3.z = Mathf.Lerp(v3.z, 0, resetSpeed);
            transform.localEulerAngles = v3;
        }
        else { resetBook = false; }
    }

    public void DragBook(int value)
    {
        v3.z += rotateSpeed * value * Time.deltaTime;

        if (v3.z > -maxRotateAngle && v3.z <= 0) 
        { 
            transform.localEulerAngles = v3;
            if(v3.z < -maxRotateAngle + (maxRotateAngle * 0.25f) && correctBook)
            {
                done = true;
            }
        }
    }
}
