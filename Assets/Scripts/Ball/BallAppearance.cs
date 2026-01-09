using SDI;
using System;
using ThemesManagement;
using UnityEngine;
using UnityEngine.Rendering;

public class BallAppearance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private GameObject _shadowPrefab;
    [SerializeField] private BallShadow _actualShadow;
    [Space(5f)]
    [Header("Settings")]
    [SerializeField] private Material _colouredMaterial;
    [SerializeField] private Material _uncolouredMaterial;

    private IThemesManager _themesManager;
    private BallController _ballController;
    private bool _isShadowsForcedDisabled;

    private void OnDestroy()
    {
        if (_actualShadow != null)
        {
            Destroy(_actualShadow.gameObject);
        }
    }

    private void Start()
    {
        _ballController = GetComponent<BallController>();
        CreateShadow();
        new PortableDI().Inject(this);
    }

    [InjectionMethod]
    public void Inject(IThemesManager themesManager)
    {
        _themesManager = themesManager;
        _themesManager.SubscribeToThemeChangeEvent(OnThemeChanged);
    }

    [Obsolete("Consider to use UpdateMaterial method")]
    public void SetMaterial(BallColoring coloring)
    {
        _meshRenderer.material = (coloring == BallColoring.Coloured) ? _colouredMaterial : _uncolouredMaterial;
    }

    public void UpdateMaterial()
    {
        _meshRenderer.material = (_ballController.Coloring == BallColoring.Coloured) ? _colouredMaterial : _uncolouredMaterial;
    }

    public void ForceDisableShadows()
    {
        _isShadowsForcedDisabled = true;
        SetCastingShadows(false);
    }

    public void SetCastingShadows(bool castingShadows)
    {
        bool castShadows = castingShadows;
        if (_isShadowsForcedDisabled) castShadows = false;
        _meshRenderer.shadowCastingMode = (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On;
        if (_actualShadow != null) _actualShadow.gameObject.SetActive(castShadows);
    }

    [Obsolete("Themes is leaving old architecture. This method will be removed soon")]
    public void SetTheme(string productID)
    {
    }

    private void CreateShadow()
    {
        if (_actualShadow == null)
        {
            GameObject shadowObject = Instantiate(_shadowPrefab, null);
            _actualShadow = shadowObject.GetComponent<BallShadow>();
        }
        _actualShadow.enabled = true;
        _actualShadow.SetTarget(_ballController);
    }

    private void OnThemeChanged(ThemeData themeData)
    {
        string specialBallClassName = _ballController.SpeicalBall.GetType().Name;
        foreach (var skin in themeData.BallSkins)
        {
            if (specialBallClassName == skin.SpecialBallClassName)
            {
                _colouredMaterial = skin.Colored;
                _uncolouredMaterial = skin.Uncolored;
                UpdateMaterial();
                break;
            }
        }
    }
}
