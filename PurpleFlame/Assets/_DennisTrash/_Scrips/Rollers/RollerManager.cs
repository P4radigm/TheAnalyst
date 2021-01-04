using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dennis
{
    public class RollerManager : LeanDrag
    {
        [SerializeField] private LayerMask layerTarget;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float moveDragThreshold;
        [SerializeField] private Animator mainDeskAnim;
        [SerializeField] private Letter letter;

        private bool readyToUse = true;
        private bool interactableHit;
        private bool swipeRecognised;
        private int symbolsCorrect;
        private float swipe;
        private RaycastHit hit;

        private List<RollerObject> rollerList = new List<RollerObject>();
        private RollerObject rollerObject;

        private void Start()
        {
            if(letter != null) { readyToUse = false; }
        }

        private void Update()
        {
            SwipeInput();
        }

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerDown(finger);
            InputTouch();
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            //if(rollerObject != null) { Debug.Log(Mathf.RoundToInt(Mathf.Round(rollerObject.v3.z * 10) / 10)); }
            base.OnFingerUp(finger);
            ObjectRotation.Instance.DisableScript(false);

            if(rollerObject != null) 
            {
                rollerObject.moving = false; 
                rollerObject.checkSymbol = true;
            }

            interactableHit = false;
            swipe = 0;

            if(!readyToUse && letter == null) 
            {
                readyToUse = true;
                for (int i = 0; i < rollerList.Count; i++)
                {
                    rollerList[i].checkSymbol = true;
                }
            }
        }

        private void InputTouch()
        {
            if (touchingFingers.Count > 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerTarget))
                {
                    if (!hit.collider.GetComponent<RollerObject>()) { return; }
                    rollerObject = hit.collider.GetComponent<RollerObject>();
                    ObjectRotation.Instance.DisableScript(true);
                    interactableHit = true;
                }
            }
        }

        private void SwipeInput()
        {
            if (!swipeRecognised && interactableHit)
            {
                swipe = touchingFingers[0].ScreenDelta.y;

                if (swipe > moveDragThreshold) { rollerObject.RotateObject(rotateSpeed); }
                if (swipe < -moveDragThreshold) { rollerObject.RotateObject(-rotateSpeed); }
            }
        }

        public void AddRollerToList(RollerObject roller)
        {
            rollerList.Add(roller);
        }

        public void SymbolsCorrectCheck(bool correct)
        {
            if (correct) { symbolsCorrect++; }
            else { symbolsCorrect--; }

            if(symbolsCorrect == 4 && readyToUse)
            {
                Debug.Log("Correct combination");
                for (int i = 0; i < rollerList.Count; i++)
                {
                    rollerList[i].enabled = false;
                }
                mainDeskAnim.SetTrigger("ComboLockCompleted");
                this.enabled = false;
            }
        }

        #region Singleton
        private static RollerManager instance;

        private void Awake()
        {
            instance = this;
        }

        public static RollerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RollerManager();
                }

                return instance;
            }
        }
        #endregion
    }
}