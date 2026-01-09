using UnityEngine;

namespace InteractiveTutorial
{
    public class LevelFourtyThreeSchedule : MonoBehaviour, ILevelTutorialSchedule
    {
        public void FormAndStart(IInteractiveTutorialScheduleBase scheduleBase, InteractiveTutorial tutorial,
            InteractiveTutorialMessage interactiveMessage, InteractiveTutorialPointer pointer,
            ObjectsHighlighter highlighter, Stash stash)
        {
            TutorialReferenceObject eight = scheduleBase.FindTutorialReferenceObject("eight") as TutorialReferenceObject;
            TutorialReferenceObject electricBarrier = scheduleBase.FindTutorialReferenceObject("electricbarrier") as TutorialReferenceObject;

            IInteractiveTutorialStage stage1 = new InteractiveTutorialStage();
            stage1
                .InsertCompletionCondition(new BallColoringCompleitionCondition(eight.BallController, BallColoring.Coloured))
                .InsertActions(
                    new HighlightObjectAction(electricBarrier.gameObject, highlighter),
                    new MessageAction(interactiveMessage, EnumerableAnchors.EnumerableAnchor.MiddleDown, new Vector2(0, 150),
                        "Locale.InteractiveTutorial.43.ElectricBarrier")
                )
                .Seal();

            tutorial
                .InsertStage(stage1)
                .Seal()
                .StartTutorial();
        }
    }
}