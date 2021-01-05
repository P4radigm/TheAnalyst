using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum performancePresetSetting
{
    Low,
    Middle,
    High
}
public enum TextureQualitySetting
{
    Low,
    Middle,
    High
}

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public int sfxLevelSetting;
    public int musicLevelSetting;
    public bool invertCameraSetting;

    public performancePresetSetting ppSetting;
    public TextureQualitySetting tqSetting;

    public bool particlesSetting;
    public bool shadowsSetting;
    public bool PostProcessingSetting;

    public int lastSfxLevelSetting;
    public int lastMusicLevelSetting;

}
