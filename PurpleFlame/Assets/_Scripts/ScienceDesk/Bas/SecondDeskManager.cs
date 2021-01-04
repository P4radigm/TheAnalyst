using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dennis;
using Lean.Touch;
using UnityEngine.UI;
using System.Linq;
using TMPro;

namespace TouchBehaviours
{
    [System.Serializable]
    public class Road
    {
        public float Distance;
        public LocationPoint CameFrom;
        public LocationPoint GoTo;
        public bool Visited = false;
    }

    [System.Serializable]
    public class SecondDeskPuzzleContainer
    {
        public string name;
        public string TriggerAnimationName;
        public List<LocationPoint> CityRoadPoints;
        public List<Vector3> RoadToComplete;
        public Image BackGroundImage;
        public bool Completed;
        public GameObject Area;

        public void InitializeRoad()
        {
            foreach(LocationPoint location in CityRoadPoints)
            {
                if(!RoadToComplete.Contains(location.gameObject.transform.position))
                    RoadToComplete.Add(location.gameObject.transform.position);
            }
        }

        public bool RoadChecker(List<Vector3> visitedPositions)
        {
            if(RoadToComplete.SequenceEqual<Vector3>(visitedPositions))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class SecondDeskManager : LeanDrag
    {
        public Animator SecondDeskAnimatorController;
        public TextMeshProUGUI RoadsPassedHoursTextField;
        public List<SecondDeskPuzzleContainer> SecondDeskPuzzles;
        public List<LocationPoint> CityPoints = new List<LocationPoint>();
        public GameObject Avatar;
        public Transform AvatarVisual;
        public float CameraDistance = 10f;
        public List<Road> AllRoads;
        public bool HasAvatarSelected;
        public LineRenderer LineRenderer;
        public List<Vector3> AllVisitedPositions;
        private Vector3 mousePosition;
        private Vector3 startPosition;
        private SecondDeskPuzzleContainer currentlyActivePuzzle;
        private List<Road> tmpRoads = new List<Road>();
        private float hours = 0;
        private LocationPoint tmpCameFromPoint;
        public List<LocationPoint> tmpLocationsVisited = new List<LocationPoint>();

        protected override void Start()
        {
            PuzzleSetup();         
        }

        public void AddVisitedPosition(Vector3 position)
        {
            if(!AllVisitedPositions.Contains(position))
                AllVisitedPositions.Add(position);
            if (AllVisitedPositions.Count < 1) return;

            LineRenderer.positionCount = AllVisitedPositions.Count;
            for (int i = 0; i < AllVisitedPositions.Count; i++)
            {
                LineRenderer.SetPosition(i, AllVisitedPositions[i]);
            }
        }

        public void ResetCityVisited()
        {
            hours = 0;
            RoadsPassedHoursTextField.text = "0";
            AllVisitedPositions.Clear();
            LineRenderer.positionCount = 0;
            AvatarVisual.transform.position = currentlyActivePuzzle.CityRoadPoints[0].transform.GetChild(0).transform.position;
            Avatar.transform.position = AvatarVisual.transform.position;
            foreach(LocationPoint p in currentlyActivePuzzle.CityRoadPoints)
            {
                foreach(Road r in p.ConectingRoads)
                {
                    r.Visited = false;
                }
            }
        }

        public void PuzzleUpdate(LocationPoint incomingLocationPoint)
        {
            //LocationPoint pointToFind = tmpLocationsVisited.Find(l => l.ConectingRoads.Where(cr => cr.CameFrom.Equals(incomingLocationPoint)).Equals(incomingLocationPoint));

            if (!tmpLocationsVisited.Contains(incomingLocationPoint))
                tmpLocationsVisited.Add(incomingLocationPoint);

            if (tmpLocationsVisited.Count < 2) return;         
            LocationPoint cameFromPoint = tmpLocationsVisited[tmpLocationsVisited.Count - 2];
            foreach(Road r in incomingLocationPoint.ConectingRoads)
            {
                if((r.CameFrom.Equals(cameFromPoint) && r.GoTo.Equals(incomingLocationPoint))|| /*hack*/ (r.CameFrom.Equals(incomingLocationPoint) && r.GoTo.Equals(cameFromPoint)))
                {
                    if(!r.Visited)
                        hours += r.Distance;
                    r.Visited = true;
                }
            }
            //Road roadToFind = AllRoads.Find(r => r.CameFrom.Equals(cameFromPoint) && r.GoTo.Equals(incomingLocationPoint));
            //hours += roadToFind.Distance;
            RoadsPassedHoursTextField.text = hours.ToString();

            //Avatar.GetComponent<AvatarSecondDesk>().UpdateNextPosition(incomingLocationPoint.transform.position);
            switch (incomingLocationPoint.GetCityState())
            {
                case CityState.InitializePuzzle:
                    PuzzleSetup();
                    break;
                case CityState.Entered:
                    Debug.Log("Already active(entered)");
                    break;
                case CityState.Visited:
                    Debug.Log("Already active(Visited)");
                    break;
                case CityState.NotVisited:
                    Debug.Log("Not active(Notvisited)");
                    incomingLocationPoint.SetCityState(CityState.Entered);
                    break;
                case CityState.Left:
                    Debug.Log("Already active(left)");
                    break;
                case CityState.Reset:
                    ResetCityVisited();
                    break;
                case CityState.Done:
                    if (currentlyActivePuzzle.RoadChecker(AllVisitedPositions))
                    {
                        SecondDeskAnimatorController.enabled = true;
                        SecondDeskAnimatorController.SetTrigger(currentlyActivePuzzle.TriggerAnimationName);
                        //StartCoroutine(TurnAnimatorController());
                        currentlyActivePuzzle.Completed = true;
                        ResetCityVisited();
                        PuzzleSetup();
                        ResetAvatarPosition();
                    }
                    else
                    {
                        ResetCityVisited();
                        ResetAvatarPosition();
                    }
                    break;
            }
        }

        private IEnumerator TurnAnimatorController()
        {
            if(SecondDeskAnimatorController.enabled)
            {
                yield return new WaitForSeconds(16f);
                SecondDeskAnimatorController.enabled = false;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }

        private void ResetAvatarPosition()
        {
            Avatar.transform.position = currentlyActivePuzzle.CityRoadPoints[0].transform.GetChild(0).transform.position;
        }

        public void PuzzleSetup()
        {
            currentlyActivePuzzle = SecondDeskPuzzles.Find(sdp => sdp.Completed == false);
            if (currentlyActivePuzzle == null)
            {
                foreach(SecondDeskPuzzleContainer element in SecondDeskPuzzles)
                {
                    element.Completed = false;
                    element.RoadToComplete.Clear();
                    element.InitializeRoad();
                }

                currentlyActivePuzzle = SecondDeskPuzzles.Find(sdp => sdp.Completed == false);

            }

            foreach (SecondDeskPuzzleContainer container in SecondDeskPuzzles)
            {
                container.Area.SetActive(false);
            }

            SecondDeskAnimatorController.SetTrigger("Begin");
            currentlyActivePuzzle.Area.SetActive(true);
            currentlyActivePuzzle.InitializeRoad();
        }

        protected override void OnFingerDown(LeanFinger finger)
        {
            base.OnFingerDown(finger);
            if(HasAvatarSelected)
            {              
                //startPosition = Camera.main.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, CameraDistance));
                
                
                //Avatar.transform.position = startPosition;
            }
        }

        protected override void OnFingerUp(LeanFinger finger)
        {
            base.OnFingerUp(finger);
            AvatarVisual.transform.position = currentlyActivePuzzle.CityRoadPoints[0].transform.GetChild(0).transform.position;
            Avatar.transform.position = AvatarVisual.transform.position;
            if (AllVisitedPositions.Count < 1) return;
            LineRenderer.positionCount = AllVisitedPositions.Count;
            for(int i = 0; i < AllVisitedPositions.Count; i++)
            {
                LineRenderer.SetPosition(i, AllVisitedPositions[i]);
            }

            ResetCityVisited();

        }

        protected override void OnFingerUpdate(LeanFinger finger)
        {
            base.OnFingerUpdate(finger);
            mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(finger.ScreenPosition.x, finger.ScreenPosition.y, 19));
            //LineRenderer.SetPosition(0, new Vector3(startPosition.x, startPosition.y, 19));
            //LineRenderer.SetPosition(1, new Vector3(mousePosition.x, mousePosition.y, 19));
            //LineRenderer.SetPositions(AllVisitedPositions.ToArray());
            
            //if finger lets lose on correspondending point to connect line renderer 
        }

    }
}