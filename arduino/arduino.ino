/*
This code handles mapping the analog input of a mysterious DDR dance pad to keyboard presses, through use of an arduino micro.
MIT License,
Kent Rasmussen
Thomas Riley Dawson
*/

/*
This struct stores
*/

struct padMap {
	int pin;
	int state;
	char key;
};

struct padMap padArray[5];

void setup()
{

	// Enable Serial port for debugging 
	Serial.begin(9600);

	// Initialize utilized pins
	pinMode(2, INPUT);
	pinMode(3, INPUT);
	pinMode(4, INPUT);
	pinMode(5, INPUT);
	pinMode(12, INPUT);

	// Enable keyboard utilities.
	Keyboard.begin();
	// Map each directional value of the keypad and store it.
	padMap Up = { 2, HIGH, 'w' };
	padMap Down = { 3, HIGH, 's' };
	padMap Left = { 4, HIGH, 'a' };
	padMap Right = { 5, HIGH, 'd' };
	padMap ULeft = { 6, HIGH, KEY_RETURN };
	padMap URight = { 7, HIGH, KEY_ESC };
	// Add the key maps to tracking array.
	padArray[0] = Up;
	padArray[1] = Down;
	padArray[2] = Left;
	padArray[3] = Right;
	padArray[4] = ULeft;
	padArray[5] = URight;
}

void loop()
{
	// Check each button state and flip if necessary
	for (int i = 0; i <= ((sizeof(padArray) / sizeof(padMap)) - 1); i++){
		Serial.print(padArray[i].state);

		if (padArray[i].state != digitalRead(padArray[i].pin)){
			//State change, flip last state of button
			padArray[i].state ^= 1 << 0;
			// Check if keyboard press needs to be released or not
			if (padArray[i].state){
				Keyboard.release(padArray[i].key);
			}
			else {
				Keyboard.press(padArray[i].key);

			}
		}

	}
	Serial.println();

	delay(5);
}