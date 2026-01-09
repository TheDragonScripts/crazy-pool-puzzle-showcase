using UnityEngine;

namespace InteractiveTutorial
{
    public class LevelThirtyThreeSchedule : MonoBehaviour, ILevelTutorialSchedule
    {
        public void FormAndStart(IInteractiveTutorialScheduleBase scheduleBase, InteractiveTutorial tutorial,
            InteractiveTutorialMessage interactiveMessage, InteractiveTutorialPointer pointer,
            ObjectsHighlighter highlighter, Stash stash)
        {
            TutorialReferenceObject barrier = scheduleBase.FindTutorialReferenceObject("barrier") as TutorialReferenceObject;
            TutorialReferenceObject destroyer = scheduleBase.FindTutorialReferenceObject("destroyer") as TutorialReferenceObject;
            TutorialReferenceObject spawnArea = scheduleBase.FindTutorialReferenceObject("spawnarea") as TutorialReferenceObject;

            IInteractiveTutorialStage stage1 = new InteractiveTutorialStage();
            stage1
                .InsertCompletionCondition(new BigOneBallCompletionCondition(stash, destroyer.BallController))
                .InsertActions(
                    new HighlightObjectAction(destroyer.gameObject, highlighter),
                    new MessageAction(interactiveMessage, EnumerableAnchors.EnumerableAnchor.MiddleCenter, "Locale.InteractiveTutorial.33.Destroyer"),
                    new ClickPointerAction(pointer, destroyer.gameObject)
                )
                .Seal();

            InteractiveTutorialStage stage2 = new InteractiveTutorialStage();
            stage2
                .InsertCompletionCondition(new BallOnBoardCompletionCondition(destroyer.BallController))
                .InsertActions(
                    new HighlightObjectAction(destroyer.gameObject, highlighter),
                    new SwipePointerAction(pointer, destroyer.gameObject, spawnArea.gameObject),
                    new MessageAction(interactiveMessage, EnumerableAnchors.EnumerableAnchor.MiddleDown, "Locale.InteractiveTutorial.33.PutDestroyerOnBoard")
                )
                .Seal();

            InteractiveTutorialStage stage3 = new InteractiveTutorialStage();
            stage3
                .InsertCompletionCondition(new ObjectBeginDissolutedCompletionCondition(barrier.DissolutionObserver))
                .InsertStepBackCondition(new BallUnavailableStepBackCondition(destroyer.BallController, stash, interactiveMessage))
                .InsertActions(
                    new HighlightObjectAction(destroyer.gameObject, highlighter),
                    new HighlightObjectAction(barrier.gameObject, highlighter, Color.red),
                    new MessageAction(interactiveMessage, EnumerableAnchors.EnumerableAnchor.MiddleCenter, "Locale.InteractiveTutorial.33.DestroyBarrier"),
                    new SwipePointerAction(pointer, destroyer.gameObject, barrier.gameObject)
                )
                .Seal();

            tutorial
                .InsertStages(stage1, stage2, stage3)
                .Seal()
                .StartTutorial();
        }
    }
}