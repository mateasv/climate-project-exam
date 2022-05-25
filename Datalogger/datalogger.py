import board
import busio
import adafruit_am2320
import time
from datetime import datetime
from gpiozero import DigitalInputDevice
import requests

backend_url = 'https://10.176.160.156:7189/api/Measurements'

# Air Humidity + Temperature
i2c = busio.I2C(board.SCL, board.SDA)
airSensor = adafruit_am2320.AM2320(i2c)
#airSensor.relative_humidity, airSensor.temperature

# Soil Wet (True/False) Sensor
soilSensor = DigitalInputDevice(22)

# counter % 3600 = send data to backend
counter = 0

while True:
    if counter % 3600 == 0:
        counter = 0
        #print('Humidity: {0}%'.format(airSensor.relative_humidity))
        #print('Temperature: {0}C'.format(airSensor.temperature))
        #if (not soilSensor.value):
        #    print('Soil is wet')
        #else:
        #    print('Soil is dry')
        measurement = {'air_humidity': airSensor.relative_humidity,
        'air_temperature': airSensor.temperature,
        'soil_wet': (not soilSensor.value)}
        x = requests.post(backend_url, data = measurement)
        print(x.text)
    counter += 1
    time.sleep(1)
