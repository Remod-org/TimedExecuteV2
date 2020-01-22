# TimedExecuteV2
Extension of the TimedExecute plugin - more detail to come

It's a very simple plugin that can execute many commands every (X) seconds!

NOTE: For the TimerOnce configuration always at the end u must execute the command(Only if you want to start again from the top to the bottom of the TimerOnce chain "resettimeronce" or "reset.timeronce". Please remember that this TIMER IS WORKING UNTILL THE RESET COMMAND.

NOTE 2: You can see in my config the command (for the TimerOnce) will get executed after 60 seconds so the second command will get executed after 60 seconds too! (60 60=120) this timer is a chain so u have to add an additional seconds for the second command!

NOTE (RealTime-Timer): The time MUST look like "HH:mm:ss" ex. "18:30:00".
Configuration
The settings and options for this plugin can be configured in the TimedExecute.json file under the oxide/config directory. The use of a JSON editor or validation site such as jsonlint.com is recommended to avoid formatting issues and syntax errors.

```json
{
  "EnableInGameTime-Timer": true,
  "EnableRealTime-Timer": true,
  "EnableTimerOnce": true,
  "EnableTimerRepeat": true,
  "InGameTime-Timer": {
    "01:00": "weather rain",
    "12:00": "command 1",
    "15:00": "command 2"
  },
  "RealTime-Timer": {
    "16:00:00": "command1 arg",
    "16:30:00": "command2 arg",
    "17:00:00": "command3 arg",
    "18:00:00": "command4 arg"
  },
  "TimerOnce": {
    "command1 'msg'": 60,
    "command2 'msg'": 120,
    "command3 arg": 180,
    "reset.timeronce": 181
  },
  "TimerRepeat": {
    "command1 arg": 300,
    "command2 'msg'": 300
  },
  "TimerWeekday": {
    "09:50:00 Saturday 2": "say Happy Saturday 2!!!",
    "09:50:00 Saturday 1": "say Happy Saturday 1!!!",
    "19:30:00 Tuesday all": "say Happy Tuesday !!!"
  }
}
```
