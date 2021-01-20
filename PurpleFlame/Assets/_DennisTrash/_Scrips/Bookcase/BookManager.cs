using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PurpleFlame
{
    public class BookManager : LeanDrag
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float swipeThreshold;
        [SerializeField] private float timeBeforeResetBook;
        [Space]
        [SerializeField] private GameObject numberCanvas;
        [SerializeField] private TextMeshProUGUI[] bookNumbersUpperRow;
        [SerializeField] private TextMeshProUGUI[] bookNumbersSideRow;
        [SerializeField] private Animator animBookCase;

        private float swipe;
        private float currentTimer;
        private bool interactableHit;
        private bool resetBook;
        private bool disableTouch = false;
        private Book lastBookSelected;

        private void Start()
        {
            numberCanvas.SetActive(false);
        }

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerDown(finger);

            if (disableTouch) { return; }

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<Book>())
                {
                    if (resetBook) 
                    {
                        resetBook = false;
                        lastBookSelected.resetBook = true;
                    }
                    interactableHit = true;
                    lastBookSelected = hit.collider.GetComponent<Book>();
                    ObjectRotation.Instance.DisableScript(true);
                }
            }
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);

            interactableHit = false;
            currentTimer = timeBeforeResetBook;

            if (!lastBookSelected) { return; }

            resetBook = true;
            if (lastBookSelected.done) { OpenBookCase(); }
        }

        private void Update()
        {
            if (interactableHit) { SwipeInput(); }
            if (resetBook) { ResetBook(); }
        }

        private void SwipeInput()
        {
            swipe = touchingFingers[0].ScreenDelta.y;

            if (swipe > swipeThreshold) { lastBookSelected.DragBook(1); }
            if (swipe < -swipeThreshold) { lastBookSelected.DragBook(-1); }
        }


        private void ResetBook()
        {
            if(currentTimer <= 0)
            {
                resetBook = false;
                lastBookSelected.resetBook = true;
            }
            else { currentTimer = Timer(currentTimer); }
        }

        private float Timer(float timer)
        {
            timer -= Time.deltaTime;
            return timer;
        }

        private void OpenBookCase()
        {
            disableTouch = true;
            numberCanvas.SetActive(false);
            animBookCase.SetTrigger("OpenBookCase");
        }

        public void SetCanvas(int firstQuestion, int secondQuestion, int thirdQuestion, int fourthQuestion)
        {
            numberCanvas.SetActive(true);

            int s = -2;
            int u = -4;

            for (int i = 0; i < 6; i++)
            {
                int first = firstQuestion;
                int second = secondQuestion;
                int third = thirdQuestion;
                int fourth = fourthQuestion;


                bookNumbersUpperRow[i].text = "";
                bookNumbersSideRow[i].text = "";

                if (second + u < 0) 
                {
                    first -= 1;
                    second = 10 + (second + u);
                    Debug.Log(first);
                    Debug.Log(second);
                }
                else { second += u; }
                bookNumbersUpperRow[i].text = first + "" + second;

                if (fourth + s < 0)
                {
                    third--;
                    fourth = 10 + (fourth + s);
                }
                else { fourth += s; }
                bookNumbersSideRow[i].text = third + "" + fourth;

                s++;
                u++;
            }
        }
    }
}