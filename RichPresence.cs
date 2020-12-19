using System;
using CellMenu;
using Discord;
using GTFO_DRP;
using GTFO_Rich_Presence;
using MelonLoader;
using SNetwork;

[assembly: MelonInfo(typeof(RichPresence), "GTFO Rich Presence", "1.0.0", "LapinoLapidus#3262")]
[assembly: MelonGame("10 Chambers Collective", "GTFO")]
namespace GTFO_Rich_Presence
{
    public class RichPresence : MelonMod
    {
        private static Discord.Discord _discord;
        private static ActivityManager _activityManager;

        private static pActiveExpedition _expPackage;
        private static DateTime _currentTime;

        private static long _clientId = 764433332330037249L;
        
        private static string _matchId = Guid.NewGuid().ToString();
        private static string _secret = Guid.NewGuid().ToString();
        
        public static Activity defaultActivity = new Activity()
        {
            State = "Playing GTFO",
            Details = "Selecting an expedition.",
            // Assets =
            // {
            //    LargeImage = "logo"
            // }
        };

        public override void OnApplicationStart()
        {
            _discord = new Discord.Discord(_clientId, 0L);
            _activityManager = _discord.GetActivityManager();
            _activityManager.RegisterSteam(493520);

            _currentTime = _currentTime.AddSeconds(2);

            _activityManager.OnActivityJoin += Events.OnActivityJoin;

            MelonLogger.Log("RichPresence created.");
            
            SetActivity(defaultActivity);
        }

        public override void OnUpdate()
        {
            _discord.RunCallbacks();
            if (_currentTime <= DateTime.Now && SNet.IsInLobby)
            {
                _expPackage = RundownManager.GetActiveExpeditionData();

                SetActivity(GetActivity());
                // Recheck status every 2 seconds.
                _currentTime = DateTime.Now.AddSeconds(2);
            }
        }

        public static Activity GetActivity()
        {
            return new Activity()
            {
                State = "Playing GTFO",
                Details = (Utility.IsInExpedition() ? "In lobby: " : "In the darkness: ") +
                          RundownManager.ActiveExpedition.Descriptive.Prefix +
                          (_expPackage.expeditionIndex + 1) + " " +
                          RundownManager.ActiveExpedition.Descriptive.PublicName,
                Party =
                {
                    Id = _matchId,
                    Size =
                    {
                        CurrentSize = SNet.LobbyPlayers.Count,
                        MaxSize = SNet.LobbyPlayers.Capacity
                    }
                },
                Secrets =
                {
                    Match = _secret,
                    Join = SNet.Lobby.Identifier.ID.ToString(),
                    Spectate = "null"
                }, Timestamps =
                {
                    Start = !Utility.IsInExpedition()
                        ? (long) (DateTime.UtcNow.AddSeconds(-Clock.ExpeditionProgressionTime) -
                                  new DateTime(1970, 1, 1)).TotalSeconds
                        : 0
                },
                // Assets =
                // {
                //     LargeImage = "logo"
                // }
            };
        }

        public static void SetActivity(Activity activity)
        {
            _activityManager.UpdateActivity(activity, (result =>
            {
                if (result != Result.Ok)
                    MelonLogger.Log("Failed: " + result);
            }));
        }
    }
}