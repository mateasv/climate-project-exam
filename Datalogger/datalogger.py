import board
import busio
import adafruit_am2320
import time
from datetime import datetime
from gpiozero import DigitalInputDevice

# Air Humidity + Temperature
i2c = busio.I2C(board.SCL, board.SDA)
airSensor = adafruit_am2320.AM2320(i2c)
#airSensor.relative_humidity, airSensor.temperature

# Soil Wet (True/False) Sensor
d0_input = DigitalInputDevice(22)

while True:
    print('Humidity: {0}%'.format(airSensor.relative_humidity))
    print('Temperature: {0}C'.format(airSensor.temperature))
    if (not d0_input.value):
        print('Soil is wet')
    else:
        print('Soil is dry')
    time.sleep(3600)
