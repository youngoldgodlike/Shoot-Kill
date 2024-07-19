using System.Collections.Generic;
using _Shoot_Kill.UI.Prefabs.Menu.SelectionMenu;
using Cysharp.Threading.Tasks;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Vector3 = UnityEngine.Vector3;

public class SelectionMenuController :  Menu<SelectionMenuController>
{
    [SerializeField] private WeaponsData _weaponsData;
    [SerializeField] private GameObject _settingMenu;
    [SerializeField] private LeanButton _leftNextArrow;
    [SerializeField] private LeanButton _rightNextArrow;
    [SerializeField] private ActiveButton _confirmButton;
    [SerializeField] private SelectionCell _selectionCellPrefab;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Transform _podiumPosition;
    
    [SerializeField] private float _timeToScroll = 0.3f;

    private List<SelectionCell> _selectionCells = new();
    private SelectionCell _currentSelectionCell;
    private SelectionCell _previousCell;
    private int _currentScrollCount = 0;
    private GameSession _gameSession;

    [Inject]
    private void Initialize(GameSession gameSession) =>
        _gameSession = gameSession;
    
    protected override void Awake()
    {
        base.Awake();
        
        foreach (var data in _weaponsData.weapons)
        {
            var cell = Instantiate(_selectionCellPrefab, _scrollRect.content);
            cell.Init(data, _podiumPosition);
            
            cell.model.SetActive(false);
            
            _selectionCells.Add(cell);
        }
        
        SetFirstWeapon().Forget();
        CheckActiveNextArrows();
    }

    public void SetWeapon(int count)
    {
        _currentScrollCount += count;
        
        _previousCell = _currentSelectionCell;
        _previousCell.model.SetActive(false);
        
        SetCurrentLot();
        
        CheckActiveNextArrows();
        GoToNextWeapon().Forget();
    }

    private async UniTaskVoid GoToNextWeapon()
    {
        var nextPosition = Mathf.Clamp01((float) _currentScrollCount / (_selectionCells.Count - 1));
        var currentPosition = _scrollRect.horizontalNormalizedPosition;
        float currentTime = 0;
        
        while (currentTime <=  _timeToScroll)
        {
            currentTime += Time.deltaTime;

            _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(currentPosition, nextPosition,  currentTime / _timeToScroll);
            
            _currentSelectionCell.transform.localScale = Vector3.Lerp(
                new Vector3(0.8f, 0.8f, 1f),
                new Vector3(1.2f, 1.2f, 1),
                currentTime / _timeToScroll );
            
            _previousCell.transform.localScale = Vector3.Lerp(
                new Vector3(1.2f, 1.2f, 1),
                new Vector3(0.8f, 0.8f, 1f),
                currentTime / _timeToScroll );
           
            await UniTask.Yield();
        }
    }

    private async UniTaskVoid SetFirstWeapon()
    {
        SetCurrentLot();
        
        float currentTime = 0;

        while (currentTime <= _timeToScroll)
        {
            currentTime += Time.deltaTime;
            
            _currentSelectionCell.transform.localScale = Vector3.Lerp(
                new Vector3(0.8f, 0.8f, 1f), new Vector3(1.2f, 1.2f, 1), currentTime / _timeToScroll );
            
            await UniTask.Yield();
        }
    }

    private void SetCurrentLot()
    {
        _currentSelectionCell = _selectionCells[_currentScrollCount];
        _currentSelectionCell.model.SetActive(true);
        
        _confirmButton.SetActive(!_currentSelectionCell.isLock);
    }

    private void CheckActiveNextArrows()
    {
         _leftNextArrow.gameObject.SetActive(_currentScrollCount != 0);
         _rightNextArrow.gameObject.SetActive(_currentScrollCount != _selectionCells.Count - 1);
    }

    public void Confirm() =>
        _gameSession.matchData.SelectGun(_currentSelectionCell.data);

    public void OpenSetting()
    {
        _settingMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
