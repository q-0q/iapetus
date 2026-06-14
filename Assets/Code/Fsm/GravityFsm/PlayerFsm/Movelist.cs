using System.Collections.Generic;

namespace Code.Fsm.GravityFsm.PlayerFsm
{
    public enum MovelistRegistrationType
    {
        Basic,
        Trick
    }
    public class MovelistRegistration
    {
        public MovelistRegistrationType MovelistRegistrationType;
        public string displayName;
        public string description;
        public int cost;
        public string lore;
    
        public string useInput;
        public string useClause;
    }

    public static class MovelistRegistry
    {
        public static readonly Dictionary<string, MovelistRegistration> MovelistRegistrations;
        public const string TrickColor = "D0C4FF";
    
        static MovelistRegistry()
        {
            MovelistRegistrations = new Dictionary<string, MovelistRegistration>();
        
            MovelistRegistrations.Add("Run", new MovelistRegistration()
            {
                MovelistRegistrationType = MovelistRegistrationType.Basic,
                displayName = "Run",
                description = "",
                lore = "",
                cost = 0,
                useInput = "Move",
                useClause = "while on the ground"
            });
            
            MovelistRegistrations.Add("Jump", new MovelistRegistration()
            {
                MovelistRegistrationType = MovelistRegistrationType.Basic,
                displayName = "Jump",
                description = "",
                lore = "",
                cost = 0,
                useInput = "Jump",
                useClause = "while on the ground"
            });
            
            MovelistRegistrations.Add("Sprint", new MovelistRegistration()
            {
                MovelistRegistrationType = MovelistRegistrationType.Basic,
                displayName = "Sprint",
                description = "",
                lore = "",
                cost = 0,
                useInput = "Sprint",
                useClause = "while moving"
            });
            
            MovelistRegistrations.Add("Dash", new MovelistRegistration()
            {
                MovelistRegistrationType = MovelistRegistrationType.Basic,
                displayName = "Dash",
                description = "A quick frontflip.",
                lore = "",
                cost = 0,
                useInput = "Sprint",
                useClause = "while in the air"
            });
            
            MovelistRegistrations.Add("Skip", new MovelistRegistration()
            {
                MovelistRegistrationType = MovelistRegistrationType.Basic,
                displayName = "Skip",
                description = "A spring-like bound that provides great distance and height.",
                lore = "",
                cost = 0,
                useInput = "Jump",
                useClause = "immediately after landing from Dash"
            });
            
            MovelistRegistrations.Add("Wallrun", new MovelistRegistration()
            {
                MovelistRegistrationType = MovelistRegistrationType.Basic,
                displayName = "Wallrun",
                description = "Run laterally alongside a wall, significantly slowing your descent.",
                lore = "",
                cost = 0,
                useInput = "Jump",
                useClause = "while Sprinting next to a wall"
            });
            
            
            MovelistRegistrations.Add("Tinsica", new MovelistRegistration()
            {
                MovelistRegistrationType = MovelistRegistrationType.Trick,
                displayName = "Tinsica",
                description = "A fast front cartwheel that crosses gaps and mounts ledges.",
                lore = "By expelling energy into their palms, Lotus Monks balance qi evenly across their bodies.",
                cost = 1,
                useInput = "Trick",
                useClause = "while on the ground"
            });
        
            MovelistRegistrations.Add("TinsicaJump", new MovelistRegistration()
            {
                MovelistRegistrationType = MovelistRegistrationType.Trick,
                displayName = "Tinsica Jump",
                description = "A floating frontflip that travels far.",
                lore = "Motion is the plucking of a string.",
                cost = 1,
                useInput = "Jump",
                useClause = "while in a Tinsica"
            });
            
        }
    }
    
}