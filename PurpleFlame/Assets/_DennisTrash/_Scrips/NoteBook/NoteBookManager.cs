using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Dennis
{
    public enum NodeState
    {
        None,
        Tutorial,
        Notebook
    }

    public class NoteBookManager : LeanDrag
    {
        [SerializeField] private NodeState startState;

        [Header("Input")]
        [SerializeField] private float moveDragThreshold; // how much do you have to swipe before you have effect

        [Header("UI")]
        [SerializeField] private GameObject book;
        [SerializeField] private Sprite emptyPage;
        //[SerializeField] private Letter beginLetter;
        [SerializeField] private Sprite[] beginSprites;
        [TextArea(15, 10)]
        [SerializeField] private string[] beginText;
        [Space]
        [SerializeField] private GameObject clearTextPage;
        [SerializeField] private TextMeshProUGUI clearText;
        [Space]
        [SerializeField] private Image imagePageOne;
        [SerializeField] private TextMeshProUGUI pageOneNumber;
        [Space]
        [SerializeField] private Image imagePageTwo;
        [SerializeField] private TextMeshProUGUI pageTwoNumber;

        [Header("Tutorial")]
        [SerializeField] private GameObject telegramCanvas;
        [SerializeField] private GameObject telegramPage;
        [SerializeField] private GameObject letterPage;

        private bool notebookOpen;
        private bool clearTextOpen;
        private bool swipeRecognised = false;
        private int currentPageSelected;
        private int totalPages = 0;
        private float swipeRight, swipeLeft;
        private NodeState currentState;

        private List<Letter> letterUnorganised = new List<Letter>();

        private List<Sprite> pagesSpriteOrganised = new List<Sprite>();
        private List<string> pagesTextOrganised = new List<string>();

        private void Update()
        {
            if (!notebookOpen) { return; }
            NoteDisplay();
            SwipeInput();
        }

        private void NoteDisplay()
        {
            if (currentPageSelected + 1 > totalPages) { return; }

            //PaginaNummers
            pageOneNumber.text = currentPageSelected + 1 + "";
            pageTwoNumber.text = currentPageSelected + 2 + "";

            //Sprites Change
            imagePageOne.sprite = pagesSpriteOrganised[currentPageSelected];
            if (currentPageSelected + 2 <= totalPages)
            {
                imagePageTwo.sprite = pagesSpriteOrganised[currentPageSelected + 1];
            }
            else
            {
                imagePageTwo.sprite = emptyPage;
            }
        }

        private void SwipeInput()
        {
            //Swipe to other pages
            if (touchingFingers.Count > 0)
            {
                if (!swipeRecognised)
                {
                    swipeRight += touchingFingers[0].ScreenDelta.x;
                    swipeLeft += -touchingFingers[0].ScreenDelta.x;

                    if (currentState == NodeState.Tutorial)
                    {
                        if (swipeRight > moveDragThreshold) 
                        {
                            telegramPage.SetActive(true);
                            letterPage.SetActive(false);
                        }
                        if (swipeLeft > moveDragThreshold) 
                        {
                            telegramPage.SetActive(false);
                            letterPage.SetActive(true);
                        }
                    }

                    if (currentState == NodeState.Notebook)
                    {
                        if (swipeRight > moveDragThreshold) { SwitchPages(false); }
                        if (swipeLeft > moveDragThreshold) { SwitchPages(true); }
                    }
                }
            }
        }

        sealed protected override void OnFingerDown(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerDown(finger);

            if (clearTextOpen)
            {
                clearTextOpen = false;
                clearTextPage.SetActive(false);
            }
        }

        sealed protected override void OnFingerUp(Lean.Touch.LeanFinger finger)
        {
            base.OnFingerUp(finger);
            swipeRight = swipeLeft = 0;
            swipeRecognised = false;
        }

        public void ShowClearText(int pageSide) //0 is links en 1 is rechts
        {
            if (clearTextOpen) { return; }
            clearTextOpen = true;
            clearText.text = pagesTextOrganised[currentPageSelected + pageSide];
            clearTextPage.SetActive(true);
        }

        public void SetNotebookVisible()
        {
            notebookOpen = !notebookOpen;
            
            if (notebookOpen)
            {
                if(currentState == NodeState.Tutorial)
                {
                    telegramCanvas.SetActive(true);
                }
                if (currentState == NodeState.Notebook)
                {
                    book.SetActive(true);
                }
            }
            else
            {
                //currentPageSelected = 0;
                book.SetActive(false);
                telegramCanvas.SetActive(false);
            }
        }

        public void AddLetters(Letter letter)
        {
            if (currentState == NodeState.Tutorial)
            {
                currentState = NodeState.Notebook;
            }

            int currentLetter = letter.letterID;
            letterUnorganised.Add(letter);
            letterUnorganised = letterUnorganised.OrderBy(x => x.letterID).ToList();
            pagesSpriteOrganised.Clear();
            pagesTextOrganised.Clear();
            totalPages = 0;

            for (int i = 0; i < letterUnorganised.Count; i++)
            {
                if(i == 0)
                {
                    for (int b = 0; b < beginSprites.Length; b++)
                    {
                        totalPages++;
                        pagesSpriteOrganised.Add(beginSprites[b]);
                        pagesTextOrganised.Add(beginText[b]);
                    }
                }

                for (int l = 0; l < letterUnorganised[i].pagesSprites.Length; l++)
                {
                    totalPages++;
                    pagesSpriteOrganised.Add(letterUnorganised[i].pagesSprites[l]);
                    pagesTextOrganised.Add(letterUnorganised[i].pagesText[l]);

                    if (currentLetter == letterUnorganised[i].letterID && l == 0) 
                    { 
                        currentPageSelected = totalPages;
                        if (currentPageSelected % 2 != 0) { currentPageSelected--; }
                        else { currentPageSelected -= 2; }
                    }
                }
            }
        }

        public void SwitchPages(bool nextPages)
        {
            swipeRecognised = true;

            //NextPages
            if (currentPageSelected < totalPages - 2) 
            {
                if (nextPages) { currentPageSelected += 2; }
            }

            //PreviousPages
            if (!nextPages) { currentPageSelected -= 2; }
            if (currentPageSelected < 0) 
            {
                currentPageSelected = 0;
            }
        }

        #region Singleton
        private static NoteBookManager instance;

        private void Awake()
        {
            currentState = NodeState.None;

            instance = this;
            currentPageSelected = 0;
            book.SetActive(false);

            for (int b = 0; b < beginSprites.Length; b++)
            {
                totalPages++;
                pagesSpriteOrganised.Add(beginSprites[b]);
                pagesTextOrganised.Add(beginText[b]);
            }

            currentState = startState;

            if (currentState == NodeState.Tutorial)
            {
                telegramCanvas.SetActive(true);
                notebookOpen = true;
            }
            if(currentState == NodeState.Notebook)
            {
                telegramCanvas.SetActive(false);

                for (int i = 0; i < beginSprites.Length; i++)
                {
                    pagesSpriteOrganised.Add(beginSprites[i]);
                    pagesTextOrganised.Add(beginText[i]);
                }
            }
        }

        public static NoteBookManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new NoteBookManager();
                }

                return instance;
            }
        }
        #endregion
    }
}