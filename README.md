# TimedExecuteV2 (Hellseek/RFC1920)
Extension of the TimedExecute plugin - You can add multiple TimerOnce instances as well as a time and day of week timer.

[Download](https://code.remod.org/TimedExecutev2.cs)

FIXME NOTE: For the TimerOnce configuration always at the end u must execute the command(Only if you want to start again from the top to the bottom of the TimerOnce chain "resettimeronce" or "reset.timeronce". Please remember that this TIMER IS WORKING UNTILL THE RESET COMMAND.

NOTE 2: You can see in my config the command (for the TimerOnce) will get executed after 60 seconds so the second command will get executed after 60 seconds too! (60 60=120) this timer is a chain so u have to add an additional seconds for the second command!

NOTE (RealTime-Timer): The time MUST look like "HH:mm:ss" ex. "18:30:00".
Configuration
The settings and options for this plugin can be configured in the TimedExecute.json file under the oxide/config directory. The use of a JSON editor or validation site such as jsonlint.com is recommended to avoid formatting issues and syntax errors.

```json
{
  "EnableInGameTime-Timer": false,
  "EnableRealTime-Timer": false,
  "EnableTimerOnce": true,
  "EnableTimerRepeat": true,
  "InGameTime-Timer": {
  "01:00": "command",
  "12:00": "command 1",
  "15:00": "command 2"
  },
  "RealTime-Timer": {
    "16:00:00": "command1 arg",
    "16:30:00": "command2 arg",
    "17:00:00": "command3 arg",
    "18:00:00": "command4 arg"
  },
  "TimerOnce": [{
    "Name": "TimerOnce1",
    "Commands": {
      "npc.guifile 419377188 Penny": 900,
      "npc.waypointoncollide 419377188 off": 900,
      "npc.respawn Penny": 900,
      "npc.waypoint Penny stop": 900,
      "npc.guishow 419377188 on": 900
    }
  },
  {
    "Name": "TimerOnce2",
    "Commands": {
      "npc.guifile 1742590713 ziva2": 1,
      "npc.guifile 1742590713 ziva": 300
    }
  }
  ],
  "TimerRepeat": {
    "oxide.reload CustomLootSpawns": 3600
  },
  "TimerWeekday": {
    "09:50:00 Saturday 1": "say Happy Saturday 1!!!",
    "09:50:00 Saturday 2": "say Happy Saturday 2!!!",
    "19:30:00 Tuesday all": "say Happy Tuesday !!!"
  }
}

```
