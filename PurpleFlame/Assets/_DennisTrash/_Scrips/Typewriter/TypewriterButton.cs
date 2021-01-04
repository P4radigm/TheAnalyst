using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypewriterButton : MonoBehaviour
{
    //0 = A, 1 = B, 2 = C, 3 = D, 4 = X
    public int answerNumber;

    private Animator anim;

    private void Start()
    {
        //anim = GetComponent<Animator>();
    }

    public void PressButtonAnim()
    {
        //anim.SetTrigger("PressButton");
    }
}
