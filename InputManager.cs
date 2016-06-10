﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/*
               _=====_                               _=====_
              / _____ \                             / _____ \
            +.-'_____'-.---------------------------.-'_____'-.+
           /   |     |  '.                       .'  |  _  |   \
          / ___| /|\ |___ \                     / ___| (_) |___ \
         / |      |      | ;  __           _   ; | _         _ | ;
         | | <---   ---> | | |__|         |_:> | |(_)       (_)| |
         | |___   |   ___| ; BACK        START ; |___   _   ___| ;
         |\    | \|/ |    /  _              _   \    | (_) |    /|
         | \   |_____|  .','" "',        ,'" "', '.  |_____|  .' |
         |  '-.______.-' /       \      /       \  '-._____.-'   |
         |               |       |------|       |                |
         |              /\       /      \       /\               |
         |             /  '.___.'        '.___.'  \              |
         |            /                            \             |
          \          /                              \           /
           \________/                                \_________/ 
*/


/*
 GamePadCapabilities capabilities = GamePad.GetCapabilities(
                                               PlayerIndex.One);
            
            // If there a controller attached, handle it
            if (capabilities.IsConnected)
            {
                // Get the current state of Controller1
                GamePadState state = GamePad.GetState(PlayerIndex.One);

                // You can check explicitly if a gamepad has support 
                for a certain feature
                if (capabilities.HasLeftXThumbStick)
                {
                    // Check teh direction in X axis of left analog 
                    stick
                    if (state.ThumbSticks.Left.X < -0.5f) 



    
1) keyboard input als vector altijd normalizen want een vector van (1,1) of (1,-1) etc. is impossible op gamepad
2) gamepad input als vector alleen normalizen als je de "unfair" pressure advantage weg wil hebben


een 2e enum, enum AnalogAction { LeftTrigger, RightTrigger, LeftStickX, LeftStickY, RightStickX, RightStickY }
en dan 2 implementaties van IsDown maken, die 2e accepteert dan zo'n AnalogAction en die gebruikt dan thresholds óf returnt een float ipv bool

*/

namespace Proto
{
    // Bind an action to multiple buttons
    // Key ghost warnings
    // Key configurable
    // thumbstick configurable, 360 degrees or 8 directions
    // previouskeyboardstate vs currentkeyboardstate
    // XINPUT VS DIRECTINPUT ??

    class InputManager : GameComponent
    {
        public int MaxGamePads = 4;
        public bool ThumbBehavesDirectional = false;
        public float DeadzoneSticks = 0.5f;
        public float DeadzoneTriggers = 0.2f;

        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;
        private List<GamePadState> previousGamepadStates;
        private List<GamePadState> currentGamepadStates;

        public enum Button
        {
            Home = 0,
            Start,
            Back,

            Up,
            Left,
            Down,
            Right,

            TopFace,
            LeftFace,
            BottomFace,
            RightFace,

            LeftShoulder,
            RightShoulder,

            LeftTrigger,
            RightTrigger,

            LeftStick,
            RightStick
        }
        
        public InputManager(Game game) : base(game)
        {
            // init
        }
        
        private void SetStates()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousGamepadStates = currentGamepadStates;
            currentGamepadStates = new List<GamePadState>();

            for (int i = 0; i < MaxGamePads - 1; i++)
            {
                var state = GamePad.GetState(i);

                if (state.IsConnected)
                {
                    currentGamepadStates.Add(state);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            SetStates();
            base.Update(gameTime);
        }

        public ButtonState CheckGamePadState(GamePadState state, Button button)
        {
            ButtonState returnState;
            switch (button)
            {
                case Button.Home:
                    returnState = state.Buttons.BigButton;
                    break;
                case Button.Start:
                    returnState = state.Buttons.Start;
                    break;
                case Button.Back:
                    returnState = state.Buttons.Back;
                    break;
                case Button.Up:
                    returnState = state.DPad.Up;
                    break;
                case Button.Left:
                    returnState = state.DPad.Left;
                    break;
                case Button.Down:
                    returnState = state.DPad.Down;
                    break;
                case Button.Right:
                    returnState = state.DPad.Right;
                    break;
                case Button.TopFace:
                    returnState = state.Buttons.Y;
                    break;
                case Button.LeftFace:
                    returnState = state.Buttons.X;
                    break;
                case Button.BottomFace:
                    returnState = state.Buttons.A;
                    break;
                case Button.RightFace:
                    returnState = state.Buttons.B;
                    break;
                case Button.LeftShoulder:
                    returnState = state.Buttons.LeftShoulder;
                    break;
                case Button.RightShoulder:
                    returnState = state.Buttons.RightShoulder;
                    break;
                case Button.LeftTrigger:
                    returnState = state.Triggers.Left > DeadzoneTriggers ? ButtonState.Pressed : ButtonState.Released;
                    break;
                case Button.RightTrigger:
                    returnState = state.Triggers.Right > DeadzoneTriggers ? ButtonState.Pressed : ButtonState.Released;
                    break;
                case Button.LeftStick:
                    returnState = state.Buttons.LeftStick;
                    break;
                case Button.RightStick:
                    returnState = state.Buttons.RightStick;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
            return returnState;
        }

        public bool IsHeld(Button button)
        {

            return true;
        }

        public bool IsUp(Button button)
        {

            return true;
        }

        public bool IsDown(Button button)
        {

            return true;
        }
    }
}
