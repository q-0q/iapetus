using Code.Misc;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrackSelectedUI : MonoBehaviour
{
    private GameObject lastSelected;

    void Update()
    {
        // Check if the EventSystem exists in the scene
        if (EventSystem.current != null)
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            // If the selection has changed, log it
            if (currentSelected != lastSelected)
            {
                if (currentSelected != null)
                {
                    Debug.Log($"<color=cyan>[UI Selection]</color> Changed to: <b>{currentSelected.transform.GetPath()}</b>", currentSelected);
                }
                else
                {
                    Debug.Log("<color=orange>[UI Selection]</color> Changed to: <b>None</b>");
                }

                // Update the tracker
                lastSelected = currentSelected;
            }
        }
    }
}