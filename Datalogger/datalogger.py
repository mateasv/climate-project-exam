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

######## START OF SETTINGS ########
# Backend IP, PORT and URL.
backend_ip = "10.176.132.59"
backend_port = "7189" #5189
backend_url = 'https://%s:%s'%(backend_ip, backend_port) # Default: /api/DataloggerMeasurements

# Interval to send measurement
send_interval_in_sec = 15 # 3600 = 1 hour

# How often to print a status message
print_status_message_interval = 15 # Prints a "Sending measurement in xx:yy:zz" every x seconds

# The ID for this datalogger
dataloggerID = 1
######## END OF SETTINGS ########



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
    "reconnect_interval": 6,
    "max_attempts": 5
  }).build()

# Callback function to be called when the SignalR connection has connected.
def openn(*args):
  print('SignalR is connected')
  print("######")
  print(args)
  print("######")
  print(args[0])
  print("######")
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
  # Check whether the backend has told us if the warning LED should light up.
  if (args[0][1]):
    print('Warning Received!')
    GPIO.output(24,GPIO.HIGH)
    GPIO.output(25,GPIO.HIGH)
  else:
    print('No warning received!')
    GPIO.output(24,GPIO.LOW)
    GPIO.output(25,GPIO.HIGH)

currentPlantID = 0 # The plantID to save the ID's for.
def newdatalogger(*args):
  print("The datalogger is now logging for plant #" + str(currentPlantID))
  currentPlantID = args[0][0]['plantId']


# Set up event handlers for the SignalR connection
hub.on_open(openn)
hub.on_close(close)
hub.on("ReceiveWarning", warning)
hub.on("ReceiveDataloggerPair", newdatalogger)
hub.start()

# Get the plant ID that this datalogger is assigned to
assigned_plant = requests.get(backend_url + '/api/plants/datalogger/' + str(dataloggerID), timeout=10, verify=False)
assigned_plant = assigned_plant.content.decode('UTF-8')
assigned_plant = eval(assigned_plant)
print("This is your assigned plant ID:")
print(assigned_plant.content.decode('UTF-8'))

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

# Don't start the datalogger until we have the assigned Plant ID
while True:
  if currentPlantID == 0:
    time.sleep(1)
  else:
    break

# The main loop responsible for giving status messages and doing
# automatic climate readings at the given interval (Default one hour)
while True:
  # Showing a status message every 20th second
  if counter % 20 == 0 and counter != 0:
    print('Sending measurement in ' + convertSecondsToHHMMSS(send_interval_in_sec - counter))
  # Make and send climate reading when the counter hits the given interval
  if counter % send_interval_in_sec == 0:
    counter = 0
    requests.post(backend_url + str("/api/DataloggerMeasurements"), timeout=8, verify=False, json={"DataloggerId": dataloggerID,
    "PlantId": currentPlantID,
    "SoilHumidity": 1,
    "AirHumidity": airSensor.relative_humidity,
    "AirTemperature": airSensor.temperature,
    "SoilIsDry": soilSensor.value == 1 if True else False})
    print('Measurement was successfully sent for plant #' + str(currentPlantID))
  counter += 1
  # Wait a second before ticking the counter again
  time.sleep(1)