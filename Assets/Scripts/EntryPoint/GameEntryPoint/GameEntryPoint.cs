using AudioManagement;
using Balls.Management;
using EnergySystem;
using EntryPoint.GameData;
using EntryPoint.GameRuler;
using EntryPoint.GameState;
using EntryPoint.Levels;
using EntryPoint.LifeCycle;
using ModificatedUISystem;
using SDI;
using ThemesManagement;
using ThemesManagement.Environment;
using ThirdPartiesIntegrations.UnitedAnalytics;
using ThirdPartiesIntegrations.UnitedAnalytics.Vendors;
using ThirdPartiesIntegrations.Services.Firebase;
using ThirdPartiesIntegrations.Services.UnityServicesIntegration;
using ThirdPartiesIntegrations.IAP;
using ThirdPartiesIntegrations.LevelPlaySystem;
using ThirdPartiesIntegrations.Resolver;
using UnityEngine;
using UnityEngine.Purchasing;
using PlayerInputs.Swipes;
using BallsMovement;
using PlayerInputs.ObjectsPicker;

namespace EntryPoint
{
    [RequireComponent(typeof(GameSaverAndLoader))]
    [RequireComponent(typeof(GameLifeCycle))]
    public class GameEntryPoint : MonoBehaviour, IGameEntryPoint
    {
        // Singleton is a temporary soultion

        [SerializeField] private int _gameLossCheckTime = 7;
        [SerializeField] private float _swipeDetection = 100;
        [SerializeField] private BallsForceControllerSettingsScriptableObject _ballsForceControllerSettings;
        [SerializeField] private EnergyManagerSettingsScriptableObject _energyManagerSettings;
        [SerializeField] private LevelPlaySettingsScriptableObject _levelPlayIntegrationSettings;
        [SerializeField] private AdsInvokerSettingsScriptableObject _adsInvokerSettings;
        [SerializeField] private ThemesSettingsScriptableObject _themesSettings;
        [SerializeField] private IAPListener _iapListener;

        private IDependencyInjector _dependencyInjector;
        private IUIInvoker _uiInvoker;
        private IGameLifeCycle _gameLifeCycle;
        private IEnergyManager _energyManager;
        private IPoliciesResolver _policiesResolver;
        private IInAppPurchasingIntegration _inAppPurchasingIntegration;
        private IGamePauseController _gamePauseController;
        private ILevelPlayIntegration _levelPlayIntegration;
        private IAdsInvoker _adsInvoker;
        private ProjectAudioController _projectAudioController;
        private IEnvironmentController _environmentController;
        private IThemesManager _themesManager;
        private IUnityServicesInitializer _unityServicesInitializer;
        private IFirebaseInitializer _firebaseInitializer;
        private IAnalyticsEventsInvoker _analyticsEventsInvoker;
        private IRaycastObjectPicker _raycastObjectPicker;
        private IBallsForceApplication _ballsForceApplication;

        public IGameSaverAndLoader GameSaverAndLoader { get; private set; }
        public ILevelSession LevelSession { get; private set; }
        public IGlobalGameState GlobalGameState { get; private set; }
        public ISwipeController SwipeController { get; private set; }
        public IBallsForceApplication BallsForceApplication => _ballsForceApplication;
        public IRaycastObjectPicker RaycastObjectPicker => _raycastObjectPicker;
        public ILevelManager LevelManager { get; private set; }
        public IGameRuler GameRuler { get; private set; }
        public IBallsRegistry BallsRegistry { get; private set; }
        public IUIFactory UIFactory { get; private set; }

        public static GameEntryPoint Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            _dependencyInjector = new DependencyInjector();
            _uiInvoker = new DefaultUIInvoker();

            GlobalGameState = new GlobalGameState();
            SwipeController = new SwipeController(_swipeDetection);
            LevelManager = new LevelManager();
            BallsRegistry = new BallsRegistry();
            LevelSession = new LevelSession();
            GameRuler = new BaseGameRuler(_gameLossCheckTime);
            GameSaverAndLoader = GetComponent<IGameSaverAndLoader>();
            _gameLifeCycle = GetComponent<IGameLifeCycle>();
            _energyManager = new EnergyManager(_energyManagerSettings);
            UIFactory = new UIFactory();
            _policiesResolver = new PoliciesResolver();
            _inAppPurchasingIntegration = new InAppPurchasingIntegration(_iapListener);
            _gamePauseController = new GamePauseController();
            _levelPlayIntegration = new LevelPlayIntegration(_levelPlayIntegrationSettings);
            _adsInvoker = new AdsInvoker(_adsInvokerSettings);
            _projectAudioController = new ProjectAudioController();
            _environmentController = new EnvironmentController();
            _themesManager = new ThemesManager(_themesSettings);
            _raycastObjectPicker = new RaycastObjectPicker();
            _ballsForceApplication = new BallsForceApplication(_ballsForceControllerSettings);

            // Start analytics
            _unityServicesInitializer = new UnityServicesInitializer(true, _policiesResolver);
            _firebaseInitializer = new FirebaseInitializer();
            _analyticsEventsInvoker = new AnalyticsEventsInvoker(
                new UnityAnalyticsVendor(_unityServicesInitializer, _policiesResolver),
                new FirebaseAnalyticsVendor(_firebaseInitializer, _policiesResolver));
            // End analytics

            GameSaverAndLoader.Initialize();

            _dependencyInjector.Register(GlobalGameState, SwipeController, LevelManager, BallsRegistry,
                LevelSession, GameRuler, GameSaverAndLoader, UIFactory, _gameLifeCycle, _energyManager,
                _policiesResolver, _inAppPurchasingIntegration, _gamePauseController, _levelPlayIntegration,
                _adsInvoker, _projectAudioController, _environmentController, _themesManager, _raycastObjectPicker, 
                _ballsForceApplication);

            _dependencyInjector.Inject(LevelManager, BallsRegistry, LevelSession, GameRuler, _uiInvoker, _energyManager,
                _policiesResolver, _levelPlayIntegration, _adsInvoker, _projectAudioController, _environmentController, _themesManager,
                _analyticsEventsInvoker, _ballsForceApplication);
        }
    }
}