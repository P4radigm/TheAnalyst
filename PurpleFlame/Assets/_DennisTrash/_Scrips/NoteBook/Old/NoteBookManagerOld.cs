using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Dennis
{
    public class NoteBookManagerOld : MonoBehaviour
    {

        [SerializeField] private GameObject book;
        [Space]
        [SerializeField] private TextMeshProUGUI textPageOne;
        [SerializeField] private TextMeshProUGUI pageOneNumber;
        [Space]
        [SerializeField] private TextMeshProUGUI textPageTwo;
        [SerializeField] private TextMeshProUGUI pageTwoNumber;

        private List<string> pageText = new List<string>();

        private int currentPageSelected;
        private int totalPages = 0;
        private bool notebookOpen;

        private void Update()
        {
            if (!notebookOpen) { return; }
            TextDisplay();
        }

        private void TextDisplay()
        {
            if (currentPageSelected + 1 > totalPages) { return; }
            textPageOne.text = pageText[currentPageSelected];
            pageOneNumber.text = currentPageSelected + 1 + "";
            pageTwoNumber.text = currentPageSelected + 2 + "";
            if (currentPageSelected + 2 <= totalPages)
            {
                textPageTwo.text = pageText[currentPageSelected + 1];
            }
            else
            {
                textPageTwo.text = "";
            }
        }

        public void SetNotebookVisible(int pages = 0)
        {
            notebookOpen = !notebookOpen;
            
            if (notebookOpen)
            {
                book.SetActive(true);
                if (pages != 0) 
                {
                    currentPageSelected = totalPages - pages;
                    if (currentPageSelected % 2 != 0) { currentPageSelected--; }
                }
            }
            else
            {
                currentPageSelected = 0;
                book.SetActive(false);
            }
        }

        public bool GetNotebookOpen()
        {
            return notebookOpen;
        }

        public void AddNewPages(string page)
        {
            totalPages++;
            pageText.Add(page);
        }

        public void SwitchPages(bool nextPages)
        {
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
        private static NoteBookManagerOld instance;

        private void Awake()
        {
            instance = this;
            currentPageSelected = 0;
            book.SetActive(false);
        }

        public static NoteBookManagerOld Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new NoteBookManagerOld();
                }

                return instance;
            }
        }
        #endregion
    }
}