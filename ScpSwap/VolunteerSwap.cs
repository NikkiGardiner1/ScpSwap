/*using Exiled.API.Features;
using PlayerRoles;

namespace ScpSwap
{
    public class VolunteerSwap
    {
        public void ScpLeft(Player scpPlayer)
        {
            if (scpPlayer.IsScp && scpPlayer.Role != RoleTypeId.Scp0492)
            {
                if (Round.ElapsedTime.TotalSeconds > Plugin.Instance.Config.SwapTimeout)
                {
                    response = "The swap period has ended.";
                    return;
                }

                if (scpPlayer.Health < Plugin.Instance.Config.RequiredHealthPercent / 100 * scpPlayer.MaxHealth)
                {
                    response = "You cannot swap as you dont have enough health";
                    return;
                }
            }
        }
    }
}*/
