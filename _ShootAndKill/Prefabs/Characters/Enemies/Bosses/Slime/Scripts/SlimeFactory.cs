using System.Collections.Generic;
using _Shoot_Kill.Architecture.Scripts.EnemySpawn;
using Architecture.GameData.Configs;
using Cysharp.Threading.Tasks;
using Enemies.EnemiesVariety;
using Helpers.Debugging;
using NaughtyAttributes;
using R3;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class SlimeFactory : MonoBehaviour
{
    [SerializeField] private GameDebug _logger;
    [SerializeField] private EnemyConfig _slimeConfig;
    [SerializeField] private Transform _spawnPlace;
    [SerializeField, ReadOnly] private Transform _target;
    [SerializeField] private float _pushPower = 1f;

    private EnemyBuilder _builder = new();
    private Dictionary<Enemy, int> _enemies = new();
    private int _creatingIndex;

    [Inject]
    private void InjectInit(GameDebug debugger) {
        _logger = debugger;
    }
    
    public void Init(Transform player) {
        _target = player;
    }
    
    public void Create() {
        var enemy = _builder.Create(_slimeConfig)
            .WithTarget(_target)
            .Build();
        _creatingIndex++;
        _enemies.Add(enemy,_creatingIndex);

        //enemy.parent.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        _logger.Log(
            $"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>Created new slime. SpawnPlacePos: {_spawnPlace.position}</color>");
        //enemy.parent.transform.position = _spawnPlace.position;
        //enemy.transform.localPosition = Vector3.zero;
        _logger.Log(
            $"EnemyParentPos: {enemy.parent.position}. BodyPosWorld: {enemy.transform.position},BodyPosLocal: {enemy.transform.localPosition}");
        enemy.collidr.isTrigger = true;
        _logger.Log($"ParentPos After LocalScale Changed:{enemy.parent.transform.position}");
        enemy.rigibody.isKinematic = false;
        enemy.rigibody.useGravity = true;
        //if(enemy.motion is AnimationRootMotion motion) motion.DisableAnimatorMove();
        
        enemy.rigibody.AddForce(new Vector3(
            Random.Range(-1f, 1f),
            1.5f,
            Random.Range(-1f, 1f)) * _pushPower, ForceMode.VelocityChange);
        
        enemy.health.onDieProcessEnd.AddListener(() => {
            enemy.rigibody.useGravity = false;
            enemy.rigibody.isKinematic = true;
            Destroy(enemy.parent.gameObject);
        });

        PostAction(enemy).Forget();
    }

    private async UniTaskVoid PostAction(Enemy enemy) {
        await UniTask.Yield();
        enemy.parent.GetComponent<NavMeshAgent>().Warp(_spawnPlace.position);
        //enemy.parent.position = _spawnPlace.position;
        //enemy.parent.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        
        // while (enemy.parent.localScale.x < 1) {
        //     var time = Time.deltaTime / 2;
        //     enemy.parent.localScale += new Vector3(time, time, time);
        //
        //     await UniTask.Yield(_target.gameObject.GetCancellationTokenOnDestroy());
        // }
        
        await UniTask.WaitForSeconds(1f);

        //if (enemy.motion is AnimationRootMotion motion) motion.EnableAnimatorMove();
        enemy.collidr.isTrigger = false;
        _logger.Log($"<color=#{ColorUtility.ToHtmlStringRGB(Color.blue)}>{_enemies[enemy]} enemy finished</color> with {enemy.parent.position} pos");
    }
}
