namespace ModificatedUISystem.UIElements
{
    /*
     * Future tip: This interface can be improved via Roslyn API
     * to prevent IUIElement implementations to classes with the generic type
     * IUIType.
     * 
     * Here is an example to better clarification:
     * Good -> public class Menu : MonoBehaviour, IUIElement<MenuType>
     * Bad -> public class Menu : MonoBehaviour, IUIElement<IUIType>
     * 
     * I just don't have the competence to create VS extension that would prevent this :)
     * 
     * And a few words about why I did it this way. It's quite simple.
     * I needed to have an ability to get UI type without creating a new
     * instance of GameObject to avoid performance hit.
     * 
     * P.S.
     * And yes, I write all my notes in English, sometimes looking to Google Translate.
     * P.P.S.
     * Just a habit.
    */
    public interface IUIOfType<out T> where T : IUIType { }
}
