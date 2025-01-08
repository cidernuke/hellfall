using UnityEngine;
using UnityEngine.PlayerLoop;


public class WeaponStats : MonoBehaviour 
{


    [SerializeField] PlayerController playerController;

    // Modifiers in Percent
    [SerializeField] public bool isCloseRangeWeapon;
    [SerializeField] public float damageModifier = 0f;
    [SerializeField] public float cooldownModifier = 0f;
    [SerializeField] public float rangeModifier = 0f;

    public WeaponStats (bool isCloseRangeWeapon, float damageModifier = 0, float cooldownModifier = 0, float rangeModifier = 0)
    {
        this.isCloseRangeWeapon = isCloseRangeWeapon;
        this.damageModifier = damageModifier;
        this.cooldownModifier = cooldownModifier;
        this.rangeModifier = rangeModifier;
    }

    void Update()
    {
        if (playerController != null)
        {
            if(isCloseRangeWeapon)
            {
                if (damageModifier != playerController.playerStats.closeDamage.GetModifier()) 
                {
                    playerController.playerStats.closeDamage.SetModifier(damageModifier);
                }
                if (cooldownModifier != playerController.playerStats.closeCooldown.GetModifier()) 
                {
                    playerController.playerStats.closeCooldown.SetModifier(cooldownModifier);
                }
                if (rangeModifier != playerController.playerStats.closeRange.GetModifier()) 
                {
                    playerController.playerStats.closeRange.SetModifier(rangeModifier);
                }
            }
            
        }
    }


}