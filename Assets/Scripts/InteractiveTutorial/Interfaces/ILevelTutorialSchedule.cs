namespace InteractiveTutorial
{
    /// <summary>
    /// Using to form and start unique tutorial for given level.
    /// </summary>
    public interface ILevelTutorialSchedule
    {
        void FormAndStart(IInteractiveTutorialScheduleBase scheduleBase, InteractiveTutorial tutorial, InteractiveTutorialMessage interactiveMessage,
            InteractiveTutorialPointer pointer, ObjectsHighlighter highlighter, Stash stash);
    }
}