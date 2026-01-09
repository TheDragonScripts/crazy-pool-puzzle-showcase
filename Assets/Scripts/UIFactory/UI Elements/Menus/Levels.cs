using ModificatedUISystem.UIButtonsActions;
using ModificatedUISystem.UIButtonsActions.Wrappers;
using ModificatedUISystem.UIElements.Animation;
using SDI;
using UnityEngine;
using UnityEngine.UI;

namespace ModificatedUISystem.UIElements
{
    public class Levels : MonoBehaviour, IUIElement, IUIOfType<MenuType>, IBlurredBackgoundUI
    {
        [SerializeField] private DelayedAnimationController _animationController;
        [SerializeField] private Button _backButton;
        private IUIButtonAction _backButtonAction;
        private IUIFactory _uiFactory;

        public IUIAnimationController AnimationController => _animationController;

        private void Start() => new PortableDI().Inject(this);

        [InjectionMethod]
        public void Inject(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            _backButton.onClick.AddListener(HandleButtonClick);
        }

        private void HandleButtonClick()
        {
            if (_animationController.IsAnimating) return;
            if (_backButtonAction == null)
                _backButtonAction
                    = new ButtonActionWithClickSoundWrapper(new BackButtonAction(_uiFactory));
            _backButtonAction.Execute();
        }
    }
}