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
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject wheelImage;
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
        AnimateReward2(rewardManager.createdRewards[rewardIndex]);



    }

    public void CheckForState()
    {
        if (rewardCount % 5 == 0)
        {
            rewardManager.rewardCollection = silverCollection;
            rewardManager.AssignRewards();
            wheelImage.GetComponent<Image>().sprite = silverWheel;
        }
        else if (rewardCount % 30 == 0)
        {
            rewardManager.rewardCollection = goldCollection;
            rewardManager.AssignRewards();
            wheelImage.GetComponent<Image>().sprite = goldWheel;
        }
        else
        {
            rewardManager.rewardCollection = bronzeCollection;
            rewardManager.AssignRewards();
            wheelImage.GetComponent<Image>().sprite = bronzeWheel;
        }

    }
    private void UpdateCountText()
    {
        countText.text = "Gained Rewards : " + rewardCount;
    }


    public void AnimateReward2(GameObject reward)
    {
        // Create a copy of the reward
        GameObject copyReward = Instantiate(reward, reward.transform.position, Quaternion.identity);
        copyReward.transform.SetParent(rewardPanel.transform); // Set parent

        Vector3 startPos = reward.transform.position;
        Vector3 centerPos = new Vector3(960, 540, 0);
        Vector3 endPos = rewardPanel.transform.position;

        Vector3 startScale = copyReward.transform.localScale;
        Vector3 centerScale = startScale * scaleMultiplier;
        Vector3 endScale = startScale; // Back to original scale

        // Create a sequence
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        // Step 1: Move to center & scale up
        sequence.Append(copyReward.transform.DOMove(centerPos, moveDuration).SetEase(Ease.InOutQuad))
                .Join(copyReward.transform.DOScale(centerScale, moveDuration).SetEase(Ease.InOutQuad));

        // Step 2: Move to top (countText position) & scale down
        sequence.Append(copyReward.transform.DOMove(endPos, moveDuration / 2f).SetEase(Ease.InOutQuad))
                .Join(copyReward.transform.DOScale(endScale, moveDuration / 2f).SetEase(Ease.InOutQuad));

        // Step 3: Destroy the object and update count text
        sequence.OnComplete(() =>
        {
            //Destroy(copyReward);
            UpdateCountText();
        });

        // Play the sequence
        sequence.Play();
    }

    #region Reward Animation
    public void AnimateReward(GameObject reward)
    {
        StartCoroutine(AnimateRewardSequence(reward));
    }

    private IEnumerator AnimateRewardSequence(GameObject reward)
    {
        Vector3 startPos = reward.transform.position;
        Vector3 centerPos = new Vector3(960, 540, 0);
        Vector3 endPos = countText.transform.position;

        GameObject copyReward = Instantiate(reward, reward.transform.position, Quaternion.identity);
        copyReward.transform.SetParent(countText.transform);
        // Step 1: Move to Center & Scale Up
        yield return MoveAndScale(copyReward, startPos, centerPos, scaleMultiplier, moveDuration);

        // Step 2: Move to Top
        yield return MoveAndScale(copyReward, centerPos, endPos, 1f, moveDuration / 2f);

        Destroy(copyReward);
        UpdateCountText();

    }



    private IEnumerator MoveAndScale(GameObject reward, Vector3 startPos, Vector3 endPos, float targetScale, float duration)
    {
        float time = 0;
        Vector3 startScale = reward.transform.localScale;
        Vector3 endScale = startScale * targetScale;

        while (time < duration)
        {
            float t = time / duration;
            reward.transform.position = Vector3.Lerp(startPos, endPos, t);
            reward.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure final position & scale are exact
        reward.transform.position = endPos;
        reward.transform.localScale = endScale;
    }

    #endregion



}
