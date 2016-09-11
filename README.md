# MonoGame-InputManager
An easy-to-use inputmanager for MonoGame

	 ___                   _   __  __                                   
	|_ _|_ __  _ __  _   _| |_|  \/  | __ _ _ __   __ _  __ _  ___ _ __ 
	 | || '_ \| '_ \| | | | __| |\/| |/ _` | '_ \ / _` |/ _` |/ _ \ '__|
	 | || | | | |_) | |_| | |_| |  | | (_| | | | | (_| | (_| |  __/ |   
	|___|_| |_| .__/ \__,_|\__|_|  |_|\__,_|_| |_|\__,_|\__, |\___|_|   
			  |_|                                       |___/           
          
			  _=====_                               _=====_
			 / _____ \                             / _____ \
		   +.-'_____'-.---------------------------.-'_____'-.+
		  /   |     |  '.           _           .'  |  _  |   \
		 / ___| /|\ |___ \       ,'" "',       / ___| (_) |___ \
		/ |      |      | ;      | A1r |      ; | _         _ | ;
		| | <---   ---> | |  __  '.___.'  __  | |(_)       (_)| |
		| |___   |   ___| ; |__|         |__> ; |___   _   ___| ;
		|\    | \|/ |    /  _              _   \    | (_) |    /|
		| \   |_____|  .','" "',        ,'" "', '.  |_____|  .' |
		|  '-.______.-' /       \      /       \  '-._____.-'   |
		|               |       |------|       |                |
		|              /\       /      \       /\               |
		|             /  '.___.'        '.___.'  \              |
		|            /                            \             |
		 \          /                              \           /
		  \________/                                \_________/ 

# Goals

- Controller first inputmanager
- Customization options
- Easy to port
- Seamless multiplayer

## Install
1. Copy `InputManager.cs` into your MonoGame project
2. ????
3. PROFIT!

## Usage

	using A1r.Input;

	// MonoGame class
    public class Peace : Game
    {
		private InputManager iM;

        public Peace()
		{
			// Construct a new inputmanager to use it in the Update function.
            iM = new InputManager(this);
		}

        protected override void Initialize()
		{
			// Register GameComponent or call `iM.Update(gameTime)` in the Game's Update function
			this.Components.Add(iM);
        }

		protected override void Update(GameTime gameTime)
        {
			// Setting up the right values
			var acceleration = 1.337f;
			var pos = new Vector2(x, y);
			// Check for an Input and handle it accordingly
            if (iM.IsPressed(Input.Home))
			{
                Exit();
			}
			// Accelerate based on user input
            if (iM.IsPressed(Input.Right))
            {
                pos.X += acceleration;
            }
            if (iM.IsPressed(Input.Left))
            {
                pos.X -= acceleration;
            }
            if (iM.IsPressed(Input.Up))
            {
                pos.Y -= acceleration;
            }
            if (iM.IsPressed(Input.Down))
            {
                pos.Y += acceleration;
            }
		}
	}

# More info

## InputManager constructor

Add a keyboard mapping to a "GamePad" to create compatibility for both keyboard and controller

	iM = new InputManager(this, new List<InputToKey>
	{
		new InputToKey(Input.Home, Keys.Space),
        new InputToKey(Input.Start, Keys.Enter),
        new InputToKey(Input.Back, Keys.Back),
        new InputToKey(Input.Up, Keys.Up),
        new InputToKey(Input.Left, Keys.Left),
        new InputToKey(Input.Right, Keys.Right),
        new InputToKey(Input.Down, Keys.Down),
    });

So code like this will still work

	if (iM.IsPressed(Input.Right))
	{
		pos.X += acceleration;
	}

## Multiplayer

To implement multiple keyboard/controller players you can do something like this:

	public class Peace : Game
    {
		// Simple player with actions
        struct myPlayer
		{
            public Input Run;
            public Input Jump;
		}
        myPlayer[] myPlayerArray;

		// In multiplayer initialisation code
		public Peace()
		{
			iM = new InputManager(this)
			{
				PlayerCount = 4
			};
			// Set up an simple player array with remappable input actions.
			myPlayerArray = new myPlayer[]
			{
				new myPlayer() { Run = Input.FaceButtonLeft, Jump = Input.FaceButtonDown }
				new myPlayer() { Run = Input.FaceButtonLeft, Jump = Input.FaceButtonDown }
			};

		}
		// In Update code
		protected override void Update(GameTime gameTime)
		{
			// Check if the number of available players has changed. You can handle it your way.
			if (!iM.AllPlayersConnected())
            {
                // Please connect controller
            }

            for (int i = 0; i < myPlayerArray.Length; i++)
            {
                var player = myPlayerArray[i];
                if (iM.IsPressed(player.Run, i))
                {
					// Let the player run
                }
                if (iM.IsPressed(player.Jump, i))
                {
					// Let the player jump
                }
            }
		}
	}

## InputManager Properties

Deadzones

![Axial deadzone](http://www.third-helix.com/images/2013/04/axial-deadzone.jpg)

    float DeadzoneSticks = 0.25f;
    float DeadzoneTriggers = 0.25f;

Inputmanager will try to find connected controllers based on this variable.

    int PlayerCount = 1;

## InputManager Methods

	public bool IsPressed(Keys key)
	public bool IsPressed(Input input)
	public bool IsPressed(Input input, int index)
	public bool IsHeld(Keys key)
	public bool IsHeld(Input input)
	public bool IsHeld(Input input, int index)
	public bool JustPressed(Keys key)
	public bool JustPressed(Input input)
	public bool JustPressed(Input input, int index)
	public bool JustReleased(Keys key)
	public bool JustReleased(Input input)
	public bool JustReleased(Input input, int index)
	public bool SomethingDown()
	public bool SomethingDown(int index)
	public float GetRaw(Input input)
	public float GetRaw(Input input, int index)
	public bool AllPlayersConnected()
    public int GetPlayerCount()

## Input Enum
Input enum used to identify which action has been pressed

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

## InputToKey struct

	public InputToKey(Input input, Keys key)

## Types of configurations

- Singleplayer PC
- Singleplayer cross-platform
- Multiplayer cross-platform

# TODO

- XINPUT VS DIRECTINPUT ??
- MousePlayer ?? mouse support
- More devices?
- Remove keyboardstates (&& mouse) if platform != Computer (just a simple boolean)
- More deadzone options http://www.third-helix.com/2013/04/12/doing-thumbstick-dead-zones-right.html

# Licence

MIT
--
