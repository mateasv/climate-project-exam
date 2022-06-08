import board
import busio
import adafruit_am2320
import time
from datetime import datetime
from gpiozero import DigitalInputDevice
import requests

## Settings
backend_url = 'http://10.176.160.179:5189/api/DataloggerMeasurements' # Default: /api/DataloggerMeasurements
send_interval_in_sec = 3600 #3600 = 1 hour
print_status_message_interval = 15 # Prints a "Sending measurement in xx:yy:zz" every x seconds

# Air Humidity + Temperature
i2c = busio.I2C(board.SCL, board.SDA)
airSensor = adafruit_am2320.AM2320(i2c)

# Soil Wet (True/False) Sensor
soilSensor = DigitalInputDevice(22) # DO label on board

# counter % 3600 = send data to backend
counter = 0

def convertSecondsToHHMMSS(sec):
    sec = sec % (24 * 3600)
    hour = sec // 3600
    sec %= 3600
    minutes = sec // 60
    sec %= 60
      
    return "%d:%02d:%02d" % (hour, minutes, sec)

while True:
    if counter % 20 == 0 & counter != 0:
        print('Sending measurement in ' + convertSecondsToHHMMSS(send_interval_in_sec - counter))
    if counter % send_interval_in_sec == 0:
        counter = 0
        print('Sending measurements now!')
        x = requests.post(backend_url, timeout=5, verify=False, json={"DataloggerId": 1,
        "PlantId": 1,
        "SoilHumidity": 1,
        "AirHumidity": airSensor.relative_humidity,
        "AirTemerature": airSensor.temperature,
        "SoilIsDry": soilSensor.value == 1 if True else False})
        print(x.text)
    counter += 1
    time.sleep(1)