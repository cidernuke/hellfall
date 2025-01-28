using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public EndBossMain mainBoss;
    [SerializeField] private GameObject healthBarObject;
    [SerializeField] private BossHealthBar healthBarScript;
    private int maxHealth;
    
    // Start is called before the first frame update
    // void Start()
    // {
    //     if (GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().IsGrounded())
    //     {
    //         mainBoss.gameObject.SetActive(true);
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !mainBoss.didBossKillPlayer)
        {
            mainBoss.gameObject.SetActive(true);
            healthBarObject.SetActive(true);
            maxHealth = (int)mainBoss.GetComponent<HealthSystem>().startingHealth;
            // gameObject.SetActive(false);
        }
        else
        {
            mainBoss.didBossKillPlayer = false;
            mainBoss.gameObject.SetActive(true);
            maxHealth = (int)mainBoss.GetComponent<HealthSystem>().startingHealth;
            //! setting healthbar back to max life not working
            healthBarScript.SetMaxHealth((int)mainBoss.GetComponent<HealthSystem>().startingHealth);
            // healthBar.GetComponent<BossHealthBar>().SetMaxHealth((int)healthBar.GetComponent<BossHealthBar>().GetComponent<HealthSystem>().startingHealth);
            healthBarObject.SetActive(true);
        }
    }
}
