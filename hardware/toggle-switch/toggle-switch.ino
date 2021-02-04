// Serial configuration
const long baudRate = 57600;

// First 6 bytes indicate the type of button (toggle), last byte(s) indicate the message type
const byte toggleButtonIdentifier[6] = {210, 196, 183, 121, 141, 28};
const byte toggleStartMessage = 57;
const byte toggleEndMessage = 184;

// Pin setup
const int inputPin = 8;
const int ledPin = 13;

// State
int defaultState;
int lastInputState;

void setup() {
  // Setup input pin
  pinMode(inputPin, INPUT_PULLUP);

  // Use the startup state as the default non-toggled state
  defaultState = digitalRead(inputPin);
  lastInputState = defaultState;

  // Start the serial connection
  Serial.begin(baudRate);  
}

void loop() {
  bool inputState = digitalRead(inputPin);

  if (inputState == lastInputState)
    return;

  sendMessage(inputState);

  lastInputState = inputState;
}

void sendMessage(int inputState) {
  byte message;
  int ledState;
  if (inputState == defaultState) {
    message = toggleEndMessage;
    ledState = LOW;
  }
  else {
    message = toggleStartMessage;
    ledState = HIGH;
  }

  digitalWrite(ledPin, ledState);

  // Send the button type
  Serial.write(toggleButtonIdentifier, sizeof(toggleButtonIdentifier));

  // Send the message
  Serial.write(message);

  // Null terminate the message
  Serial.write(0);
}
