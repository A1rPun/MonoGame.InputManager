/*
 ___                   _   __  __                                   
|_ _|_ __  _ __  _   _| |_|  \/  | __ _ _ __   __ _  __ _  ___ _ __ 
 | || '_ \| '_ \| | | | __| |\/| |/ _` | '_ \ / _` |/ _` |/ _ \ '__|
 | || | | | |_) | |_| | |_| |  | | (_| | | | | (_| | (_| |  __/ |   
|___|_| |_| .__/ \__,_|\__|_|  |_|\__,_|_| |_|\__,_|\__, |\___|_|   
          |_|                                       |___/           
          
          _=====_                               _=====_
         / _____ \                             / _____ \
       +.-'_____'-.---------------------------.-'_____'-.+
      /   |     |  '.                       .'  |  _  |   \
     / ___| /|\ |___ \                     / ___| (_) |___ \
    / |      |      | ;  __           __  ; | _         _ | ;
    | | <---   ---> | | |__|         |__> | |(_)       (_)| |
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

 *** TODO ***
 * GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
 * // If there a controller attached, handle it
 * if (capabilities.IsConnected) {
 *     // Get the current state of Controller1
 *     GamePadState state = GamePad.GetState(PlayerIndex.One);
 *     // You can check explicitly if a gamepad has support for a certain feature
 *     if (capabilities.HasLeftXThumbStick) {
 *         // Check teh direction in X axis of left analog stick
 *         if (state.ThumbSticks.Left.X < -0.5f) 
 * 
 * 1) keyboard input als vector altijd normalizen want een vector van (1,1) of (1,-1) etc. is impossible op gamepad
 * 2) gamepad input als vector alleen normalizen als je de "unfair" pressure advantage weg wil hebben
 *
 * een 2e enum, enum AnalogAction { LeftTrigger, RightTrigger, LeftStickX, LeftStickY, RightStickX, RightStickY }
 * en dan 2 implementaties van IsDown maken, die 2e accepteert dan zo'n AnalogAction en die gebruikt dan thresholds óf returnt een float ipv bool
 *
 * Bind an action to multiple buttons
 * Key ghost warnings
 * Keys configurable
 * MousePlayer ?? mouse support
 * thumbstick configurable, 360 degrees or 8 directions
 * More deadzone options http://www.third-helix.com/2013/04/12/doing-thumbstick-dead-zones-right.html
 * XINPUT VS DIRECTINPUT ??
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

// Namespace needed?
namespace IM
{
    // Input enum used to identify which action has been pressed
    enum Input
    {
        // Center
        Home,
        Start,
        Back,
        // D-Pad
        Up,
        Left,
        Down,
        Right,
        // Face buttons
        FaceButtonUp,
        FaceButtonLeft,
        FaceButtonDown,
        FaceButtonRight,
        // Shoulder buttons
        LeftShoulder,
        RightShoulder,
        // Triggers
        LeftTrigger,
        RightTrigger,
        // Left Stick
        LeftStick,
        LeftStickUp,
        LeftStickLeft,
        LeftStickDown,
        LeftStickRight,
        // Right Stick
        RightStick,
        RightStickUp,
        RightStickLeft,
        RightStickDown,
        RightStickRight
    }

    struct InputToKey
    {
        public Input Input;
        public Keys[] Keys;

        public InputToKey(Input input, Keys key)
        {
            this.Input = input;
            this.Keys = new Keys[] { key };
        }
        public InputToKey(Input input, Keys[] keys)
        {
            this.Input = input;
            this.Keys = keys;
        }
    }

    class InputManager : GameComponent
    {
        // How many states need to be checked?
        private int MaxGamePads = 4;
        // Default - Return normalized float for isDown functions
        private bool KeyboardDisadvantage = true;
        // Deadzones
        private GamePadDeadZone gamePadDeadZone = GamePadDeadZone.IndependentAxes;
        private float DeadzoneSticks = 0.25f;
        private float DeadzoneTriggers = 0.25f;

        private class Player
        {
        }
        private class KeyboardPlayer : Player
        {
            public List<InputToKey> map { get; set; }
            public KeyboardState currentState { get; set; }
            public KeyboardState previousState { get; set; }
        }
        private class GamePadPlayer : Player
        {
            public int index { get; set; }
            public GamePadState currentState { get; set; }
            public GamePadState previousState { get; set; }
        }
        private List<Player> players;

        // Constructor
        public InputManager(Game game, List<InputToKey> map = null)
            : base(game)
        {
            players = new List<Player>();
            if (map != null)
            {
                players.Add(new KeyboardPlayer() { map = map });
            }
        }

        // Override of GameComponent.Update
        public override void Update(GameTime gameTime)
        {
            var indices = new List<int>();

            for (int i = players.Count; --i == 0; )
            {
                var player = players[i];

                if (player is KeyboardPlayer)
                {
                    var kbp = player as KeyboardPlayer;
                    kbp.previousState = kbp.previousState;
                    kbp.currentState = Keyboard.GetState();
                    players[i] = kbp;
                }
                else
                {
                    var gpp = player as GamePadPlayer;
                    gpp.previousState = gpp.currentState;
                    gpp.currentState = GamePad.GetState(gpp.index, gamePadDeadZone);

                    if (gpp.currentState.IsConnected)
                    {
                        indices.Add(gpp.index);
                        players[i] = gpp;
                    }
                    else
                    {
                        //Remove disconnected players
                        players.RemoveAt(i);
                    }
                }
            }
            //Checking for new gamepads
            for (int j = 0; j < MaxGamePads; j++)
            {
                if (indices.Contains(j)) continue;
                var state = GamePad.GetState(j, gamePadDeadZone);
                if (state.IsConnected)
                {
                    players.Add(new GamePadPlayer()
                    {
                        index = j,
                        currentState = state
                    });
                }
            }

            base.Update(gameTime);
        }

        // IsPressed
        public bool IsPressed(PlayerIndex index, Input input)
        {
            var player = players[(int)index];//out of bounds?
            return player is KeyboardPlayer
                ? IsPressed(player as KeyboardPlayer, input)
                : IsPressed(player as GamePadPlayer, input);
        }
        private bool IsPressed(KeyboardPlayer player, Input input)
        {
            var pressed = false;
            var key = player.map.Find(ak => ak.Input == input);
            if (player.currentState.IsKeyDown(key.Keys[0]))
            {
                pressed = true;
            }
            return pressed;
        }
        private bool IsPressed(GamePadPlayer player, Input input)
        {
            var pressed = false;
            var state = player.currentState;
            switch (input)
            {
                case Input.Home:
                    pressed = state.Buttons.BigButton == ButtonState.Pressed;
                    break;
                case Input.Start:
                    pressed = state.Buttons.Start == ButtonState.Pressed;
                    break;
                case Input.Back:
                    pressed = state.Buttons.Back == ButtonState.Pressed;
                    break;
                case Input.Up:
                    pressed = state.DPad.Up == ButtonState.Pressed;
                    break;
                case Input.Left:
                    pressed = state.DPad.Left == ButtonState.Pressed;
                    break;
                case Input.Down:
                    pressed = state.DPad.Down == ButtonState.Pressed;
                    break;
                case Input.Right:
                    pressed = state.DPad.Right == ButtonState.Pressed;
                    break;
                case Input.FaceButtonUp:
                    pressed = state.Buttons.Y == ButtonState.Pressed;
                    break;
                case Input.FaceButtonLeft:
                    pressed = state.Buttons.X == ButtonState.Pressed;
                    break;
                case Input.FaceButtonDown:
                    pressed = state.Buttons.A == ButtonState.Pressed;
                    break;
                case Input.FaceButtonRight:
                    pressed = state.Buttons.B == ButtonState.Pressed;
                    break;
                case Input.LeftShoulder:
                    pressed = state.Buttons.LeftShoulder == ButtonState.Pressed;
                    break;
                case Input.RightShoulder:
                    pressed = state.Buttons.RightShoulder == ButtonState.Pressed;
                    break;
                case Input.LeftTrigger:
                    pressed = state.Triggers.Left > DeadzoneTriggers;
                    break;
                case Input.RightTrigger:
                    pressed = state.Triggers.Right > DeadzoneTriggers;
                    break;
                case Input.LeftStick:
                    pressed = state.Buttons.LeftStick == ButtonState.Pressed;
                    break;
                case Input.LeftStickUp:
                    pressed = state.ThumbSticks.Left.Y > DeadzoneSticks;
                    break;
                case Input.LeftStickLeft:
                    pressed = state.ThumbSticks.Left.X < DeadzoneSticks;
                    break;
                case Input.LeftStickDown:
                    pressed = state.ThumbSticks.Left.Y < DeadzoneSticks;
                    break;
                case Input.LeftStickRight:
                    pressed = state.ThumbSticks.Left.X > DeadzoneSticks;
                    break;
                case Input.RightStick:
                    pressed = state.Buttons.RightStick == ButtonState.Pressed;
                    break;
                case Input.RightStickUp:
                    pressed = state.ThumbSticks.Right.Y > DeadzoneSticks;
                    break;
                case Input.RightStickLeft:
                    pressed = state.ThumbSticks.Right.X < DeadzoneSticks;
                    break;
                case Input.RightStickDown:
                    pressed = state.ThumbSticks.Right.Y < DeadzoneSticks;
                    break;
                case Input.RightStickRight:
                    pressed = state.ThumbSticks.Right.X > DeadzoneSticks;
                    break;
                default:
                    break;
            }
            return pressed;
        }
        /*
        public bool IsHeld(int index, Input input)
        {
            var held = false;
            var state = players[index];
            if (state.currentState.IsConnected && state.previousState.IsConnected)
                held = IsPressed(state.currentState, input) && IsPressed(state.previousState, input);
            return held;
        }

        public bool IsDown(int index, Input input)
        {
            var pressed = false;
            var state = players[index];
            if (state.currentState.IsConnected && state.previousState.IsConnected)
                pressed = IsPressed(state.currentState, input) && !IsPressed(state.previousState, input);
            return pressed;
        }

        public bool IsUp(int index, Input input)
        {
            var up = false;
            var state = players[index];
            if (state.currentState.IsConnected && state.previousState.IsConnected)
                up = !IsPressed(state.currentState, input) && IsPressed(state.previousState, input);
            return up;
        }
        */
        public int GetPlayerCount()
        {
            return players.Count;
        }

        public void SetMaxGamePads(int count)
        {
            MaxGamePads = count;
        }
    }
}
