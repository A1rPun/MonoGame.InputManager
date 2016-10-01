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
        None,
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

    public enum MouseInput
    {
        None,
        LeftButton,
        MiddleButton,
        RightButton,
        Button1,
        Button2
    }

    public class InputManager : GameComponent
    {
        private class Player
        {
            public int Index;
            public int GamePadIndex;
            public bool Subscribed;
            public GamePadState CurrentState;
            public GamePadState PreviousState;
            public Dictionary<Input, Keys> KeyboardMap;
            public Dictionary<Input, MouseInput> MouseMap;
        }
        private List<Player> players;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private GamePadDeadZone gamePadDeadZone = GamePadDeadZone.IndependentAxes;
        private Array inputValues;
        private Array mouseValues;
        private int MaximumGamePadCount = 8;
        private const int KEYBOARD_INDEX = -1;
        private const int MOUSE_INDEX = -2;
        public float DeadzoneSticks = 0.25f;
        public float DeadzoneTriggers = 0.25f;
        public int PlayerCount = 1;
        public bool PartyMode;

        public InputManager(Game game) : base(game)
        {
            inputValues = Enum.GetValues(typeof(Input));
            mouseValues = Enum.GetValues(typeof(MouseInput));
            players = new List<Player>();
        }

        public override void Update(GameTime gameTime)
        {
            // Save the one and only (if available) keyboardstate 
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            // Save the one and only (if available) mousestate 
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            for (int i = players.Count - 1; i >= 0; i--)
            {
                var player = players[i];
                if (player.GamePadIndex >= 0)
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
                    var player = players[i];
                    if (player.GamePadIndex > -1)
                        indices.RemoveAt(player.GamePadIndex);
                }

                foreach (var j in indices)
                {
                    var state = GamePad.GetState(j, gamePadDeadZone);
                    if (state.IsConnected)
                    {
                        AddPlayer(new Player()
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

        private Player getPlayer(int index)
        {
            return index >= 0 && index < players.Count ? players[index] : null;
        }

        public bool IsPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public bool IsPressed(MouseInput input)
        {
            return IsPressed(currentMouseState, input);
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
            var player = getPlayer(index);
            if (player != null)
            {
                switch (player.GamePadIndex)
                {
                    case KEYBOARD_INDEX:
                        Keys key = player.KeyboardMap.TryGetValue(input, out key) ? key : default(Keys);
                        return currentKeyboardState.IsKeyDown(key);
                    case MOUSE_INDEX:
                        MouseInput mouse = player.MouseMap.TryGetValue(input, out mouse) ? mouse : default(MouseInput);
                        return IsPressed(currentMouseState, mouse);
                    default:
                        return IsPressed(player.CurrentState, input);
                }
            }
            return false;
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

        private bool IsPressed(MouseState state, MouseInput input)
        {
            switch (input)
            {
                case MouseInput.LeftButton:
                    return state.LeftButton == ButtonState.Pressed;
                case MouseInput.MiddleButton:
                    return state.MiddleButton == ButtonState.Pressed;
                case MouseInput.RightButton:
                    return state.RightButton == ButtonState.Pressed;
                case MouseInput.Button1:
                    return state.XButton1 == ButtonState.Pressed;
                case MouseInput.Button2:
                    return state.XButton2 == ButtonState.Pressed;
            }
            return false;
        }

        public bool IsHeld(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
        }

        public bool IsHeld(MouseInput input)
        {
            return IsPressed(currentMouseState, input) && IsPressed(previousMouseState, input);
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
            var player = getPlayer(index);
            if (player != null)
            {
                switch (player.GamePadIndex)
                {
                    case KEYBOARD_INDEX:
                        Keys key = player.KeyboardMap.TryGetValue(input, out key) ? key : default(Keys);
                        return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
                    case MOUSE_INDEX:
                        MouseInput mouse = player.MouseMap.TryGetValue(input, out mouse) ? mouse : default(MouseInput);
                        return IsPressed(currentMouseState, mouse) && IsPressed(currentMouseState, mouse);
                    default:
                        return IsPressed(player.CurrentState, input) && IsPressed(player.PreviousState, input);
                }
            }
            return false;
        }

        public bool JustPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }

        public bool JustPressed(MouseInput input)
        {
            return IsPressed(currentMouseState, input) && !IsPressed(previousMouseState, input);
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
            var player = getPlayer(index);
            if (player != null)
            {
                switch (player.GamePadIndex)
                {
                    case KEYBOARD_INDEX:
                        Keys key = player.KeyboardMap.TryGetValue(input, out key) ? key : default(Keys);
                        return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
                    case MOUSE_INDEX:
                        MouseInput mouse = player.MouseMap.TryGetValue(input, out mouse) ? mouse : default(MouseInput);
                        return IsPressed(currentMouseState, mouse) && !IsPressed(currentMouseState, mouse);
                    default:
                        return IsPressed(player.CurrentState, input) && !IsPressed(player.PreviousState, input);
                }
            }
            return false;
        }

        public bool JustReleased(Keys key)
        {
            return !currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
        }

        public bool JustReleased(MouseInput input)
        {
            return !IsPressed(currentMouseState, input) && IsPressed(previousMouseState, input);
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
            var player = getPlayer(index);
            if (player != null)
            {
                switch (player.GamePadIndex)
                {
                    case KEYBOARD_INDEX:
                        Keys key = player.KeyboardMap.TryGetValue(input, out key) ? key : default(Keys);
                        return !currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
                    case MOUSE_INDEX:
                        MouseInput mouse = player.MouseMap.TryGetValue(input, out mouse) ? mouse : default(MouseInput);
                        return !IsPressed(currentMouseState, mouse) && IsPressed(currentMouseState, mouse);
                    default:
                        return !IsPressed(player.CurrentState, input) && IsPressed(player.PreviousState, input);
                }
            }
            return false;
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
            var player = getPlayer(index);
            if (player != null)
            {
                switch (player.GamePadIndex)
                {
                    case KEYBOARD_INDEX:
                        return currentKeyboardState.GetPressedKeys().Length > 0;
                    case MOUSE_INDEX:
                        foreach (MouseInput key in mouseValues)
                            if (IsPressed(currentMouseState, key))
                                return true;
                        break;
                    default:
                        foreach (Input key in inputValues)
                            if (IsPressed(player.CurrentState, key))
                                return true;
                        break;
                }
            }
            return false;
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
            var player = getPlayer(index);
            if (player != null && player.GamePadIndex > -1)
            {
                var state = player.CurrentState;
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
            var count = players.Count;
            return count == 0 ? false : count == PlayerCount;
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

        public void AddKeyboardPlayer(Dictionary<Input, Keys> map)
        {
            if (map != null)
            {
                var keyboardPlayer = new Player();
                keyboardPlayer.GamePadIndex = -1;
                keyboardPlayer.KeyboardMap = map;
                AddPlayer(keyboardPlayer);
            }
        }

        public void AddMousePlayer(Dictionary<Input, MouseInput> map)
        {
            if (map != null)
            {
                var mousePlayer = new Player();
                mousePlayer.GamePadIndex = -2;
                mousePlayer.MouseMap = map;
                AddPlayer(mousePlayer);
            }
        }

        public void Subscribe(int index)
        {
            var player = getPlayer(index);
            if (player != null)
                player.Subscribed = true;
        }

        public void Unsubscribe(int index)
        {
            var player = getPlayer(index);
            if (player != null)
                player.Subscribed = false;
        }

        public bool IsSubscribed(int index)
        {
            var player = getPlayer(index);
            return player == null ? false : player.Subscribed;
        }

        public void Flush()
        {
            var newPlayers = new List<Player>();
            for (int i = 0, l = players.Count; i < l; i++)
            {
                var player = players[i];
                if (player.Subscribed)
                {
                    player.Subscribed = false;
                    newPlayers.Add(player);
                }
            }
            players = newPlayers;
            PlayerCount = players.Count;
        }

        public bool SetGamepadVibration(int index, float left, float right)
        {
            var player = getPlayer(index);
            return player == null || player.GamePadIndex == -1
                ? false
                : GamePad.SetVibration(player.GamePadIndex, left, right);
        }

        public GamePadCapabilities GetCapabilities(int index)
        {
            var player = getPlayer(index);
            return player == null || player.GamePadIndex == -1
                ? new GamePadCapabilities()
                : GamePad.GetCapabilities(player.GamePadIndex);
        }

        public Point GetMousePosition()
        {
            return currentMouseState.Position;
        }

        public int GetMouseScroll()
        {
            return currentMouseState.ScrollWheelValue;
        }
    }
}
