using System.Collections.Generic;

namespace InteractiveTutorial
{
    /// <summary>
    /// Provides the most functionality for Interactive Tutorial System.
    /// </summary>
    public interface IInteractiveTutorial
    {
        int CurrentStage { get; }
        List<IInteractiveTutorialStage> Stages { get; }
        bool IsSealed { get; }
        IInteractiveTutorial Seal();
        IInteractiveTutorial InsertStages(params IInteractiveTutorialStage[] stages);
        IInteractiveTutorial InsertStage(IInteractiveTutorialStage stage);
        IInteractiveTutorial StartTutorial();
    }
}