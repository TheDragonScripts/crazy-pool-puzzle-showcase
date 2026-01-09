using UnityEngine;

namespace InteractiveTutorial
{
    public class LevelEightyEightSchedule : MonoBehaviour, ILevelTutorialSchedule
    {
        public void FormAndStart(IInteractiveTutorialScheduleBase scheduleBase, InteractiveTutorial tutorial,
            InteractiveTutorialMessage interactiveMessage, InteractiveTutorialPointer pointer,
            ObjectsHighlighter highlighter, Stash stash)
        {
            TutorialReferenceObject boardFlipper = scheduleBase.FindTutorialReferenceObject("boardflipper") as TutorialReferenceObject;

            IInteractiveTutorialStage stage1 = new InteractiveTutorialStage();
            stage1
                .InsertCompletionCondition(new ObjectBeginDissolutedCompletionCondition(boardFlipper.DissolutionObserver))
                .InsertActions(
                    new HighlightObjectAction(boardFlipper.gameObject, highlighter),
                    new MessageAction(interactiveMessage, EnumerableAnchors.EnumerableAnchor.MiddleDown, new Vector2(0, 100),
                        "Locale.InteractiveTutorial.88.BoardFlipper")
                )
                .Seal();

            tutorial
                .InsertStage(stage1)
                .Seal()
                .StartTutorial();
        }
    }
}