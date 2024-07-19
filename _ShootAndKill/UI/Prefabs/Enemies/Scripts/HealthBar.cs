using System;
using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.HealthVariety;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] protected Health _hp;
        [SerializeField] protected GameObject _background;
        [SerializeField] protected Image _hpCurrent, _hpBelated;

        private IDisposable _disposable;
        
        protected virtual void Awake() {
            _background.SetActive(false);
            _hpCurrent.fillAmount = 1f;
            _hpBelated.fillAmount = 1f;

            _hp.onHealthChange.AddListener(ChangeView);
            CreateStackTrace();
        }

        private void OnEnable() => _hpBelated.fillAmount = 1f;

        private void CreateStackTrace() {
            Observable.EveryUpdate(destroyCancellationToken)
                .Where(_ => _hpBelated.fillAmount - _hpCurrent.fillAmount > 0)
                .Subscribe(_ => {
                    var changeSpeed = Mathf.Clamp(_hpBelated.fillAmount - _hpCurrent.fillAmount, 0.5f, 1f);
                    _hpBelated.fillAmount -= changeSpeed * Time.deltaTime;
                });
        }

        private void ChangeView() {
            _background.SetActive(_hp.health < _hp.maxHealth && _hp.health != 0);
            _hpCurrent.fillAmount = _hp.health / _hp.maxHealth;
        }
    }
}