using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardCollection", menuName = "Wheel/Reward Collection")]
public class RewardCollection : ScriptableObject
{
    public int bombIndex = 0; // Index of the bomb 

    public List<string> rewardNames; // List of rewards

    public List<string> rewardAmount; // List of amounts
}
