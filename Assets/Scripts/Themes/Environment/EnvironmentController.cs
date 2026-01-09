using SDI;
using UnityEngine;

namespace ThemesManagement.Environment
{
    public class EnvironmentController : IEnvironmentController
    {
        private Light _light;
        private IThemesManager _themesManager;
        private const string LightingPrefabResourcePath = "Prefabs/Light/Directional Light";

        public EnvironmentController()
        {
            _light = GameObject.Instantiate(Resources.Load<Light>(LightingPrefabResourcePath));
            if (_light == null)
            {
                CSDL.LogError($"{nameof(EnvironmentController)} failed to instantiate prefab " +
                    $"from path {LightingPrefabResourcePath}");
                return;
            }
            RenderSettings.sun = _light;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        }

        [InjectionMethod]
        public void Inject(IThemesManager themesManager)
        {
            _themesManager = themesManager;
            _themesManager.SubscribeToThemeChangeEvent(OnThemeChanged);
        }

        private void OnThemeChanged(ThemeData themeData)
        {
            ChangeLighting(themeData.LightingSettings);
            ChangeEnvironment(themeData.EnvironmentSettings);
        } 

        private void ChangeEnvironment(EnvironmentSettings environmentSettings)
        {
            RenderSettings.ambientEquatorColor = environmentSettings.EquatorColor;
            RenderSettings.ambientGroundColor = environmentSettings.GroundColor;
            RenderSettings.ambientSkyColor = environmentSettings.SkyColor;
        }

        private void ChangeLighting(LightingSettings lightingSettings)
        {
            _light.intensity = lightingSettings.Intesity;
            _light.color = lightingSettings.Color;
        }
    }
}
