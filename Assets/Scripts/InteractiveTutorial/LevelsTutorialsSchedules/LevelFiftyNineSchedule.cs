using UnityEngine;

namespace InteractiveTutorial
{
    public class LevelFiftyNineSchedule : MonoBehaviour, ILevelTutorialSchedule
    {
        public void FormAndStart(IInteractiveTutorialScheduleBase scheduleBase, InteractiveTutorial tutorial,
            InteractiveTutorialMessage interactiveMessage, InteractiveTutorialPointer pointer,
            ObjectsHighlighter highlighter, Stash stash)
        {
            TutorialReferenceObject bonusball = scheduleBase.FindTutorialReferenceObject("bonusball") as TutorialReferenceObject;
            
            IInteractiveTutorialStage stage1 = new InteractiveTutorialStage();
            stage1
                .InsertCompletionCondition(new ObjectDestroyedCompletionCondition(bonusball.gameObject))
                .InsertActions(
                    new HighlightObjectAction(bonusball.gameObject, highlighter, Color.blue, 8f),
                    new MessageAction(interactiveMessage, EnumerableAnchors.EnumerableAnchor.MiddleCenter, new Vector2(0,0),
                        "Locale.InteractiveTutorial.59.BonusBall")
                )
                .Seal();

            tutorial
                .InsertStage(stage1)
                .Seal()
                .StartTutorial();
        }
    }
}