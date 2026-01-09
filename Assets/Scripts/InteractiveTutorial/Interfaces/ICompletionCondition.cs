namespace InteractiveTutorial
{
    /// <summary>
    /// Condition that supposed to be completed to move to the next
    /// tutorial stage.
    /// </summary>
    public interface ICompletionCondition
    {
        bool Check();
    }
}