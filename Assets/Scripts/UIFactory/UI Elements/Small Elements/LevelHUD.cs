using ModificatedUISystem.UIElements.Animation;
using UnityEngine;

namespace ModificatedUISystem.UIElements
{
    public class LevelHUD : MonoBehaviour, IUIElement, IUIOfType<HudType>
    {
        [SerializeField] private BaseAnimationController _baseAnimationController;

        public IUIAnimationController AnimationController => _baseAnimationController;
    }
}