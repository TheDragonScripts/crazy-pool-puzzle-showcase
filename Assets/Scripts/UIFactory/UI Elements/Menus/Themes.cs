using ModificatedUISystem.UIButtonsActions;
using ModificatedUISystem.UIButtonsActions.Wrappers;
using ModificatedUISystem.UIElements.Animation;
using SDI;
using System.Collections.Generic;
using ThemesManagement;
using ThirdPartiesIntegrations.IAP;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace ModificatedUISystem.UIElements
{
    public class Themes : MonoBehaviour, IUIElement, IUIOfType<MenuType>
    {
        [SerializeField] private DelayedAnimationController _animationController;

        [SerializeField] private Button _backButton;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private TMP_Dropdown _themeDropdown;
        [SerializeField] private GameObject _shopUnavailablePlate;

        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private LocalizeStringEvent _purchaseButtonLocalizeEvent;

        private IUIFactory _uiFactory;
        private IThemesManager _themesManager;
        private IInAppPurchasingIntegration _inAppPurchasingIntegration;
        private ThemeData[] _themeDatas;
        private bool _isShoppingAvailable;

        public IUIAnimationController AnimationController => _animationController;

        private enum ThemeState { Selected, Purchase, Select }

        private void Start() => new PortableDI().Inject(this);

        private void OnDestroy()
        {
            _inAppPurchasingIntegration.ProductPurchased -= OnProductPurchased;
            LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
        }

        [InjectionMethod]
        public void Inject(IUIFactory uiFactory, IThemesManager themesManager,
            IInAppPurchasingIntegration inAppPurchasingIntegration)
        {
            _uiFactory = uiFactory;
            _themesManager = themesManager;
            _inAppPurchasingIntegration = inAppPurchasingIntegration;
            _themeDatas = _themesManager.ThemeDatas;

            _backButton.onClick.AddListener(OnBackButtonClicked);
            _purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
            _themeDropdown.onValueChanged.AddListener(OnThemeDropdownValueChanged);
            _inAppPurchasingIntegration.SubscribeToFetchEvent(OnProductsFetched);
            _inAppPurchasingIntegration.ProductPurchased += OnProductPurchased;
            LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;

            _purchaseButton.interactable = false;

            SetupDropdownOptionsAndValue();
            PreviewTheme();
        }

        private void SetupDropdownOptionsAndValue()
        {
            _themeDropdown.ClearOptions();
            List<string> options = new List<string>();
            int defaultDropdownValue = 0;
            for (int i = 0; i < _themeDatas.Length; i++)
            {
                options.Add(_themeDatas[i].DisplayName.GetLocalizedString());
                if (ActualPlayerData.Data.SelectedTheme == _themeDatas[i].ThemeProductId)
                {
                    defaultDropdownValue = i;
                }
            }
            _themeDropdown.AddOptions(options);
            _themeDropdown.value = defaultDropdownValue;
        }

        private void PreviewTheme()
        {
            ThemeData themeData = _themeDatas[_themeDropdown.value];
            _priceText.text = _inAppPurchasingIntegration.GetProductPriceLocalizedString(themeData.ThemeProductId);
            _descriptionText.text = themeData.DisplayDescription.GetLocalizedString();
            _themesManager.PreviewTheme(themeData.ThemeProductId);
            UpdatePurchaseButtonVisuals();
        }

        private void UpdatePurchaseButtonVisuals()
        {
            if (!_isShoppingAvailable)
            {
                return;
            }

            ThemeState themeState = GetThemeState();
            string localeEntry = themeState switch
            {
                ThemeState.Selected => "Locale.Selected",
                ThemeState.Select => "Locale.Select",
                ThemeState.Purchase => "Locale.Purchase",
                _ => "Loacle.Selected"
            };

            _purchaseButton.interactable = themeState != ThemeState.Selected;
            _purchaseButtonLocalizeEvent.SetEntry(localeEntry);
        }

        private ThemeState GetThemeState()
        {
           string themeId = _themeDatas[_themeDropdown.value].ThemeProductId;

            if (ActualPlayerData.Data.SelectedTheme == themeId)
            {
                return ThemeState.Selected;
            }
            if (_themesManager.DefaultThemeId == themeId || _inAppPurchasingIntegration.IsSpecificProductPurchased(themeId))
            {
                return ThemeState.Select;
            }
            return ThemeState.Purchase;
        }

        private void OnThemeDropdownValueChanged(int value) => PreviewTheme();

        private void OnProductPurchased(Product product) => UpdatePurchaseButtonVisuals();

        private void OnProductsFetched()
        {
            _isShoppingAvailable = true;
            _purchaseButton.interactable = true;
            _shopUnavailablePlate.SetActive(false);
        }

        private void OnLanguageChanged(Locale locale)
        {
            SetupDropdownOptionsAndValue();
            PreviewTheme();
        }

        private void OnPurchaseButtonClicked()
        {
            if (!_isShoppingAvailable || _animationController.IsAnimating)
            {
                return;
            }
            string themeId = _themeDatas[_themeDropdown.value].ThemeProductId;
            ThemeState themeState = GetThemeState();
            if (themeState == ThemeState.Purchase)
            {
                _inAppPurchasingIntegration.Purchase(themeId);
            }
            else if (themeState == ThemeState.Select)
            {
                _themesManager.ApplyTheme(themeId);
            }
            UpdatePurchaseButtonVisuals();
        }

        private void OnBackButtonClicked()
        {
            if (!_animationController.IsAnimating)
            {
                new ButtonActionWithClickSoundWrapper(new BackButtonAction(_uiFactory)).Execute();
            }
        }
    }
}