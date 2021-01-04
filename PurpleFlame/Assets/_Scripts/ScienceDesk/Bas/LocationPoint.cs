using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchBehaviours
{
    public enum CityState
    {
        Entered,
        Left,
        Visited,
        NotVisited,
        Reset,
        Done,
        InitializePuzzle,
    }

    public interface ICity
    {
    }

    public class LocationPoint : MonoBehaviour, ICity
    {
        public LocationPoint NextPoint;
        public List<Road> ConectingRoads;

        public CityState CityState;

        private bool readyCheck = true;

        public CityState GetCityState()
        {
            return CityState;
        }

        public void SetCityState(CityState incoming)
        {
            CityState = incoming;
        }

        private void OnMouseOver()
        {
            if (CityState == CityState.Reset)
            {

            }
            else
            {
                //GameManager.Instance.SecondDeskManager.AddVisitedPosition(this.transform.position);
            }

            //GameManager.Instance.SecondDeskManager.PuzzleUpdate(this);

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!readyCheck) return;
            if(other.gameObject.name == "Avatar")
            {
                if (CityState == CityState.Reset)
                {

                }
                else 
                {
                    GameManager.Instance.SecondDeskManager.AddVisitedPosition(this.transform.position);
                }

                GameManager.Instance.SecondDeskManager.PuzzleUpdate(this);

                Debug.Log("Avatar Collided");

            }
        }

        private void OnTriggerExit(Collider other)
        {
            //StartCoroutine(UnReadyForMoment());

        }

        private IEnumerator UnReadyForMoment()
        {
            readyCheck = false;
            yield return new WaitForSeconds(1f);
            readyCheck = true;
        }
    }
}