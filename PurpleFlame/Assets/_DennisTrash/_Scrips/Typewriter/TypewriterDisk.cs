using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class TypewriterDisk : MonoBehaviour
    {
        //[HideInInspector] public int currentRound;
        //public float currentPercentageDisk;

        //[SerializeField] private float angleThreshHold;
        //[SerializeField] private float rotateSpeed;

        //[SerializeField] private float[] answerFloats;
        //[SerializeField] private Transform middlePoint;
        [SerializeField] private TypewriterUI typeWriterUI;
        [SerializeField] private GameObject layerMask;
        [SerializeField] private Transform rollerCog;

        private float angleLastFrame = 0;
        private float angle = 0;
        //private float xRot;
        //private float xRotLastFrame;
        //private Camera camera;

        private void Start()
        {
            //camera = Camera.main;
        }

        /*
        public void UpdateDisk(int value)
        {
            moving = true;
            //v3.y += rotateSpeed * value * Time.deltaTime;

            if (currentPercentageDisk < 0 && currentPercentageDisk > -8) 
            { 
                //transform.localEulerAngles = v3; 
            }

            if (v3.y >= 360) 
            {
                currentRound++;
               // v3.y -= 360; 
            }
            if (v3.y <= -360) 
            {
                currentRound--;
                //v3.y += 360; 
            }

            if(currentRound >= 0) { currentRound = 0; }
            if (currentRound <= -8) { currentRound = -8; }

            currentPercentageDisk = (v3.y / 360) + currentRound;
            if (currentPercentageDisk > 0) { currentPercentageDisk = 0; }
            if (currentPercentageDisk < -8) { currentPercentageDisk = -8; }
        }*/


        //public void UpdateDiskPos(Vector3 hitPos, Vector3 hitNormal)
        //{
        //    Vector3 mouseDir = hitPos - LayerMask.transform.position;
        //    //var angle = Vector3.SignedAngle(this.transform.forward, mouseDir, transform.up); OLD
        //    //var angle = Vector3.SignedAngle(LayerMask.transform.up, mouseDir, LayerMask.transform.right);
        //    if (Mathf.Abs(angle) > angleThreshHold)
        //    {
        //        //transform.Rotate(Vector3.up, rotateSpeed * Time.fixedDeltaTime * Mathf.Sign(angle));
        //        yRotLastFrame = transform.eulerAngles.y;
        //        Debug.DrawLine(LayerMask.transform.position, angle * 100f, Color.green);
        //        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(mouseDir, LayerMask.transform.right), rotateSpeed * Time.deltaTime);
        //        yRot = transform.eulerAngles.y;

        //        if (angle - angleLastFrame > 0) { typeWriterUI.UpdatePaper(1); }
        //        else { typeWriterUI.UpdatePaper(-1); }
        //        angleLastFrame = angle;

        //        //if (yRot - yRotLastFrame > 0) { typeWriterUI.UpdatePaper(1); }
        //        //else { typeWriterUI.UpdatePaper(-1); }
        //    }
        //}

        public void UpdateDiskPos(Vector3 hitPos)
        {
            //xRotLastFrame = transform.eulerAngles.x;

            Vector3 dirVec = hitPos - layerMask.transform.position;

            if(dirVec.magnitude < 0.02f) { return; }

            dirVec = dirVec.normalized;

            //float _newAngle = Vector3.Angle(LayerMask.transform.up, dirVec);

            Debug.DrawLine(layerMask.transform.position, layerMask.transform.position + layerMask.transform.InverseTransformDirection(dirVec), Color.red);
            Debug.DrawLine(layerMask.transform.position, layerMask.transform.position + layerMask.transform.up, Color.green);

            Vector3 localDirVec = layerMask.transform.InverseTransformDirection(dirVec);

            float _angle = Mathf.Atan2(localDirVec.z, localDirVec.y) * Mathf.Rad2Deg;

            angle = _angle;

            transform.localRotation = Quaternion.Euler(0, _angle, 0);
            rollerCog.localRotation = Quaternion.Euler(0, 0, _angle*-1);


            float _difference = angle - angleLastFrame;

            //if(_difference != 0)
            //Debug.Log($"angle = {angle}, angleLastFrame = {angleLastFrame}");
            //Debug.Log($"_difference = {_difference}");

            if(Mathf.Abs(_difference) < 200)
            {
                typeWriterUI.UpdatePaper(_difference);
            }

            angleLastFrame = angle;

            //if(hitPos.x < LayerMask.transform.position.x)
            //{
            //    angle += _newAngle - angleLastFrame;
            //}
            //else
            //{
            //    angle -= _newAngle - angleLastFrame;
            //}

            //transform.localEulerAngles = Vector3.right * angle;

            //xRot = transform.eulerAngles.x;            

            //angleLastFrame = _newAngle;
        }
    }
}