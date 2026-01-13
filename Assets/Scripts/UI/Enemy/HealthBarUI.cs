using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public InterfaceReference<IUsesHealth> health;
    public InterfaceReference<IUsesShields> shields;
    [SerializeField] private Image healthBar;
    private void Start()
    {
        health.Value.HealthSystem.OnHealthChanged += health =>
        {
            gameObject.SetActive(true);
            healthBar.fillAmount = (float)health / this.health.Value.HealthSystem.MaxHealth;
            if(health <= 0)
                gameObject.SetActive(false);
            
        };
        shields.Value.OnShieldCountChanged += count =>
        {
            gameObject.SetActive(count <= 0);
        };
        gameObject.SetActive(false);
    }
}
