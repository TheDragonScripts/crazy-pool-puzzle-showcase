using System;

namespace PlayerInputs.Swipes
{
    public interface ISwipeController
    {
        event Action SwipeUp;
        event Action SwipeDown;
        event Action SwipeLeft;
        event Action SwipeRight;
    }
}