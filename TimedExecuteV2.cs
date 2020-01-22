using System;
using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;
using System.Linq;

namespace Oxide.Plugins
{
    [Info("Timed Execute v2", "PaiN/Hellseek", "0.7.9", ResourceId = 1937)]
    [Description("Execute commands every (x) seconds.")]
    class TimedExecutev2 : CovalencePlugin
    {
        public static TimedExecutev2 Plugin;

        public enum Types
        {
            RealTime,
            InGameTime,
            Repeater,
            TimerOnce
        };

        class TimerType
        {
            public Types Type;
            public string Name;

            public TimerType(Types type, string name)
            {
                Type = type;
                Name = name;
            }
        }

        #region Classes
        class Timers
        {
            public static Dictionary<Timer, TimerType> AllTimers = new Dictionary<Timer, TimerType>();
            public static Timer InGame;
            public static Timer Real;
            public static Timer Repeat;
            public static Timer Once;

            public static void ResetTimer(Types type, string timerName = "")
            {
                switch (type)
                {
                    case Types.InGameTime:
                        RunTimer(Types.InGameTime);
                        break;
                    case Types.RealTime:
                        RunTimer(Types.RealTime);
                        break;
                    case Types.Repeater:
                        RunTimer(Types.Repeater);
                        break;
                    case Types.TimerOnce:
                        RunTimer(Types.TimerOnce, timerName);
                        break;
                }
            }

            public static void StopTimer(Types type, string timerName = "")
            {
                switch (type)
                {
                    case Types.InGameTime:
                        Destroy(Types.InGameTime);
                        break;
                    case Types.RealTime:
                        Destroy(Types.RealTime);
                        break;
                    case Types.Repeater:
                        Destroy(Types.Repeater);
                        break;
                    case Types.TimerOnce:
                        Destroy(Types.TimerOnce, timerName);
                        break;
                }
            }

            public static void RunTimer(Types type, string timerName = "")
            {
                float timeinterval = 1f;
                #if RUST
                    timeinterval = 4.5f;
                #endif

                Destroy(type);

                switch (type)
                {
                    case Types.InGameTime:
                        Plugin.Puts($"The {type} timer has started");

                        var inGameTimers = Plugin.Config["InGameTime-Timer"] as Dictionary<string, object>;

                        if (inGameTimers == null)
                            return;

                        var inGame = Plugin.timer.Repeat(timeinterval, 0, () =>
                        {
                            foreach (var cmd in inGameTimers)
                            {
                                if (Plugin.covalence.Server.Time.ToShortTimeString() == cmd.Key)
                                {
                                    Plugin.covalence.Server.Command(cmd.Value.ToString());
                                    Plugin.Puts(string.Format("Ran CMD: {0}", cmd.Value));
                                }
                            }
                        });

                        AllTimers.Add(inGame, new TimerType(type, string.Empty));
                        break;
                    case Types.RealTime:
                        Plugin.Puts($"The {type} timer has started");

                        var realTimeTimers = Plugin.Config["RealTime-Timer"] as Dictionary<string, object>;

                        if (realTimeTimers == null)
                            return;

                        var real = Plugin.timer.Repeat(1, 0, () =>
                        {
                            foreach (var cmd in realTimeTimers)
                                if (System.DateTime.Now.ToString("HH:mm:ss") == cmd.Key.ToString())
                                {
                                    Plugin.covalence.Server.Command(cmd.Value.ToString());
                                    Plugin.Puts(string.Format("Ran CMD: {0}", cmd.Value));
                                }
                        });

                        AllTimers.Add(real, new TimerType(type, string.Empty));
                        break;
                    case Types.Repeater:
                        Plugin.Puts($"The {type} timer has started");

                        var repeatTimers = Plugin.Config["TimerRepeat"] as Dictionary<string, object>;

                        if (repeatTimers == null)
                            return;

                        foreach (var cmd in repeatTimers)
                        {
                            var repeat = Plugin.timer.Repeat(Convert.ToSingle(cmd.Value), 0, () => {
                                Plugin.covalence.Server.Command(cmd.Key);
                                Plugin.Puts(string.Format("Ran CMD: {0}", cmd.Key));
                            });

                            AllTimers.Add(repeat, new TimerType(type, string.Empty));
                        }
                        break;
                    case Types.TimerOnce:
                        if (string.IsNullOrEmpty(timerName))
                            Plugin.Puts($"The {type} timer has started");

                        var onceTimers = Plugin.Config["TimerOnce"] as List<object>;

                        if (onceTimers == null)
                            return;

                        foreach (var onceTimer in onceTimers)
                        {
                            var timer = onceTimer as Dictionary<string, object>;

                            if (timer == null || timer.Count != 2)
                                continue;

                            string name = timer["Name"]?.ToString();

                            if (!string.IsNullOrEmpty(timerName))
                            {
                                if (name != timerName)
                                    continue;
                            }

                            if (!string.IsNullOrEmpty(timerName))
                                Plugin.Puts($"The {type} timer '{timerName}' has started");

                            var commands = timer["Commands"] as Dictionary<string, object>;

                            if (commands == null)
                                continue;

                            foreach (var cmd in commands)
                            {
                                var once = Plugin.timer.Once(Convert.ToSingle(cmd.Value), () =>
                                {
                                    Plugin.covalence.Server.Command(cmd.Key);
                                    Plugin.Puts(string.Format("Ran CMD: {0}", cmd.Key));
                                });

                                AllTimers.Add(once, new TimerType(type, name));
                            }
                        }
                        break;
                }
            }

            public static void Destroy(Types type, string timerName = "")
            {
                foreach (var data in AllTimers)
                {
                    var timerType = data.Value as TimerType;

                    if (timerType == null)
                        continue;

                    if (timerType.Type != type)
                        continue;

                    if (!string.IsNullOrEmpty(timerName))
                    {
                        if (timerType.Name != timerName)
                            continue;
                    }

                    var timer = data.Key as Timer;

                    if (timer == null)
                        continue;

                    timer.Destroy();
                }
            }

            public static void DestroyAll()
            {
                foreach (var data in AllTimers)
                {
                    var timer = data.Key as Timer;

                    if (timer == null)
                        continue;

                    timer.Destroy();
                }
            }

            public static void RunAll()
            {
                var enableInGameTimer = Plugin.Config["EnableInGameTime-Timer"] as bool?;
                if (enableInGameTimer != null && (bool)enableInGameTimer)
                    RunTimer(Types.InGameTime);

                var enableRealTimeTimer = Plugin.Config["EnableRealTime-Timer"] as bool?;
                if (enableRealTimeTimer != null && (bool)enableRealTimeTimer)
                    RunTimer(Types.RealTime);

                var enableRepeatTimer = Plugin.Config["EnableTimerRepeat"] as bool?;
                if (enableRepeatTimer != null && (bool)enableRepeatTimer)
                    RunTimer(Types.Repeater);

                var enableOnceTimer = Plugin.Config["EnableTimerOnce"] as bool?;
                if (enableOnceTimer != null && (bool)enableOnceTimer)
                    RunTimer(Types.TimerOnce);
            }
        }
        #endregion

        void OnServerInitialized()
        {
            Plugin = this;
            Timers.RunAll();
        }

        void Unloaded()
        {
            Timers.DestroyAll();
        }

        class TimerOnce
        {
            public string Name;
            public Dictionary<string, object> Commands = new Dictionary<string, object>();

            public TimerOnce(string name)
            {
                Name = name;
            }
        }

        protected override void LoadDefaultConfig()
        {
            var repeatcmds = new Dictionary<string, object>();
            var realtimecmds = new Dictionary<string, object>();
            var ingamecmds = new Dictionary<string, object>();
            var chaincmds = new HashSet<TimerOnce>();

            repeatcmds.Add("command1 arg", 300);
            repeatcmds.Add("command2 'msg'", 300);
            Puts("Creating a new configuration file!");
            if (Config["TimerRepeat"] == null)
                Config["TimerRepeat"] = repeatcmds;

            var timerOnce = new TimerOnce("TimerOnce1");
            timerOnce.Commands.Add("command1 'msg'", 60);
            timerOnce.Commands.Add("command2 'msg'", 120);
            timerOnce.Commands.Add("command3 arg", 180);
            timerOnce.Commands.Add("reset.timeronce", 181);
            chaincmds.Add(timerOnce);
            if (Config["TimerOnce"] == null)
                Config["TimerOnce"] = chaincmds;

            if (Config["EnableTimerRepeat"] == null)
                Config["EnableTimerRepeat"] = true;
            if (Config["EnableTimerOnce"] == null)
                Config["EnableTimerOnce"] = true;
            if (Config["EnableRealTime-Timer"] == null)
                Config["EnableRealTime-Timer"] = true;
            if (Config["EnableInGameTime-Timer"] == null)
                Config["EnableInGameTime-Timer"] = true;

            realtimecmds.Add("16:00:00", "command1 arg");
            realtimecmds.Add("16:30:00", "command2 arg");
            realtimecmds.Add("17:00:00", "command3 arg");
            realtimecmds.Add("18:00:00", "command4 arg");
            if (Config["RealTime-Timer"] == null)
                Config["RealTime-Timer"] = realtimecmds;

            ingamecmds.Add("01:00", "weather rain");
            ingamecmds.Add("12:00", "command 1");
            ingamecmds.Add("15:00", "command 2");
            if (Config["InGameTime-Timer"] == null)
                Config["InGameTime-Timer"] = ingamecmds;
        }

        [Command("reset.timeronce", "resettimeronce")]
        private void CmdResetOnce(IPlayer player, string cmd, string[] args)
        {
            if (!player.IsAdmin)
                return;

            Timers.ResetTimer(Types.TimerOnce, args.Length == 0 ? "" : args[0]);
        }

        [Command("stop.timeronce", "stoptimeronce")]
        private void CmdStopOnce(IPlayer player, string cmd, string[] args)
        {
            if (!player.IsAdmin)
                return;

            string timerName = args.Length == 0 ? "" : args[0];

            if (string.IsNullOrEmpty(timerName))
                Puts($"The {Types.TimerOnce} timers has stopped");
            else
                Puts($"The {Types.TimerOnce} timer '{timerName}' has stopped");

            Timers.StopTimer(Types.TimerOnce, args.Length == 0 ? "" : args[0]);
        }

        [Command("stop.timerrepeat", "stoptimerrepeat")]
        private void CmdStopRepeat(IPlayer player, string cmd, string[] args)
        {
            if (!player.IsAdmin)
                return;

            Puts($"The {Types.Repeater} timers has stopped");
            Timers.StopTimer(Types.Repeater);
        }

        [Command("stop.timerrealtime", "stoptimerrealtime")]
        private void CmdStopRealTime(IPlayer player, string cmd, string[] args)
        {
            if (!player.IsAdmin)
                return;

            Puts($"The {Types.RealTime} timers has stopped");
            Timers.StopTimer(Types.RealTime);
        }

        [Command("stop.timeringame", "stoptimeringame")]
        private void CmdStopInGame(IPlayer player, string cmd, string[] args)
        {
            if (!player.IsAdmin)
                return;

            Puts($"The {Types.InGameTime} timers has stopped");
            Timers.StopTimer(Types.InGameTime);
        }
    }
}
