using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Crashlytics;

namespace ThirdPartiesIntegrations.Services.Firebase
{
    public class FirebaseInitializer : ServiceInitializer, IFirebaseInitializer
    {
        public FirebaseInitializer(bool isManualInitilizationEnabled = false) : base(isManualInitilizationEnabled) { }

        protected override void Initialize()
        {
            _ = InitializeAsync();
        }

        private async UniTaskVoid InitializeAsync()
        {
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                base.Initialize();
            }
            else
            {
                CSDL.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        }
    }
}
