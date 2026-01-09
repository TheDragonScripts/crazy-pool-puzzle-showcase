using Cysharp.Threading.Tasks;
using ModificatedUISystem.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;

namespace ModificatedUISystem
{
    public class UIFactory : IUIFactory
    {
        private const string PrefabsPath = "Prefabs/UI/";

        private Dictionary<string, InnerUIEntry> _allOpenedUIs = new();
        private List<string> _requesedUIIds = new();
        private string _previousMenu;

        public event UIFactoryShowUIEventHandler UIWasShown;
        public event UIFactoryHideUIEventHandler UIWasHidden;

        private class InnerUIEntry
        {
            public IUIElement UI;
            public string ID;
            public bool IsHidden;

            public InnerUIEntry(IUIElement ui, string id)
            {
                UI = ui;
                ID = id;
            }
        }

        public async UniTask<T> GetByIdAsync<T>(string id) where T : class
        {
            if (!TryAddToRequested<T>(id))
            {
                CSDL.LogWarning($"{nameof(UIFactory)} too many requests of {id}. Returning null");
                return null;
            }
            InnerUIEntry entry = _allOpenedUIs.ContainsKey(id) 
                ? _allOpenedUIs[id] 
                : TryCreateAndRegisterNewInstance<T>();
            await ShowUI(entry);
            RemoveFromRequested<T>(id);
            return (T)entry.UI;
        }

        public async UniTask<T> GetAsync<T>() where T : class
        {
            if (!TryAddToRequested<T>())
            {
                CSDL.LogWarning($"{nameof(UIFactory)} too many requests of {typeof(T).Name}. Returning null");
                return null;
            }
            InnerUIEntry entry = _allOpenedUIs.Where(e => e.Value.UI is T).Select(e => e.Value).FirstOrDefault();
            entry ??= TryCreateAndRegisterNewInstance<T>();
            await ShowUI(entry);
            RemoveFromRequested<T>();
            return (T)entry.UI;
        }

        public async UniTask<(T ui, string id)> GetNewInstanceAsync<T>() where T : class
        {
            InnerUIEntry entry = TryCreateAndRegisterNewInstance<T>();
            await ShowUI(entry);
            return ((T)entry.UI, entry.ID);
        }

        public void OpenPreviousMenu()
        {
            if (!string.IsNullOrEmpty(_previousMenu))
            {
                InnerUIEntry entry = _allOpenedUIs.Where(e => e.Value.UI.GetType().Name == _previousMenu)
                    .Select(e => e.Value)
                    .FirstOrDefault();
                entry ??= TryCreateAndRegisterNewInstance<IUIElement>(_previousMenu);
                _previousMenu = null;
                _ = ShowUI(entry);
            }
        }

        public void Close<T>()
        {
            foreach (var ui in _allOpenedUIs)
            {
                if (ui.Value.UI is T && !ui.Value.IsHidden)
                {
                    _ = HideUI(ui.Value);
                }
            }
        }

        public void Close(string id)
        {
            foreach (var ui in _allOpenedUIs)
            {
                if (ui.Key == id && !ui.Value.IsHidden)
                {
                    _ = HideUI(ui.Value);
                    break;
                }
            }
        }

        public bool IsUIOpened<T>(string id = null)
        {
            id ??= typeof(T).Name;
            foreach (var ui in _allOpenedUIs)
            {
                if (ui.Key == id && ui.Value.UI is T)
                {
                    return true;
                }
            }
            return false;
        }

        public IUIElement[] GetAllOpenedUIs() => _allOpenedUIs.Values.Where(u => !u.IsHidden).Select(u => u.UI).ToArray();

        private void RemoveFromRequested<T>(string id = null)
        {
            id ??= typeof(T).Name;
            _requesedUIIds.Remove(id);
        }

        private bool TryAddToRequested<T>(string id = null)
        {
            id ??= typeof(T).Name;
            if (!_requesedUIIds.Contains(id))
            {
                _requesedUIIds.Add(id);
                return true;
            }
            return false;
        }

        private InnerUIEntry TryCreateAndRegisterNewInstance<T>(string customUIName = null)
        {
            (IUIElement, GameObject) instanceData;
            try
            {
                instanceData = CreateInstance<T>(customUIName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            (IUIElement uiElement, GameObject instance) = instanceData;
            InnerUIEntry entry;
            try
            {
                entry = RegisterUI(uiElement);
            }
            catch (Exception ex)
            {
                GameObject.Destroy(instance);
                throw ex;
            }
            return entry;
        }

        private InnerUIEntry RegisterUI(IUIElement uiElement)
        {
            InnerUIEntry CreateAndInsertEntry(string id)
            {
                InnerUIEntry newEntry = new InnerUIEntry(uiElement, id);
                _allOpenedUIs.Add(id, newEntry);
                return newEntry;
            }

            Type uiElementType = uiElement.GetType();
            InnerUIEntry[] sameEntries = _allOpenedUIs
                .Where(e => e.Value.UI.GetType() == uiElementType)
                .Select(e => e.Value)
                .ToArray();

            if (sameEntries.Length > 0)
            {
                if (uiElement is IMultiInstancableUI)
                {
                    string newId = uiElementType.Name + sameEntries.Length;
                    return CreateAndInsertEntry(newId);
                }
                else
                {
                    throw new InvalidOperationException($"UIFactory can't register {uiElement} beacause it's " +
                        $"not marked as {nameof(IMultiInstancableUI)} and instance is already created.");
                }
            }
            else
            {
                return CreateAndInsertEntry(uiElementType.Name);
            }
        }

        private (IUIElement, GameObject) CreateInstance<T>(string customUIName = null)
        {
            string uiName = customUIName ?? typeof(T).Name;
            string path = PrefabsPath + uiName;
            GameObject instance = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            if (instance == null)
            {
                throw new InvalidPathException($"UIFactory can't instantiate object, because prefab " +
                    $"{path} is invalid");
            }
            if (instance.TryGetComponent(out IUIElement uiElement))
            {
                if (uiElement is IFrequentlyUsedUI)
                {
                    GameObject.DontDestroyOnLoad(instance);
                }
                return (uiElement, instance);
            }
            else
            {
                GameObject.Destroy(instance);
                throw new MissingComponentException($"UIFactory can't find IUIElement component on " +
                    $"instantiated object of type {uiName}");
            }
        }

        private async UniTask HideUI(InnerUIEntry entry)
        {
            if (entry.IsHidden || !_allOpenedUIs.ContainsKey(entry.ID))
            {
                return;
            }
            if (entry.UI is IFrequentlyUsedUI)
            {
                entry.IsHidden = true;
                entry.UI.AnimationController.HideTemporary();
            }
            else
            {
                _allOpenedUIs.Remove(entry.ID);
                entry.UI.AnimationController.Hide();
            }
            await UniTask.WaitUntil(() => !entry.UI.AnimationController.IsAnimating);
            if (entry.UI is IUIOfType<MenuType>)
            {
                _previousMenu = entry.UI.GetType().Name;
            }
            UIWasHidden?.Invoke(entry.ID, entry.IsHidden);
        }

        private async UniTask ShowUI(InnerUIEntry entry)
        {
            if (entry.UI is IUIOfType<MenuType>)
            {
                await CloseAllMenusIfPossibleExcept(entry);
            }
            if (!_allOpenedUIs.ContainsKey(entry.ID))
            {
                return;
            }
            entry.IsHidden = false;
            entry.UI.AnimationController.Show();
            if (entry.UI is IAutoHidableUI)
            {
                _ = CloseAutoHideableUIWhenPossibleAsync(entry);
            }
            await UniTask.WaitUntil(() => !entry.UI.AnimationController.IsAnimating);
            UIWasShown?.Invoke(entry.ID, entry.UI);
        }

        private async UniTaskVoid CloseAutoHideableUIWhenPossibleAsync(InnerUIEntry entry)
        {
            if (entry.UI is IAutoHidableUI autoHidableUI)
            {
                await UniTask.WaitUntil(() => autoHidableUI.IsAvailableForAutoHide);
                _ = HideUI(entry);
            }
        }

        private async UniTask CloseAllMenusIfPossibleExcept(InnerUIEntry excludedEntry)
        {
            InnerUIEntry[] entries = _allOpenedUIs
                .Where(e => e.Value != excludedEntry && e.Value.UI is IUIOfType<MenuType>)
                .Select(e => e.Value)
                .ToArray();
            foreach (var entry in entries)
            {
                await HideUI(entry);
            }
        }
    }
}