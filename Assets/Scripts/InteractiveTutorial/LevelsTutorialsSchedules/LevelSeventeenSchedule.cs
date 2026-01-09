using UnityEngine;

namespace InteractiveTutorial
{
    public class LevelSeventeenSchedule : MonoBehaviour, ILevelTutorialSchedule
    {
        public void FormAndStart(IInteractiveTutorialScheduleBase scheduleBase, InteractiveTutorial tutorial,
            InteractiveTutorialMessage interactiveMessage, InteractiveTutorialPointer pointer,
            ObjectsHighlighter highlighter, Stash stash)
        {
            TutorialReferenceObject barrier = scheduleBase.FindTutorialReferenceObject("barrier") as TutorialReferenceObject;
            TutorialReferenceObject eight = scheduleBase.FindTutorialReferenceObject("eight") as TutorialReferenceObject;
            IInteractiveTutorialStage stage1 = new InteractiveTutorialStage();
            stage1
                .InsertCompletionCondition(new BallColoringCompleitionCondition(eight.BallController, BallColoring.Coloured))
                .InsertActions(
                    new HighlightObjectAction(barrier.gameObject, highlighter, Color.red),
                    new MessageAction(interactiveMessage, EnumerableAnchors.EnumerableAnchor.MiddleCenter, new Vector2(0, -50),
                        "Locale.InteractiveTutorial.17.Barrier")
                )
                .Seal();
            tutorial
                .InsertStage(stage1)
                .Seal()
                .StartTutorial();
        }
    }
}