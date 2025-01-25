using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public EndBossMain mainBoss;
    // Start is called before the first frame update
    void Start()
    {
        mainBoss.gameObject.SetActive(true);
    }
}
