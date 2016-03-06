#include <Adafruit_NeoPixel.h>
#include <Arduino.h>
#include <string.h>
#include <inttypes.h>

// strip parameters
#define NUM_STRIPS 2
#define LIGHTS_PER_STRIP 30

#define PIN_A 5
#define PIN_B 6

// options
#define START_BYTE 32
#define SEND_RESPONSE 128
#define INCREASE_SPEED 12
#define DECREASE_SPEED 13

// modes
#define OFF 0
#define INDIVIDUAL_LIGHTS 1
#define RAINBOW_GLOW 2
#define THEATER_CHASE 3
#define THEATER_CHASE_RAINBOW 4
#define COLOR_WIPE 5

#define FREQ_BAND_MUSIC 6
#define FREQ_BAND_RAINBOW 7
#define FREQ_BAND_PICK 8

#define FREQ_BAND_SPLIT_MUSIC 9
#define PREQ_BAND_SPLIT_RAINBOW 10
#define FREQ_BAND_SPLIT_PICK 11


// Parameter 1 = number of pixels in strip
// Parameter 2 = Arduino pin number (most are valid)
// Parameter 3 = pixel type flags, add together as needed:
//   NEO_KHZ800  800 KHz bitstream (most NeoPixel products w/WS2812 LEDs)
//   NEO_KHZ400  400 KHz (classic 'v1' (not v2) FLORA pixels, WS2811 drivers)
//   NEO_GRB     Pixels are wired for GRB bitstream (most NeoPixel products)
//   NEO_RGB     Pixels are wired for RGB bitstream (v1 FLORA pixels, not v2)
//   NEO_RGBW    Pixels are wired for RGBW bitstream (NeoPixel RGBW products)

Adafruit_NeoPixel strips[NUM_STRIPS] = {
	Adafruit_NeoPixel(LIGHTS_PER_STRIP, PIN_A, NEO_GRB + NEO_KHZ800),
	Adafruit_NeoPixel(LIGHTS_PER_STRIP, PIN_B, NEO_GRB + NEO_KHZ800)
};

// the global delay time, can be changed with increase or decrease speed options.
byte wait;

// IMPORTANT: To reduce NeoPixel burnout risk, add 1000 uF capacitor across
// pixel power leads, add 300 - 500 Ohm resistor on first pixel's data input
// and minimize distance between Arduino and first pixel.  Avoid connecting
// on a live circuit...if you must, connect GND first.

void setup() {

	Serial.begin(9600);

	//initialize delay time and strips.
	wait = 20;
	for (int i = 0; i < NUM_STRIPS; i++) {
		strips[i].begin();
		strips[i].show(); // Initialize all pixels to 'off'
	}

	Serial.println("<Arduino is ready>");
}

void loop() {
	// waits for the start byte and the mode associated with it. 
	byte start;
	while (true) {
		if (Serial.available() < 2) {
			continue;
		}
		start = Serial.read();
		if (start == START_BYTE) {
			break;
		}
	}

	byte mode = Serial.read();
	
	switch (mode) {
		case SEND_RESPONSE:
			Serial.print("HELLO FROM ARDUINO");
			break;
		case OFF:
			off();
			break;
		case INDIVIDUAL_LIGHTS:
			individualLights();
			break;
		case RAINBOW_GLOW:
			allRainbowCycle();
			break;
		case THEATER_CHASE:
			theaterChase();
			break;
		case THEATER_CHASE_RAINBOW:
			theaterChaseRainbow();
			break;
		case COLOR_WIPE:
			colorWipe();
			break;
		case FREQ_BAND_MUSIC:
			//TDOD
			break;
		case FREQ_BAND_RAINBOW:

			break;
		case FREQ_BAND_PICK:

			break;
		case FREQ_BAND_SPLIT_MUSIC:

			break;
		case PREQ_BAND_SPLIT_RAINBOW:

			break;
		case FREQ_BAND_SPLIT_PICK:

			break;
		case INCREASE_SPEED:
			wait = wait < 5 ? wait : wait - 5;
			break;
		case DECREASE_SPEED:
			wait = wait >= 100 ? wait : wait + 5;
			break;
	}

	
	// Some example procedures showing how to display to the pixels:
	//colorWipe(strips[0].Color(255, 0, 0), 50); // Red
	//colorWipe(strips[0].Color(0, 255, 0), 50); // Green
	//colorWipe(strips[0].Color(0, 0, 255), 50); // Blue
	//colorWipe(strip.Color(0, 0, 0, 255), 50); // White RGBW
	// Send a theater pixel chase in...
	//theaterChase(strips[0].Color(127, 127, 127), 50); // White
	//theaterChase(strips[0].Color(127, 0, 0), 50); // Red
	//theaterChase(strips[0].Color(0, 0, 127), 50); // Blue

	//rainbow(20);
	//rainbowCycle(20);

	//allRainbowCycle(20);
	//theaterChaseRainbow(50);
}

void individualLights() {
	while (true) {
		if (Serial.available() >= 5) {
			break;
		}
	}
	byte stripNo = Serial.read();
	byte pos = Serial.read();
	byte r = Serial.read();
	byte g = Serial.read();
	byte b = Serial.read();

	strips[stripNo].setPixelColor((uint16_t)pos, strips[0].Color(r, g, b));
	strips[stripNo].show();
}

void off() {
	for (int i = 0; i < NUM_STRIPS; i++) {
		for (int j = 0; j < LIGHTS_PER_STRIP; j++) {
			strips[i].setPixelColor(j, strips[0].Color(0, 0, 0));
		}
		strips[i].show();
	}
}

// Fill the dots one after the other with a color
void colorWipe() {
	while (true) {
		if (Serial.available() >= 4) {
			break;
		}
	}
	byte stripNo = Serial.read();
	byte r = Serial.read();
	byte g = Serial.read();
	byte b = Serial.read();
	
	for (uint16_t i = 0; i < strips[stripNo].numPixels(); i++) {
		strips[stripNo].setPixelColor(i, strips[0].Color(r,g,b));
		strips[stripNo].show();
		delay(wait);
	}
}

// All strips are glowing rainbow with this setting.
void allRainbowCycle() {
	uint16_t i, j;

	for (j = 0; j<256 * 5; j++) { // 5 cycles of all colors on wheel
		if (Serial.available() > 0) {
			return;
		}
		for (int s = 0; s < NUM_STRIPS; s++) {
			for (i = 0; i< strips[s].numPixels(); i++) {
				strips[s].setPixelColor(i, Wheel(((i * 256 / strips[s].numPixels()) + j) & 255));
			}
			strips[s].show();
		}
		delay(wait);
	}
}

//Theatre-style crawling lights.
void theaterChase() {
	while (true) {
		if (Serial.available() >= 4) {
			break;
		}
	}
	byte stripNo = Serial.read();
	byte r = Serial.read();
	byte g = Serial.read();
	byte b = Serial.read();

	for (int j = 0; j<10; j++) {
		for (int q = 0; q < 3; q++) {
			if (Serial.available() > 0) {
				return;
			}
			for (uint16_t i = 0; i < strips[stripNo].numPixels(); i = i + 3) {
				strips[stripNo].setPixelColor(i + q, strips[0].Color(r,g,b));    //turn every third pixel on
			}
			strips[stripNo].show();
			delay(wait);

			for (uint16_t i = 0; i < strips[stripNo].numPixels(); i = i + 3) {
				strips[stripNo].setPixelColor(i + q, 0);        //turn every third pixel off
			}
		}
	}
}

//Theatre-style crawling lights with rainbow effect
void theaterChaseRainbow() {
	while (true) {
		if (Serial.available() >= 1) {
			break;
		}
	}
	byte stripNo = Serial.read();

	for (int j = 0; j < 256; j++) {     // cycle all 256 colors in the wheel
		for (int q = 0; q < 3; q++) {
			if (Serial.available() > 0) {
				return;
			}
			for (uint16_t i = 0; i < strips[stripNo].numPixels(); i = i + 3) {
				strips[stripNo].setPixelColor(i + q, Wheel((i + j) % 255));    //turn every third pixel on
			}
			strips[stripNo].show();

			delay(wait);

			for (uint16_t i = 0; i < strips[stripNo].numPixels(); i = i + 3) {
				strips[stripNo].setPixelColor(i + q, 0);        //turn every third pixel off
			}
		}
	}
}

// Input a value 0 to 255 to get a color value.
// The colours are a transition r - g - b - back to r.
// creates a rainbow.
uint32_t Wheel(byte WheelPos) {
	WheelPos = 255 - WheelPos;
	if (WheelPos < 85) {
		return strips[0].Color(255 - WheelPos * 3, 0, WheelPos * 3);
	}
	if (WheelPos < 170) {
		WheelPos -= 85;
		return strips[0].Color(0, WheelPos * 3, 255 - WheelPos * 3);
	}
	WheelPos -= 170;
	return strips[0].Color(WheelPos * 3, 255 - WheelPos * 3, 0);
}


//void rainbow() {
//	for (int s = 0; s < NUM_STRIPS; s++) {
//		uint16_t i, j;
//		
//		for (j = 0; j<256; j++) {
//			for (i = 0; i<strips[s].numPixels(); i++) {
//				strips[s].setPixelColor(i, Wheel((i + j) & 255));
//			}
//			strips[s].show();
//			delay(wait);
//		}
//	}
//}
//
//// Slightly different, this makes the rainbow equally distributed throughout
//void rainbowCycle() {
//	for (int s = 0; s < NUM_STRIPS; s++) {
//		uint16_t i, j;
//
//
//		for (j = 0; Serial.available() <= 0; j++) { // 5 cycles of all colors on wheel
//			for (i = 0; i< strips[s].numPixels(); i++) {
//				strips[s].setPixelColor(i, Wheel(((i * 256 / strips[s].numPixels()) + j) & 255));
//			}
//			strips[s].show();
//			delay(wait);
//		}
//	}
//}