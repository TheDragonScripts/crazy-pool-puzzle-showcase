using System.Collections.Generic;

namespace InteractiveTutorial
{
    /// <summary>
    /// Interactive tutorial stage of tutorial, that contains
    /// actions, and condtion to move to the next tutorial stage.
    /// </summary>
    /// <remarks>
    /// Classes that implements this interface supposed to support
    /// sealing themselves.
    /// </remarks>
    public interface IInteractiveTutorialStage
    {
        bool IsSealed { get; }
        List<IInteractiveTutorialAction> Actions { get; }
        ICompletionCondition Condition { get; }
        IStepBackCondition StepBackCondition { get; }
        IInteractiveTutorialStage Seal();
        IInteractiveTutorialStage InsertActions(params IInteractiveTutorialAction[] actions);
        IInteractiveTutorialStage InsertAction(IInteractiveTutorialAction action);
        IInteractiveTutorialStage InsertCompletionCondition(ICompletionCondition condition);
        IInteractiveTutorialStage InsertStepBackCondition(IStepBackCondition condition);
    }
}