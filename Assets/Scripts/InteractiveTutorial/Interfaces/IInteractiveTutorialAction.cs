namespace InteractiveTutorial
{
    /// <summary>
    /// Interactive tutorial action such as message on screen,
    /// pointer to swipe or click action.
    /// </summary>
    public interface IInteractiveTutorialAction
    {
        void Do();
        void Undo();
    }
}