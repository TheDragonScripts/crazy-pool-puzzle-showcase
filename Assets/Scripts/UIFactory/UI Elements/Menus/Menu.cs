using EntryPoint.Levels;
using ModificatedUISystem.UIButtonsActions;
using ModificatedUISystem.UIButtonsActions.Wrappers;
using ModificatedUISystem.UIElements.Animation;
using SDI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ModificatedUISystem.UIElements
{
    public class Menu : MonoBehaviour, IUIElement, IUIOfType<MenuType>, IFrequentlyUsedUI, IBlurredBackgoundUI
    {
        [SerializeField] private DelayedAnimationController _animationController;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _levelsButton;
        [SerializeField] private Button _themesButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _aboutButton;
        [SerializeField] private Button _tutorialsButton;

        private Dictionary<Button, Func<IUIButtonAction>> _buttonsActionsFactory = new();
        private Dictionary<Button, IUIButtonAction> _cachedButtonsActions = new();
        private ILevelManager _levelManager;
        private IUIFactory _uiFactory;

        public IUIAnimationController AnimationController => _animationController;

        private void Start() => new PortableDI().Inject(this);

        [InjectionMethod]
        public void Inject(ILevelManager levelManager, IUIFactory uiFactory)
        {
            _levelManager = levelManager;
            _uiFactory = uiFactory;

            _buttonsActionsFactory[_continueButton] = () => new ContinueButtonAction(levelManager);
            _buttonsActionsFactory[_levelsButton] = () => new LevelButtonAction(uiFactory);
            _buttonsActionsFactory[_themesButton] = () => new ThemesButtonAction(uiFactory);
            _buttonsActionsFactory[_settingsButton] = () => new SettingsButtonAction(uiFactory);
            _buttonsActionsFactory[_aboutButton] = () => new AboutButtonAction(uiFactory);
            _buttonsActionsFactory[_tutorialsButton] = () => new TutorialsButtonAction(uiFactory);

            foreach (var kvp in _buttonsActionsFactory)
                kvp.Key.onClick.AddListener(() => HandleButtonClick(kvp.Key));
        }

        private void HandleButtonClick(Button button)
        {
            if (_animationController.IsAnimating) return;
            if (!_cachedButtonsActions.TryGetValue(button, out _))
                _cachedButtonsActions[button]
                    = new ButtonActionWithClickSoundWrapper(_buttonsActionsFactory[button].Invoke());
            _cachedButtonsActions[button].Execute();
        }
    }
}