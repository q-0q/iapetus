using System.Collections.Generic;

namespace Code.Managers
{

    public class CompletionProfile
    {
        public List<string> bells;
        public List<string> lemons;
        public List<string> trials;
    }
    
    public class CompletionSystem
    {
        public static readonly Dictionary<string, CompletionProfile> CompletionProfiles =
            new Dictionary<string, CompletionProfile>()
            {
                ["c1"] = new CompletionProfile()
                {
                    bells = new List<string>() { "c1-piton-upper", "c1-cave" },
                    lemons = new List<string>() { "c1-entrance", "c1-cave-outer", "c1-little-tower", "c1-building", "c1-piton-bramble", "c1-cave-entrance", "c1-cave-pillars" }, 
                    trials = new List<string>() { "c1-piton-outskirts", "c1-piton-temple-cogworks", "c1-cave-slide" }
                }
            };
    }
}