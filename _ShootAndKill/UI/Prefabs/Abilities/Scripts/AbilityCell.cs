using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class AbilityCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [field: SerializeField] public AbilityTag id { get; protected set; }
        [field: SerializeField] public Image currentDefaultIcon { get; private set; }
        [field: SerializeField] public Image outliningImage{ get; private set; }
        [SerializeField] private Image _currentSelectedIcon;
        
        [SerializeField] protected TextMeshProUGUI text;

        [NonSerialized] public UnityEvent onClick = new();
        [NonSerialized] public UnityEvent<string> onPointerEnter = new();
        [NonSerialized] public UnityEvent onPointerExit = new();
        [NonSerialized] public UnityEvent<AbilityCell> OnUpgrade = new();


        public AbilityRarity rarity { get; protected set; }

        private AbilityRarityDataScriptableObject _abilityRarityData;

        [Space]
        
        [SerializeField, TextArea] private string _description;

        protected GameSession gameSession;

        protected virtual void Awake()
        {
            SetText();
        }

        public void Initialize(GameSession gameSession)
        {
            this.gameSession = gameSession;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                onClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke(_description);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke();
        }

        public virtual void SetRarity(AbilityRarity rarity, AbilityRarityDataScriptableObject rarityData)
        {
            _abilityRarityData = rarityData;
            
            this.rarity = rarity;
            
            foreach (var ability in _abilityRarityData.abilityRarityDatas)
            {
                 if (ability.rarity != rarity) continue;

                outliningImage.sprite = ability.outliningImage;
            }
        }
        
        public void Hide()
        {
            currentDefaultIcon.gameObject.SetActive(true);
            _currentSelectedIcon.gameObject.SetActive(false);
        }

        public void Show()
        {
            currentDefaultIcon.gameObject.SetActive(false);
            _currentSelectedIcon.gameObject.SetActive(true);
        }

        public virtual void Upgrade()
        {
           OnUpgrade?.Invoke(this);
        }

        protected virtual void SetText()
        {
            throw new Exception("not override");
        }
    }
}
