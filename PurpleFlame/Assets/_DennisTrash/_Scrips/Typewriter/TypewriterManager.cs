using PurpleFlame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TypewriterManager : LeanDrag
{
    [HideInInspector] public int currentAnswer = 0;
    [HideInInspector] public bool paperPickedUp = false;
    [HideInInspector] public float[] paperZPosPerQuestion = new float[4];

    [SerializeField] private float swipeThreshold;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask layerMaskWheel;
    [SerializeField] private SnapShotCamera snapshotCam;
    [SerializeField] private BookManager bookManager;
    [SerializeField] private Animator bookCaseAnim;
    [Space(20)]
    [SerializeField] private AnimationCurve verticalAlignCurve;
    [SerializeField] private float verticalAlignDurationMultiplier;
    [SerializeField] private float horizontalOffsetAmount;
    //[SerializeField] private Animator deskAnim;

    [Header("Audio")]
    [SerializeField] private UnityEvent paperPickUpSound;

    private float[] conceptZposArray = new float[4];
    private int[] answerNumbers = new int[4];
    private float swipe;
    private bool pressingTypeWriterDisk;
    private bool buttonHit;
    public bool firstHit;
    private bool swipeRecognised;
    private Vector3 hitNormal;
    private Vector3 position;
    private TypewriterButton currentButtonPressed;
    private TypewriterDisk typeWriterDisk;
    private TypewriterUI typeWriterUI;
    private SwitchPages switchPages;
    private Coroutine verticalAlignRoutine;

    private void Start()
    {
        firstHit = true;
        typeWriterUI = GetComponent<TypewriterUI>();
        switchPages = GetComponent<SwitchPages>();

        for (int i = 0; i < answerNumbers.Length; i++)
        {
            answerNumbers[i] = -1;
        }

        for (int i = 0; i < 4; i++)
        {
            conceptZposArray[i] = typeWriterUI.paper.transform.localPosition.z;
        }

        paperZPosPerQuestion = conceptZposArray;
    }

    sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
    {
        base.OnFingerDown(finger);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject.GetComponent<TypewriterDisk>())
            {
                pressingTypeWriterDisk = true;
                typeWriterDisk = hit.collider.GetComponent<TypewriterDisk>();
                switchPages.SwitchPaper(true);
            }

            if (paperPickedUp) { return; }

            if (hit.collider.gameObject.GetComponent<TypewriterButton>())
            {
                buttonHit = true;
                currentButtonPressed = hit.collider.GetComponent<TypewriterButton>();
                switchPages.SwitchPaper(true);
            }
            if (hit.collider.gameObject.GetComponent<TypewriterPaper>() && typeWriterUI.readyToPickUp && !paperPickedUp)
            {
                paperPickedUp = true;
                bookManager.SetCanvas(answerNumbers[0] + 1, answerNumbers[1] + 1, answerNumbers[2] + 1, answerNumbers[3] + 1);
                bookCaseAnim.SetTrigger("OpenBookCaseDoors");

                paperPickUpSound.Invoke();
                //snapshotCam.CallTakeSnapshot();
                //Destroy(hit.collider.GetComponent<TypewriterPaper>().gameObject);
            }
        }
    }

    sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
    {
        base.OnFingerUp(finger);

        VerticalAlign();

        buttonHit = false;
        swipeRecognised = false;
        pressingTypeWriterDisk = false;
        swipe = 0;
    }

    private void Update()
    {
        if (buttonHit && firstHit) { SwipeInputButton(); }
        if (pressingTypeWriterDisk) { SwipeInputDisk(); }
        //if (deskAnim.GetCurrentAnimatorStateInfo(0).IsName("Done")) { GetComponent<Animator>().SetTrigger("WeightPuzzleSolved"); }
    }

    private void VerticalAlign()
    {
        if(currentAnswer != -1)
        {
            if (Mathf.Abs(typeWriterUI.paper.transform.localPosition.y - typeWriterUI.answerPos[currentAnswer]) > typeWriterUI.maxAnswerDisForAlign)
            {
                if (verticalAlignRoutine != null) { return; }
                verticalAlignRoutine = StartCoroutine(VerticalAlignIE());
            }
        }
    }

    private IEnumerator VerticalAlignIE()
    {
        float _startYvalue = typeWriterUI.paper.transform.localPosition.y;

        float _timeValue = 0;

        float _duration = verticalAlignDurationMultiplier * Mathf.Abs(typeWriterUI.paper.transform.localPosition.y - typeWriterUI.answerPos[currentAnswer]);

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / _duration;

            float _evaluatedLerpTimeButton = verticalAlignCurve.Evaluate(_timeValue);
            float _newY = Mathf.Lerp(_startYvalue, typeWriterUI.answerPos[currentAnswer], _evaluatedLerpTimeButton);

            typeWriterUI.paper.transform.localPosition = new Vector3(typeWriterUI.paper.transform.localPosition.x, _newY, typeWriterUI.paper.transform.localPosition.z);

            yield return null;
        }

        verticalAlignRoutine = null;
        yield return null;
    }

    private void SwipeInputButton()
    {
        firstHit = false;
        //Debug.Log(currentAnswer);
        if(currentAnswer >= 0 && currentAnswer <= answerNumbers.Length)
        {
            if(currentButtonPressed.answerNumber < 4)
            {
                if(answerNumbers[currentAnswer] == -1) { typeWriterUI.answersCount++; }
                answerNumbers[currentAnswer] = currentButtonPressed.answerNumber;
            }
            else
            {
                answerNumbers[currentAnswer] = -1;
                typeWriterUI.answersCount--;
            }
            //typeWriterUI.AnswerInsert(currentAnswer, currentButtonPressed.answerNumber + 1);
            if (currentButtonPressed.answerNumber + 1 != 5)
            {
                if (typeWriterUI.answersText[currentAnswer].text.Length == typeWriterUI.xAnswersText[currentAnswer].text.Length)
                {
                    currentButtonPressed.StartPressButtonAnim(this, true);
                }
                else
                {
                    currentButtonPressed.StartPressButtonAnim(this, false);
                }
            }
            else
            {
                if (typeWriterUI.answersText[currentAnswer].text.Length != typeWriterUI.xAnswersText[currentAnswer].text.Length)
                {
                    currentButtonPressed.StartPressButtonAnim(this, true);
                }
                else
                {
                    currentButtonPressed.StartPressButtonAnim(this, false);
                }
            }      
        }
    }

    public void PunchLetter()
    {
        typeWriterUI.AnswerInsert(currentAnswer, currentButtonPressed.answerNumber + 1);
    }

    public void HorizontalOffset()
    {
        if(currentAnswer >= 0 && currentAnswer <= answerNumbers.Length)
        {
            paperZPosPerQuestion[currentAnswer] += horizontalOffsetAmount;
        }

        typeWriterUI.paper.transform.localPosition = new Vector3(typeWriterUI.paper.transform.localPosition.x, typeWriterUI.paper.transform.localPosition.y, paperZPosPerQuestion[currentAnswer]);
    }

    private void SwipeInputDisk()
    {
        swipe = touchingFingers[0].ScreenDelta.x;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(touchingFingers[0].ScreenPosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskWheel))
        {
            position = hit.point;
            hitNormal = hit.normal;
        }
        typeWriterDisk.UpdateDiskPos(position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(position, 0.1f);
    }
}