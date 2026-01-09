using EntryPoint;
using EntryPoint.Levels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;

namespace Tutorial
{
    [Serializable]
    public class TutorialChapter
    {
        public string UniqueName;
        public LocalizedString Information;
        public Sprite AccompanyingImage;
    }

    [Serializable]
    public class TutorialTheme
    {
        public string UniqueName;
        public bool IsPermanent;
        public TutorialChapter[] Chapters;
    }
    /*
     * Code here is temporary disabled because it references old UI system.
     * Pending refactoring.
     */
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private TutorialTheme[] _tutorials;

        private ILevelManager _levelManager;

        public TutorialTheme[] AvailableThemes => _tutorials;

        public static Tutorial Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            //UIManager.Instance.TutorialFrame.OnTutorialCompleted += MarkTutorialAsCompleted;
            _levelManager = GameEntryPoint.Instance.LevelManager;
            _levelManager.PlayableLevelLoaded += ShowRulesTutorial;
            StartCoroutine(IntroductionTutorial());
        }

        private void ShowRulesTutorial(int level)
        {
            StartCoroutine(GameRulesTutorial());
        }

        private IEnumerator GameRulesTutorial()
        {
            yield return new WaitForSeconds(1f);
            TryToShowTutorial("GameRules");
        }

        private IEnumerator IntroductionTutorial()
        {
            yield return new WaitForSeconds(3f);
            TryToShowTutorial("Introduction");
        }

        public void TryToShowTutorial(string name)
        {
            if (ActualPlayerData.Data == null || GetTutorialThemeByName(name) == null) return;
            TutorialTheme theme = GetTutorialThemeByName(name);
            if (IsTutorialCompleted(theme)) return;
            //UIManager.Instance.TutorialFrame.SetTheme(theme);
        }

        public void ForceShowTutorial(string name)
        {
            if (GetTutorialThemeByName(name) == null) return;
            TutorialTheme theme = GetTutorialThemeByName(name);
            //UIManager.Instance.TutorialFrame.SetTheme(theme);
        }

        private void MarkTutorialAsCompleted(TutorialTheme tutorialTheme)
        {
            if (IsTutorialThemeCompleted(tutorialTheme.UniqueName)) return;
            string[] arrayCopy = new string[ActualPlayerData.Data.CompletedTutorials.Length + 1];
            for (int i = 0; i < ActualPlayerData.Data.CompletedTutorials.Length; i++)
                arrayCopy[i] = ActualPlayerData.Data.CompletedTutorials[i];
            arrayCopy[arrayCopy.Length - 1] = tutorialTheme.UniqueName;
            ActualPlayerData.Data.CompletedTutorials = arrayCopy;
        }

        private bool IsTutorialThemeCompleted(string name)
        {
            bool answer = false;
            TutorialTheme theme = GetTutorialThemeByName(name);
            if (theme == null || theme.IsPermanent) return false;
            foreach (string tutorialTheme in ActualPlayerData.Data.CompletedTutorials)
            {
                if (tutorialTheme == name)
                {
                    answer = true;
                    break;
                }
            }
            return answer;
        }

        private bool IsTutorialCompleted(TutorialTheme theme)
        {
            bool answer = false;
            if (theme.IsPermanent) return false;
            foreach (string tutorialTheme in ActualPlayerData.Data.CompletedTutorials)
            {
                if (tutorialTheme == theme.UniqueName)
                {
                    answer = true;
                    break;
                }
            }
            return answer;
        }

        private TutorialTheme GetTutorialThemeByName(string name)
        {
            TutorialTheme foundTheme = null;
            foreach (TutorialTheme theme in _tutorials)
                if (theme.UniqueName == name) foundTheme = theme;
            return foundTheme;
        }
    }
}