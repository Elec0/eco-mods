using Eco.Core.Tests;
using Eco.Gameplay.Players;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Systems.Chat;
using Eco.Shared.Localization;
using Eco.Shared.Networking;
using System;

namespace Eco.Mods
{
    public class RespecCommands : IChatCommandHandler
    {
        [ChatSubCommand("Skills", "Respec a given user", ChatAuthorizationLevel.Admin)]
        public static void RespecUser(User user, User targetUser)
        {
            foreach(Skill s in targetUser.Skillset.Skills)
            {
                // Skills/Skilltrees can be containers for other skills, (i.e. professions) so only set the ones that are not
                // Abandoning a spec does not un-discover it
                if (!s.IsRoot)
                {
                    if (s.Name.Equals("SelfImprovementSkill"))
                        s.Level = 1;
                    else
                        s.AbandonSpecialty(targetUser.Player);
                }
            }

            targetUser.Player.Client.RPCAsync<bool>("PopupConfirmBox", targetUser.Player.Client, Localizer.Format("Your skills have been respecced. Please disconnect and reconnect immediately."));
        }

        [ChatSubCommand("Skills", "Set a given user's (you if not specified) level.", ChatAuthorizationLevel.Admin)]
        public static void SetLevel(User user, int points, User targetUser = null)
        {
            User toSet = targetUser == null ? user : targetUser;
            if(toSet.Skillset.SpecialtyCount > points)
            {
                user.Player.MsgLocStr("Level set to less than user's current SpecialtyCount.");
            }
            toSet.SpecialtyPoints = points;

            user.Player.Msg(Localizer.Format("Set player {0} level to {1}", toSet.Name, points));
        }

        [ChatSubCommand("Skills", "Add the amount of skill points to the given skill for yourself or a given player.", ChatAuthorizationLevel.Admin)]
        public static void LevelUpSkill(User user, string skillName, float skillPoints, User targetUser = null)
        {
            User toSet = targetUser == null ? user : targetUser;

            Skill foundSkill = null;
            foreach (Skill s in toSet.Skillset.Skills)
            {
                if (!s.IsRoot && s.Name.ToLower().Contains(skillName.ToLower()))
                {
                    foundSkill = s;
                    break;
                }
            }
            if (foundSkill == null)
            {
                user.Player.Error(Localizer.Format("Skill {0} not found.", skillName));
                return;
            }
            toSet.Skillset.AddExperience(foundSkill.Type, skillPoints, Localizer.DoStr("dev command"));
            user.Player.Msg(Localizer.Format("Added {0} to skill {1}.", skillPoints, foundSkill.Name));
        }
    }

}
