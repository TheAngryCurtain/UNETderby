using UnityEngine;
using System.Collections;

namespace GameInput
{
    public interface IControllable
    {
        void HandleButtonPress(Button b);
        void HandleButtonRelease(Button b);
        void HandleButtonHeld(Button b, float duration);

        void HandleAxis(Axis a, float h, float v);
    }
}
