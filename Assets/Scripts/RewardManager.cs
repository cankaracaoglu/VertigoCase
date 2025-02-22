using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.U2D;


public class RewardManager : MonoBehaviour
{
    public GameObject rewardPrefab;  // Prefab for reward icons
    public Transform[] rewardSlots;  // Array of reward slots (set in Inspector)
    //public Sprite[] rewardSprites;   // Different reward icons

    public SpriteAtlas rewardAtlas;  // SpriteAtlas for rewards
    public string[] rewardSpriteNames;     // Names of sprites in atlas

    void Start()
    {
        AssignRewards();
    }

    void AssignRewards()
    {
        // Clear existing rewards
        foreach (Transform slot in rewardSlots)
        {
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);
        }

        // Assign new rewards
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            // Instantiate reward icon
            GameObject reward = Instantiate(rewardPrefab, rewardSlots[i]);
            reward.transform.localPosition = Vector3.zero; // Center inside slot

            // Assign a random sprite
            Image rewardImage = reward.GetComponent<Image>();
            // Pick a random sprite name from the atlas
            string spriteName = rewardSpriteNames[i];

            // Load the sprite from the atlas
            rewardImage.sprite = rewardAtlas.GetSprite(spriteName);

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
