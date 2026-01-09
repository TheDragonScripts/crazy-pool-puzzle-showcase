namespace ModificatedUISystem.UIElements.Animation
{
    public interface IUIAnimationController
    {
        event AnimationControllerEventHandler AnimationFinished;
        bool IsTemporaryHidden { get; }
        bool IsAnimating { get; }
        void Show();
        void Hide();
        void HideTemporary();
    }
}