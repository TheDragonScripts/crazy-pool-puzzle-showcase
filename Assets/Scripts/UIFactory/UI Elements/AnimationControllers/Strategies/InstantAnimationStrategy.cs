using UnityEngine;

namespace ModificatedUISystem.UIElements.Animation.Strategies
{
    public class InstantAnimationStrategy : MonoBehaviour, IUIAnimationStrategy
    {
        public event AnimationControllerEventHandler AnimationFinished;

        public void Hide() => AnimationFinished?.Invoke(AnimationType.Hide);

        public void Show() => AnimationFinished?.Invoke(AnimationType.Show);
    }
}
