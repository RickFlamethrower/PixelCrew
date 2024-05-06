using System;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimation : MonoBehaviour
    {
        [SerializeField] private int _frameRate; // фремрейт
        [SerializeField] private UnityEvent<string> _onComplete;       // ивент когда все закончится
        [SerializeField] private AnimationClip[] _clips;

        private SpriteRenderer _renderer;  

        private float _secondsPerFrame;  // время на показ одного спрайта
        private int _currentFrame;    // идекс текущего спрайта
        private float _nextFrameTime;   // врем до следующего апдейта
        private bool _isPlaying;

        private int _currentClip;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _secondsPerFrame = 1f / _frameRate;

            StartAnimation();
        }

        private void OnBecameVisible()
        {
            enabled = _isPlaying;
        }

        private void OnBecameInvisible()
        {
            enabled = false;
        }

        public void SetClip(string clipName)
        {
            for (var i = 0; i < _clips.Length; i++)
            {
                if (_clips[i].Name == clipName)
                {
                    _currentClip = i;
                    StartAnimation();
                    return;
                }
            }
            enabled = _isPlaying = false;
        }


        private void StartAnimation()
        {
            _nextFrameTime = Time.time + _secondsPerFrame;
           enabled = _isPlaying = true;
            _currentFrame = 0;
        }
        
        private void OnEnable() 
        {
            _nextFrameTime = Time.time + _secondsPerFrame;  // задаем следующий апдейт кадра
        }

        private void Update()
        {
            if(_nextFrameTime > Time.time) return;

            var clip = _clips[_currentClip];
            if(_currentFrame >= clip.Sprites.Length) // проверка не вышли ли мы за пределы массива
            {
                if(clip.Loop)
                {
                    _currentFrame = 0;    // если циклящаяся анимация, то сбрасываем до 0
                }
                else
                {
                    enabled = _isPlaying = clip.AllowNextClip;
                    clip.OnComplete?.Invoke();
                    _onComplete?.Invoke(clip.Name);
                    if (clip.AllowNextClip)
                    {
                        _currentFrame = 0;
                        _currentFrame = (int) Mathf.Repeat(_currentClip + 1, _clips.Length);
                    }
                    return;
                }
            }

            _renderer.sprite = clip.Sprites[_currentFrame]; // если не вышли за пределы массива, то меняем спрайт
            _nextFrameTime += _secondsPerFrame;  //обновляем время до следующего изменения
            _currentFrame++; // устанавливаем след спрайт.
        }

        [Serializable]
        public class AnimationClip
        {
            [SerializeField] private string _name;
            [SerializeField] private Sprite[] _sprites; 
            [SerializeField] private bool _loop; 
            [SerializeField] private bool _allowNextClip;
            [SerializeField] private UnityEvent _onComplete; 

            public string Name => _name;
            public Sprite[] Sprites => _sprites;
            public bool Loop => _loop;
            public bool AllowNextClip => _allowNextClip;
            public UnityEvent OnComplete => _onComplete;
        }
    }
}

