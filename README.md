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
            if (iM.IsDown(Input.Home))
			{
                Exit();
			}
			// Accelerate based on user input
            if (iM.IsDown(Input.Right))
            {
                pos.X += acceleration;
            }
            if (iM.IsDown(Input.Left))
            {
                pos.X -= acceleration;
            }
            if (iM.IsDown(Input.Up))
            {
                pos.Y -= acceleration;
            }
            if (iM.IsDown(Input.Down))
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
		new InputToKey(Input.Home, new Keys[] { Keys.Escape, Keys.Space, Keys.Enter }),
        new InputToKey(Input.Start, Keys.Enter),
        new InputToKey(Input.Back, Keys.Back),
        new InputToKey(Input.Up, Keys.Up),
        new InputToKey(Input.Left, Keys.Left),
        new InputToKey(Input.Right, Keys.Right),
        new InputToKey(Input.Down, Keys.Down),
    });

So code like this will still work

	if (iM.IsDown(Input.Right))
	{
		pos.X += acceleration;
	}

## Multiplayer

To implement multiple keyboard/controller players you can do something like this:

	// In multiplayer initialisation code
	public Peace()
    {
		iM = new InputManager()
		{
		}
	}
	int NumberOfPlayers = iM.getPlayerCount();
	// In Update code
	protected override void Update(GameTime gameTime)
    {
		// Check if the number of players has changed. You can handle it your way.
		if(iM.getPlayerCount() != NumberOfPlayers)
		{
			// Please connect controller
		}
		
		yourPlayerClass[] myArray;
		foreach(var player in myArray)
		{
			// Use an index to retrieve the player action
			if(iM.IsDown(Input.Home, player.index)){ /**/ }
		}
	}

	

## InputManager Properties
## InputManager Methods
## Input Enum
## InputToKey struct

## Types of configurations

- Singleplayer PC
- Singleplayer cross-platform
- Multiplayer cross-platform

# Licence

MIT
--
