using UnityEngine;
using EnumerableAnchors;

namespace InteractiveTutorial
{
    public class LevelOneSchedule : MonoBehaviour, ILevelTutorialSchedule
    {
        public void FormAndStart(IInteractiveTutorialScheduleBase scheduleBase, InteractiveTutorial tutorial,
            InteractiveTutorialMessage interactiveMessage, InteractiveTutorialPointer pointer,
            ObjectsHighlighter highlighter, Stash stash)
        {
            TutorialReferenceObject stashBall = scheduleBase.FindTutorialReferenceObject("stashball") as TutorialReferenceObject;
            IInteractiveTutorialStage stage1 = new InteractiveTutorialStage()
                .InsertCompletionCondition(new BigOneBallCompletionCondition(stash, stashBall.BallController))
                .InsertActions(
                    new ClickPointerAction(pointer, stashBall.gameObject),
                    new HighlightObjectAction(stashBall.gameObject, highlighter),
                    new MessageAction(interactiveMessage, EnumerableAnchor.MiddleCenter, "Locale.InteractiveTutorial.1.PickBallFromStash")
                )
                .Seal();

            TutorialReferenceObject spawnArea = scheduleBase.FindTutorialReferenceObject("spawnarea") as TutorialReferenceObject;
            IInteractiveTutorialStage stage2 = new InteractiveTutorialStage()
                .InsertCompletionCondition(new BallOnBoardCompletionCondition(stashBall.BallController))
                .InsertActions(
                    new SwipePointerAction(pointer, stashBall.gameObject, spawnArea.gameObject),
                    new HighlightObjectAction(stashBall.gameObject, highlighter),
                    new MessageAction(interactiveMessage, EnumerableAnchor.MiddleUp, "Locale.InteractiveTutorial.1.PutBallOnTable")
                )
                .Seal();

            TutorialReferenceObject eightBall = scheduleBase.FindTutorialReferenceObject("eightball") as TutorialReferenceObject;
            IInteractiveTutorialStage stage3 = new InteractiveTutorialStage()
                .InsertCompletionCondition(new BallColoringCompleitionCondition(eightBall.BallController, BallColoring.Coloured))
                .InsertStepBackCondition(new BallUnavailableStepBackCondition(stashBall.BallController, stash, interactiveMessage))
                .InsertActions(
                    new SwipePointerAction(pointer, stashBall.gameObject, eightBall.gameObject),
                    new HighlightObjectAction(stashBall.gameObject, highlighter, Color.red),
                    new HighlightObjectAction(eightBall.gameObject, highlighter),
                    new MessageAction(interactiveMessage, EnumerableAnchor.MiddleUp, "Locale.InteractiveTutorial.1.HitEightBall"),
                    new MessageAction(interactiveMessage, EnumerableAnchor.MiddleDown, new Vector2(0, 250), "Locale.InteractiveTutorial.FingerPower")
                )
                .Seal();

            tutorial
                .InsertStages(stage1, stage2, stage3)
                .Seal()
                .StartTutorial();
        }
    }
}