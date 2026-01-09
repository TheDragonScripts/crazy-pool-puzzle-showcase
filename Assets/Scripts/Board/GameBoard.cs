using SDI;
using System;
using ThemesManagement;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _sideTriggers;
    [SerializeField] private Transform _edges;
    [SerializeField] private BoardSkinObject _gameBoardSkin;

    private bool _isFlipped;
    private IThemesManager _themesManager;

    public bool IsFlipped => _isFlipped;

    public delegate void GameBoardVoidDelegate();
    public event GameBoardVoidDelegate OnGameBoardFlipped;

    private void Start()
    {
        new PortableDI().Inject(this);
    }

    [InjectionMethod]
    public void Inject(IThemesManager themesManager)
    {
        _themesManager = themesManager;
        _themesManager.SubscribeToThemeChangeEvent(OnThemeChanged);
    }

    [Obsolete("GameBoard now uses new ThemesManagement module. Themes handling is private.")]
    public void SetTheme(string theme)
    {
        /*Theme themeData = ThemesSettings.Instance.GetThemeByProductID(theme);
        if (themeData == null) throw new NullReferenceException("Theme id is incorrect. No theme data found");
        if (_theme == themeData.ProductID) return;
        _theme = themeData.ProductID;
        UpdateTheme(themeData);*/
    }

    public void Flip()
    {
        if (_sideTriggers == null || _edges == null) return;
        Vector3 flippedVectorY = new Vector3(0, 180, 0);
        Vector3 flippedVectorZ = new Vector3(0, 0, 180);
        _isFlipped = !_isFlipped;
        _sideTriggers.eulerAngles += flippedVectorY;
        _edges.eulerAngles = flippedVectorY;
        foreach (var part in _gameBoardSkin.Model) part.eulerAngles += flippedVectorZ;
        if (OnGameBoardFlipped != null) OnGameBoardFlipped();
    }

    private void OnThemeChanged(ThemeData themeData)
    {
        Destroy(_gameBoardSkin?.gameObject);
        GameObject gameBoardSkinInstance = Instantiate(themeData.GameBoardPrefab, transform);
        _gameBoardSkin = gameBoardSkinInstance.GetComponent<BoardSkinObject>();
    }
}
