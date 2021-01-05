using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum UImenuState
{
    main,
    credits,
    sherlocked,
    options,
    newProfile
}

public class MainMenuController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] Vector3 mainMenuPosition;
    [SerializeField] Vector3 optionsPosition;
    [SerializeField] Vector3 sherlockedPosition;
    [SerializeField] Vector3 creditsPosition;
    [SerializeField] Vector3 newProfilePosition;
    [SerializeField] float durationOptions;
    [SerializeField] float durationSherlocked;
    [SerializeField] float durationCredits;
    [SerializeField] float durationNewProfile;
    [SerializeField] AnimationCurve animCurve;
    private Coroutine animRoutine;
    private Vector3 startPos;

    [Header("Gameplay")]
    [SerializeField] private TextMeshProUGUI ics_Title;
    [SerializeField] private TextMeshProUGUI ics_Setting;

    [Header("Graphics")]
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;
    private Coroutine sideBarRoutine;
    private int panelWidth;
    [SerializeField] private float sideBarAnimDuration;
    [SerializeField] AnimationCurve sideBarCurve;
    [SerializeField] private TextMeshProUGUI preset_Title;
    [SerializeField] private TextMeshProUGUI preset_Setting;
    [SerializeField] private TextMeshProUGUI tq_Title;
    [SerializeField] private TextMeshProUGUI tq_Setting;
    [SerializeField] private TextMeshProUGUI p_Title;
    [SerializeField] private TextMeshProUGUI p_Setting;
    [SerializeField] private TextMeshProUGUI s_Title;
    [SerializeField] private TextMeshProUGUI s_Setting;
    [SerializeField] private TextMeshProUGUI pp_Title;
    [SerializeField] private TextMeshProUGUI pp_Setting;

    [Header("Sound")]
    [SerializeField] private Image mM_SfxToggle; 
    [SerializeField] private Image mM_MusicToggle; 
    [SerializeField] private Image oM_SfxToggle;
    [SerializeField] private Image oM_MusicToggle; 
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI sfxPercentage;
    [SerializeField] private TextMeshProUGUI musicPercentage;

    [Header("Profile")]
    [SerializeField] private GameObject newProfile;
    [SerializeField] private GameObject resetProfile;
    [SerializeField] private GameObject aysProfile;

    private OptionsManager oM;
    private UImenuState menuState;
    private MainMenuEventManager mmEm;
    private int screenWidth;
    public List<Button> buttonList = new List<Button>();

    private void Start()
    {
        menuState = UImenuState.main;
        UpdateButtonList();
        screenWidth = Screen.width;
        panelWidth = (screenWidth - 1920) / 2;
        RectTransform _leftPanelTransform = leftPanel.GetComponent<RectTransform>();
        RectTransform _rightPanelTransform = rightPanel.GetComponent<RectTransform>();
        _leftPanelTransform.anchoredPosition = new Vector2(panelWidth / 2, _leftPanelTransform.anchoredPosition.y);
        _rightPanelTransform.anchoredPosition = new Vector2(panelWidth / 2 * -1, _rightPanelTransform.anchoredPosition.y);
        _leftPanelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
        _rightPanelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
        oM = OptionsManager.instance;
        mmEm = MainMenuEventManager.instance;
        sfxSlider.value = oM.sfxLevelSetting;
        musicSlider.value = oM.musicLevelSetting;
        UICameraControl();
        UIPerformancePreset();

        if(panelWidth == 0)
        {
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
        }

        mmEm.OnPinchedInwards += BackToMainPinch;
    }

    #region MainMenu

    public void SherlockedButton()
    {
        Debug.Log($"Camera goes to sherlocked pos");
        MoveCamera(durationSherlocked, sherlockedPosition);
        menuState = UImenuState.sherlocked;
    }

    public void NewProfileButton()
    {
        Debug.Log($"Camera goes to new profile pos, but starts game for now");
        //MoveCamera(durationNewProfile, newProfilePosition);
        //camState = UIcameraState.newProfile;
        SceneManager.LoadScene("Act2_Dev");
        menuState = UImenuState.main;
    }

    public void OptionsButton()
    {
        Debug.Log($"Camera goes to options pos");
        MoveCamera(durationOptions, optionsPosition);
        menuState = UImenuState.options;
    }

    public void CreditsButton()
    {
        Debug.Log($"Camera goes to credits pos");
        Debug.Log($"Plays credits animation");
        MoveCamera(durationCredits, creditsPosition);
        GetComponent<CreditsScroll>().enabled = true;
        GetComponent<CreditsScroll>().ResetScroll();
        menuState = UImenuState.credits;
    }

    public void BackToMain(float _duration)
    {
        Debug.Log($"Camera goes to main menu pos");
        MoveCamera(_duration, mainMenuPosition);
        GetComponent<CreditsScroll>().enabled = false;
        menuState = UImenuState.main;
    }

    private void BackToMainPinch()
    {
        if(menuState == UImenuState.credits)
        {
            MoveCamera(durationCredits, mainMenuPosition);
            GetComponent<CreditsScroll>().enabled = false;
        }
        else if(menuState == UImenuState.sherlocked)
        {
            MoveCamera(durationSherlocked, mainMenuPosition);
        }
        else if(menuState == UImenuState.options)
        {
            MoveCamera(durationOptions, mainMenuPosition);
        }
        else if(menuState == UImenuState.newProfile)
        {
            MoveCamera(durationNewProfile, mainMenuPosition);
        }
        else
        {
            Debug.Log($"Camera is already on main");
        }

        menuState = UImenuState.main;
    }

    public void ResetProfileButton()
    {
        aysProfile.SetActive(true);
        resetProfile.SetActive(false);
    }

    public void AYSButton(bool _yn)
    {
        if (_yn)
        {
            Debug.Log($"reset progress");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            aysProfile.SetActive(false);
            resetProfile.SetActive(true);
        }
    }

    public void ContinueButton()
    {
        Debug.Log($"Continue game where the player last left off");
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region Options

    #region Gameplay

    public void UpdateCameraControl()
    {
        if (oM.invertCameraSetting)
        {
            oM.invertCameraSetting = false;
        }
        else
        {
            oM.invertCameraSetting = true;
        }
    }

    public void UICameraControl()
    {
        if (oM.invertCameraSetting)
        {
            ics_Title.text = $"Invert Camera Control........";
            ics_Setting.text = $"On";
        }
        else
        {
            ics_Title.text = $"Invert Camera Control.......";
            ics_Setting.text = $"Off";
        }
    }

    #endregion

    #region GraphicsPreset

    public void UpdatePerformancePreset()
    {
        if (oM.ppSetting == performancePresetSetting.High)
        {
            oM.ppSetting = performancePresetSetting.Middle;
        }
        else if (oM.ppSetting == performancePresetSetting.Middle)
        {
            oM.ppSetting = performancePresetSetting.Low;
        }
        else if (oM.ppSetting == performancePresetSetting.Low)
        {
            oM.ppSetting = performancePresetSetting.High;
        }
        else
        {
            oM.ppSetting = performancePresetSetting.Middle;
        }
    }

    public void UIPerformancePreset()
    {
        if (oM.ppSetting == performancePresetSetting.High)
        {
            preset_Title.text = $"Performance Preset..........";
            preset_Setting.text = $"High";

            tq_Title.text = $"Texture Quality..................";
            tq_Setting.text = $"High";

            p_Title.text = $"Particles....................................";
            p_Setting.text = $"On";

            s_Title.text = $"Shadows....................................";
            s_Setting.text = $"On";

            pp_Title.text = $"Post Processing......................";
            pp_Setting.text = $"On";
        }
        else if (oM.ppSetting == performancePresetSetting.Middle)
        {
            preset_Title.text = $"Performance Preset..";
            preset_Setting.text = $"Middle";

            tq_Title.text = $"Texture Quality.............";
            tq_Setting.text = $"Middle";

            p_Title.text = $"Particles....................................";
            p_Setting.text = $"Off";

            s_Title.text = $"Shadows....................................";
            s_Setting.text = $"On";

            pp_Title.text = $"Post Processing......................";
            pp_Setting.text = $"Off";
        }
        else if (oM.ppSetting == performancePresetSetting.Low)
        {
            preset_Title.text = $"Performance Preset...........";
            preset_Setting.text = $"Low";

            tq_Title.text = $"Texture Quality...................";
            tq_Setting.text = $"Low";

            p_Title.text = $"Particles....................................";
            p_Setting.text = $"Off";

            s_Title.text = $"Shadows....................................";
            s_Setting.text = $"Off";

            pp_Title.text = $"Post Processing......................";
            pp_Setting.text = $"Off";
        }
        else
        {
            preset_Title.text = $"Performance Preset....";
            preset_Setting.text = $"Custom";
        }
    }

    #endregion

    #region Graphics

    public void UpdateTextureQuality()
    {
        preset_Title.text = $"Performance Preset....";
        preset_Setting.text = $"Custom";
        //update renderpipeline asset

        if (oM.tqSetting == TextureQualitySetting.High)
        {
            oM.tqSetting = TextureQualitySetting.Middle;
        }
        else if (oM.tqSetting == TextureQualitySetting.Middle)
        {
            oM.tqSetting = TextureQualitySetting.Low;
        }
        else if (oM.tqSetting == TextureQualitySetting.Low)
        {
            oM.tqSetting = TextureQualitySetting.High;
        }
    }

    public void UITextureQuality()
    {
        if (oM.tqSetting == TextureQualitySetting.High)
        {
            tq_Title.text = $"Texture Quality..................";
            tq_Setting.text = $"High";
        }
        else if (oM.tqSetting == TextureQualitySetting.Middle)
        {
            tq_Title.text = $"Texture Quality.............";
            tq_Setting.text = $"Middle";
        }
        else
        {
            tq_Title.text = $"Texture Quality...................";
            tq_Setting.text = $"Low";
        }
    }

    public void UpdateParticles()
    {
        if (oM.particlesSetting)
        {
            oM.particlesSetting = false;
        }
        else
        {
            oM.particlesSetting = true;
        }
    }

    public void UIParticles()
    {
        preset_Title.text = $"Performance Preset....";
        preset_Setting.text = $"Custom";
        //update renderpipeline asset

        if (oM.particlesSetting)
        {
            p_Title.text = $"Particles....................................";
            p_Setting.text = $"On";
        }
        else
        {
            p_Title.text = $"Particles....................................";
            p_Setting.text = $"Off";
        }
    }

    public void UpdateShadows()
    {
        if (oM.shadowsSetting)
        {
            oM.shadowsSetting = false;
        }
        else
        {
            oM.shadowsSetting = true;
        }
    }

    public void UIShadows()
    {
        preset_Title.text = $"Performance Preset....";
        preset_Setting.text = $"Custom";
        //update renderpipeline asset

        if (oM.shadowsSetting)
        {
            s_Title.text = $"Shadows....................................";
            s_Setting.text = $"On";
        }
        else
        {
            s_Title.text = $"Shadows....................................";
            s_Setting.text = $"Off";
        }
    }

    public void UpdatePostProcessing()
    {
        if (oM.PostProcessingSetting)
        {
            oM.PostProcessingSetting = false;
        }
        else
        {
            oM.PostProcessingSetting = true;
        }
    }

    public void UIPostProcessing()
    {
        preset_Title.text = $"Performance Preset....";
        preset_Setting.text = $"Custom";
        //update renderpipeline asset

        if (oM.PostProcessingSetting)
        {
            pp_Title.text = $"Post Processing......................";
            pp_Setting.text = $"On";
        }
        else
        {
            pp_Title.text = $"Post Processing......................";
            pp_Setting.text = $"Off";
        }
    }

    #endregion

    #region SoundToggles

    public void ToggleSfx()
    {
        if(oM.sfxLevelSetting == 0)
        {
            oM.sfxLevelSetting = oM.lastSfxLevelSetting;
            mM_SfxToggle.color = new Color(1, 1, 1, 0);
            oM_SfxToggle.color = new Color(1, 1, 1, 0);
            sfxSlider.value = oM.sfxLevelSetting;
        }
        else
        {
            oM.lastSfxLevelSetting = oM.sfxLevelSetting;
            oM.sfxLevelSetting = 0;
            mM_SfxToggle.color = new Color(1, 1, 1, 1);
            oM_SfxToggle.color = new Color(1, 1, 1, 1);
            sfxSlider.value = oM.sfxLevelSetting;
        }
    }

    public void ToggleMusic()
    {
        if (oM.musicLevelSetting == 0)
        {
            oM.musicLevelSetting = oM.lastMusicLevelSetting;
            mM_MusicToggle.color = new Color(1, 1, 1, 0);
            oM_MusicToggle.color = new Color(1, 1, 1, 0);
            musicSlider.value = oM.musicLevelSetting;
        }
        else
        {
            oM.lastMusicLevelSetting = oM.musicLevelSetting;
            oM.musicLevelSetting = 0;
            mM_MusicToggle.color = new Color(1, 1, 1, 1);
            oM_MusicToggle.color = new Color(1, 1, 1, 1);
            musicSlider.value = oM.musicLevelSetting;
        }
    }

    #endregion

    #region SoundSliders
    public void SfxValueUpdate(Slider sfxSlider)
    {
        oM.sfxLevelSetting = Mathf.RoundToInt(sfxSlider.value);
        sfxPercentage.text = $"{oM.sfxLevelSetting}%";
        if(sfxSlider.value == 0)
        {
            mM_SfxToggle.color = new Color(1, 1, 1, 1);
            oM_SfxToggle.color = new Color(1, 1, 1, 1);
        }
        else
        {
            mM_SfxToggle.color = new Color(1, 1, 1, 0);
            oM_SfxToggle.color = new Color(1, 1, 1, 0);
        }
    }

    public void MusicValueUpdate(Slider musicSlider)
    {
        oM.musicLevelSetting = Mathf.RoundToInt(musicSlider.value);
        musicPercentage.text = $"{oM.musicLevelSetting}%";
        if (musicSlider.value == 0)
        {
            mM_MusicToggle.color = new Color(1, 1, 1, 1);
            oM_MusicToggle.color = new Color(1, 1, 1, 1);
        }
        else
        {
            mM_MusicToggle.color = new Color(1, 1, 1, 0);
            oM_MusicToggle.color = new Color(1, 1, 1, 0);
        }
    }

    #endregion

    #endregion

    public void removeSideBars()
    {
        if (sideBarRoutine != null) { StopCoroutine(sideBarRoutine); }
        sideBarRoutine = StartCoroutine(IERemoveSidebars());
    }

    private IEnumerator IERemoveSidebars()
    {
        RectTransform _leftPanelTransform = leftPanel.GetComponent<RectTransform>();
        RectTransform _rightPanelTransform = rightPanel.GetComponent<RectTransform>();

        float _timeValue = 0;

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / sideBarAnimDuration;
            float _evaluatedTimeValue = sideBarCurve.Evaluate(_timeValue);
            float _newXPos = Mathf.Lerp(panelWidth / 2, 0, _evaluatedTimeValue);
            float _newScale = Mathf.Lerp(panelWidth, 0, _evaluatedTimeValue);

            _leftPanelTransform.anchoredPosition = new Vector2(_newXPos, _leftPanelTransform.anchoredPosition.y);
            _rightPanelTransform.anchoredPosition = new Vector2(-_newXPos, _rightPanelTransform.anchoredPosition.y);
            _leftPanelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _newScale);
            _rightPanelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _newScale);

            yield return null;
        }

        animRoutine = null;
    }

    private void MoveCamera(float _duration, Vector3 _endPos)
    {
        if (animRoutine != null) { StopCoroutine(animRoutine); }
        animRoutine = StartCoroutine(IECameraAnim(_duration, _endPos));
    }

    private IEnumerator IECameraAnim(float _duration, Vector3 _endPos)
    {
        startPos = transform.position;
        float _timeValue = 0;

        while (_timeValue < 1)
        {
            _timeValue += Time.deltaTime / _duration;
            float _evaluatedTimeValue = animCurve.Evaluate(_timeValue);
            transform.position = Vector3.Lerp(startPos, _endPos, _evaluatedTimeValue);

            yield return null;
        }

        animRoutine = null;
    }

    public void UpdateButtonList()
    {
        buttonList.Clear();

        Button[] _buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < _buttons.Length; i++)
        {
            buttonList.Add(_buttons[i]);
        }
    }
}
