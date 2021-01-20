using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using TouchBehaviours;
using UnityEngine;

namespace PurpleFlame
{
    public class ObjectRotation : LeanDrag
    {
        [SerializeField] private float clampValueX;
        [SerializeField] private float clampValueY;
        [SerializeField] private float forceMultiplier;
        [SerializeField] private bool invertedCam;

        public float ClampValueX { get => clampValueX; set => value = clampValueX; }
        public float ClampValueY { get => clampValueY; set => value = clampValueY; }

        private Vector3 firstpoint, secondpoint;
        private Vector3 momentum;
        private Vector3 force;
        private float yAngle, xAngle, yAngTemp, xAngTemp = 0f;
        private float xDis;
        private float timePressed;
        private float timer;
        private int invert;
        private bool pressing;
        private bool scriptDisabled = false;

        private Rigidbody rb;

        protected override void Start()
        {
            base.Start();

            if (invertedCam) { invert = -1; }
            else { invert = 1; }

            //this.transform.rotation = Quaternion.Euler(xAngle, yAngle, 0f);
            rb = GetComponent<Rigidbody>();
        }

        public void ResetClamp(Iinspectable incoming)
        {
            //clampValueX = incoming.ClampX().y;
            //clampValueY = incoming.ClampY().y;
            rb.angularDrag = 0;
        }

        private void Update()
        {
            xAngle = ClampAngle(transform.eulerAngles.x, -clampValueX, clampValueX);
            yAngle = ClampAngle(transform.eulerAngles.y, -clampValueY, clampValueY);
            this.transform.rotation = Quaternion.Euler(xAngle, yAngle, 0f);

            if (scriptDisabled) 
            {
                timer = 0.5f;
                return; 
            }
            else
            {
                if(timer > 0) 
                {
                    timer = Timer(timer);
                    return; 
                }
            }


            RotateInput();


            if (pressing)
            {
                timePressed += Time.deltaTime;
            }
            else
            {
                if(transform.eulerAngles.x > 20)
                {
                    rb.angularDrag = 10;
                }
                else if (transform.eulerAngles.x < -20) 
                {
                    rb.angularDrag = 10;
                }

                SlowDownCam();
            }
        }

        private void SlowDownCam()
        {
            if (timePressed > 0)
            {
                timePressed -= Time.deltaTime * 0.25f;
                rb.angularDrag += 0.05f;
                return;
            }

            if (rb.angularDrag > 10) { return; }
            rb.angularDrag += 0.05f;
        }

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            
            if (scriptDisabled) { return; }
            base.OnFingerDown(finger);
            Reset();
            pressing = true;
            firstpoint = new Vector3(touchingFingers[0].ScreenPosition.x * invert, touchingFingers[0].ScreenPosition.y * invert, 0f);
            yAngTemp = yAngle;
            xAngTemp = xAngle;

        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);
            if (scriptDisabled) { return; }
            FingerReleased();
        }

        private void FingerReleased()
        {
            pressing = false;
            //AddForce();
        }

        private void RotateInput()
        {
            if (touchingFingers.Count > 0 && pressing)
            {
                secondpoint = new Vector3(touchingFingers[0].ScreenPosition.x * invert, touchingFingers[0].ScreenPosition.y * invert, 0f);
                xAngle = ClampAngle(xAngTemp - (secondpoint.y - firstpoint.y) * 90f / Screen.height, -clampValueX, clampValueX);
                yAngle = ClampAngle(yAngTemp + (secondpoint.x - firstpoint.x) * 180f / Screen.width, -clampValueY, clampValueY);
                xDis = secondpoint.x - firstpoint.x;
                this.transform.rotation = Quaternion.Euler(xAngle, yAngle, 0f);
                if (Mathf.Abs(xDis) > 1200) { FingerReleased(); }
            }
        }

        private void AddForce()
        {
            momentum = secondpoint - firstpoint;
            Vector2 _force = new Vector2(momentum.x * forceMultiplier, momentum.y * forceMultiplier);
            float forceX = momentum.x / timePressed;
            float forceY = momentum.y / timePressed;
            if (Mathf.Abs(forceX) > 1000) 
            {
                rb.AddTorque(transform.up * _force.x); 
            }
            if (Mathf.Abs(forceY) > 300)
            {
                rb.AddTorque(-transform.right * _force.y);
            }

            //rb.AddTorque(transform.forward * _force.y);
        }

        static float ClampAngle(float angle, float min, float max)
        {
            if (min < 0 && max > 0 && (angle > max || angle < min))
            {
                angle -= 360;
                if (angle > max || angle < min)
                {
                    if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                    else return max;
                }
            }
            else if (min > 0 && (angle > max || angle < min))
            {
                angle += 360;
                if (angle > max || angle < min)
                {
                    if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                    else return max;
                }
            }

            if (angle < min) return min;
            else if (angle > max) return max;
            else return angle;
        }

        private void Reset()
        {
            timePressed = 0f;
            rb.angularDrag = 0f;
        }

        public void DisableScript(bool b)
        {
            scriptDisabled = b;
        }

        public void InvertCameraInput()
        {
            invertedCam = !invertedCam;

            if (invertedCam) { invert = -1; }
            else { invert = 1; }
        }

        private float Timer(float timer)
        {
            timer -= Time.deltaTime;
            return timer;
        }

        #region Singleton
        private static ObjectRotation instance;

        private void Awake()
        {
            instance = this;
        }

        public static ObjectRotation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ObjectRotation();
                }

                return instance;
            }
        }
        #endregion
    }
}