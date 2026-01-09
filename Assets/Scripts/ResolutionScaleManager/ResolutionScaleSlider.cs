using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ResolutionScale
{
    public class ResolutionScaleSlider : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scaleTitle;
        [SerializeField] private Slider _scaleSlider;

        private void Start() =>
            UpdateScaleTitle(_scaleSlider.value);

        public void UpdateScaleTitle(float value) =>
            _scaleTitle.text = value.ToString("0.00");
    }
}