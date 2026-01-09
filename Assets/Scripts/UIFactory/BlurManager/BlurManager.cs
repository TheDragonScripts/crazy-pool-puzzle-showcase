using UnityEngine;

namespace ModificatedUISystem.Blur
{
    public class BlurManager : MonoBehaviour, IBlurManager
    {
        private void Awake()
        {
            CSDL.LogWarning($"Be advised, {nameof(BlurManager)} is replaced with the stub.");
        }
    }
}