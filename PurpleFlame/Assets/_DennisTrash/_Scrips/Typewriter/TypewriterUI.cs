using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Dennis
{
    public class TypewriterUI : MonoBehaviour
    {
        [HideInInspector] public int answersCount;
        [HideInInspector] public bool readyToPickUp = false;

        [SerializeField] private GameObject paper;
        [SerializeField] private Material greenMat;
        [SerializeField] private Material startMat;
        [SerializeField] private MeshRenderer meshRenderer;
        [Space]
        [SerializeField] private TextMeshProUGUI[] answersText;
        [SerializeField] private TypewriterManager typewriterManager;
        [Space]
        [SerializeField] private float[] answerPos;
        [SerializeField] private float maxAnswerDis;
        [SerializeField] private float scrollSpeed;
        [SerializeField] private float paperLength;
        [SerializeField] private float pickupLength;
        [SerializeField] private float beginLength;

        private float newPos;
        private string[] answerHistory;
        private Vector3 v3;
        private Vector3 startPos;

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

            if (answersCount != 4)
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

            paper.transform.localPosition = v3;
            CheckNearbyAnswerFields();
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
                answersText[currentQuestion].text = " " + currentAnswer;
            }
            else
            {
                answersText[currentQuestion].text = " " + "X";
            }
        }
    }
}