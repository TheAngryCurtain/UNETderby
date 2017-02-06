using UnityEngine;
using System;
using System.Collections;
using XInputDotNetPure;

namespace GameInput
{
    public enum Button
    {
        A, B, X, Y, Back, Start, LBumper, RBumper, Guide, LStick, RStick, DPadL, DPadUp, DPadR, DPadDown
    }

    public enum Axis
    {
        LStick, RStick, LTrigger, RTrigger
    }

    public class InputController : MonoBehaviour
    {
        public class PlayerController
        {
            public PlayerIndex PlayerIndex;
            public GamePadState CurrentState;
            public GamePadState PreviousState;
            public IControllable Controllable = null;

            public float HoldStartTime = 0f;
            public bool Enabled = true;

            public PlayerController(PlayerIndex index)
            {
                PlayerIndex = index;
            }
        }

        public static InputController Instance;

        /// <summary>
        /// Controller connection update callback.
        /// @Params index of the controller, true if controller was connected
        /// </summary>
        public System.Action<int, bool> OnControllerConnectionChanged;

        private int MaxPlayers = 4;
        public int MaxNumberOfPlayers { get { return MaxPlayers; } }

        private PlayerController[] _controllers;
        private PlayerController _currentController;
        private IControllable _currentControllable;

        void Awake()
        {
            Instance = this;

            _controllers = new PlayerController[MaxPlayers];
            for (int i = 0; i < MaxPlayers; ++i)
            {
                _controllers[i] = new PlayerController((PlayerIndex)i);
            }
        }

        void Update()
        {
            for (int i = 0; i < MaxPlayers; ++i)
            {
                _currentController = _controllers[i];

                // check for controller state changes
                UpdateControllerStates(_currentController);

                // current controller is not connected or is disabled, skip it
                if (!_currentController.CurrentState.IsConnected || !_currentController.Enabled) continue;

                // poll for input
                ProcessInput(_currentController);
            }
        }

        // set the controllable object of a given player
        public void SetPlayerControllable(int playerIndex, IControllable c)
        {
            _controllers[playerIndex].Controllable = c;
        }

        // enable a given players controller
        public void EnablePlayerInput(int playerIndex, bool enable)
        {
            _controllers[playerIndex].Enabled = enable;
        }

        private void UpdateControllerStates(PlayerController controller)
        {
            controller.PreviousState = controller.CurrentState;
            controller.CurrentState = GamePad.GetState(controller.PlayerIndex);

            if (!controller.PreviousState.IsConnected && controller.CurrentState.IsConnected)
            {
                // new controller connected
                Debug.LogFormat("Controller {0} Connected", controller.PlayerIndex.ToString());
                if (OnControllerConnectionChanged != null)
                {
                    OnControllerConnectionChanged((int)controller.PlayerIndex, true);
                }
            }
            else if (controller.PreviousState.IsConnected && !controller.CurrentState.IsConnected)
            {
                // controller disconnected
                Debug.LogFormat("Controller {0} Disconnected", controller.PlayerIndex.ToString());
                if (OnControllerConnectionChanged != null)
                {
                    OnControllerConnectionChanged((int)controller.PlayerIndex, false);
                }
            }
        }

        private void ProcessInput(PlayerController controller)
        {
            _currentControllable = controller.Controllable;
            if (_currentControllable != null)
            {
                UpdateButton(Button.A, controller);
                UpdateButton(Button.B, controller);
                UpdateButton(Button.X, controller);
                UpdateButton(Button.Y, controller);
                UpdateButton(Button.Back, controller);
                UpdateButton(Button.Start, controller);
                UpdateButton(Button.Guide, controller);
                UpdateButton(Button.LBumper, controller);
                UpdateButton(Button.RBumper, controller);
                UpdateButton(Button.LStick, controller);
                UpdateButton(Button.RStick, controller);

                UpdateButton(Button.DPadL, controller);
                UpdateButton(Button.DPadUp, controller);
                UpdateButton(Button.DPadR, controller);
                UpdateButton(Button.DPadDown, controller);

                UpdateAxis(Axis.LStick, controller);
                UpdateAxis(Axis.RStick, controller);
                UpdateAxis(Axis.LTrigger, controller);
                UpdateAxis(Axis.RTrigger, controller);
            }
        }

        private void UpdateAxis(Axis a, PlayerController pc)
        {

            float hValue = 0f;
            float vValue = 0f;
            switch(a)
            {
                case Axis.LStick:
                    hValue = pc.CurrentState.ThumbSticks.Left.X;
                    vValue = pc.CurrentState.ThumbSticks.Left.Y;
                    break;

                case Axis.RStick:
                    hValue = pc.CurrentState.ThumbSticks.Right.X;
                    vValue = pc.CurrentState.ThumbSticks.Right.Y;
                    break;

                case Axis.RTrigger:
                    hValue = pc.CurrentState.Triggers.Right;
                    break;

                case Axis.LTrigger:
                    hValue = pc.CurrentState.Triggers.Left;
                    break;
            }

            _currentControllable.HandleAxis(a, hValue, vValue);
        }

        private void UpdateButton(Button b, PlayerController pc)
        {
            ButtonState prev;
            ButtonState current;
            switch (b)
            {
                case Button.A:
                    prev = pc.PreviousState.Buttons.A;
                    current = pc.CurrentState.Buttons.A;
                    break;

                case Button.B:
                    prev = pc.PreviousState.Buttons.B;
                    current = pc.CurrentState.Buttons.B;
                    break;

                case Button.X:
                    prev = pc.PreviousState.Buttons.X;
                    current = pc.CurrentState.Buttons.X;
                    break;

                case Button.Y:
                    prev = pc.PreviousState.Buttons.Y;
                    current = pc.CurrentState.Buttons.Y;
                    break;

                case Button.Back:
                    prev = pc.PreviousState.Buttons.Back;
                    current = pc.CurrentState.Buttons.Back;
                    break;

                default:
                case Button.Start:
                    prev = pc.PreviousState.Buttons.Start;
                    current = pc.CurrentState.Buttons.Start;
                    break;

                case Button.Guide:
                    prev = pc.PreviousState.Buttons.Guide;
                    current = pc.CurrentState.Buttons.Guide;
                    break;

                case Button.LBumper:
                    prev = pc.PreviousState.Buttons.LeftShoulder;
                    current = pc.CurrentState.Buttons.LeftShoulder;
                    break;

                case Button.RBumper:
                    prev = pc.PreviousState.Buttons.RightShoulder;
                    current = pc.CurrentState.Buttons.RightShoulder;
                    break;

                case Button.LStick:
                    prev = pc.PreviousState.Buttons.LeftStick;
                    current = pc.CurrentState.Buttons.LeftStick;
                    break;

                case Button.RStick:
                    prev = pc.PreviousState.Buttons.RightStick;
                    current = pc.CurrentState.Buttons.RightStick;
                    break;

                case Button.DPadL:
                    prev = pc.PreviousState.DPad.Left;
                    current = pc.CurrentState.DPad.Left;
                    break;

                case Button.DPadUp:
                    prev = pc.PreviousState.DPad.Up;
                    current = pc.CurrentState.DPad.Up;
                    break;

                case Button.DPadR:
                    prev = pc.PreviousState.DPad.Right;
                    current = pc.CurrentState.DPad.Right;
                    break;

                case Button.DPadDown:
                    prev = pc.PreviousState.DPad.Down;
                    current = pc.CurrentState.DPad.Down;
                    break;

            }

            if (CheckButtonPressed(prev, current))
            {
                _currentControllable.HandleButtonPress(b);
                pc.HoldStartTime = Time.time;
            }
            else if (CheckButtonReleased(prev, current))
            {
                _currentControllable.HandleButtonRelease(b);
                pc.HoldStartTime = 0f;
            }
            else if (CheckButtonHeld(prev, current))
            {
                _currentControllable.HandleButtonHeld(b, (Time.time - pc.HoldStartTime));
            }
        }

        private bool CheckButtonPressed(ButtonState prev, ButtonState current)
        {
            return prev == ButtonState.Released && current == ButtonState.Pressed;
        }

        private bool CheckButtonReleased(ButtonState prev, ButtonState current)
        {
            return prev == ButtonState.Pressed && current == ButtonState.Released;
        }

        private bool CheckButtonHeld(ButtonState prev, ButtonState current)
        {
            return prev == ButtonState.Pressed && current == ButtonState.Pressed;
        }
    }
}
