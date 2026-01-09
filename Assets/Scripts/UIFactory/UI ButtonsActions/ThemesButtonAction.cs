using ModificatedUISystem.UIElements;

namespace ModificatedUISystem.UIButtonsActions
{
    public class ThemesButtonAction : IUIButtonAction
    {
        private IUIFactory _uiFactory;

        public ThemesButtonAction(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void Execute()
        {
            _uiFactory.GetAsync<Themes>();
        }
    }
}