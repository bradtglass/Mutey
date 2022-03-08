# Hardware Buttons

Information about producing the partner hardware for Mutey

## Example
The example mute button is created using the following hardware:
- Arduino Nano (or equivalent)
- 1/4" TS jack socket
- Project box
- Footswitch with 1/4" TS jack output

The sketch for the Arduino can be found in [toggle-button](toggle-button), this sketch sets the board up to send a message whenever the input changes from high to low or vice versa on a digital input pin. The messages includes the following information:
- [6] The first 6 bytes contain a reference identifier for the type of button (so far only TOGGLE type buttons are supported but this allows for the use of other types like on/off latching in future versions)
    - TOGGLE: {210, 196, 183, 121, 141, 28}
- [n] The following 1 or more bytes contain the information about the message type
    - START: 57
    - END: 184
- [1] A final null byte ({0}) to indicate the end of the message