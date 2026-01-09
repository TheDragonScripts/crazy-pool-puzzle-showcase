using System.Collections.Generic;
using UnityEngine;
using System;

namespace EnumerableAnchors
{
    /// <summary>
    /// Utility that allows simply change anchored object position and alignment to
    /// predefined position.
    /// </summary>
    public static class EnumerableAnchorsUtility
    {
        private static readonly Dictionary<EnumerableAnchor, AnchorData> _anchors = new()
        {
            [EnumerableAnchor.MiddleCenter] =
                new AnchorData(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero),
            [EnumerableAnchor.MiddleDown] =
                new AnchorData(new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 50)),
            [EnumerableAnchor.MiddleUp] =
                new AnchorData(new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -50)),
        };

        /// <summary>
        /// Sets the enumerable position and alignment to reference object.
        /// </summary>
        /// <param name="anchor">Predefined anchor to align and with.</param>
        /// <param name="target">Referenced rect transform target to perform with.</param>
        /// <exception cref="ArgumentNullException">Throws when target is null.</exception>
        public static void SetEnumerableAnchor(EnumerableAnchor anchor, ref RectTransform target)
        {
            if (target == null)
            {
                throw new ArgumentNullException($"{nameof(target)} cannot be null.");
            }
            AnchorData anchorData = _anchors[anchor];
            target.anchorMin = anchorData.Min;
            target.anchorMax = anchorData.Max;
            target.pivot = anchorData.Pivot;
            target.anchoredPosition = anchorData.Offset;
        }

        /// <summary>
        /// Sets the enumerable position and alignment to reference object with custom position
        /// offset.
        /// </summary>
        /// <param name="anchor">Predefined anchor to align and with.</param>
        /// <param name="customOffset">Custom Vector2 position to offset the position.</param>
        /// <param name="target">Referenced rect transform target to perform with.</param>
        /// <exception cref="ArgumentNullException">Throws when target or offset is null</exception>
        public static void SetEnumerableAnchor(EnumerableAnchor anchor, Vector2 customOffset, ref RectTransform target)
        {
            if (target == null || customOffset == null)
            {
                throw new ArgumentNullException($"Enumerable anchor argument is null. Missing" +
                    $"{(customOffset == null ? nameof(customOffset) : "")}" +
                    $"{(target == null ? nameof(target) : "")}");
            }
            AnchorData anchorData = _anchors[anchor];
            target.anchorMin = anchorData.Min;
            target.anchorMax = anchorData.Max;
            target.pivot = anchorData.Pivot;
            target.anchoredPosition = customOffset;
        }
    }
}