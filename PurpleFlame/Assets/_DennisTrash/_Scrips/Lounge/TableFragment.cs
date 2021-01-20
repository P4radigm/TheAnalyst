using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PurpleFlame
{
    public enum axis
    {
        red,
        blue,
        red_backwards,
        blue_backwards
    }

    public class TableFragment : MonoBehaviour
    {
        [SerializeField] private axis moveAxis;
        [SerializeField] private float moveDistance;
        [SerializeField] float moveAnimDuration;
        [SerializeField] AnimationCurve moveAnimCurve;
        private Coroutine moveAnimRoutine;
        private Vector3 moveVector;



        //[SerializeField] private float moveTableSpeed;

        //[HideInInspector] public bool moveTable = false;
        //private Vector3 startPosition;
        //private Vector3 endPosition;

        private void Start()
        {
            if(moveAxis == axis.red)
            {
                moveVector = transform.right;
            }
            else if(moveAxis == axis.blue)
            {
                moveVector = transform.forward;
            }
            else if(moveAxis == axis.red_backwards)
            {
                moveVector = transform.right * -1;
            }
            else if(moveAxis == axis.blue_backwards)
            {
                moveVector = transform.forward * -1;
            }

            //startPosition = this.transform.position;
            //endPosition = startPosition + moveToPos;
        }

        //private void Update()
        //{
        //    if (moveTable && transform.position != endPosition)
        //    {
        //        this.transform.position = Vector3.Lerp(this.transform.position, endPosition, moveTableSpeed);
        //    }

        //    if (!moveTable && transform.position != startPosition)
        //    {
        //        this.transform.position = Vector3.Lerp(this.transform.position, startPosition, moveTableSpeed);
        //    }
        //}

        public void StartMoveTableAnim()
        {
            if (moveAnimRoutine != null) { return; }
            else { moveAnimRoutine = StartCoroutine(MoveTableAnimIE()); }
        }

        private IEnumerator MoveTableAnimIE()
        {
            RopeButtonSeperate[] _connectedButtons = GetComponentsInChildren<RopeButtonSeperate>();

            _connectedButtons[0].rM.enabled = false;

            float _timeValue = 0;
            Vector3 _oldPos = transform.position;
            Vector3 _newPos = transform.position + moveVector * moveDistance;

            while(_timeValue < 1)
            {
                _timeValue += Time.deltaTime / moveAnimDuration;
                float _evaluatedLerpTime = moveAnimCurve.Evaluate(_timeValue);
                Vector3 _updatePos = Vector3.Lerp(_oldPos, _newPos, _evaluatedLerpTime);

                transform.position = _updatePos;
                
                yield return null;
            }

            _connectedButtons[0].rM.enabled = true;

            if (_connectedButtons != null)
            {
                for (int i = 0; i < _connectedButtons.Length; i++)
                {
                    _connectedButtons[i].CalculatePositions();
                }
            }

            moveAnimRoutine = null;
        }

        //public void TableStatus(bool tableStatus)
        //{
        //    moveTable = tableStatus;
        //}
    }
}
