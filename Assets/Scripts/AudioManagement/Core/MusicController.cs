using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace AudioManagement.Core
{
    public class MusicController : IAudioController
    {
        private bool _isMuted;
        private bool _isNextTrackRequested;
        private float _defaultMusicVolume;
        private AudioSource _musicAudioSource;

        private string _audioSourcePrefabResourcePath;
        private string _musicClipsResourcesPath;

        private List<AudioClip> _trackList;
        private Stack<AudioClip> _previousTracks;

        private CancellationTokenSource _playNextTrackCts;
        private CancellationTokenSource _trackEndFadeCts;

        private const string MusicSourceGameObjectName = "MusicObject";
        private const int SecondsBeforeEndToStartFade = 10;

        public MusicController(string audioSourcePrefabResourcePath, string defaultMusicClipsResourcesPath)
        {
            _previousTracks = new Stack<AudioClip>();
            _trackList = new List<AudioClip>();
            _audioSourcePrefabResourcePath = audioSourcePrefabResourcePath;
            _musicClipsResourcesPath = defaultMusicClipsResourcesPath;
            CreateAudioSourceInstance();
            CreateTrackList();
        }

        public void Mute()
        {
            _isMuted = true;
            Pause();
        }

        public void Unmute()
        {
            _isMuted = false;
            Play();
        }

        public void ProvideAudioClipsPath(string path)
        {
            _musicClipsResourcesPath = path;
            Pause();
            CreateTrackList();
        }

        public void Play()
        {
            if (_isMuted)
            {
                CSDL.LogWarning($"Music is muted. Call {nameof(Unmute)} method first.");
                return;
            }
            if (_isNextTrackRequested || _musicAudioSource.clip == null 
                || _musicAudioSource.time >= _musicAudioSource.clip.length)
            {
                _isNextTrackRequested = false;
                AddClipToPrevious(_musicAudioSource.clip);
                _musicAudioSource.clip = FindNextTrack();
                _musicAudioSource.Play();
            }
            else
            {
                _musicAudioSource.UnPause();
            }
            _ = PlayNextTrackAfterAsync();
            _ = StartEndOfTrackFadeAsync();
        }

        private void Pause()
        {
            _musicAudioSource?.Pause();
            _playNextTrackCts?.Cancel();
            _trackEndFadeCts?.Cancel();
        }

        private void Reset()
        {
            _isNextTrackRequested = true;
            _musicAudioSource.clip = null;
            _trackList.Clear();
            _previousTracks.Clear();
        }

        private void AddClipToPrevious(AudioClip clip)
        {
            if (clip != null)
            {
                _previousTracks.Push(clip);
            }
        }

        private AudioClip FindNextTrack()
        {
            AudioClip[] possibleNextTracks = _trackList.Except(_previousTracks).ToArray();
            if (possibleNextTracks.Length == 0)
            {
                _previousTracks.Clear();
                possibleNextTracks = _trackList.ToArray();
            }
            AudioClip nextTrack = possibleNextTracks[0];
            return nextTrack;
        }

        private void CreateAudioSourceInstance()
        {
            GameObject instance = GameObject.Instantiate(Resources.Load(_audioSourcePrefabResourcePath)) as GameObject;
            instance.name = MusicSourceGameObjectName;
            _musicAudioSource = instance.GetComponent<AudioSource>();
            _defaultMusicVolume = _musicAudioSource.volume;
        }

        private void CreateTrackList()
        {
            Reset();
            _trackList.AddRange(Resources.LoadAll<AudioClip>(_musicClipsResourcesPath));
            if (_trackList.Count == 0)
            {
                throw new InvalidOperationException(
                    $"{nameof(MusicController)} can't create track list from given path " +
                    $"{_musicClipsResourcesPath}. There is no suitable audio files.");
            }
        }

        private CancellationTokenSource RenewCancellationTokenSource(ref CancellationTokenSource fieldCtsToRenew)
        {
            CancellationTokenSource newCts = new CancellationTokenSource();
            CancellationTokenSource oldCts = Interlocked.Exchange(ref fieldCtsToRenew, newCts);
            oldCts?.Cancel();
            oldCts?.Dispose();
            return newCts;
        }

        private async UniTask FadeTrackVolumeAsync(CancellationToken cancellationToken)
        {
            float fadeStep = (_musicAudioSource.volume / SecondsBeforeEndToStartFade) * Time.deltaTime;
            while (_musicAudioSource.volume > 0f)
            {
                await UniTask.WaitForEndOfFrame(cancellationToken : cancellationToken);
                if (cancellationToken != null && !cancellationToken.IsCancellationRequested)
                {
                    _musicAudioSource.volume -= fadeStep;
                }
                else
                {
                    break;
                }
            }
        }

        private async UniTaskVoid StartEndOfTrackFadeAsync()
        {
            CancellationTokenSource localCts = RenewCancellationTokenSource(ref _trackEndFadeCts);
            _musicAudioSource.volume = _defaultMusicVolume;
            float waitTime = _musicAudioSource.clip.length - _musicAudioSource.time - SecondsBeforeEndToStartFade;
            await UniTask.WaitForSeconds(
                duration: waitTime,
                cancellationToken: localCts.Token)
                .SuppressCancellationThrow();
            if (localCts != null && !localCts.IsCancellationRequested)
            {
                await FadeTrackVolumeAsync(localCts.Token);
            }
        }

        private async UniTaskVoid PlayNextTrackAfterAsync()
        {
            CancellationTokenSource localCts = RenewCancellationTokenSource(ref _playNextTrackCts);
            float waitTime = Mathf.Max(0f, _musicAudioSource.clip.length - _musicAudioSource.time);
            await UniTask.WaitForSeconds(
                duration: waitTime,
                cancellationToken: localCts.Token)
                .SuppressCancellationThrow();
            if (localCts != null && !localCts.IsCancellationRequested)
            {
                _isNextTrackRequested = true;
                Play();
            }
        }
    }
}
