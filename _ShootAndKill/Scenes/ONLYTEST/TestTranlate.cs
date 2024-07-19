using UnityEngine;

public class TestTranlate : MonoBehaviour
{
   [SerializeField]private float _speed = 10f;

    void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
}
