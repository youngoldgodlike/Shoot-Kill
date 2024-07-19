using System;
using System.Collections;
using Lean.Gui;
using UnityEngine;
using Zenject;

public class AdMenu : Menu<AdMenu>
{
   public event Action<float> onTickShow;

   [SerializeField] private GameObject _adPanel;
   [SerializeField] private LeanButton _button;
   
   [SerializeField] private float _showTimeAdPanel;
   [SerializeField] private float _preShowTimeAdPanel;
   
   private GameSession _gameSession;
   private IEnumerator _current;

   [Inject]
   private void Initialize(GameSession gameSession) =>
      _gameSession = gameSession;
   
   private void Start()
   {
      _adPanel.SetActive(false); 
      StartState(StartCountdownToShowAdButton());
   }

   public void OnClickAdButton()
   {
      _adPanel.SetActive(false); 
      
      StopCoroutine(_current);
      StartState(StartCountdownToShowAdButton());
   }

   private IEnumerator StartCountdownToShowAdButton()
   {
      var timer = _preShowTimeAdPanel;
      
      while (timer > 0)
      {
         yield return null;
         
         if (_gameSession.UIIsActive.Value)
            yield return new WaitUntil(() => !_gameSession.UIIsActive.Value);
         
         timer -= Time.deltaTime;
      }
      
      _adPanel.SetActive(true);
      StartState(ShowButton());
   }

   private IEnumerator ShowButton()
   {
      var timer = _showTimeAdPanel;
      
      while (timer > 0)
      {
         yield return null;

         onTickShow?.Invoke(timer);

         if (_gameSession.UIIsActive.Value)
         {
            _button.interactable = false;
            yield return new WaitUntil(() => !_gameSession.UIIsActive.Value);
            _button.interactable = true;
         }
         
         timer -= Time.deltaTime;
      }
      
      animator.SetTrigger(IsClose);
      StartState(StartCountdownToShowAdButton());

      yield return new WaitForSeconds(1f);
      
      _adPanel.SetActive(false);
   }
   
   private void StartState(IEnumerator coroutine)
   {
      _current = coroutine;
      StartCoroutine(coroutine);
   }
  
      
}
