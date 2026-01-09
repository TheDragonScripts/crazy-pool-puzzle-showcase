using ModificatedUISystem.UIElements.Animation;
using UnityEngine;

namespace ModificatedUISystem.UIElements
{
    public class Pause : MonoBehaviour, IUIElement, IUIOfType<MenuType>, IBlurredBackgoundUI
    {
        [SerializeField] private DelayedAnimationController _animationController;
        public IUIAnimationController AnimationController => _animationController;
    }
}