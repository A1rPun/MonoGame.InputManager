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

# Goal

- Simplified
- Customization
- Portable
- Multiplayer

## Install
1. Copy `InputManager.cs` into your MonoGame project
2. ????
3. PROFIT!

## Usage

	// -- Example Code -- //
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

	iM = new InputManager(this, new List<InputToKey>{
        new InputToKey(Input.Home, Keys.Space),
        new InputToKey(Input.Up, Keys.Up),
        new InputToKey(Input.Left, Keys.Left),
        new InputToKey(Input.Right, Keys.Right),
        new InputToKey(Input.Down, Keys.Down)
    });

## InputManager Properties
## InputManager Methods
## Input Enum
## InputToKey struct

# Licence

MIT
--
