using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RewardPanelManager : MonoBehaviour
{
    public Transform rewardPanel; // The panel holding reward items
    private List<GameObject> rewardsList = new List<GameObject>(); // List of rewards
    private int maxItems = 10; // Maximum number of rewards

    public void AddReward(GameObject rewardInstance)
    {
        // Create a copy of the reward and add it to the panel
        //GameObject rewardInstance = Instantiate(newReward, rewardPanel);

        // If more than max items, remove the first one and shift others
        if (rewardsList.Count == maxItems)
        {
            RemoveFirstReward();
        }

        // Add to list
        rewardsList.Add(rewardInstance);

        rewardInstance.transform.SetParent(rewardPanel);
        

        
        }

    private void RemoveFirstReward()
    {
        if (rewardsList.Count > 0)
        {
            GameObject firstReward = rewardsList[0];

            // Remove from list
            rewardsList.RemoveAt(0);

            // Check if the object is still valid before animating it
            if (firstReward != null)
            {
                // Animate fading out before destroying
                firstReward.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
                {
                    // Check again before destroying in case it has been destroyed
                    if (firstReward != null)
                    {
                        Destroy(firstReward);
                    }
                });
            }
        }
    }

}
