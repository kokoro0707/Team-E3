using UnityEngine;

[System.Serializable]
public class StageData
{
    public int enemyCount;
    public GameObject[] spawners;

    public void ActivateSpawners()
    {
        foreach (var spawner in spawners)
        {
            spawner.SetActive(true);
        }
    }

    public void DeactivateSpawners()
    {
        foreach (var spawner in spawners)
        {
            spawner.SetActive(false);
        }
    }
}