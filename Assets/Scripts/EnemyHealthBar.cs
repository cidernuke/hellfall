using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform transform;



    public void updateHealthBar(float currentHealth, float maxHealth)
    {
        slider.value = currentHealth / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraRotation = camera.transform.rotation.eulerAngles;        
        transform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
        
    }
}
