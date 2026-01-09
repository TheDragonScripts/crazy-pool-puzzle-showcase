using ModificatedUISystem.UIElements;

namespace ModificatedUISystem.UIButtonsActions
{
    public class AboutButtonAction : IUIButtonAction
    {
        private IUIFactory _uiFactory;

        public AboutButtonAction(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void Execute()
        {
            _uiFactory.GetAsync<About>();
        }
    }
}