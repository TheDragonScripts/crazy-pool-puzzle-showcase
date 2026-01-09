namespace ModificatedUISystem.UIElements.Animation.Strategies
{
    public interface IUIAnimationStrategy
    {
        event AnimationControllerEventHandler AnimationFinished;
        void Hide();
        void Show();
    }
}