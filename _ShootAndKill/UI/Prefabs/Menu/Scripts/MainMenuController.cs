using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string _nameLoadScene;
    [SerializeField] private GameObject _selectionMenu;
    [SerializeField] private GameObject _tutorialPanel;
    
    private GameSession _gameSession;

    [Inject]
    private void Initialize(GameSession gameSession) =>
        _gameSession = gameSession;
    
    public void StartGame()
    {
        SelectionMenuController.Instance.Confirm();
        _gameSession.UIIsActive.Value = false;
        SceneManager.LoadScene(_nameLoadScene);
    }

    public void ShowTutorial()
    {
        _selectionMenu.SetActive(false);
        _tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        _tutorialPanel.SetActive(false);
        _selectionMenu.SetActive(true);
    }
}
