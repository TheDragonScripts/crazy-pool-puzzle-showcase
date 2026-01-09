using UnityEngine;

namespace InteractiveTutorial
{
    public class LevelSeventyThreeSchedule : MonoBehaviour, ILevelTutorialSchedule
    {
        public void FormAndStart(IInteractiveTutorialScheduleBase scheduleBase, InteractiveTutorial tutorial,
            InteractiveTutorialMessage interactiveMessage, InteractiveTutorialPointer pointer,
            ObjectsHighlighter highlighter, Stash stash)
        {
            TutorialReferenceObject explosiveBall = scheduleBase.FindTutorialReferenceObject("explosiveball") as TutorialReferenceObject;
            
            InteractiveTutorialStage stage1 = new InteractiveTutorialStage();
            stage1
                .InsertCompletionCondition(new ObjectDestroyedCompletionCondition(explosiveBall.gameObject))
                .InsertActions(
                    new HighlightObjectAction(explosiveBall.gameObject, highlighter),
                    new MessageAction(interactiveMessage, EnumerableAnchors.EnumerableAnchor.MiddleDown, new Vector2(0, 100),
                        "Locale.InteractiveTutorial.73.ExplosiveBall")
                )
                .Seal();

            tutorial
                .InsertStage(stage1)
                .Seal()
                .StartTutorial();
        }
    }
}