using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class RollerObject : MonoBehaviour
    {
        [HideInInspector] public bool moving;
        [HideInInspector] public bool checkSymbol;

        [SerializeField] private int faces;
        [SerializeField] private int correctSymbol;
        [SerializeField] private float delayPosTime;

        public Vector3 v3;
        private bool correct = false;
        private int symbol = 0;
        private float angle;
        private float angleLerp;
        private float currentTime;

        private void Start()
        {
            checkSymbol = true;
            CorrectSymbol();
            RollerManager.Instance.AddRollerToList(this);
            transform.eulerAngles = v3;
            angle = 360 / faces;
            currentTime = delayPosTime;
        }

        private void Update()
        {
            if (moving) { return; }
            if(currentTime > 0) 
            {
                currentTime = Timer(currentTime);
                return;
            }

            LerpRotation();
            //CorrectSymbol();
        }

        private void LerpRotation()
        {
            if (Mathf.RoundToInt(Mathf.Round(v3.z*10)/10) % angle != 0)
            {
                float checkFLoats = 0;
                checkFLoats = (angle / 2);

                for (int i = 0; i < faces; i++)
                {

                    if (v3.z > (i * angle) - checkFLoats && v3.z < (i * angle) + checkFLoats)
                    {
                        angleLerp = i * angle;
                        symbol = i;
                    }

                    if (i == 0)
                    {
                        if (v3.z > 360 - checkFLoats)
                        {
                            angleLerp = 360;
                            symbol = 0;
                        }
                    }
                }

                v3.z = Mathf.Lerp(v3.z, angleLerp, 0.07f);
                transform.eulerAngles = v3;
            }
            else
            {
                float checkFLoats = (angle / 2);

                for (int i = 0; i < faces; i++)
                {

                    if (v3.z > (i * angle) - checkFLoats && v3.z < (i * angle) + checkFLoats)
                    {
                        symbol = i;
                    }
                }

                CorrectSymbol();
            }
        }

        public void CorrectSymbol()
        {
            if (!checkSymbol) { return; }
            checkSymbol = false;

            if (correctSymbol == symbol && !correct)
            {
                correct = true;
                RollerManager.Instance.SymbolsCorrectCheck(correct);
            }
            if(correctSymbol != symbol && correct)
            {
                correct = false;
                RollerManager.Instance.SymbolsCorrectCheck(correct);
            }
        }

        public void RotateObject(float speed)
        {
            moving = true;
            currentTime = delayPosTime;

            v3.z += speed * Time.deltaTime;
            transform.eulerAngles = v3;

            if(v3.z >= 360) { v3.z -= 360; }
            if (v3.z <= -0) { v3.z += 360; }
        }

        private float Timer(float timer)
        {
            timer -= Time.deltaTime;
            return timer;
        }
    }
}