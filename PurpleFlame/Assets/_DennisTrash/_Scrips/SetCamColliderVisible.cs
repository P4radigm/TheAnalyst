using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamColliderVisible : MonoBehaviour
{
    [SerializeField] private Collider typeCamCollider;
    [SerializeField] private Collider chessCamCollider;

    private void Awake()
    {
        typeCamCollider.enabled = false;
    }

    public void SetCamColVisible()
    {
        typeCamCollider.enabled = true;
        chessCamCollider.enabled = false;
    }
}
