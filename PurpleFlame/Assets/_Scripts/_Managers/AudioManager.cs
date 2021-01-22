using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public Sound[] _Sounds;
    [SerializeField]
    public System.Enum enumerator;
    List<string> soundNames = new List<string>();
    public static AudioManager Instance;
    /*
    //Create new enum from arrays
    public static System.Enum CreateEnumFromArrays(List<string> list)
    {

        System.AppDomain currentDomain = System.AppDomain.CurrentDomain;
        AssemblyName aName = new AssemblyName("SoundEnum");
        AssemblyBuilder ab = currentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
        ModuleBuilder mb = ab.DefineDynamicModule(aName.Name);
        EnumBuilder enumerator = mb.DefineEnum("SoundEnum", TypeAttributes.Public, typeof(int));

        int i = 0;
        enumerator.DefineLiteral("None", i); //Here = enum{ None }

        foreach (string names in list)
        {
            i++;
            enumerator.DefineLiteral(names, i);
            Debug.Log(names);
        }
    
    System.Type finished = enumerator.CreateType();

    return (System.Enum) System.Enum.ToObject(finished,0);
    }


    private void OnEnable()
    {
        foreach(Sound sound in _Sounds)
        {
            if(sound.name != null)
                soundNames.Add(sound.name);
        }
        // = CreateEnumFromArrays(soundNames);
    }
    */
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);

        foreach(Sound s in _Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;

            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
            s.Source.spatialBlend = s.SpacialSound;
        }


    }

    private void Start()
    {
        //Play("Theme");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(_Sounds, sound => sound.name == name);
        if(s == null)
        {
            //Debug.LogWarning("Sound:" + name + "is not found!");
            return;
        }
        if (s.Source.isPlaying) { return; }
        s.Source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(_Sounds, sound => sound.name == name);
        if(s == null)
        {
            //Debug.LogWarning("Sound:" + name + "is not found!");
            return;
        }
        s.Source.Stop();
    }
}
