using ModificatedUISystem.UIElements;

namespace ModificatedUISystem.UIButtonsActions
{
    public class TutorialsButtonAction : IUIButtonAction
    {
        private IUIFactory _uiFactory;

        public TutorialsButtonAction(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void Execute()
        {
            _uiFactory.GetAsync<Tutorials>();
        }
    }
}