using EnergySystem;
using ModificatedUISystem.SpecialElements;
using ModificatedUISystem.UIElements.Animation;
using SDI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModificatedUISystem.UIElements
{
    public class EnergyRestoreOfferScreen : MonoBehaviour, IUIElement, IUIOfType<MessageBoxType>
    {
        [SerializeField] private BaseAnimationController _baseAnimationController;
        [SerializeField] private ScrollDownPointer _scrollDownPointer;
        [SerializeField] private TextMeshPro _energyCapacityTitle;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _restoreButton;
        private IUIFactory _uiFactory;
        private IEnergyManager _energyManager;

        public IUIAnimationController AnimationController => _baseAnimationController;

        private void Start() => new PortableDI().Inject(this);

        [InjectionMethod]
        public void Inject(IUIFactory uiFactory, IEnergyManager energyManager)
        {
            _uiFactory = uiFactory;
            _energyManager = energyManager;
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            _restoreButton.onClick.AddListener(OnRestoreButtonClicked);
        }

        private void OnRestoreButtonClicked()
        {
            CSDL.LogWarning("RestoreOfferScreen: restore logic not implemented yet!");
        }

        private void OnCloseButtonClicked()
        {
            _uiFactory.Close<EnergyRestoreOfferScreen>();
        }
    }
}