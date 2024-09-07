using System.Collections.Generic;
using UnityEngine;

public class UpgradeData : MonoBehaviour
{
    [Tooltip("Place your object upgrades in this list, with 0 being the first upgrade")]

    [SerializeField] List<GameObject> upgradePrefabs;
    [SerializeField] List<int> upgradeTimes;    

    [Min(1)]
    [SerializeField] int currentUpgradeLevel = 1;

    public List<GameObject> UpgradePrefabs
    {
        get { return upgradePrefabs; }
        set { upgradePrefabs = value; }
    }
    public List<int> UpgradeTimes
    {
        get { return upgradeTimes; }
        set { upgradeTimes = value; }
    }
    public int CurrentUpgradeLevel
    {
        get { return currentUpgradeLevel; }
        set { currentUpgradeLevel = value; }
    }
}
