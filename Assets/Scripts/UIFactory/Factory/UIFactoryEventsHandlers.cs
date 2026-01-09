using ModificatedUISystem.UIElements;

namespace ModificatedUISystem
{
    public delegate void UIFactoryShowUIEventHandler(string id, IUIElement uiElement);
    public delegate void UIFactoryHideUIEventHandler(string id, bool isTemporaryHidden);
}
