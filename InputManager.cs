using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace A1r.Input
{
    // Input enum used to identify which action has been pressed
    public enum Input
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

    public struct InputToKey
    {
        public Input Input;
        public Keys Key;
        public InputToKey(Input input, Keys key)
        {
            Input = input;
            Key = key;
        }
    }

    public class InputManager : GameComponent
    {
        // Types of player input
        private class Player
        {
            public int Index { get; set; }
        }
        private class KeyboardPlayer : Player
        {
            public InputToKey[] Map;
        }
        private class GamePadPlayer : Player
        {
            public int GamePadIndex;
            public GamePadState CurrentState;
            public GamePadState PreviousState;
        }

        private List<Player> players;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private GamePadDeadZone gamePadDeadZone = GamePadDeadZone.IndependentAxes;
        private Array inputValues;
        public float DeadzoneSticks = 0.25f;
        public float DeadzoneTriggers = 0.25f;
        public int PlayerCount = 1;
        public int MaximumGamePadCount = 8;
        public bool PartyMode;

        public InputManager(Game game) : base(game)
        {
            inputValues = Enum.GetValues(typeof(Input));
            players = new List<Player>();
        }

        public override void Update(GameTime gameTime)
        {
            // Save the one and only (if available) keyboardstate 
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            for (int i = players.Count - 1; i >= 0; i--)
            {
                var player = players[i] as GamePadPlayer;
                if (player != null)
                {
                    var state = GamePad.GetState(player.GamePadIndex, gamePadDeadZone);
                    if (state.IsConnected)
                    {
                        // Update gamepad state
                        player.PreviousState = player.CurrentState;
                        player.CurrentState = state;
                        players[i] = player;
                    }
                    else // Remove disconnected players
                        players.RemoveAt(i);
                }
            }
            // Checking for new gamepads
            if (players.Count < PlayerCount)
            {
                var indices = Enumerable.Range(0, MaximumGamePadCount).ToList();
                for (int i = 0, l = players.Count; i < l; i++)
                {
                    var player = players[i] as GamePadPlayer;
                    if (player != null)
                        indices.RemoveAt(player.GamePadIndex);
                }

                foreach (var j in indices)
                {
                    var state = GamePad.GetState(j, gamePadDeadZone);
                    if (state.IsConnected)
                    {
                        AddPlayer(new GamePadPlayer()
                        {
                            GamePadIndex = j,
                            CurrentState = state
                        });

                        if (players.Count == PlayerCount)
                            break;
                    }
                }
            }
            base.Update(gameTime);
        }

        private int getNewIndex()
        {
            var count = players.Count;
            for (int i = 1; i < count; i++)
                if (players[i].Index > i)
                    return players[i - 1].Index + 1;
            return count;
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
            return currentKeyboardState.IsKeyDown(key);
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
            var p = players[index];
            if (p is KeyboardPlayer)
            {
                var player = (KeyboardPlayer)p;
                var key = getKey(player.Map, input);
                return currentKeyboardState.IsKeyDown(key);
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
                    return state.ThumbSticks.Left.X < -DeadzoneSticks;
                case Input.LeftStickDown:
                    return state.ThumbSticks.Left.Y < -DeadzoneSticks;
                case Input.LeftStickRight:
                    return state.ThumbSticks.Left.X > DeadzoneSticks;
                case Input.RightStick:
                    return state.Buttons.RightStick == ButtonState.Pressed;
                case Input.RightStickUp:
                    return state.ThumbSticks.Right.Y > DeadzoneSticks;
                case Input.RightStickLeft:
                    return state.ThumbSticks.Right.X < -DeadzoneSticks;
                case Input.RightStickDown:
                    return state.ThumbSticks.Right.Y < -DeadzoneSticks;
                case Input.RightStickRight:
                    return state.ThumbSticks.Right.X > DeadzoneSticks;
            }
            return false;
        }

        public bool IsHeld(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
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
            var p = players[index];
            if (p is KeyboardPlayer)
            {
                var player = (KeyboardPlayer)p;
                var key = getKey(player.Map, input);
                return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
            }
            else
            {
                var player = (GamePadPlayer)p;
                return IsPressed(player.CurrentState, input) && IsPressed(player.PreviousState, input);
            }
        }

        public bool JustPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
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
            var p = players[index];
            if (p is KeyboardPlayer)
            {
                var player = (KeyboardPlayer)p;
                var key = getKey(player.Map, input);
                return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
            }
            else
            {
                var player = (GamePadPlayer)p;
                return IsPressed(player.CurrentState, input) && !IsPressed(player.PreviousState, input);
            }
        }

        public bool JustReleased(Keys key)
        {
            return !currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
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
            var p = players[index];
            if (p is KeyboardPlayer)
            {
                var player = (KeyboardPlayer)p;
                var key = getKey(player.Map, input);
                return !currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
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
            var p = players[index];
            if (p is KeyboardPlayer)
                return currentKeyboardState.GetPressedKeys().Length > 0;
            else
            {
                var player = (GamePadPlayer)p;
                foreach (Input key in inputValues)
                    if (IsPressed(player.CurrentState, key))
                        return true;
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
            var p = players[index] as GamePadPlayer;
            if (p != null)
            {
                var state = p.CurrentState;
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

        private void AddPlayer(Player player)
        {
            // PartyMode will just add an available controller to the list while NormalMode (bool=false) keeps the player indexes in place
            if (PartyMode)
                players.Add(player);
            else
            {
                var index = getNewIndex();
                player.Index = index;
                players.Insert(index, player);
            }
        }

        public void AddKeyBoardPlayer(InputToKey[] map)
        {
            if (map != null)
            {
                var keyboardPlayer = new KeyboardPlayer();
                keyboardPlayer.Map = map;
                AddPlayer(keyboardPlayer);
            }
        }

        public bool SetVibration(int index, float left, float right)
        {
            GamePadPlayer player = null;
            if (index < players.Count)
                player = players[index] as GamePadPlayer;
            return player == null ? false : GamePad.SetVibration(player.GamePadIndex, left, right);
        }

        public GamePadCapabilities GetCapabilities(int index)
        {
            GamePadPlayer player = null;
            if (index < players.Count)
                player = players[index] as GamePadPlayer;
            return player == null ? new GamePadCapabilities() : GamePad.GetCapabilities(player.GamePadIndex);
        }
    }
}
