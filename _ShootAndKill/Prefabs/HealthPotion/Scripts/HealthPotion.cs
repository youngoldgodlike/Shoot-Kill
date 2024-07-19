using Assets.Prefabs.Characters.MainHero.Scripts;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private float _value = 10f;
    [SerializeField] private float _rotationSpeed = 10f;
    
    private void Update()
    {
        float currentRotation = transform.rotation.eulerAngles.y;
        
        currentRotation += _rotationSpeed * Time.deltaTime;
        
        transform.rotation = Quaternion.Euler(30 , currentRotation, transform.rotation.z);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HeroHealth health))
        {
            if (health.isFullHealth) return;
            
            health.Heal(_value);
            Destroy(gameObject);
        }
    }
}
