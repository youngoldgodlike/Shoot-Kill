using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using R3;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cysharp.Threading.Tasks.UniTask;
using Observable = R3.Observable;

namespace SpawnSystem.TestSpawner
{
    public class Vector2Slider : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private SliderDot _dot1;
        [SerializeField] private SliderDot _dot2;
        [SerializeField] private TextMeshProUGUI _minValueText, _maxValueText;
        [SerializeField] private RectTransform _slide;

        [Header("Properties"), Space] 
        [SerializeField] private SerializableReactiveProperty<float> _minValue = new();
        [SerializeField] private SerializableReactiveProperty<float> _maxValue = new();
        [SerializeField,] private float _defaultMin, _defaultMax;
        [SerializeField, ReadOnly] private float _dot1WorldStartPos;
        [SerializeField, ReadOnly] private float _dot2WorldStartPos;
        [SerializeField, ReadOnly] private RectTransform _dot1Rect,_dot2Rect;

        private CancellationTokenSource _dot1Cts = new(), _dot2Cts = new();
        private IDisposable _disposable;
        
        private float dotWidth => _dot1Rect.rect.width;
        private float slideWidth => _slide.rect.width;
        private float dotsWayRange => slideWidth - dotWidth;
        private float dot1XPos => _dot1Rect.position.x;
        private float dot2XPos => _dot2Rect.position.x;
        [ShowNativeProperty] private Vector2 dot1XPosRange => new(_dot1WorldStartPos, _dot1WorldStartPos + dotsWayRange);
        [ShowNativeProperty] private Vector2 dot2XPosRange => new(_dot2WorldStartPos, _dot2WorldStartPos + dotsWayRange);
        private static Mouse mouse => Mouse.current;

        private ReactiveProperty<Vector2> _value;
        public ReadOnlyReactiveProperty<Vector2> value => _value;

        private void Awake() {
            _value = new ReactiveProperty<Vector2>();
            _value.Value = new Vector2(_minValue.Value, _maxValue.Value);
            _value.Subscribe(_ => {
                _minValueText.text = $"{MathF.Round(_value.Value.x, 1)}";
                _maxValueText.text = $"{MathF.Round(_value.Value.y, 1)}";
            });

            _minValueText.text = $"{Mathf.Clamp(_defaultMin, _minValue.Value, _maxValue.Value)}";
            _maxValueText.text = $"{Mathf.Clamp(_defaultMax, _minValue.Value, _maxValue.Value)}";
            
            RefreshValue();
        }

        private void OnValidate() {
            _defaultMin = Mathf.Clamp(_defaultMin, _minValue.Value, _maxValue.Value);
            _defaultMax = Mathf.Clamp(_defaultMax, _minValue.Value, _maxValue.Value);
        }

        private void OnEnable() {
            var dot1HandTracker = _dot1.isHandled.Subscribe(x => {
                if (x) {
                    _dot1Cts = new CancellationTokenSource();
                    MoveDot(_dot1Rect, dot1XPosRange, _dot1Cts).Forget();
                } else _dot1Cts.Cancel();
            });
            var dot2HandTracker = _dot2.isHandled.Subscribe(x => {
                if (x) {
                    _dot2Cts = new CancellationTokenSource();
                    MoveDot(_dot2Rect, dot2XPosRange, _dot2Cts).Forget();
                } else _dot2Cts.Cancel();
            });
            
            var dot1PosTracker = Observable
                .EveryValueChanged(this, slider => slider.dot1XPos, UnityFrameProvider.Update)
                // .Do(_ => Debug.Log($"<color=red>Left Dot Moving</color>"))
                .Subscribe(x => RefreshValue());
            var dot2PosTracker = Observable
                .EveryValueChanged(this, slider => slider.dot2XPos, UnityFrameProvider.Update)
                // .Do(_ => Debug.Log($"<color=#FFAE00>Right Dot Moving</color>"))
                .Subscribe(x => RefreshValue());

            _disposable = Disposable.Combine(dot1PosTracker, dot2PosTracker, dot1HandTracker, dot2HandTracker);
        }

        private void OnDisable() {
            if(_disposable.IsUnityNull()) return;
            _disposable.Dispose();
        }

        private void RefreshValue() {
            ClampPositions();

            var absoluteRange = dot1XPosRange.y - dot1XPosRange.x;
            var valueDiff = Mathf.Abs(_maxValue.Value) - Mathf.Abs(_minValue.Value);
            var newValue = new Vector2();
            
            // Setting current Min & Max value
            var dot1RelativePos = DotRelativePos(absoluteRange, dot1XPos, dot1XPosRange);
            newValue.x = valueDiff * dot1RelativePos + _minValue.Value;

            var dot2RelativePos = DotRelativePos(absoluteRange, dot2XPos, dot2XPosRange);
            newValue.y = valueDiff * dot2RelativePos + _minValue.Value;

            _value.Value = newValue;
        }

        private float DotRelativePos(float absoluteRange, float dotXPos, Vector2 dotXPosRange) {
            var dot1XLocalPos = dotXPos - dotXPosRange.x;
            var dot1PositionDiff = absoluteRange - (absoluteRange - dot1XLocalPos);
            var dot1RelativePos = dot1PositionDiff / absoluteRange;
            return dot1RelativePos;
        }

        private void ClampPositions() {
            var dot2Pos = _dot2Rect.position;
            var dot1Pos = _dot1Rect.position;
            var dot1XPos = dot1Pos.x;
            var dot2XPos = dot2Pos.x;

            dot1XPos = Mathf.Clamp(dot1XPos, dot1XPosRange.x, dot1XPosRange.y);
            _dot1Rect.position = new Vector3(dot1XPos, dot1Pos.y, dot1Pos.z);
            dot2XPos = Mathf.Clamp(dot2XPos, _dot1Rect.position.x + dotWidth / 2, dot2XPosRange.y);
            _dot2Rect.position = new Vector3(dot2XPos, dot2Pos.y, dot2Pos.z);
        }

        private async UniTask MoveDot(RectTransform dot, Vector2 range, CancellationTokenSource cts) {
            while (true) {
                var position = dot.position;
                position = new Vector3(Mathf.Clamp(mouse.position.x.value, range.x, range.y), position.y, position.z);
                
                dot.position = position;
                
                await Yield(cts.Token);
            }
        }
        
        [Button("Hash Dots Position"),ContextMenu("Hash Dots Position")]
        private void HashDotsWorldPos() {
            _dot1Rect = _dot1.GetComponent<RectTransform>();
            _dot2Rect = _dot2.GetComponent<RectTransform>();
            
            
            _dot1Rect.localPosition = new Vector3(_slide.localPosition.x - slideWidth / 2, 0, 0);
            _dot2Rect.localPosition = new Vector3(_slide.localPosition.x - slideWidth / 2 + dotWidth / 2, 0, 0);
            
            _dot1WorldStartPos = _dot1Rect.position.x;
            _dot2WorldStartPos = _dot2Rect.position.x;
        }
    }
}