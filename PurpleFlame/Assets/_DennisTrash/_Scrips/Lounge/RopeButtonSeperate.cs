using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public class RopeButtonSeperate : MonoBehaviour
    {

        [HideInInspector] public RopeManager rM;
        [SerializeField] private RopeSymbol symbol;
        [SerializeField] private float moveDistance;
        [SerializeField] private float buttonAnimDuration;
        [SerializeField] private AnimationCurve buttonAnimCurve;
        private Collider col;
        private Coroutine buttonAnimRoutine;
        private Vector3 inPos;
        private Vector3 outPos;

        void Start()
        {
            col = GetComponent<Collider>();
            CalculatePositions();
        }

        public void CalculatePositions()
        {
            inPos = transform.position;
            outPos = transform.position + moveDistance * transform.forward;
        }

        public void ButtonMove(bool _GoOut)
        {
            if (buttonAnimRoutine != null)
            {
                StopCoroutine(buttonAnimRoutine);
            }
            buttonAnimRoutine = StartCoroutine(ButtonMoveIE(_GoOut));
        }

        private IEnumerator ButtonMoveIE(bool _GoOut)
        {
            float _timeValue = 0;
            Vector3 _currentPos = transform.position;
            Vector3 _newPos;

            if (_GoOut)
            {
                _newPos = outPos;
            }
            else
            {
                _newPos = inPos;
            }

            while (_timeValue < 1)
            {
                _timeValue += Time.deltaTime / buttonAnimDuration;
                float _evaluatedLerpTime = buttonAnimCurve.Evaluate(_timeValue);
                Vector3 _updatePos = Vector3.Lerp(_currentPos, _newPos, _evaluatedLerpTime);

                transform.position = _updatePos;

                yield return null;
            }

            buttonAnimRoutine = null;

            if (_GoOut)
            {
                col.enabled = true;
            }
            else
            {
                col.enabled = false;
                rM.ropeList[(int)symbol].StartPullAnim(false);
            }
        }
    }
}
