using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TypewriterButton : MonoBehaviour
{
    //0 = A, 1 = B, 2 = C, 3 = D, 4 = X
    public int answerNumber;

    [Header("ButtonAnim")]
    [SerializeField] private float highZrot;
    [SerializeField] private float lowZrot;
    [SerializeField] private float lowerButtonAnimDruation;
    [SerializeField] private float higherButtonAnimDruation;
    [SerializeField] private AnimationCurve lowerButtonCurve;
    [SerializeField] private AnimationCurve higherButtonCurve;
    [SerializeField] private float negativeAnimDruation;
    [SerializeField] private AnimationCurve negativeCurve;

    [Header("Hammer Anim")]
    [SerializeField] private Transform hammerTransform;
    [SerializeField] private float idleRot;
    [SerializeField] private float contactRot;
    [SerializeField] private AnimationCurve firstHammerCurve;
    [SerializeField] private AnimationCurve returnHammerCurve;

    [Header("Audio")]
    [SerializeField] private UnityEvent pressSound;

    //private Animator anim;
    private Coroutine presssButtonRoutine;
    private TypewriterManager tM;

    private void Start()
    {
        //anim = GetComponent<Animator>();
    }

    public void StartPressButtonAnim(TypewriterManager _tM, bool _isAllowed)
    {
        tM = _tM;
        if (presssButtonRoutine != null) { return; }
        if (_isAllowed) { presssButtonRoutine = StartCoroutine(LowerButtonIE()); }
        else { presssButtonRoutine = StartCoroutine(NegativeAnimIE()); }
        
    }

    private IEnumerator LowerButtonIE()
    {
        //Move paper if needed
        
        float _lowerButtonTimeValue = 0;

        while (_lowerButtonTimeValue < 1)
        {
            _lowerButtonTimeValue += Time.deltaTime / lowerButtonAnimDruation;

            float _evaluatedLerpTimeButton = lowerButtonCurve.Evaluate(_lowerButtonTimeValue);
            float newAngleButton = Mathf.Lerp(highZrot, lowZrot, _evaluatedLerpTimeButton);

            float _evaluatedLerpTimeHammer = firstHammerCurve.Evaluate(_lowerButtonTimeValue);
            float newAngleHammer = Mathf.Lerp(idleRot, contactRot, _evaluatedLerpTimeHammer);

            transform.parent.localEulerAngles = new Vector3(0, 0, newAngleButton);

            hammerTransform.localEulerAngles = new Vector3(0, 0, newAngleHammer);

            yield return null;
        }

        pressSound.Invoke();
        tM.PunchLetter();

        StartCoroutine(higherButtonIE());
    }

    private IEnumerator higherButtonIE()
    {
        float _higherButtonTimeValue = 0;

        while (_higherButtonTimeValue < 1)
        {
            _higherButtonTimeValue += Time.deltaTime / higherButtonAnimDruation;

            float _evaluatedLerpTimeButton = higherButtonCurve.Evaluate(_higherButtonTimeValue);
            float newAngleButton = Mathf.Lerp(highZrot, lowZrot, _evaluatedLerpTimeButton);

            float _evaluatedLerpTimeHammer = returnHammerCurve.Evaluate(_higherButtonTimeValue);
            float newAngleHammer = Mathf.Lerp(idleRot, contactRot, _evaluatedLerpTimeHammer);

            transform.parent.localEulerAngles = new Vector3(0, 0, newAngleButton);

            hammerTransform.localEulerAngles = new Vector3(0, 0, newAngleHammer);

            yield return null;
        }

        tM.firstHit = true;
        presssButtonRoutine = null;
    }

    private IEnumerator NegativeAnimIE()
    {
        float _negativeTimeValue = 0;

        while (_negativeTimeValue < 1)
        {
            _negativeTimeValue += Time.deltaTime / negativeAnimDruation;

            float _evaluatedLerpTimeNegative = negativeCurve.Evaluate(_negativeTimeValue);
            float newAngleButton = Mathf.Lerp(highZrot, lowZrot, _evaluatedLerpTimeNegative);

            transform.parent.localEulerAngles = new Vector3(0, 0, newAngleButton);

            yield return null;
        }

        tM.firstHit = true;
        presssButtonRoutine = null;
    }
}
