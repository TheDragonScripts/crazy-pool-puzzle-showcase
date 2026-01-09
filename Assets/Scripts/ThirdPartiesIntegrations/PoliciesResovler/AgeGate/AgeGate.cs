using Cysharp.Threading.Tasks;
using ModificatedUISystem;
using ModificatedUISystem.UIElements;
using SDI;
using System;

namespace ThirdPartiesIntegrations.AgeGateSystem
{
    /// <summary>
    /// Provides functionality for determining a user's age group by displaying a UI form with simple
    /// math expression to determine whether the user is a child.
    /// </summary>
    /// <remarks>Developed to resolve COPPA policy.</remarks>
    public class AgeGate : IAgeGate
    {
        private IUIFactory _uiFactory;
        private AgeGateForm _ageGateForm;

        public bool IsUserAgeGroupObtained { get; private set; }
        public bool IsUserAdult { get; private set; }

        public event AgeGateEventHandler UserAgeGroupObtained;

        public AgeGate(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            Initialize();
        }

        public void ChageUserAgeGroupFromOutside()
        {
            _ = RequestAgeGateForm();
        }

        public void SubscribeToAgeGroupObtainingEvent(AgeGateEventHandler callback)
        {
            if (IsUserAgeGroupObtained)
            {
                callback(IsUserAdult);
            }
            else
            {
                void AutoUnsubscribe(bool isUserAdult)
                {
                    UserAgeGroupObtained -= AutoUnsubscribe;
                    callback(isUserAdult);
                }
                UserAgeGroupObtained += AutoUnsubscribe;
            }
        }

        private void Initialize()
        {
            if (ActualPlayerData.Data.IsAgeGateShown == 1)
            {
                VerifyAge(Convert.ToBoolean(ActualPlayerData.Data.IsUserAdult));
            }
            else
            {
                _ = RequestAgeGateForm();
            }
        }

        private async UniTaskVoid RequestAgeGateForm()
        {
            if (_ageGateForm == null)
            {
                _ageGateForm = await _uiFactory.GetAsync<AgeGateForm>();
                _ageGateForm.UserAgeGroupObtained += OnUserAgeGateObtainedFromForm;
            }
        }

        private void OnUserAgeGateObtainedFromForm(bool isUserAdult)
        {
            VerifyAge(isUserAdult);
        }

        private void VerifyAge(bool isUserAdult)
        {
            CSDL.Log("Is user adult: " + isUserAdult);
            IsUserAdult = isUserAdult;
            IsUserAgeGroupObtained = true;
            ActualPlayerData.Data.IsAgeGateShown = 1;
            ActualPlayerData.Data.IsUserAdult = isUserAdult ? 1 : 0;
            UserAgeGroupObtained?.Invoke(isUserAdult);
        }
    }
}
