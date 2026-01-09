namespace InteractiveTutorial
{
    /// <summary>
    /// Interface that allows action to do an active fallback.
    /// For example checks if ball is not available to touch and change it
    /// to another one.
    /// </summary>
    public interface IFallbackableAction
    {
        void CheckForFallback();
    }
}