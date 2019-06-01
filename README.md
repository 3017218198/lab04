# lab04
C# lab4, by Lu Yuan, in June 1st, 2019

## Introduction
1. This project provides an interface to connect between pc and arduino
2. This project limited coversation format to Midi format, all interactions are based on midi protocol (more details in lab introduction)
3. The data in this project is discrete for I've added a time delay of nearly 2 seconds each command
4. For the time delay, you need to input your command twice if there's no reponse in 2 seconds
5. The arduino file can be viewed in folder "arduino"

## Functions
1. Exchange data between pc and arduino based on Midi protocol
[!screen](screencut/1.gif)
Attention that the input format should be xx-xx-xx(hexadecimal), for example, 0xFF 0xFF 0xFF, and the output data is decimal
2. Use sliders to change LED by PWM
[!screen](screencut/2.gif)
Attention that the input of sliders have been reversed to fit the common scene
3. Dynamically draw the data-time graph

## Change Log
#### first version(0b804a3), released in 9:16, June 1st, 2019
1. Without json file record of received data
2. This version is not stable enough
