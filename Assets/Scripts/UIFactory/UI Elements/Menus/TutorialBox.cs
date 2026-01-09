using ModificatedUISystem.UIElements.Animation;
using UnityEngine;

namespace ModificatedUISystem.UIElements
{
    public class TutorialBox : MonoBehaviour, IUIElement, IUIOfType<MessageBoxType>
    {
        [SerializeField] private BaseAnimationController _baseAnimationController;

        public IUIAnimationController AnimationController => _baseAnimationController;
    }
}