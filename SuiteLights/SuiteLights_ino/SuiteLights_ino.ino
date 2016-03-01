#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
  #include <avr/power.h>
#endif

#define NUM_STRIPS 2
#define LIGHTS_PER_STRIP 30

#define START_BYTE 32
#define SEND_RESPONSE 65

#define PIN_A 5
#define PIN_B 6

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

// IMPORTANT: To reduce NeoPixel burnout risk, add 1000 uF capacitor across
// pixel power leads, add 300 - 500 Ohm resistor on first pixel's data input
// and minimize distance between Arduino and first pixel.  Avoid connecting
// on a live circuit...if you must, connect GND first.

void setup() {
  
  Serial.begin(9600);
  
  // This is for Trinket 5V 16MHz, you can remove these three lines if you are not using a Trinket
  #if defined (__AVR_ATtiny85__)
    if (F_CPU == 16000000) clock_prescale_set(clock_div_1);
  #endif
  // End of trinket special code

  for (int i = 0; i < NUM_STRIPS; i++) {
    strips[i].begin();
    strips[i].show(); // Initialize all pixels to 'off'
  }
  
  Serial.println("<Arduino is ready>");
}

byte start;
byte stripNo;
byte pos;
byte r;
byte g;
byte b;

void loop() {
  // waits for the strip selection, position, and color.
  byte lastStart;
  while (true) {
    if (Serial.available() < 6) {
     continue; 
    }
    start = Serial.read();
    if (start == START_BYTE) {
      break;
    } else {
     Serial.print("invalid start byte: "); Serial.println(start); 
    }
    //delay(50);
  }
    //delay(100);    

  stripNo = Serial.read() - 'a';
  pos = Serial.read() - 'a';
  r = Serial.read();
  g = Serial.read();
  b = Serial.read();
  
  Serial.print("Entered: "); 
  Serial.print(start); Serial.print(" "); 
  Serial.print(stripNo); Serial.print(" "); 
  Serial.print(pos); Serial.print(" "); 
  Serial.print(r); Serial.print(" "); 
  Serial.print(b); Serial.print(" ");
  Serial.println(g);

  if (stripNo == SEND_RESPONSE) {
     Serial.print("HELLO FROM ARDUINO");
     return;
  } else if (stripNo < 0 || stripNo >= NUM_STRIPS) {
    Serial.print("error occurred - strip number invalid: "); Serial.println(stripNo); 
    return;
  } else if (pos < 0 || pos >= LIGHTS_PER_STRIP) {
    Serial.print("error occurred - no led position for "); Serial.println(pos); 
  }
  
  strips[stripNo].setPixelColor((uint32_t) pos, strips[0].Color(r,g,b));
  strips[stripNo].show();  
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
  //
  
  
  
  
  //allRainbowCycle(20);
  
  
  
  
  //theaterChaseRainbow(50);
}

// Fill the dots one after the other with a color
void colorWipe(uint32_t c, uint8_t wait) {
   for (uint32_t s = 0; s < NUM_STRIPS; s++) {
      for(uint16_t i=0; i<strips[s].numPixels(); i++) {
        strips[s].setPixelColor(i, c);
        strips[s].show();
        delay(wait);
      }
      
      if (s == 2) {
        strips[0].setPixelColor(s,100);
       strips[0].show();
      }
   }
   strips[0].setPixelColor(1,100);
      strips[0].show();
}

void rainbow(uint8_t wait) {
  for (int s = 0; s < NUM_STRIPS; s++) {
    uint16_t i, j;
  
    for(j=0; j<256; j++) {
      for(i=0; i<strips[s].numPixels(); i++) {
        strips[s].setPixelColor(i, Wheel((i+j) & 255));
      }
      strips[s].show();
      delay(wait);
    }
  }
}

// Slightly different, this makes the rainbow equally distributed throughout
void rainbowCycle(uint8_t wait) {
  for (int s = 0; s < NUM_STRIPS; s++) {          
      uint16_t i, j;
    
      for(j=0; j<256*5; j++) { // 5 cycles of all colors on wheel
        for(i=0; i< strips[s].numPixels(); i++) {
          strips[s].setPixelColor(i, Wheel(((i * 256 / strips[s].numPixels()) + j) & 255));
        }
        strips[s].show();
        delay(wait);
      }
  }
}

// Slightly different, this makes the rainbow equally distributed throughout
void allRainbowCycle(uint8_t wait) {          
      uint16_t i, j;
    
      for(j=0; j<256*5; j++) { // 5 cycles of all colors on wheel
      for (int s = 0; s < NUM_STRIPS; s++) {
        for(i=0; i< strips[s].numPixels(); i++) {
          strips[s].setPixelColor(i, Wheel(((i * 256 / strips[s].numPixels()) + j) & 255));
        }
        strips[s].show();
      }
        delay(wait);
      }
}

//Theatre-style crawling lights.
void theaterChase(uint32_t c, uint8_t wait) {
  for (int s = 0; s < NUM_STRIPS; s++) {    
    for (int j=0; j<10; j++) {  //do 10 cycles of chasing
      for (int q=0; q < 3; q++) {
        for (uint16_t i=0; i < strips[s].numPixels(); i=i+3) {
          strips[s].setPixelColor(i+q, c);    //turn every third pixel on
        }
        strips[s].show();
  
        delay(wait);
  
        for (uint16_t i=0; i < strips[s].numPixels(); i=i+3) {
          strips[s].setPixelColor(i+q, 0);        //turn every third pixel off
        }
      }
    }
  }
}

//Theatre-style crawling lights with rainbow effect
void theaterChaseRainbow(uint8_t wait) {
  for (int s = 0; s < NUM_STRIPS; s++) {      
    for (int j=0; j < 256; j++) {     // cycle all 256 colors in the wheel
      for (int q=0; q < 3; q++) {
        for (uint16_t i=0; i < strips[s].numPixels(); i=i+3) {
          strips[s].setPixelColor(i+q, Wheel( (i+j) % 255));    //turn every third pixel on
        }
        strips[s].show();
  
        delay(wait);
  
        for (uint16_t i=0; i < strips[s].numPixels(); i=i+3) {
          strips[s].setPixelColor(i+q, 0);        //turn every third pixel off
        }
      }
    }
  }
}

// Input a value 0 to 255 to get a color value.
// The colours are a transition r - g - b - back to r.
uint32_t Wheel(byte WheelPos) {
  WheelPos = 255 - WheelPos;
  if(WheelPos < 85) {
    return strips[0].Color(255 - WheelPos * 3, 0, WheelPos * 3);
  }
  if(WheelPos < 170) {
    WheelPos -= 85;
    return strips[0].Color(0, WheelPos * 3, 255 - WheelPos * 3);
  }
  WheelPos -= 170;
  return strips[0].Color(WheelPos * 3, 255 - WheelPos * 3, 0);
}

char receivedChar;
boolean newData = false;

void getInput() {
  // waits to recieve a char
    while(!Serial.available()) { }
  
      	if (Serial.available() > 0) {
  		receivedChar = Serial.read();
  		newData = true;
  	}
}

