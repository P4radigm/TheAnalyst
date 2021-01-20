using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public enum RopeSymbol
    {
        Death,
        Legacy,
        Family,
        Science,
        Secrets,
        Ethics
    }

    public class Rope : MonoBehaviour
    {
        [SerializeField] private RopeSymbol ropeSymbol;
        [SerializeField] private float pulledDownDis;
        //[SerializeField] private float pulledDownSpeed;

        [HideInInspector] public bool pulledDown = false;
        private Vector3 startPosition;
        private Vector3 endPosition;
        [SerializeField] private float pullAnimDuration;
        [SerializeField] private AnimationCurve pullAnimCurve;
        private Coroutine pullRoutine;
        [HideInInspector] public RopeManager rM;

        private void Start()
        {
            startPosition = this.transform.position;
            endPosition = new Vector3(startPosition.x, startPosition.y - pulledDownDis, startPosition.z);
        }


        public void StartPullAnim(bool _goDown)
        {
            if (pullRoutine != null) { StopCoroutine(pullRoutine); }
            pullRoutine = StartCoroutine(PullAnimIE(_goDown));
        }

        private IEnumerator PullAnimIE(bool _goDown)
        {
            float _timeValue = 0;
            Vector3 _currentPos = transform.position;
            Vector3 _newPos = transform.position;
            if (_goDown) { _newPos = endPosition; pulledDown = true; }
            else { _newPos = startPosition; pulledDown = false; }

            while (_timeValue < 1)
            {
                _timeValue += Time.deltaTime / pullAnimDuration;
                float _evaluatedLerpTime = pullAnimCurve.Evaluate(_timeValue);
                Vector3 _updatePos = Vector3.Lerp(_currentPos, _newPos, _evaluatedLerpTime);

                transform.position = _updatePos;

                yield return null;
            }

            if (_goDown) { rM.buttonList[(int)ropeSymbol].ButtonMove(true);}

            rM.CheckCombinations();

            pullRoutine = null;
        }


        //private void Update()
        //{
        //    if(pulledDown && this.transform.position != endPosition)
        //    {
        //        this.transform.position = Vector3.Lerp(this.transform.position, endPosition, pulledDownSpeed);
        //    }

        //    if (!pulledDown && this.transform.position != startPosition)
        //    {
        //        this.transform.position = Vector3.Lerp(this.transform.position, startPosition, pulledDownSpeed);
        //    }
        //}

        //public void RopeStatus(bool pullRope)
        //{
        //    pulledDown = pullRope;
        //}
    }
}