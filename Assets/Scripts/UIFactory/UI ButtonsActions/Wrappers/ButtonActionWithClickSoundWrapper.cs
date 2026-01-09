using UnityEngine;

namespace ModificatedUISystem.UIButtonsActions.Wrappers
{
    /*
     * Code here is temporary disabled because it references old Audio system.
     * Pending refactoring.
     */
    public class ButtonActionWithClickSoundWrapper : IUIButtonAction
    {
        private IUIButtonAction _wrapper;

        public ButtonActionWithClickSoundWrapper(IUIButtonAction wrapper)
        {
            _wrapper = wrapper;
        }

        public void Execute()
        {
            //AudioManager.Instance.UIClickSFX.CopyAndPlay(Vector3.zero);
            _wrapper.Execute();
        }
    }
}