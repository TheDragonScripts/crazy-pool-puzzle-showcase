using ModificatedUISystem.UIElements.Animation;
using UnityEngine;

namespace ModificatedUISystem.UIElements
{
    public class About : MonoBehaviour, IUIElement, IUIOfType<MenuType>, IBlurredBackgoundUI
    {
        [SerializeField] private DelayedAnimationController _animationController;
        public IUIAnimationController AnimationController => _animationController;
    }
}