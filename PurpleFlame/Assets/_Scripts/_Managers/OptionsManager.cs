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

    [Header("Settings")]
    public int sfxLevelSetting;
    public int musicLevelSetting;
    public bool invertCameraSetting;

    public bool notchSetting;

    public performancePresetSetting ppSetting;

    public TextureQualitySetting tqSetting;
    public bool particlesSetting;
    public bool shadowsSetting;
    public bool PostProcessingSetting;

    public int lastSfxLevelSetting;
    public int lastMusicLevelSetting;

    [Header("Other")]
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private RectTransform inGameUImaster;

    private ScreenOrientation screenOrientation;

    private void Start()
    {
        DontDestroyOnLoad(inGameCanvas);
        DontDestroyOnLoad(pauseCanvas);
    }

    private void Update()
    {
        if(screenOrientation != Screen.orientation)
        {
            if(Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                UpdateNotchDisplay();
                screenOrientation = ScreenOrientation.LandscapeLeft;
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                UpdateNotchDisplay();
                screenOrientation = ScreenOrientation.LandscapeRight;
            }
            else
            {
                Debug.Log("ScreenOrientation not supported");
            }
        }
    }

    public void UpdateNotchDisplay()
    {
        if (!notchSetting)
        {
            inGameUImaster.offsetMin = new Vector2(0, 0);
            inGameUImaster.offsetMax = new Vector2(0, 0);
        }
        else
        {
            float horizontalOffset = Screen.width - Screen.safeArea.width;
            float verticalOffset = Screen.height - Screen.safeArea.height;

            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                inGameUImaster.offsetMin = new Vector2(horizontalOffset, verticalOffset / 2);
                inGameUImaster.offsetMax = new Vector2(0, verticalOffset / 2);
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                inGameUImaster.offsetMin = new Vector2(0, verticalOffset / 2);
                inGameUImaster.offsetMax = new Vector2(-horizontalOffset, verticalOffset / 2);
            }
            else
            {
                Debug.LogError($"Non supported screen orientation");
            }
            Debug.Log($"Displaying Safe Area, width = {Screen.safeArea.width}, height = {Screen.safeArea.height}");
        }
    }
}
