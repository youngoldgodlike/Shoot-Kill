using System;
using System.Linq;
using Characters;
using Cysharp.Threading.Tasks;
using Enemies.DmgInfliction;
using Enemies.EnemiesVariety;
using UnityEngine;

[RequireComponent(typeof(SlimeFactory))]
public class SlimeBossAttack : DamageInfliction
{
    [Header("Dependencies")] 
    [SerializeField] private Enemy _enemy;
    [SerializeField] private SlimeFactory _factory;
    [SerializeField] private ParticleSystem _attackParticle;
    [SerializeField] private Transform _vfxPoint;

    [Header("BossAttacks Params")]
    [SerializeField] private float _damageRadius;
    // [SerializeField] private bool _changeVFXRadius;
    // [SerializeField, ShowIf(nameof(_changeVFXRadius))] private float _vfxRadius;
    [SerializeField] private float _repulsiveTorque;
    [SerializeField, Min(1f)] private float _abilityCooldown;
    [SerializeField, Min(0.2f)] private float _spawnDelay = 0.5f;

    private static readonly int Ability = Animator.StringToHash("Ability");
    private bool _isDied = false;

    private void Start() {
        _factory.Init(target);
        AbilitySpam().Forget();
        _enemy.health.onPreDie.AddListener(() => _isDied = true);
    }

    public override void Assault() {
        if (Attack(out var targets, _damageRadius)) return;

        targets.ToList().ForEach(col => {
            try {
                col.attachedRigidbody.AddExplosionForce(_repulsiveTorque, _vfxPoint.position, _damageRadius, 1f,
                    ForceMode.Impulse);
                if (col.TryGetComponent<IDamageable>(out var dmg)) dmg.DealDamage(damageDeal);
            }
            catch (Exception e) {
                Debug.Log(e);
            }
        });
    }

    public void WeakJumpAttack() {
        if (Attack(out var targets, _damageRadius / 2)) return;

        targets.ToList().ForEach(col => {
            try {
                col.attachedRigidbody.AddExplosionForce(_repulsiveTorque, _vfxPoint.position, _damageRadius/2, 1f,
                    ForceMode.Impulse);
                if (col.TryGetComponent<IDamageable>(out var dmg))
                    dmg.DealDamage(new EnemyDamageDealer(damageDeal.damage / 2, gameObject));
            }
            catch (Exception e) {
                Debug.Log(e);
            }
        });
    }

    private bool Attack(out Collider[] targets,float radius) {
        InstantiateVfx(radius);
        
        targets = Physics.OverlapSphere(position, radius, targetMask);

        return targets.Length == 0;
    }

    private void InstantiateVfx(float radius) {
        var vfx = Instantiate(_attackParticle, _vfxPoint.position, Quaternion.identity);
        var emission = vfx.emission;
        var shape = vfx.shape;
        var main = vfx.main;

        shape.radius = radius;
        main.startSize = radius;

        emission.enabled = true;
        Destroy(vfx.gameObject, 5f);
    }
    
    private async UniTaskVoid AbilitySpam() {
        while (true) {
            await UniTask.WaitForSeconds(_abilityCooldown, cancellationToken: this.GetCancellationTokenOnDestroy());

            await AbilityUsing();
        }
    }

    [SerializeField] private int _enemiesCount = 10;
    private async UniTask AbilityUsing() {
        Debug.Log("AAAA БОСС АБИЛКУ ЮЗНУЛ!!!");
        animator.SetBool(Ability, true);
        
        var enemiesCount = 0;
        while (enemiesCount < _enemiesCount && !_isDied) {
            _factory.Create();
            enemiesCount++;

            await UniTask.WaitForSeconds(_spawnDelay, cancellationToken: destroyCancellationToken);
        }

        animator.SetBool(Ability, false);
    }
}
