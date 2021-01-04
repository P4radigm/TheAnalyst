using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerCustomEditor : Editor
{
    AudioManager audioManager;

    public void OnEnable()
    {
        audioManager = (AudioManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
         //audioManager.enumerator = EditorGUILayout.EnumPopup(audioManager.enumerator);

    }
}
