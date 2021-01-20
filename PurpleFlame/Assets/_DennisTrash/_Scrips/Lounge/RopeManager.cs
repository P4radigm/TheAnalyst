using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class RopeManager : LeanDrag
    {
        [Header("Gameplay")]
        [SerializeField] private RopeCombination[] ropeCombination;

        [Header("Necessary data")]
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float moveDragThreshold;
        [SerializeField] private float moveTableDelay;

        [Header("Table Objects")]
        public Rope[] ropeList;
        [SerializeField] private TableFragment[] tableFragmentList;
        public RopeButtonSeperate[] buttonList;
        [SerializeField] private Letter letter;

        [Header("Chess piece")]
        [SerializeField] private GameObject pedestal;
        [SerializeField] private float pedestalMoveTo;
        [SerializeField] private float rewardAnimDuration;
        [SerializeField] private AnimationCurve rewardAnimCurve;
        //[SerializeField] private float pedestalMoveSpeed;

        private float swipeDistance;
        //private float timeToReset;
        private int currentLevel;
        private bool swipeRecognised;
        private bool readyToUse = true;
        private bool finished;
        private Rope selectedRope;
        private RopeButton ropeButton;
        private RopeButtonSeperate ropeButtonSeperate;

        private Vector3 endPosPedestal;

        [System.Serializable]
        public class RopeCombination
        {
            public bool[] combinations;
            [HideInInspector] public bool isCompleted = false;
        }

        private void Start()
        {
            for (int i = 0; i < buttonList.Length; i++)
            {
                buttonList[i].rM = this;
            }

            for (int i = 0; i < ropeList.Length; i++)
            {
                ropeList[i].rM = this;
            }

            if (letter != null) { readyToUse = false; }
            endPosPedestal = new Vector3(pedestal.transform.position.x, pedestal.transform.position.y + pedestalMoveTo, pedestal.transform.position.z);
        }

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerDown(finger);
            if (finished) { return; }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<Rope>())
                {
                    selectedRope = hit.collider.gameObject.GetComponent<Rope>();
                    ObjectRotation.Instance.DisableScript(true);
                }

                //if (hit.collider.gameObject.GetComponent<RopeButton>())
                //{
                //    ropeButton = hit.collider.gameObject.GetComponent<RopeButton>();
                //    ropeButton.PressedButton();
                //    ButtonPressed();
                //}

                if (hit.collider.gameObject.GetComponent<RopeButtonSeperate>())
                {
                    ropeButtonSeperate = hit.collider.gameObject.GetComponent<RopeButtonSeperate>();
                    ropeButtonSeperate.ButtonMove(false);
                }
            }
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);
            if (finished) { return; }
            selectedRope = null;
            swipeRecognised = false;
            swipeDistance = 0;
            ObjectRotation.Instance.DisableScript(false);

            if(letter == null && !readyToUse) 
            { 
                readyToUse = true;
                //ButtonPressed();

                for (int i = 0; i < buttonList.Length; i++)
                {
                    buttonList[i].ButtonMove(false);
                }
            }
        }

        private void Update()
        {
            if (finished) { return; }
            //if(finished && pedestal.transform.position != endPosPedestal)
            //{
            //    pedestal.transform.position = Vector3.Lerp(pedestal.transform.position, endPosPedestal, pedestalMoveSpeed);
            //}
            //else if (finished) { return; }


            //MoveTable();
            if (selectedRope == null) { return; }
            SwipeInput();
        }

        //private void MoveTable()
        //{
        //    if (timeToReset != 0)
        //    {
        //        timeToReset = Timer(timeToReset);

        //        if (timeToReset < 0.1f)
        //        {
        //            timeToReset = 0f;
        //            tableFragmentList[currentLevel].StartMoveTableAnim();
        //            //tableFragmentList[currentLevel].TableStatus(true);
        //            currentLevel++;
        //            if (currentLevel == 4)
        //            {
        //                finished = true;
        //                return;
        //            }
        //            //ButtonPressed();
        //            for (int i = 0; i < buttonList.Length; i++)
        //            {
        //                buttonList[i].ButtonMove(false);
        //            }
        //        }
        //    }
        //}

        private IEnumerator MoveTable(int _segment)
        {
            yield return new WaitForSeconds(moveTableDelay-1f);

            for (int i = 0; i < buttonList.Length; i++)
            {
                buttonList[i].ButtonMove(false);
            }

            ropeCombination[_segment].isCompleted = true;
            yield return new WaitForSeconds(1);

            tableFragmentList[_segment].StartMoveTableAnim();
            currentLevel++;
            if (currentLevel == 4)
            {
                finished = true;
                //Start final anim
                StartCoroutine(RewardAnim());
                yield break;
            }
        }

        private IEnumerator RewardAnim()
        {
            yield return new WaitForSeconds(1.5f);

            float _timeValue = 0;
            Vector3 _oldPos = pedestal.transform.position;
            Vector3 _newPos = endPosPedestal;

            while (_timeValue < 1)
            {
                _timeValue += Time.deltaTime / rewardAnimDuration;
                float _evaluatedLerpTime = rewardAnimCurve.Evaluate(_timeValue);
                Vector3 _updatePos = Vector3.Lerp(_oldPos, _newPos, _evaluatedLerpTime);

                pedestal.transform.position = _updatePos;

                yield return null;
            }

        }

        private void SwipeInput()
        {
            if (touchingFingers.Count > 0)
            {
                if (!swipeRecognised)
                {
                    swipeDistance -= touchingFingers[0].ScreenDelta.y;

                    if(swipeDistance > moveDragThreshold)
                    {
                        swipeRecognised = true;
                        selectedRope.StartPullAnim(true);
                        //CheckCombination();
                    }
                }
            }
        }

        //private void CheckCombination()
        //{
        //    if (!readyToUse) { return; }
        //    int totalCorrect = 0;

        //    for (int i = 0; i < ropeList.Length; i++)
        //    {
        //        if(ropeList[i].pulledDown != ropeCombination[currentLevel].combinations[i]) { return; }
        //        else { totalCorrect++; }
        //    }

        //    if(totalCorrect == 6)
        //    {
        //        timeToReset = 1f;
        //    }
        //}

        public void CheckCombinations()
        {
            if (!readyToUse) { return; }

            for (int i = 0; i < ropeCombination.Length; i++)
            {
                int check = 0;

                for (int j = 0; j < ropeList.Length; j++)
                {
                    if(ropeList[j].pulledDown == ropeCombination[i].combinations[j])
                    {
                        check++;
                    }
                }

                if(check == ropeList.Length && !ropeCombination[i].isCompleted)
                {
                    StartCoroutine(MoveTable(i));
                }
            }
        }

        //public void ButtonPressed()
        //{
        //    for (int i = 0; i < ropeList.Length; i++)
        //    {
        //        ropeList[i].StartPullAnim(false);
        //    }
        //}

        //private float Timer(float timer)
        //{
        //    timer -= Time.deltaTime;
        //    return timer;
        //}
    }
}