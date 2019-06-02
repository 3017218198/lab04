unsigned long timeoutStart;    // find out whether timeout

unsigned long seconds = 0;     // CMU timer

int continued = 1;

// functions here
// error function
void writesError() {
  byte buffer[3];
  buffer[0] = (byte)0xFA;
  buffer[1] = (byte)0x7F;
  buffer[2] = (byte)0x7F;
  Serial.write(buffer, 3);
  delay(200);
}

// valid function
void writesValid() {
  byte b[3];
  b[0] = (byte)0xFA;
  b[1] = (byte)0x55;
  b[2] = (byte)0x55;
  Serial.write(b, 3);
  delay(200);
}

// success function: output success while change LED
void writesSuccess(){
  byte b[3];
  b[0] = 0x11;
  b[1] = 0x11;
  b[2] = 0x11;
  Serial.write(b,3);
  delay(200);
}


double getTemperature(double ADreads)
{
  double R5=10000.0;
  double R0=10000.0;
  double T0=25+273.15;
  double B=3435.0;
  double temperature_fah = (B*T0)/(B + T0*(log(1024/ADreads - 1) + log(R5/R0)));
  return temperature_fah;
}

// temperature function
void writesTemperature() {
  double value = (double)analogRead(A0);
  double dvalue = getTemperature(value);
  int xvalue = (int)dvalue;
  byte buffer[3];
  buffer[0] = 0xE0 | (A0 - 14);
  buffer[1] = xvalue;
  buffer[2] = (byte)((xvalue >> 7) & 0x7F);
  if (continued == 1)
  {
    Serial.write(buffer, 3);
    delay(200);
  }
}

// light function
void writesLight() {
  int value = analogRead(A1);
  byte buffer[3];
  buffer[0] = 0xE0 | (A1 - 14);
  buffer[1] = (byte)(value & 0xFF);
  buffer[2] = (byte)((value >> 7) & 0x7F);
  Serial.write(buffer, 3);
  delay(200);
}

// LED state function
void writesLedState(int Pin) {
  int value = digitalRead(Pin);
  byte buffer[3];
  buffer[0] = 0xC0 | (Pin);
  buffer[1] = (byte)0;
  buffer[2] = (byte)value;
  Serial.write(buffer, 3);
  delay(200);
}

// LED PWM control function
void controlLedPWM(int Pin, int value) {
  analogWrite(Pin, value);
  writesSuccess();
}

// LED PWM read function (unimplemented)
void writesLedPWM() {

}

// student number function
// original student number : 3017218 198
// hex student number (last 3 digits): c6
void writesStudentNumber() {
  int value = 198;
  byte buffer[3];
  buffer[0] = 0xF9;
  buffer[1] = (byte)(value & 0x7F);
  buffer[2] = (byte)((value >> 7) & 0x7F);
  Serial.write(buffer, 3);
  delay(200);
}

// CMU time
void writesCMUTime(unsigned long seconds) {
  byte b[3];
  b[0] = 0xFF;
  b[1] = (byte)(seconds & 0x7F);
  b[2] = (byte)((seconds >> 7) & 0x7F);
  Serial.write(b, 3);
  delay(200);
}

// resolute according to the input command
void resoluteByte(byte b, byte c, byte d) {
  // AD output value (checked)
  // **error with resoluting byte d
  if ((b & 0xF0) == 0xE0) {
    // resolute the pin number
    int ADPin = (b & 0xF);
    if (ADPin == 0)
    {
      // deal with next two bytes
      int temp = resoluteSecondByteInCase1(c);
      if (temp == 0) {
        writesTemperature();
      }
      else {
        writesError();
      }
    }
    else if (ADPin == 1)
    {
      // deal with next two bytes
      int temp2 = resoluteSecondByteInCase1(c);
      if (temp2 == 0) {
        writesLight();
      }
      else {
        writesError();
      }
    }
    else {
      writesError();
    }
  }

  // LED state control (checked)
  // 0 for off, other for on
  else if ((b & 0xF0) == 0x90) {
    // resolute the pin number
    int DOPin = (b & 0xF);
    int state = resoluteSecondByteInCase2(d);
    digitalWrite(DOPin, state);
    writesSuccess();
  }

  // LED state read (checked)
  else if ((b & 0xF0) == 0xC0) {
    // resolute the pin number
    int DOPin = (b & 0xF);
    if ((c & 0x66) == 0x66)
    {
      writesLedState(DOPin);
    }
    else
    {
      writesError();
    }
  }

  // LED PWM control (checked error)
  // ** error with resoluting byte d
  else if ((b & 0xF0) == 0xD0) {
    // resolute the pin number
    int DOPin = (b & 0xF);
    int k = resoluteSecondByteInCase3(c);
    analogWrite(DOPin, k);
    writesSuccess();
  }

  // CMU time (checked)
  else if ((b & 0xFF) == 0xFF) {
    if ((c & 0x55) == 0x55)
    {
      seconds = millis();
      writesCMUTime(seconds);
    }
    else
    {
      writesError();
    }
  }

  // student number (checked)
  else if ((b & 0xFF) == 0xF9) {
    if (((c & 0x55) == 0x55) && ((d & 0x55) == 0x55))
    {
      writesStudentNumber();
    }
    else
    {
      writesError();
    }
  }

  // invalid input (checked)
  else {
    writesError();
  }
}

// resolute the second byte of case 1: AD output
int resoluteSecondByteInCase1(byte n) {
  if ((n & 0xFF) == 0x11)
  {
    return 0;
  }
  else
  {
    return 1;
  }
}

// resolute the second byte of case 2: LED state control
int resoluteSecondByteInCase2(byte c) {
  byte x = 0x01;  // off
  byte y = 0x00;  // on
  if (c == y)
  {
    return 1;
  }
  // invalid input
  else
  {
    return 0;
  }
}

// resolute the second byte of case 3: LED PWM control
int resoluteSecondByteInCase3(byte c) {
  return (int)c;
}

void setup() {
  // put your setup code here, to run once:
  Serial.begin(38400);
  timeoutStart = 0;

  // set up the pin mode
  pinMode(2, OUTPUT);       // yellow led
  pinMode(3, OUTPUT);       // green led
  pinMode(4, OUTPUT);       // red led
  pinMode(5, OUTPUT);       // blue led
  pinMode(6, OUTPUT);       // white led
}

int number = 0;
byte buffer[3];

void loop() {
  // put your main code here, to run repeatedly:
//  unsigned long currentime = millis();  
  unsigned long timer1 = millis();    // find out input time out
  if (timer1 - timeoutStart >= 1000)
  {
    timeoutStart = timer1;
//    writesTemperature();
    writesLight();
  }
//  else{
  // read data into buffer array
  unsigned long timer2 = millis();
  if (Serial.available() > 0)
  {
    
    int data = Serial.read();
    if (number == 0)
    {
//      timeoutStart = timer1;
      timeoutStart = timer2;
      buffer[0] = data;
      number++;
//      Serial.write(0);
    }
    else if (number == 1)
    {
      buffer[1] = data;
      number++;
    }
    else if (number == 2)
    {
      buffer[2] = data;
      number++;
    }
    else {
      number = 0;
    }
  }

  if (timer2 - timeoutStart >= 1000)
  {
    // reset the buffer byte array if time out
    number = 0;
    //    byte test = 0xFF;
    //    Serial.write(test);
  }

  if (number == 3)
  {
//    Serial.write(buffer[0]);
//    Serial.write(buffer[1]);
//    Serial.write(buffer[2]);
    
    resoluteByte(buffer[0], buffer[1], buffer[2]);
    delay(100);
//    writesValid();
//    delay(100);
    number = 0;
  }
//  }
}
