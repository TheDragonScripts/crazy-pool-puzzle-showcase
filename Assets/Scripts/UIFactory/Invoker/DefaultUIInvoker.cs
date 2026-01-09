using EntryPoint.Levels;
using ModificatedUISystem.UIElements;
using SDI;

namespace ModificatedUISystem
{
    public class DefaultUIInvoker : IUIInvoker
    {
        private IUIFactory _uiFactory;
        private ILevelManager _levelManager;

        [InjectionMethod]
        public void Inject(IUIFactory uiFactory, ILevelManager levelManager)
        {
            _uiFactory = uiFactory;
            _levelManager = levelManager;

            _levelManager.LevelLoaded += LevelLoaded;
        }

        private void LevelLoaded(int level)
        {
            if (level >= _levelManager.FirstPlayableLevelID)
            {
                _uiFactory.GetAsync<OnLevel>();
            }
            else
            {
                _uiFactory.GetAsync<Menu>();
            }
        }
    }
}
