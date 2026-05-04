using System;
using UnityEngine;

public class CultistCamp : MonoBehaviour
{
    public int campId;

    private void Awake()
    {

        var advancing = transform.Find("Advancing");
        var refuse = transform.Find("Refuse");
        var saveCampId = SaveSystem.GetCultLocationCampId();


        if (saveCampId < campId)
        {
            advancing.gameObject.SetActive(false);
            refuse.gameObject.SetActive(false);
        } else if (saveCampId > campId)
        {
            advancing.gameObject.SetActive(false);
            foreach (var campfire in refuse.GetComponentsInChildren<CultCampFire>())
            {
                campfire.transform.Find("ParticlesParent").gameObject.SetActive(false);
                campfire.transform.Find("Light").gameObject.SetActive(false);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
