import board
import busio
import adafruit_am2320
import time
from datetime import datetime
from gpiozero import DigitalInputDevice
import requests
import asyncio
from signalr_async.netcore import Hub, Client
from signalr_async.netcore.protocols import MessagePackProtocol
import RPi.GPIO as GPIO

## Settings
backend_ip = "10.176.132.59"
backend_port = "5189"
backend_url = 'http://%s:%s/api/DataloggerMeasurements'%(backend_ip, backend_port) # Default: /api/DataloggerMeasurements
send_interval_in_sec = 3600 # 3600 = 1 hour
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

# MyHub class with various events
class MyHub(Hub):
  async def on_connect(self, connection_id: str) -> None:
    print("SignalR is connected.")

  async def on_disconnect(self) -> None:
    # Turn off the green LED
    GPIO.output(25,GPIO.LOW)
    # Turn on the red LED
    GPIO.output(24,GPIO.HIGH)
    print("SignalR disconnected.")

  def ReceiveWarning(self, x: str) -> None:
    # Since we got a warning, we turn on the red LED
    GPIO.output(24,GPIO.HIGH)
    print("Warning was received.")
    print(self)
    print("###")
    print(x)

  def ReceiveDataloggerPair (self, x: str) -> None:
    print("Datalogger was paired with new device.")
    print(self)
    print("###")
    print(x)

hub = MyHub("treehub")

@hub.on("ReceiveWarning")
async def ReceiveWarning() -> None:
  GPIO.output(24,GPIO.HIGH)
  print("Warning was received.---")
  print(self)
  print("###")
  print(x)
  pass

@hub.on("ReceiveDataloggerPair")
async def ReceiveDataloggerPair() -> None:
  print("Datalogger was paired with new device.----")
  print(self)
  print("###")
  print(x)
  pass

# Subscribe to the SignalR events
hub.on('ReceiveWarning', ReceiveWarning)
hub.on('ReceiveDataloggerPair', ReceiveDataloggerPair)


def convertSecondsToHHMMSS(sec):
  sec = sec % (24 * 3600)
  hour = sec // 3600
  sec %= 3600
  minutes = sec // 60
  sec %= 60
    
  return "%d:%02d:%02d" % (hour, minutes, sec)

async def main():
  print("Registering datalogger in the backend")
  token = "mytoken"
  headers = {"Authorization": f"Bearer {token}"}
  print("Starting Client")
  async with Client(
    "http://%s:%s"%(backend_ip, backend_port),
    hub,
    connection_options={
      "http_client_options": {"headers": headers, "verify_ssl": False},
      "ws_client_options": {"headers": headers, "timeout": 1.0, "verify_ssl": False},
      "protocol": MessagePackProtocol()
    },
  ) as client:
    await hub.invoke("RegisterDatalogger", 1)
    print("Datalogger is now registered and working")
    # Light up the green LED
    GPIO.output(25,GPIO.HIGH)
    
    global counter
    while True:
      if counter > 3:
        break
      if counter % 20 == 0 and counter != 0:
        print('Sending measurement in ' + convertSecondsToHHMMSS(send_interval_in_sec - counter))
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
      time.sleep(1)

asyncio.run(main())