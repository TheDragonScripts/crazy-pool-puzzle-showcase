namespace ModificatedUISystem.UIButtonsActions
{
    public class BackButtonAction : IUIButtonAction
    {
        private IUIFactory _uiFactory;

        public BackButtonAction(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void Execute()
        {
            _uiFactory.OpenPreviousMenu();
        }
    }
}