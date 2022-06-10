import board
import busio
import adafruit_am2320
import time
from datetime import datetime
from gpiozero import DigitalInputDevice
import requests
import RPi.GPIO as GPIO
from signalrcore.protocol.messagepack_protocol import MessagePackHubProtocol
from signalrcore.hub_connection_builder import HubConnectionBuilder
import signal
import urllib3
urllib3.disable_warnings()

## Settings
backend_ip = "10.176.132.59"
backend_port = "7189" #5189
backend_url = 'https://%s:%s/api/DataloggerMeasurements'%(backend_ip, backend_port) # Default: /api/DataloggerMeasurements
send_interval_in_sec = 15 # 3600 = 1 hour
print_status_message_interval = 15 # Prints a "Sending measurement in xx:yy:zz" every x seconds

# LED setup
GPIO.setmode(GPIO.BCM)
GPIO.setup(24,GPIO.OUT) # Red
GPIO.setup(25,GPIO.OUT) # Green
# Make sure both are off when the program starts
GPIO.output(24,GPIO.LOW)
GPIO.output(25,GPIO.LOW)


# Air Humidity + Temperature
i2c = busio.I2C(board.SCL, board.SDA)
airSensor = adafruit_am2320.AM2320(i2c)

# Soil Wet (True/False) Sensor
soilSensor = DigitalInputDevice(22) # DO label on board

# counter % 3600 = send data to backend
counter = 0

# Setup SignalR hub connection,
# disabled verify_ssl due to self signed certificate.
hub = HubConnectionBuilder()\
  .with_url("https://%s:%s/treehub"%(backend_ip, backend_port), options={"verify_ssl":False})\
  .with_automatic_reconnect({
    "type": "raw",
    "keep_alive_interval": 10,
    "reconnect_interval": 5,
    "max_attempts": 5
  }).build()

# Callback function to be called when the SignalR connection has connected.
def openn():
  print('SignalR is connected')
  print('Registering Datalogger...')
  hub.send("RegisterDatalogger", [1])
  GPIO.output(24,GPIO.LOW)
  GPIO.output(25,GPIO.HIGH)

# Callback function to be called when the SignalR connection has been closed.
def close():
  print('SignalR connection closed')
  GPIO.output(24,GPIO.HIGH)
  GPIO.output(25,GPIO.LOW)

# Callback function to be called when a warning is received from the backend.
def warning(*args):
  
  print(args[0][0])
  if (args[0][1]):
    print('Warning Received!')
    GPIO.output(24,GPIO.HIGH)
    GPIO.output(25,GPIO.HIGH)
  else:
    print('No warning received!')
    GPIO.output(24,GPIO.LOW)
    GPIO.output(25,GPIO.HIGH)
  
  print(args[0][1])

# Set up event handlers for the SignalR connection
hub.on_open(openn)
hub.on_close(close)
hub.on("ReceiveWarning", warning)
hub.start()

# Function to handle SIGINT signal.
# To close the SignalR connection properly before exiting.
def sigint_handler(signum, frame):
  print("Turning off datalogger..")
  hub.stop()
  print("Datalogger off!")
  time.sleep(0.25)
  exit(1)

# Register sigint handler
signal.signal(signal.SIGINT, sigint_handler)

# Convert seconds to a better looking formatted string HH:MM:SS
def convertSecondsToHHMMSS(sec):
  sec = sec % (24 * 3600)
  hour = sec // 3600
  sec %= 3600
  minutes = sec // 60
  sec %= 60
    
  return "%d:%02d:%02d" % (hour, minutes, sec)

# The main loop responsible for giving status messages and doing
# automatic climate readings at the given interval (Default one hour)
while True:
  # Showing a status message every 20th second
  if counter % 20 == 0 and counter != 0:
    print('Sending measurement in ' + convertSecondsToHHMMSS(send_interval_in_sec - counter))
  # Make and send climate reading when the counter hits the given interval
  if counter % send_interval_in_sec == 0:
    counter = 0
    x = requests.post(backend_url, timeout=5, verify=False, json={"DataloggerId": 1,
    "PlantId": 1,
    "SoilHumidity": 1,
    "AirHumidity": airSensor.relative_humidity,
    "AirTemperature": 900,#airSensor.temperature,
    "SoilIsDry": soilSensor.value == 1 if True else False})
    print('Measurement was successfully sent')
  counter += 1
  # Wait a second before ticking the counter again
  time.sleep(1)