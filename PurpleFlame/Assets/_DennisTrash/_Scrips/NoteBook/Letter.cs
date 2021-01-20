using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PurpleFlame
{
    public class Letter : MonoBehaviour
    {
        public int letterID;
        public Sprite[] pagesSprites;
        [TextArea(15, 20)] public string[] pagesText;

        public void PutInNotebook()
        {
            NoteBookManager.Instance.AddLetters(this);
            NoteBookManager.Instance.SetNotebookVisible();
            Destroy(this.gameObject);
        }
    }
}