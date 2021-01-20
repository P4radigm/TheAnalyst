using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace PurpleFlame
{
    public class PauseManager : MonoBehaviour
    {
        public static PauseManager instance;

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

        [Header("General")]
        private CameraTarget cT;
        [SerializeField] private Canvas inGameCanvas;
        [SerializeField] private Canvas pauseCanvas;
        [SerializeField] private GameObject pauseMain;
        [SerializeField] private GameObject pauseSettings;
        [Header("Settings")]
        [SerializeField] private Image M_sfxToggle;
        [SerializeField] private Image S_sfxToggle;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TextMeshProUGUI sfxPercentage;
        [SerializeField] private Image M_musicToggle;
        [SerializeField] private Image S_musicToggle;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private TextMeshProUGUI musicPercentage;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI ics_Title;
        [SerializeField] private TextMeshProUGUI ics_Setting;
        [SerializeField] private TextMeshProUGUI preset_Title;
        [SerializeField] private TextMeshProUGUI preset_Setting;
        [SerializeField] private TextMeshProUGUI tq_Title;
        [SerializeField] private TextMeshProUGUI tq_Setting;
        [SerializeField] private TextMeshProUGUI nC_Title;
        [SerializeField] private TextMeshProUGUI nC_Setting;
        [SerializeField] private TextMeshProUGUI s_Title;
        [SerializeField] private TextMeshProUGUI s_Setting;
        [SerializeField] private TextMeshProUGUI pp_Title;
        [SerializeField] private TextMeshProUGUI pp_Setting;
        private OptionsManager oM;
        private float usualtime;

        private void Start()
        {
            oM = OptionsManager.instance;
            

            ToggleSfx();
            ToggleSfx();
            ToggleMusic();
            ToggleMusic();
            sfxSlider.value = oM.sfxLevelSetting;
            musicSlider.value = oM.musicLevelSetting;
            UIPerformancePreset();
        }

        #region Navigation

        public void Pause()
        {
            inGameCanvas.gameObject.SetActive(false);
            pauseCanvas.gameObject.SetActive(true);
            pauseSettings.SetActive(false);
            pauseMain.SetActive(true);
            //usualtime = Time.timeScale;
            //Time.timeScale = 0;
            ObjectRotation.Instance.enabled = false;
            cT = ObjectRotation.Instance.gameObject.GetComponent<CameraTarget>();
            cT.enabled = false;
            //Block in-game interactions or time or smth like that
        }

        public void Continue()
        {
            pauseCanvas.gameObject.SetActive(false);
            inGameCanvas.gameObject.SetActive(true);
            //Time.timeScale = usualtime;
            ObjectRotation.Instance.enabled = true;
            cT.enabled = true;
            //Resume in-game interactions or time
        }

        public void Settings()
        {
            pauseMain.SetActive(false);
            pauseSettings.SetActive(true);
        }

        public void Back()
        {
            pauseSettings.SetActive(false);
            pauseMain.SetActive(true);
        }

        public void MainMenu()
        {
            Debug.Log($"Goes back to Main Menu");
            //SceneManager.LoadScene("MainMenu");
        }

        #endregion

        #region SoundSettings

        public void ToggleSfx()
        {
            if (oM.sfxLevelSetting == 0)
            {
                oM.sfxLevelSetting = oM.lastSfxLevelSetting;
                M_sfxToggle.color = new Color(1, 1, 1, 0);
                S_sfxToggle.color = new Color(1, 1, 1, 0);
                sfxSlider.value = oM.sfxLevelSetting;
            }
            else
            {
                oM.lastSfxLevelSetting = oM.sfxLevelSetting;
                oM.sfxLevelSetting = 0;
                M_sfxToggle.color = new Color(1, 1, 1, 1);
                S_sfxToggle.color = new Color(1, 1, 1, 1);
                sfxSlider.value = oM.sfxLevelSetting;
            }
        }

        public void ToggleMusic()
        {
            if (oM.musicLevelSetting == 0)
            {
                oM.musicLevelSetting = oM.lastMusicLevelSetting;
                M_musicToggle.color = new Color(1, 1, 1, 0);
                S_musicToggle.color = new Color(1, 1, 1, 0);
                musicSlider.value = oM.musicLevelSetting;
            }
            else
            {
                oM.lastMusicLevelSetting = oM.musicLevelSetting;
                oM.musicLevelSetting = 0;
                M_musicToggle.color = new Color(1, 1, 1, 1);
                S_musicToggle.color = new Color(1, 1, 1, 1);
                musicSlider.value = oM.musicLevelSetting;
            }
        }

        public void SfxValueUpdate(Slider sfxSlider)
        {
            oM.sfxLevelSetting = Mathf.RoundToInt(sfxSlider.value);
            sfxPercentage.text = $"{oM.sfxLevelSetting}%";
            if (sfxSlider.value == 0)
            {
                M_sfxToggle.color = new Color(1, 1, 1, 1);
                S_sfxToggle.color = new Color(1, 1, 1, 1);
            }
            else
            {
                M_sfxToggle.color = new Color(1, 1, 1, 0);
                S_sfxToggle.color = new Color(1, 1, 1, 0);
            }
        }

        public void MusicValueUpdate(Slider musicSlider)
        {
            oM.musicLevelSetting = Mathf.RoundToInt(musicSlider.value);
            musicPercentage.text = $"{oM.musicLevelSetting}%";
            if (musicSlider.value == 0)
            {
                M_musicToggle.color = new Color(1, 1, 1, 1);
                S_musicToggle.color = new Color(1, 1, 1, 1);
            }
            else
            {
                M_musicToggle.color = new Color(1, 1, 1, 0);
                S_musicToggle.color = new Color(1, 1, 1, 0);
            }
        }

        #endregion

        #region GraphicsSettings

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

        public void UpdateNotch()
        {
            if (oM.notchSetting)
            {
                oM.notchSetting = false;
            }
            else
            {
                oM.notchSetting = true;
            }

            oM.UpdateNotchDisplay();
        }

        public void UINotch()
        {
            if (oM.notchSetting)
            {
                nC_Title.text = $"Notch Correction..................";
                nC_Setting.text = $"On";
            }
            else
            {
                nC_Title.text = $"Notch Correction..................";
                nC_Setting.text = $"Off";
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

        public void UpdatePerformancePreset()
        {
            if (oM.ppSetting == performancePresetSetting.High)
            {
                oM.ppSetting = performancePresetSetting.Middle;
                oM.tqSetting = TextureQualitySetting.Middle;
                //oM.notchSetting = false;
                oM.shadowsSetting = true;
                oM.PostProcessingSetting = false;
            }
            else if (oM.ppSetting == performancePresetSetting.Middle)
            {
                oM.ppSetting = performancePresetSetting.Low;
                oM.tqSetting = TextureQualitySetting.Low;
                //oM.notchSetting = false;
                oM.shadowsSetting = false;
                oM.PostProcessingSetting = false;
            }
            else if (oM.ppSetting == performancePresetSetting.Low)
            {
                oM.ppSetting = performancePresetSetting.High;
                oM.tqSetting = TextureQualitySetting.High;
                //oM.notchSetting = true;
                oM.shadowsSetting = true;
                oM.PostProcessingSetting = true;
            }
            else
            {
                oM.ppSetting = performancePresetSetting.Middle;
                oM.tqSetting = TextureQualitySetting.Middle;
                //oM.notchSetting = false;
                oM.shadowsSetting = true;
                oM.PostProcessingSetting = false;
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

                s_Title.text = $"Shadows....................................";
                s_Setting.text = $"On";

                pp_Title.text = $"Post Processing......................";
                pp_Setting.text = $"On";
            }
            else if (oM.ppSetting == performancePresetSetting.Middle)
            {
                preset_Title.text = $"Performance Preset.....";
                preset_Setting.text = $"Middle";

                tq_Title.text = $"Texture Quality.............";
                tq_Setting.text = $"Middle";

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
    }
}
