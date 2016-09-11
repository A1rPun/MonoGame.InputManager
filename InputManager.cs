using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace A1r.Input
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
        public Keys Key;
        public InputToKey(Input input, Keys key)
        {
            this.Input = input;
            this.Key = key;
        }
    }

    class InputManager : GameComponent
    {
        // Types of player input
        private class Player { }
        private class KeyboardPlayer : Player
        {
            public InputToKey[] Map { get; set; }
            public KeyboardState CurrentState { get; set; }
            public KeyboardState PreviousState { get; set; }
        }
        private class GamePadPlayer : Player
        {
            public int index { get; set; }
            public GamePadState CurrentState { get; set; }
            public GamePadState PreviousState { get; set; }
        }

        private List<Player> players;
        private KeyboardPlayer keyboardPlayer;
        private GamePadDeadZone gamePadDeadZone = GamePadDeadZone.IndependentAxes;
        private Array inputValues;
        public float DeadzoneSticks = 0.25f;
        public float DeadzoneTriggers = 0.25f;
        public int PlayerCount = 1;

        public InputManager(Game game, InputToKey[] map = null)
            : base(game)
        {
            inputValues = Enum.GetValues(typeof(Input));
            players = new List<Player>();
            keyboardPlayer = new KeyboardPlayer();
            if (map != null)
            {
                keyboardPlayer.Map = map;
                players.Add(keyboardPlayer);
            }
        }

        public override void Update(GameTime gameTime)
        {
            var indices = new List<int>();
            // Save the one and only (if available) keyboardstate 
            keyboardPlayer.PreviousState = keyboardPlayer.CurrentState;
            keyboardPlayer.CurrentState = Keyboard.GetState();

            for (int i = players.Count; --i == 0; )
            {
                var player = players[i];
                if (player is GamePadPlayer)
                {
                    var gpp = (GamePadPlayer)player;
                    var state = GamePad.GetState(gpp.index, gamePadDeadZone);

                    if (state.IsConnected)
                    {
                        // Update gamepad state
                        gpp.PreviousState = gpp.CurrentState;
                        gpp.CurrentState = state;
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
            if (players.Count < PlayerCount)
            {
                for (int j = 0, l = GamePad.MaximumGamePadCount; j < l; j++)
                {
                    if (indices.Contains(j)) continue;
                    var state = GamePad.GetState(j, gamePadDeadZone);
                    if (state.IsConnected)
                    {
                        players.Add(new GamePadPlayer()
                        {
                            index = j,
                            CurrentState = state
                        });

                        if (players.Count == PlayerCount)
                        {
                            break;
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        private Keys getKey(InputToKey[] map, Input input)
        {
            for (int i = 0; i < map.Length; i++)
            {
                var inputToKey = map[i];
                if (inputToKey.Input == input)
                    return inputToKey.Key;
            }
            return Keys.None;
        }

        public bool IsPressed(Keys key)
        {
            return keyboardPlayer.CurrentState.IsKeyDown(key);
        }

        public bool IsPressed(Input input)
        {
            for (int i = 0; i < players.Count; i++)
                if (IsPressed(input, i))
                    return true;
            return false;
        }

        public bool IsPressed(Input input, int index)
        {
            if (index >= players.Count) return false;
            var p = players[(int)index];
            if (p is KeyboardPlayer)
            {
                var player = (KeyboardPlayer)p;
                var key = getKey(player.Map, input);
                return player.CurrentState.IsKeyDown(key);
            }
            else
            {
                return IsPressed(((GamePadPlayer)p).CurrentState, input);
            }
        }

        private bool IsPressed(GamePadState state, Input input)
        {
            switch (input)
            {
                case Input.Home:
                    return state.Buttons.BigButton == ButtonState.Pressed;
                case Input.Start:
                    return state.Buttons.Start == ButtonState.Pressed;
                case Input.Back:
                    return state.Buttons.Back == ButtonState.Pressed;
                case Input.Up:
                    return state.DPad.Up == ButtonState.Pressed;
                case Input.Left:
                    return state.DPad.Left == ButtonState.Pressed;
                case Input.Down:
                    return state.DPad.Down == ButtonState.Pressed;
                case Input.Right:
                    return state.DPad.Right == ButtonState.Pressed;
                case Input.FaceButtonUp:
                    return state.Buttons.Y == ButtonState.Pressed;
                case Input.FaceButtonLeft:
                    return state.Buttons.X == ButtonState.Pressed;
                case Input.FaceButtonDown:
                    return state.Buttons.A == ButtonState.Pressed;
                case Input.FaceButtonRight:
                    return state.Buttons.B == ButtonState.Pressed;
                case Input.LeftShoulder:
                    return state.Buttons.LeftShoulder == ButtonState.Pressed;
                case Input.RightShoulder:
                    return state.Buttons.RightShoulder == ButtonState.Pressed;
                case Input.LeftTrigger:
                    return state.Triggers.Left > DeadzoneTriggers;
                case Input.RightTrigger:
                    return state.Triggers.Right > DeadzoneTriggers;
                case Input.LeftStick:
                    return state.Buttons.LeftStick == ButtonState.Pressed;
                case Input.LeftStickUp:
                    return state.ThumbSticks.Left.Y > DeadzoneSticks;
                case Input.LeftStickLeft:
                    return state.ThumbSticks.Left.X < DeadzoneSticks;
                case Input.LeftStickDown:
                    return state.ThumbSticks.Left.Y < DeadzoneSticks;
                case Input.LeftStickRight:
                    return state.ThumbSticks.Left.X > DeadzoneSticks;
                case Input.RightStick:
                    return state.Buttons.RightStick == ButtonState.Pressed;
                case Input.RightStickUp:
                    return state.ThumbSticks.Right.Y > DeadzoneSticks;
                case Input.RightStickLeft:
                    return state.ThumbSticks.Right.X < DeadzoneSticks;
                case Input.RightStickDown:
                    return state.ThumbSticks.Right.Y < DeadzoneSticks;
                case Input.RightStickRight:
                    return state.ThumbSticks.Right.X > DeadzoneSticks;
            }
            return false;
        }

        public bool IsHeld(Keys key)
        {
            return keyboardPlayer.CurrentState.IsKeyDown(key) && keyboardPlayer.PreviousState.IsKeyDown(key);
        }

        public bool IsHeld(Input input)
        {
            for (int i = 0; i < players.Count; i++)
                if (IsHeld(input, i))
                    return true;
            return false;
        }

        public bool IsHeld(Input input, int index)
        {
            if (index >= players.Count) return false;
            var p = players[(int)index];
            if (p is KeyboardPlayer)
            {
                var player = (KeyboardPlayer)p;
                var key = getKey(player.Map, input);
                return player.CurrentState.IsKeyDown(key) && player.PreviousState.IsKeyDown(key);
            }
            else
            {
                var player = (GamePadPlayer)p;
                return IsPressed(player.CurrentState, input) && IsPressed(player.PreviousState, input);
            }
        }

        public bool JustPressed(Keys key)
        {
            return keyboardPlayer.CurrentState.IsKeyDown(key) && !keyboardPlayer.PreviousState.IsKeyDown(key);
        }

        public bool JustPressed(Input input)
        {
            for (int i = 0; i < players.Count; i++)
                if (JustPressed(input, i))
                    return true;
            return false;
        }

        public bool JustPressed(Input input, int index)
        {
            if (index >= players.Count) return false;
            var p = players[(int)index];
            if (p is KeyboardPlayer)
            {
                var player = (KeyboardPlayer)p;
                var key = getKey(player.Map, input);
                return player.CurrentState.IsKeyDown(key) && !player.PreviousState.IsKeyDown(key);
            }
            else
            {
                var player = (GamePadPlayer)p;
                return IsPressed(player.CurrentState, input) && !IsPressed(player.PreviousState, input);
            }
        }

        public bool JustReleased(Keys key)
        {
            return !keyboardPlayer.CurrentState.IsKeyDown(key) && keyboardPlayer.PreviousState.IsKeyDown(key);
        }

        public bool JustReleased(Input input)
        {
            for (int i = 0; i < players.Count; i++)
                if (JustReleased(input, i))
                    return true;
            return false;
        }

        public bool JustReleased(Input input, int index)
        {
            if (index >= players.Count) return false;
            var p = players[(int)index];
            if (p is KeyboardPlayer)
            {
                var player = (KeyboardPlayer)p;
                var key = getKey(player.Map, input);
                return !player.CurrentState.IsKeyDown(key) && player.PreviousState.IsKeyDown(key);
            }
            else
            {
                var player = (GamePadPlayer)p;
                return !IsPressed(player.CurrentState, input) && IsPressed(player.PreviousState, input);
            }
        }

        public bool SomethingDown()
        {
            for (int i = 0; i < players.Count; i++)
                if (SomethingDown(i))
                    return true;
            return false;
        }

        public bool SomethingDown(int index)
        {
            if (index >= players.Count) return false;
            var p = players[(int)index];
            if (p is KeyboardPlayer)
            {
                return keyboardPlayer.CurrentState.GetPressedKeys().Length > 0;
            }
            else
            {
                var player = (GamePadPlayer)p;
                foreach (Input key in inputValues)
                {
                    if (IsPressed(player.CurrentState, key))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public float GetRaw(Input input)
        {
            for (int i = 0, l = players.Count; i < l; i++)
            {
                var raw = GetRaw(input, i);
                if (raw != 0f)
                    return raw;
            }
            return 0f;
        }

        public float GetRaw(Input input, int index)
        {
            if (index >= players.Count) return 0f;
            var p = players[(int)index];
            if (p is GamePadPlayer)
            {
                var state = ((GamePadPlayer)p).CurrentState;
                switch (input)
                {
                    case Input.LeftTrigger:
                        return state.Triggers.Left;
                    case Input.RightTrigger:
                        return state.Triggers.Right;
                    case Input.LeftStickUp:
                        return state.ThumbSticks.Left.Y;
                    case Input.LeftStickLeft:
                        return state.ThumbSticks.Left.X;
                    case Input.LeftStickDown:
                        return state.ThumbSticks.Left.Y;
                    case Input.LeftStickRight:
                        return state.ThumbSticks.Left.X;
                    case Input.RightStickUp:
                        return state.ThumbSticks.Right.Y;
                    case Input.RightStickLeft:
                        return state.ThumbSticks.Right.X;
                    case Input.RightStickDown:
                        return state.ThumbSticks.Right.Y;
                    case Input.RightStickRight:
                        return state.ThumbSticks.Right.X;
                }
            }
            return 0f;
        }

        public bool AllPlayersConnected()
        {
            return players.Count == PlayerCount;
        }

        public int GetPlayerCount()
        {
            return players.Count;
        }

        public bool SetVibration(int index, float left, float right)
        {
            if (index >= players.Count) return false;
            var player = players[index];
            if (player is GamePadPlayer)
            {
                var gpp = (GamePadPlayer)player;
                return GamePad.SetVibration(gpp.index, left, right);
            }
            return false;
        }

        public GamePadCapabilities GetCapabilities(int index)
        {
            if (index >= players.Count) return new GamePadCapabilities();
            var player = players[index];
            if (player is GamePadPlayer)
            {
                var gpp = (GamePadPlayer)player;
                return GamePad.GetCapabilities(gpp.index);
            }
            return new GamePadCapabilities();
        }
    }
}
