using ModificatedUISystem.UIElements.Animation;
using ThirdPartiesIntegrations.AgeGateSystem;
using UnityEngine;

namespace ModificatedUISystem.UIElements
{
    public class AgeGateForm : MonoBehaviour, IUIElement, IUIOfType<MessageBoxType>
    {
        [SerializeField] private BaseAnimationController _baseAnimationController;

        public IUIAnimationController AnimationController => _baseAnimationController;

        public event AgeGateEventHandler UserAgeGroupObtained;
    }
}