using System.Collections.Generic;

namespace InteractiveTutorial
{
    /// <summary>
    /// Interface that determines interactive tutorial schedule.
    /// Using to provide basis functionality for tutorials on levels.
    /// </summary>
    public interface IInteractiveTutorialScheduleBase
    {
        Dictionary<string, ITutorialReferenceObject> TutorialReferencesObjects { get; }
        ITutorialReferenceObject FindTutorialReferenceObject(string id);
    }
}