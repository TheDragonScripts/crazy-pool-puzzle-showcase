using ModificatedUISystem.UIElements.Animation;

namespace ModificatedUISystem.UIElements
{
    public interface IUIElement
    {
        IUIAnimationController AnimationController { get; }
    }
}