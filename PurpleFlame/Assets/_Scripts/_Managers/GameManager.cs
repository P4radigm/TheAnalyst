using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BasInventory;
using UnityEngine.EventSystems;

namespace TouchBehaviours
{
    public enum GameState
    {
        Tutorial,
        Act1,
        Act2,
        Act3
    }

    [System.Serializable]
    public class PuzzleContainer
    {
        public GameState PuzzleState;
        public UnityEvent PuzzleDoneEvent;   
        
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public LayerMask TouchableLayers;
        public string CurrentlyViewing;
        public Dictionary<Iinspectable, GameObject> InspectableGameObjects = new Dictionary<Iinspectable, GameObject>();
        public Inventory InventoryManager;
        public CameraController CameraController;
        public GameState CurrentGameState;
        public List<PuzzleContainer> Puzzles = new List<PuzzleContainer>();
        public float DeactivationTimeOfObjects = 2f;
        public Image FadePanel;
        public SecondDeskManager SecondDeskManager;
        //[HideInInspector] public GameObject currentTarget;
        private bool paused = false;

        public void Pause(bool save = true)
        {
            if(!paused)
            {
                paused = true;
            }
        }

        public void UnPause()
        {
            if (paused)
                paused = false;
        }

        public bool IsPaused
        {
            get { return paused; }
        }


        private void Awake()
        {
            if(Instance !=null &&Instance !=this)
            {
                Destroy(gameObject);
            }

            Instance = this;
            Resources.UnloadUnusedAssets();


        }

        [SerializeField] private GameObject loadingScreen;

        [SerializeField] private EventSystem eventSystem;

        public void LoadScene(int scene)
        {
            StartCoroutine(LoadAsync(scene));
        }

        private IEnumerator LoadAsync(int scene)
        {
            yield return new WaitForSeconds(2f);
            loadingScreen.SetActive(true);

            yield return new WaitForSecondsRealtime(1f);

            AsyncOperation unload = Resources.UnloadUnusedAssets();

            while (!unload.isDone)
            {
                yield return null;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

            while (!operation.isDone)
            {
                yield return null;
            }
        }
        public void GoToNextScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            int index = scene.buildIndex + 1;
            if (index > SceneManager.sceneCount) LoadScene(scene.buildIndex);
            else LoadScene(index);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(0);
        }

        public void DissableAfterTime(Animator objToDissable)
        {
            StartCoroutine(DissableAFterTimeIE(objToDissable));
        }

        IEnumerator DissableAFterTimeIE(Animator obj)
        {
            yield return new WaitForSeconds(DeactivationTimeOfObjects);
            obj.enabled = false;
        }

        public void FadeOut(float time)
        {
            FadePanel.gameObject.SetActive(true);
            FadePanel.canvasRenderer.SetAlpha(0.0f);
            FadePanel.CrossFadeAlpha(1.0f, time, false);
        }

        public void PuzzleDone(GameState puzzle)
        {
            PuzzleContainer item = Puzzles.Find(pc => pc.PuzzleState.Equals(puzzle));
            item.PuzzleDoneEvent.Invoke();
        }

        public void ReInitializeInspectables(GameObject _exception)
        {
            foreach(KeyValuePair<Iinspectable, GameObject> keyValuePair in InspectableGameObjects)
            {
                keyValuePair.Value.GetComponent<Collider>().enabled = true;
            }

            if(_exception != null)
            {
                _exception.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
