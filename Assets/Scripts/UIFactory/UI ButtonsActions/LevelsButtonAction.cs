using ModificatedUISystem.UIElements;

namespace ModificatedUISystem.UIButtonsActions
{
    public class LevelButtonAction : IUIButtonAction
    {
        private IUIFactory _uiFactory;

        public LevelButtonAction(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void Execute()
        {
            _uiFactory.GetAsync<Levels>();
        }
    }
}