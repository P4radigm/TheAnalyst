using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PurpleFlame
{
    public class TypewriterUI : MonoBehaviour
    {
        //public int answersCount;
        public bool answersDone = false;
        [HideInInspector] public bool readyToPickUp = false;

        public GameObject paper;
        [SerializeField] private Material greenMat;
        [SerializeField] private Material startMat;
        [SerializeField] private MeshRenderer meshRenderer;
        [Space]
        public TextMeshProUGUI[] answersText;
        public TextMeshProUGUI[] xAnswersText;
        [SerializeField] private TypewriterManager typewriterManager;
        [Space]
        public float[] answerPos;
        [SerializeField] private float maxAnswerDis;
        public float maxAnswerDisForAlign;
        [SerializeField] private float scrollSpeed;
        [SerializeField] private float paperLength;
        [SerializeField] private float pickupLength;
        [SerializeField] private float beginLength;

        [Header("Horizontal Align Anim")]
        [SerializeField] private float haDuration;
        [SerializeField] private AnimationCurve haCurve;
        private Coroutine haRoutine;

        private float newPos;
        private string[] answerHistory;
        private Vector3 v3;
        private Vector3 startPos;
        private int lastAnswer = -2;

        private void Start()
        {
            startPos = paper.transform.localPosition;
            v3 = startPos;
            paper.transform.localPosition = v3;
        }

        public void UpdatePaper(float value)
        {
            v3.y += value * scrollSpeed;

            if (v3.y < startPos.y - beginLength)
            {
                v3.y = startPos.y - beginLength;
            }

            if (!answersDone)
            {
                if (v3.y > startPos.y + paperLength)
                {
                    v3.y = startPos.y + paperLength;
                }
            }
            else
            {
                CheckForPickupLocation();
            }

            paper.transform.localPosition = new Vector3(v3.x, v3.y, paper.transform.localPosition.z);
            CheckNearbyAnswerFields();

            for (int i = 0; i < answerPos.Length; i++)
            {
                if(paper.transform.localPosition.y > (answerPos[i] - maxAnswerDisForAlign*1.5f) || paper.transform.localPosition.y < (answerPos[i] + maxAnswerDisForAlign * 1.5f))
                {
                    if(lastAnswer != typewriterManager.currentAnswer)
                    {
                        if (haRoutine == null) { haRoutine = StartCoroutine(HorizontalAlign(typewriterManager.currentAnswer)); }
                    }
                }
            }
        }

        private IEnumerator HorizontalAlign(int _current)
        {
            if(_current < 0 || _current > typewriterManager.paperZPosPerQuestion.Length) { yield break; }

            float _timeValue = 0;

            float _currentZpos = paper.transform.localPosition.z;

            while (_timeValue < 1)
            {
                _timeValue += Time.deltaTime / haDuration;
                float _evaluatedTimeValue = haCurve.Evaluate(_timeValue);
                float _newZpos = Mathf.Lerp(_currentZpos, typewriterManager.paperZPosPerQuestion[_current], _evaluatedTimeValue);

                paper.transform.localPosition = new Vector3(paper.transform.localPosition.x, paper.transform.localPosition.y, _newZpos);

                yield return null;
            }

            lastAnswer = typewriterManager.currentAnswer;
            haRoutine = null;
        }

        private void CheckForPickupLocation()
        {
            if (v3.y > startPos.y + paperLength + pickupLength)
            {
                v3.y = startPos.y + paperLength + pickupLength;
            }

            if (typewriterManager.paperPickedUp)
            {
                meshRenderer.material = greenMat;
                return;
            }

            if (v3.y < startPos.y + paperLength)
            {
                readyToPickUp = false;
                meshRenderer.material = startMat;
            }
            if (v3.y > startPos.y + paperLength)
            {
                readyToPickUp = true;
                meshRenderer.material = greenMat;
            }
            
        }

        private void CheckNearbyAnswerFields()
        {
            int intClose = 0;

            for (int i = 0; i < answerPos.Length; i++)
            {
                if (Mathf.Abs(v3.y - answerPos[i]) < maxAnswerDis)
                {
                    intClose++;
                    typewriterManager.currentAnswer = i;
                }
            }

            if (intClose == 0) { typewriterManager.currentAnswer = -1; }
        }

        public void AnswerInsert(int currentQuestion, int currentAnswer)
        {
            if(currentAnswer != 5)
            {
                answersText[currentQuestion].text += $" {currentAnswer}";
            }
            else
            {
                xAnswersText[currentQuestion].text += $" X";
                typewriterManager.HorizontalOffset();
            }
        }
    }
}