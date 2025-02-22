using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.U2D;
using TMPro;


public class RewardManager : MonoBehaviour
{
    public GameObject rewardPrefab;  // Prefab for reward icons
    public Transform[] rewardSlots;  // Array of reward slots (set in Inspector)

    public SpriteAtlas rewardAtlas;  // SpriteAtlas for rewards

    public RewardCollection rewardCollection; // Collection of rewards

    public List<GameObject> createdRewards;

    void Start()
    {
        AssignRewards();
    }

    public void AssignRewards()
    {

        if (rewardCollection == null || rewardCollection.rewardNames.Count == 0) return;

        // Clear existing rewards
        foreach (Transform slot in rewardSlots)
        {
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);
        }

        // Clear the list of created rewards
        createdRewards = new List<GameObject>();

        // Assign new rewards
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            if (i >= rewardCollection.rewardNames.Count) break; // Prevent overflow

            string rewardName = rewardCollection.rewardNames[i];
            // Instantiate reward icon
            GameObject reward = Instantiate(rewardPrefab, rewardSlots[i]);
            reward.transform.localPosition = Vector3.zero; // Center inside slot
            reward.GetComponentInChildren<TextMeshProUGUI>().text = rewardCollection.rewardAmount[i];
            createdRewards.Add(reward);

            // Assign a random sprite
            Image rewardImage = reward.GetComponent<Image>();
            // Load the sprite from the atlas
            rewardImage.sprite = rewardAtlas.GetSprite(rewardName);

            // Dynamically resize reward to fit slot
            ResizeReward(rewardImage);
        }
    }

    void ResizeReward(Image rewardImage)
    {
        // Get original sprite size
        float width = rewardImage.sprite.rect.width;
        float height = rewardImage.sprite.rect.height;

        // Define a max size (prevents large rewards from breaking layout)
        float maxSize = 60f;  // Adjust as needed

        // Scale down proportionally
        float scaleFactor = maxSize / Mathf.Max(width, height);
        rewardImage.rectTransform.sizeDelta = new Vector2(width * scaleFactor, height * scaleFactor);
    }
}
