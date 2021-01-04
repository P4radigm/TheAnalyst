using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamColliderVisible : MonoBehaviour
{
    [SerializeField] private Collider camCollider;

    private void Awake()
    {
        camCollider.enabled = false;
    }

    public void SetCamColVisible()
    {
        camCollider.enabled = true;
    }
}
