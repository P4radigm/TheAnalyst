using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    /// <summary>
    /// name with lower case to define element
    /// </summary>
    public string name;

    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Volume;
    [Range(.1f, 3f)]
    public float Pitch;
    [Range(0f, 1f)]
    public float SpacialSound;

    public bool Loop;



    [HideInInspector]
    public AudioSource Source;
}
