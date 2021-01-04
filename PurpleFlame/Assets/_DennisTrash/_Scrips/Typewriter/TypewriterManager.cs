using Dennis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypewriterManager : LeanDrag
{
    [HideInInspector] public int currentAnswer = 0;
    [HideInInspector] public bool paperPickedUp = false;

    [SerializeField] private float swipeThreshold;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask layerMaskWheel;
    [SerializeField] private SnapShotCamera snapshotCam;
    [SerializeField] private BookManager bookManager;
    [SerializeField] private Animator bookCaseAnim;
    //[SerializeField] private Animator deskAnim;

    private int[] answerNumbers = new int[4];
    private float swipe;
    private bool pressingTypeWriterDisk;
    private bool interactableHit;
    private bool swipeRecognised;
    private Vector3 hitNormal;
    private Vector3 position;
    private TypewriterButton lastButtonPressed;
    private TypewriterDisk typeWriterDisk;
    private TypewriterUI typeWriterUI;
    private SwitchPages switchPages;

    private void Start()
    {
        typeWriterUI = GetComponent<TypewriterUI>();
        switchPages = GetComponent<SwitchPages>();

        for (int i = 0; i < answerNumbers.Length; i++)
        {
            answerNumbers[i] = -1;
        }
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
                interactableHit = true;
                lastButtonPressed = hit.collider.GetComponent<TypewriterButton>();
            }
            if (hit.collider.gameObject.GetComponent<TypewriterPaper>() && typeWriterUI.readyToPickUp && !paperPickedUp)
            {
                paperPickedUp = true;
                bookManager.SetCanvas(answerNumbers[0] + 1, answerNumbers[1] + 1, answerNumbers[2] + 1, answerNumbers[3] + 1);
                bookCaseAnim.SetTrigger("OpenBookCaseDoors");

                //snapshotCam.CallTakeSnapshot();
                //Destroy(hit.collider.GetComponent<TypewriterPaper>().gameObject);
            }
        }
    }

    sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
    {
        base.OnFingerUp(finger);

        interactableHit = false;
        swipeRecognised = false;
        pressingTypeWriterDisk = false;
        swipe = 0;
    }

    private void Update()
    {
        if (interactableHit && !swipeRecognised) { SwipeInputButton(); }
        if (pressingTypeWriterDisk) { SwipeInputDisk(); }
        //if (deskAnim.GetCurrentAnimatorStateInfo(0).IsName("Done")) { GetComponent<Animator>().SetTrigger("WeightPuzzleSolved"); }
    }

    private void SwipeInputButton()
    {
        swipe = touchingFingers[0].ScreenDelta.y;

        if (swipe < -swipeThreshold) 
        {
            swipeRecognised = true;
            Debug.Log(currentAnswer);
            if(currentAnswer >= 0 && currentAnswer <= answerNumbers.Length)
            {
                if(answerNumbers[currentAnswer] == -1) { typeWriterUI.answersCount++; }
                answerNumbers[currentAnswer] = lastButtonPressed.answerNumber;
                lastButtonPressed.PressButtonAnim();
                typeWriterUI.AnswerInsert(currentAnswer, lastButtonPressed.answerNumber + 1);
            }
        }
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