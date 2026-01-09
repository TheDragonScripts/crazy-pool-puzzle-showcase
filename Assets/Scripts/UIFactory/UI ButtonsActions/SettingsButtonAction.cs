using ModificatedUISystem.UIElements;

namespace ModificatedUISystem.UIButtonsActions
{
    public class SettingsButtonAction : IUIButtonAction
    {
        private IUIFactory _uiFactory;

        public SettingsButtonAction(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void Execute()
        {
            _uiFactory.GetAsync<Settings>();
        }
    }
}