using Cysharp.Threading.Tasks;
using ModificatedUISystem.UIElements;

namespace ModificatedUISystem
{
    public interface IUIFactory
    {
        event UIFactoryShowUIEventHandler UIWasShown;
        event UIFactoryHideUIEventHandler UIWasHidden;
        UniTask<(T ui, string id)> GetNewInstanceAsync<T>() where T : class;
        UniTask<T> GetByIdAsync<T>(string id) where T : class;
        UniTask<T> GetAsync<T>() where T : class;
        void OpenPreviousMenu();
        void Close<T>();
        void Close(string id);
        bool IsUIOpened<T>(string id = null);
        IUIElement[] GetAllOpenedUIs();
    }
}