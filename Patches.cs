using Harmony;
using SNetwork;

namespace GTFO_Rich_Presence
{
    public class Patches
    {
        [HarmonyPatch(typeof(SNet_LobbyManager), "LeaveLobby")]
        class SNet_LobbyManager_LeaveLobby
        {
            private static void Postfix()
            {
                RichPresence.SetActivity(RichPresence.defaultActivity);
            }
        }
    }
}