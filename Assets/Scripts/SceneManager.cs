using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RewardManager rewardManager;
    [SerializeField] private RewardPanelManager rewardPanelManager;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject wheel;
    [SerializeField] private GameObject wheelImageBase;
    [SerializeField] private GameObject needleImage;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject rewardPanel;

    [Header("Reward Collections")]
    [SerializeField] private RewardCollection bronzeCollection;
    [SerializeField] private RewardCollection silverCollection;
    [SerializeField] private RewardCollection goldCollection;

    [Header("Wheel Sprites")]
    [SerializeField] private Sprite bronzeWheel;
    [SerializeField] private Sprite silverWheel;
    [SerializeField] private Sprite goldWheel;

    [Header("Niddle Sprites")]
    [SerializeField] private Sprite bronzeNeedle;
    [SerializeField] private Sprite silverNeedle;
    [SerializeField] private Sprite goldNeedle;

    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button giveUpButton;

    [SerializeField] private float moveDuration = 1.5f; // Duration of movement

    private int rewardCount = 0;
    private float scaleMultiplier = 5f; // How big the reward gets before moving up

    public float MoveDuration { get { return moveDuration; }}

    // Start is called before the first frame update
    void Start()
    {
        retryButton.onClick.AddListener(() => Retry());
        giveUpButton.onClick.AddListener(() => GiveUp());
    }

    void Retry()
    {
        failPanel.SetActive(false);
        //Clear children of reward panel
        foreach (Transform child in rewardPanel.transform)
        {
            Destroy(child.gameObject);
        }
        rewardCount = 0;
        UpdateCountText();
    }

    void GiveUp()
    {
        Application.Quit();
    }

    public void DetectWinningReward(double angle)
    {
        angle = (angle + 22.5f) % 360; // Offset to align with reward slices
        int rewardIndex = (int)(angle / 45f); // Assuming 8 slices of 45° each
        Debug.Log("Reward Index: " + rewardIndex);

        if (rewardIndex == rewardManager.rewardCollection.bombIndex)
        {
            failPanel.SetActive(true);
            return;
        }
        rewardCount++;
        //AnimateReward(rewardManager.createdRewards[rewardIndex]);
        AnimateReward(rewardManager.createdRewards[rewardIndex]);



    }

    public void CheckForState()
    {
        if (rewardCount % 5 == 0 && rewardCount != 0)
        {
            AnimateZoneTransition(silverCollection, silverWheel,silverNeedle);

        }
        else if (rewardCount % 30 == 0 && rewardCount != 0)
        {
            AnimateZoneTransition(goldCollection, goldWheel,goldNeedle);
        }
        else if (rewardCount % 5 == 1 && rewardCount != 1)
        {
            AnimateZoneTransition(bronzeCollection, bronzeWheel,bronzeNeedle);
        }

    }

    private void AnimateZoneTransition(RewardCollection collection, Sprite wheelSprite,Sprite needleSprite)
    {
        wheel.transform.DOScale(Vector3.zero, 1.5f).OnComplete(() =>
        {
            rewardManager.rewardCollection = collection;
            rewardManager.AssignRewards();
            wheelImageBase.GetComponent<Image>().sprite = wheelSprite;
            needleImage.GetComponent<Image>().sprite = needleSprite;
            wheel.transform.DOScale(Vector3.one, 1.5f);

        });
    }
    private void UpdateCountText()
    {
        countText.text = rewardCount.ToString();
    }


    public void AnimateReward(GameObject reward)
    {
        // Create a copy of the reward
        GameObject copyReward = Instantiate(reward, reward.transform.position, Quaternion.identity);
        copyReward.transform.SetParent(reward.transform.parent); // Set parent

        Vector3 startPos = reward.transform.position;

        // Calculate the center position of the rewardPanel
        RectTransform rewardPanelRect = rewardManager.GetComponent<RectTransform>();
        Vector3 centerPos = rewardPanelRect.TransformPoint(rewardPanelRect.rect.center);

        Vector3 endPos = rewardPanel.transform.position;

        Vector3 startScale = Vector3.one;
        Vector3 centerScale = startScale * scaleMultiplier;
        Vector3 endScale = startScale * 2.0f; // Back to original scale

        // Create a sequence
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        // Step 1: Move to center & scale up
        sequence.Append(copyReward.transform.DOMove(centerPos, moveDuration).SetEase(Ease.InOutQuad))
                .Join(copyReward.transform.DOScale(centerScale, moveDuration).SetEase(Ease.InOutQuad));

        // Step 2: Move to top (countText position) & scale down
        sequence.Append(copyReward.transform.DOMove(endPos, moveDuration / 2f ).SetEase(Ease.InOutQuad))
                .Join(copyReward.transform.DOScale(endScale, moveDuration / 2f).SetEase(Ease.InOutQuad));

        // Step 3: Destroy the object and update count text
        sequence.OnComplete(() =>
        {
            //Destroy(copyReward);
            UpdateCountText();
            rewardPanelManager.AddReward(copyReward);
            CheckForState();
        });

        // Play the sequence
        sequence.Play();
    }
}
