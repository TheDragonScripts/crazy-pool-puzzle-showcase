using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace SDI
{
    public class PortableDI : IPortableDI
    {
        private const int MaxTries = 5;
        private const float TryDelay = 0.5f;

        private int _currentTry;
        private bool _isInjected;

        private static IDependencyInjector _injector;

        public PortableDI() { }
        public PortableDI(IDependencyInjector injector) : this() => SetPrimaryInjector(injector);
        public void SetPrimaryInjector(IDependencyInjector injector) => _injector = injector;

        public void Inject<T>(T obj)
        {
            CheckForInjector();
            TryInjectDependencies(obj);
        }

        private async UniTaskVoid DoDelaydInjection<T>(T target)
        {
            await UniTask.WaitForSeconds(TryDelay);
            if (this == null || _isInjected)
            {
                return;
            }
            TryInjectDependencies(target);
        }

        private void TryInjectDependencies<T>(T target)
        {
            if (_isInjected)
            {
                return;
            }
            bool isExceptionCaused = false;
            try
            {
                _injector.Inject(target);
            }
            catch (InvalidOperationException ioex)
            {
                isExceptionCaused = true;
                throw new InvalidOperationException(ioex.Message);
            }
            catch (MissingReferenceException)
            {
                isExceptionCaused = true;
                _ = DoDelaydInjection(target);
            }
            finally
            {
                if (isExceptionCaused && _currentTry >= MaxTries)
                {
                    throw new MissingReferenceException("Injection failed probably due to missing " +
                        "dependencies in primary DependencyInjector.");
                }
                else
                {
                    _isInjected = true;
                }
            }
        }

        private void CheckForInjector()
        {
            if (_injector == null)
            {
                CSDL.LogError("Injector is not provided to GameObjectsDI.");
                return;
            }
        }
    }
}