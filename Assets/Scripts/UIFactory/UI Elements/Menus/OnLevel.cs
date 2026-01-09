using ModificatedUISystem.UIElements.Animation;
using UnityEngine;

namespace ModificatedUISystem.UIElements
{
    public class OnLevel : MonoBehaviour, IUIElement, IUIOfType<MenuType>
    {
        [SerializeField] private BaseAnimationController _animationController;
        public IUIAnimationController AnimationController => _animationController;
    }
}