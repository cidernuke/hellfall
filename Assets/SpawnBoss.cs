using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    public EndBossMain mainBoss;
    [SerializeField] private GameObject healthBarObject;
    [SerializeField] private BossHealthBar healthBarScript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !mainBoss.didBossKillPlayer)
        {
            mainBoss.gameObject.SetActive(true);
            healthBarObject.SetActive(true);
            // gameObject.SetActive(false);
        }
        else
        {
            mainBoss.didBossKillPlayer = false;
            mainBoss.gameObject.SetActive(true);
            if ((int)mainBoss.GetComponent<HealthSystem>().startingHealth > (int)mainBoss.GetComponent<HealthSystem>().currentHealth)
            {
                healthBarScript.SetMaxHealth((int)mainBoss.GetComponent<HealthSystem>().startingHealth);
                mainBoss.GetComponent<HealthSystem>().currentHealth = mainBoss.GetComponent<HealthSystem>().startingHealth;
            }
            healthBarObject.SetActive(true);
        }
    }
}
