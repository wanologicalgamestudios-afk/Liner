using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDoneUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> spriteDisplayObjects = new List<GameObject>();
    [SerializeField] private float waitingTimeAnimation;

    void Start()
    {
        ActivateRandomSpriteObject();
        Invoke(nameof(ActiveLevelCompletePanel), waitingTimeAnimation);
    }

    private void ActiveLevelCompletePanel()
    {
        UIManager.GetInstance().BackButtonIsPressed();
        UIManager.GetInstance().SpawnNextPanel(nameof(LevelCompleteUI), false);
    }


    public void ActivateRandomSpriteObject()
    {
        int randomIndex = Random.Range(0, spriteDisplayObjects.Count);

        for (int i = 0; i < spriteDisplayObjects.Count; i++)
        {
            if (spriteDisplayObjects[i] != null)
            {
                if (i == randomIndex)
                {
                    spriteDisplayObjects[i].SetActive(true);

            
                }
                else
                {
                    spriteDisplayObjects[i].SetActive(false);
                }
            }
        }

        Debug.Log("Activated sprite at index: " + randomIndex);
    }
}
