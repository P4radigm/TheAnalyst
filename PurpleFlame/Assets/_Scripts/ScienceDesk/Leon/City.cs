using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public bool isVisited;
    public RoadLock[] connectedRoadLocks;

    private ScienceDeskController sdC;

    private void Start()
    {
        sdC = ScienceDeskController.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "AvatarModel")
        {
            sdC.AvatarHasEnteredCity(this);
            sdC.avatarIsInCity = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "AvatarModel")
        {
            isVisited = true;
            sdC.avatarIsInCity = false;
        }
    }

}
