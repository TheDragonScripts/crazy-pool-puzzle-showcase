using EntryPoint.Levels;

namespace ModificatedUISystem.UIButtonsActions
{
    public class ContinueButtonAction : IUIButtonAction
    {
        private ILevelManager _levelManager;

        public ContinueButtonAction(ILevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        public void Execute()
        {
            CSDL.Log("Continue button action has been temporary disabled until level scenes clearing is complete.");
            //bool tryLevelLoad = _levelManager.PlayLevel(ActualPlayerData.Data.LastLevel);
        }
    }
}