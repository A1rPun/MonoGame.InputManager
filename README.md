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
		/ |      '      | ;      | A1r |      ; | _         _ | ;
		| | <--     --> | |  __  '.___.'  __  | |(_)       (_)| |
		| |___   .   ___| ; |__|         |__> ; |___   _   ___| ;
		|\    | \|/ |    /  _              _   \    | (_) |    /|
		| \   |_____|  .','" "',        ,'" "', '.  |_____|  .' |
		|  '-.______.-' /       \      /       \  '-._____.-'   |
		|               |       |------|       |                |
		|              /\       /      \       /\               |
		|             /  '.___.'        '.___.'  \              |
		|            /                            \             |
		 \          /                              \           /
		  \________/                                \_________/ 


- [Goals](#goals)
- [Install](#install)
- [Methods](#goals)
- [Usage](#usage)
- [More Info](#moreinfo)
- [Local Multiplayer](#localmp)
- [Handling player selection](#selectmp)
- [Porting](#porting)
- [Enums](#enums)


## <a name="goals">Goals

- Controller first inputmanager
- Customization options
- Easy to port
- Seamless multiplayer

## <a name="install">Install
1. Copy `InputManager.cs` into your MonoGame project
2. ????
3. PROFIT!

## <a name="methods">Methods

**Gamepad methods**  
These methods are also used for mapped keyboards or a mapped mouse.

    bool IsPressed(Input input)
	bool IsPressed(Input input, int index)
    bool IsHeld(Input input)
    bool IsHeld(Input input, int index)
    bool JustPressed(Input input)
	bool JustPressed(Input input, int index)
	bool JustReleased(Input input)
	bool JustReleased(Input input, int index)
	bool SomethingDown()
	bool SomethingDown(int index)
    float GetRaw(Input input)
	float GetRaw(Input input, int index)
    bool SetGamepadVibration(int index, float left, float right)
    GamePadCapabilities GetCapabilities(int index)
    void Subscribe(int index)
    void Unsubscribe(int index)
    bool IsSubscribed(int index)
    void Flush()

**Keyboard methods**

	bool IsPressed(Keys key)
    bool IsHeld(Keys key)
    bool JustPressed(Keys key)
    bool JustReleased(Keys key)
    void AddKeyboardPlayer(Dictionary<Input, Keys> map)
    

**Mouse methods**

    bool IsPressed(MouseInput input)
    bool IsHeld(MouseInput input)
    bool JustPressed(MouseInput input)
    bool JustReleased(MouseInput input)
    Point GetMousePosition()
    int GetMouseScroll()
    void AddMousePlayer(Dictionary<Input, MouseInput> map)
    
**Generic**
	
	bool AllPlayersConnected()
    int GetPlayerCount()

**MonoGameComponent**

    public void Update()

## <a name="props">Properties

Deadzones

![Axial deadzone](http://www.third-helix.com/images/2013/04/axial-deadzone.jpg)

    float DeadzoneSticks = 0.25f;
    float DeadzoneTriggers = 0.25f;

Inputmanager will try to find connected controllers based on this variable.

    int PlayerCount = 1;


## <a name="usage">Usage

    using A1r.Input
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
            Exit();
				
            // Accelerate based on user input
            if (iM.IsPressed(Input.Right))
            pos.X += acceleration;
				
            if (iM.IsPressed(Input.Left))
            pos.X -= acceleration;
				
            if (iM.IsPressed(Input.Up))
            pos.Y -= acceleration;
				
            if (iM.IsPressed(Input.Down))
            pos.Y += acceleration;
        }
    }

## <a name="moreinfo">More info

### <a name="localmp">Local Multiplayer

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
        		PlayerCount = 2
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
                // Please connect controller

            for (int i = 0; i < myPlayerArray.Length; i++)
            {
                var player = myPlayerArray[i];
                if (iM.IsPressed(player.Run, i))
                    // Let the player run
        			
                if (iM.IsPressed(player.Jump, i))
                    // Let the player jump
            }
        }
	}

### <a name="selectmp">Handling player selection

    public class Peace : Game
    {
        public Input JoinGame = Input.FaceButtonDown;
        public Input LeaveGame = Input.FaceButtonRight;
        public Input StartGame = Input.Start;
        // In multiplayer initialisation code
        public Peace()
        {
        	iM = new InputManager(this)
        	{
        		PlayerCount = 8
        	};
        }
        // In Update code
        protected override void Update(GameTime gameTime)
        {
            var count = iM.GetPlayerCount();
            bool start;
            for (int i = 0; i < count; i++)
            {
                if(iM.IsSubscribed())
                {
                    if (iM.IsPressed(LeaveGame, i))
                        iM.Unsubscribe(i);
                    else if (iM.IsPressed(StartGame, i))
                        start = true;
                }
                else if (iM.IsPressed(JoinGame, i))
                    iM.Subscribe(i);
            }

            if (start)
                iM.Flush();
            // This function flushes the unsubscribed players and sets the player count to the subscribed players
        }
	}

## <a name="porting">Porting
If you plan to write an controller first game you may end up supporting other platforms.
To add keyboard or mouse support

    iM.AddKeyboardPlayer(new Dictionary<Input, Keys>
    {
        { Input.Up, Keys.Up },
        { Input.Left, Keys.Left },
        { Input.Right, Keys.Right },
        { Input.Down, Keys.Down },
        { Input.Back, Keys.Space }
    });
    iM.AddMousePlayer(new Dictionary<Input, MouseInput>
    {
        { Input.Up, MouseInput.Button2 },
        { Input.Left, MouseInput.LeftButton },
        { Input.Right, MouseInput.RightButton },
        { Input.Down, MouseInput.Button1 },
        { Input.Back, MouseInput.MiddleButton }
    });


## <a name="enums">Enums

**For Gamepads**

Use the `Input` enum to identify which action has been pressed

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

**For Keyboard**

Use the `Microsoft.Xna.Framework.Input.Keys` enum to identify which action has been pressed


**For Mouse**

Use the `MouseInput` enum to identify which action has been pressed

    public enum MouseInput
    {
        None,
        LeftButton,
        MiddleButton,
        RightButton,
        Button1,
        Button2
    }

## Types of configurations

- Singleplayer PC
- Singleplayer cross-platform
- Multiplayer cross-platform

# TODO

- Remove keyboardstates (&& mouse) if platform != Computer (just a simple boolean)
- [More deadzone options](http://www.third-helix.com/2013/04/12/doing-thumbstick-dead-zones-right.html)

# Licence

MIT
--
