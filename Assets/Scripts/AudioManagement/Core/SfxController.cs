using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AudioManagement.Core
{
    public class SfxController : IAudioController
    {
        private string _audioSorucePrefabPath;
        private string _audioClipsPath;
        private bool _isMuted;
        private List<AudioSource> _activeAudioSources;
        private IObjectPool<AudioSource> _objectsPool;
        private Stack<AudioClip> _requestedClips;

        private const int DefaultPoolCapacity = 10;
        private const int MaxPoolCapacity = 100;
        private const bool DoCollectionCheck = true;
        private const string SfxPooledGameObjectName = "SfxPooledObject";

        public SfxController(string audioSorucePrefabPath, string defaultAudioClipsPath)
        {
            _audioSorucePrefabPath = audioSorucePrefabPath;
            _audioClipsPath = defaultAudioClipsPath;
            _activeAudioSources = new List<AudioSource>();
            _requestedClips = new Stack<AudioClip>();
            _objectsPool = new ObjectPool<AudioSource>(CreateAudioSource, OnGetFromPool, OnReleaseFromPool, OnDestroyPooledObject,
                DoCollectionCheck, DefaultPoolCapacity, MaxPoolCapacity);
        }

        public void Play(string sfxName)
        {
            if (_isMuted)
            {
                return;
            }
            AudioClip audioClip = Resources.Load(_audioClipsPath + sfxName) as AudioClip;
            if (audioClip != null)
            {
                _requestedClips.Push(audioClip);
                _objectsPool.Get();
            }
            else
            {
                CSDL.LogError($"{nameof(SfxController)} can't find {sfxName} in {_audioClipsPath}");
            }
        }

        public void Mute()
        {
            _isMuted = true;
            PauseAllSfxsImmediately();
        }

        public void Unmute()
        {
            _isMuted = false;
        }

        public void ProvideAudioClipsPath(string path)
        {
            _audioClipsPath = path;
        }

        private void OnDestroyPooledObject(AudioSource source)
        {
            _activeAudioSources.Remove(source);
            source.Pause();
            GameObject.Destroy(source.gameObject);
        }

        private void OnReleaseFromPool(AudioSource source)
        {
            _activeAudioSources.Remove(source);
            source.Pause();
            source.gameObject.SetActive(false);
        }

        private void OnGetFromPool(AudioSource source)
        {
            source.gameObject.SetActive(true);
            if (_requestedClips.TryPop(out AudioClip clip))
            {
                _activeAudioSources.Add(source);
                source.clip = clip;
                source.Play();
                _ = ReturnToPoolAfterPlayAsync(source);
            }
            else
            {
                _objectsPool.Release(source);
            }
        }

        private AudioSource CreateAudioSource()
        {
            GameObject instance = GameObject.Instantiate(Resources.Load(_audioSorucePrefabPath)) as GameObject;
            instance.name = SfxPooledGameObjectName;
            return instance.GetComponent<AudioSource>();
        }

        private void PauseAllSfxsImmediately()
        {
            foreach (AudioSource audioSource in _activeAudioSources)
            {
                audioSource.Pause();
            }
        }

        private async UniTaskVoid ReturnToPoolAfterPlayAsync(AudioSource audioSoruce)
        {
            await UniTask.WaitForSeconds(audioSoruce.clip.length);
            if (audioSoruce != null && this != null)
            {
                _objectsPool.Release(audioSoruce);
            }
        }
    }
}
