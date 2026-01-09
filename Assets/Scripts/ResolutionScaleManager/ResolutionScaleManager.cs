using EntryPoint;
using EntryPoint.GameData;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ResolutionScale
{
    /*
     * Code here is temporary disabled because it references old UI system.
     * Pending refactoring.
     */
    public class ResolutionScaleManager : MonoBehaviour
    {
        [SerializeField, Range(0.25f, 1f)] private float _defaultResolutionScale = 0.75f;
        private IGameSaverAndLoader _gameSaverAndLoader;
        private Slider _resolutionScaleSlider;
        private Resolution _nativeResolution;
        public static ResolutionScaleManager Instance { get; private set; }
        public static float DefaultResolutionScale { get; private set; } = 0.75f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                //StartCoroutine(Initialize());
                DefaultResolutionScale = _defaultResolutionScale;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private Resolution GetNativeResolution()
        {
            Resolution nativeResolution = new Resolution();
            if (Screen.resolutions != null && Screen.resolutions.Length > 0)
            {
                nativeResolution.width = Screen.resolutions[Screen.resolutions.Length - 1].width;
                nativeResolution.height = Screen.resolutions[Screen.resolutions.Length - 1].height;
            }
            else
            {
                nativeResolution.width = Display.main.systemWidth;
                nativeResolution.height = Display.main.systemHeight;
            }
            return nativeResolution;
        }

        private void SwitchResolution()
        {
            float resolutionScale = ActualPlayerData.Data.ResolutionScale;
            int width = (int)(_nativeResolution.width * resolutionScale);
            int height = (int)(_nativeResolution.height * resolutionScale);
            Screen.SetResolution(width, height, Screen.fullScreenMode);
        }

        /*private IEnumerator Initialize()
        {
            yield return new WaitUntil(() => UIManager.Instance != null && ActualPlayerData.Data != null
                && GameEntryPoint.Instance != null);
            _nativeResolution = GetNativeResolution();
            _gameSaverAndLoader = GameEntryPoint.Instance.GameSaverAndLoader;
            _resolutionScaleSlider = UIManager.Instance.ResolutionScaleSlider;
            UIManager.Instance.OnUIFinishedAnimation += SwitchResolutionUsingSlider;
            _gameSaverAndLoader.GameBeginSaved += SaveScaleSliderValue;
            float resolutionScale = ActualPlayerData.Data.ResolutionScale == 0f ? _defaultResolutionScale : ActualPlayerData.Data.ResolutionScale;
            _resolutionScaleSlider.value = resolutionScale;
            StartCoroutine(SetResolutionDelayed());
        }*/

        private IEnumerator SetResolutionDelayed()
        {
            yield return new WaitForSeconds(2f);
            SwitchResolution();
        }

        private void SaveScaleSliderValue() => ActualPlayerData.Data.ResolutionScale = _resolutionScaleSlider.value;

        /*private void SwitchResolutionUsingSlider(UIElementsGroup ui, UIAnimations animtype)
        {
            if (ui.name == "Settings" && animtype == UIAnimations.Hide)
            {
                ActualPlayerData.Data.ResolutionScale = _resolutionScaleSlider.value;
                SwitchResolution();
            }
        }*/
    }
}