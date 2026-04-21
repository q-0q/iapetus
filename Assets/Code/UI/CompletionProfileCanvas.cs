using System.Collections;
using System.Collections.Generic;
using Code.Managers;
using TMPro;
using UnityEngine;

public class CompletionProfileCanvas : MonoBehaviour
{
    
    public Color completionColor = Color.green;

    public TextMeshProUGUI bellTmp;
    public TextMeshProUGUI lemonTmp;
    public TextMeshProUGUI trialTmp;
    public TextMeshProUGUI totalTmp;
    
    private const int bellPoints = 8;
    private const int lemonPoints = 2;
    private const int trialPoints = 4;

    public void UpdateCompletionProfile(string profileName)
    {
        var profile = CompletionSystem.CompletionProfiles[profileName];
        var saveData = SaveSystem.LoadCachedSaveData();

        var maxBells = 0;
        var playerBells = 0;
        
        var maxLemons = 0;
        var playerLemons = 0;
        
        var maxTrials = 0;
        var playerTrials = 0;

        foreach (var bell in profile.bells)
        {
            maxBells++;
            if (SaveSystem.GetBell(bell)) playerBells++;
        }
        
        foreach (var lemon in profile.lemons)
        {
            maxLemons++;
            if (SaveSystem.GetLemonCollection(lemon)) playerLemons++;
        }
        
        foreach (var trial in profile.trials)
        {
            maxTrials++;
            if (SaveSystem.GetTrialGolded(trial)) playerTrials++;
        }

        bellTmp.text = playerBells.ToString() + " / " + maxBells.ToString();
        lemonTmp.text = playerLemons.ToString() + " / " + maxLemons.ToString();
        trialTmp.text = playerTrials.ToString() + " / " + maxTrials.ToString();

        if (playerBells == maxBells) bellTmp.color = completionColor;
        if (playerLemons == maxLemons) lemonTmp.color = completionColor;
        if (playerTrials == maxTrials) trialTmp.color = completionColor;

        var pointsNum = (playerBells * bellPoints + playerLemons * lemonPoints + playerTrials * trialPoints);
        var pointsDenom = (maxBells * bellPoints + maxLemons * lemonPoints + maxTrials * trialPoints);
        var totalPercentage = pointsDenom > 0 ? (float)pointsNum / (float)pointsDenom : 0;
        totalTmp.text = Mathf.Floor(totalPercentage * 100f) + "%";
        if (totalTmp.text == "100%") totalTmp.color = completionColor;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
