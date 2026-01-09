using UnityEngine;

namespace InteractiveTutorial
{
    public class LevelFourtyFourSchedule : MonoBehaviour, ILevelTutorialSchedule
    {
        public void FormAndStart(IInteractiveTutorialScheduleBase scheduleBase, InteractiveTutorial tutorial,
            InteractiveTutorialMessage interactiveMessage, InteractiveTutorialPointer pointer,
            ObjectsHighlighter highlighter, Stash stash)
        {
            TutorialReferenceObject healthBarrier = scheduleBase.FindTutorialReferenceObject("healthbarrier") as TutorialReferenceObject;

            IInteractiveTutorialStage stage1 = new InteractiveTutorialStage();
            stage1
                .InsertCompletionCondition(new ObjectBeginDissolutedCompletionCondition(healthBarrier.DissolutionObserver))
                .InsertActions(
                    new HighlightObjectAction(healthBarrier.gameObject, highlighter, new Color(0.871f, 0.192f, 0.388f)),
                    new MessageAction(interactiveMessage, EnumerableAnchors.EnumerableAnchor.MiddleCenter, new Vector2(0, -150),
                        "Locale.InteractiveTutorial.44.HealthBarrier")
                )
                .Seal();

            tutorial
                .InsertStage(stage1)
                .Seal()
                .StartTutorial();
        }
    }
}