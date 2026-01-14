# crazy-pool-puzzle-showcase
Showcase of the Crazy Pool Puzzle game with a new architecture.
> [!CAUTION]
> #### License
> The license included with the repo applies only to our code and resources and **does not in any way apply to other free assets** present in the game. The third-party assets used here were free, at least at the time of writing. We permit anyone to create a public fork of this repository to modify **our code and resources** in accordance with the license provided.
## Quick game status review
Is Android build possible? - **No**.\
Is core gameplay available? - **No**.\
Confirmed working scene - **Assets/Scenes/Init.unity**\
Confirmed working system (with ability to see the actual result via UI) - **Themes**

Yes, that's all. The other refactored systems are ready, but not tested, so I can't confirm whether they work properly or not. And even more so, I can’t confirm that the core gameplay works.
## Detailed description
This is a showcase representation of the refactoring branch with the new architecture of our project on [Google Play](https://play.google.com/store/apps/details?id=com.gornostaistudio.crazypoulepuzzle&hl=en) called Crazy Pool Puzzle. At the moment this branch is unplayable and its build is not yet possible. Yes, this is just a demonstration of architecture and it's very different from what's currently in production. The following description will be in Russian for employers who will consider it as my portfolio.
## Подробное описание.
Это репрезентация ветки рефакторинга нашего проекта из [Google Play](https://play.google.com/store/apps/details?id=com.gornostaistudio.crazypoulepuzzle&hl=en) под названием Crazy Pool Puzzle. На данный момент игра неиграбельна и собрать релизный билд не получится. Да, это просто архитектурная демка, которая сильно отличается от того, что на данный момент находится в продакшене.
## Про код
Весь проект буквально является одной большой игровой площадкой, где я экспериментировал с разными стилями, даже, когда делал новую архитектуру. Поэтому стиль кода может разниться. Вот пример таковой разницы:
```c#
// Первый вариант
if (!condition) return;

// Второй вариант
if (!condition)
{
  return;
}

// Третий вариант
if (!condition)
  return;
```
Почему так происходит? Всё достаточно прозаично - я не мог определиться. Второй вариант, например, взят из официального руководства [Create a C# style guide: Write cleaner code that scales](https://unity.com/resources/create-code-c-sharp-style-guide-e-book?isGated=false). Почему так делается, я прекрасно понимаю, но на мой взгляд это излишне, поэтому третий вариант - это мой основной выбор сейчас.
## Комментарии в коде и документация
Если вы будете особо внимательны, то заметите, что даже те редкие комментарии в коде, написаны на английском. Причина проста - это помогает мне практиковать английский язык и пополнять мой словарный запас. Описание к коммитам я тоже делаю на английском, по тем же причинам.
## Использование нейросетей
В проекте они использовались, но не в большом количестве. Например, я иногда спрашиваю у ИИ про нейминг полей или классов, если сомневаюсь в своих вариантах.
## Что сделано и на что стоит посмотреть?
Во-первых, сразу скажу, что ещё не все модули здесь переделаны, в них может быть нарушена логика, стиль кода, нейминг и прочее. Было бы странно, если бы всё было отрефакторено, но при этом игра оставалась неиграбельной, как я уже упоминал ранее. Не так ли?) Что касается тех модулей, которые уже переписаны - они тщательно не протестированы и их ещё предстоит подружить друг с другом. Собственно говоря - это и есть причина по которой даже core gameplay сейчас недоступен. Единственное, что точно работает - это темы и соответствующая интеграция с Unity In App Purchasing. Но в любом случае, код модулей из раздела "Новая архитектура" крайне рекомендован к ознакомлению, особенно Dependency Injector, ибо это фундамент новой архитектуры, с него всё начинается и на нём всё держится.
# Модули
Здесь показана не полная иерархия, только базовая, чтобы сформировать общее представление о проекте и его структуре. Все модули можно найти в папке [Scripts](Assets/Scripts).

**Новая архитектура**
* AudioManagement
* BallsMovement
* BallsRegistry
* CustomAttributes
* DependencyInjector
* EnergyManager
* EnumerableAnchors
* PlayerInputs
    * RaycastObjectPicker
    * SwipeController
* Themes
* ThirdPartiesIntegrations
    * GPP
    * IAP
    * LevelPlay
    * PoliciesResolver
    * Services
    * UnitedAnalytics
* UIFactory
* WeightsRandom
* CSDL (Single file)

**Стык старой и новой архитектуры**
* EntryPoint
    * GameEntryPoint
    * GameRuler
    * GameSaverAndLoader
    * GameState
    * Levels
    * LifeCycle

**Старая архитектура**
* Ball
* Barriers
* Board
* InteractiveTutorial
* PlayerData
* ResolutionScaleManager
* Tutorial
* Stash (Single file)

> [!NOTE]
> GameEntryPoint находится на стыке новой и старой архитектуры, ибо он работает, как Singleton. Это временное решение, чтобы предотвратить выбрасывание исключений старыми модулями.
> 
> GPP модуль - это временная затычка с данными, которые внесены туда посредством hardcode-а. Полноценный парсер GPP строки со всеми заголовками на данный момент в разработке.

## Использованные паттерны
* Factory [UIFactory](Assets/Scripts/UIFactory/)
* [Dependency Injector](Assets/Scripts/DependencyInjector/)
* Builder [InteractiveTutorial](Assets/Scripts/InteractiveTutorial/)
* Wrapper [UIFactory/ButtonsActions/Wrappers](Assets/Scripts/UIFactory/UI%20ButtonsActions/Wrappers/ButtonActionWithClickSoundWrapper.cs)
* Strategy [UIFacotry/AnimtaionControllers](Assets/Scripts/UIFactory/UI%20Elements/AnimationControllers/Strategies), [Ball/SpecialBalls](Assets/Scripts/Ball/Special%20Balls/)
* Command [UIFactory/ButtonsActions](Assets/Scripts/UIFactory/UI%20ButtonsActions)

#### О паттерне Factory
Это не чистый Factory, а скорее некий гибрид, ибо он выполняет не только функцию создания, но и управляет, созданными UI.
#### О паттерне Builder
Сам модуль, в котором представлен этот паттерн, ещё не был отрефакторен. Но так или иначе, на мой взгляд, он соответствует общепринятому шаблону - создаёт сложные объекты, объединенные едиными интерфейсами, но отличающихся своим поведением в зависимости от вводных данных. Также для удобства объекты строителей возвращают самих себя в методах, чтобы можно было через точку, красиво и лаконично прописать их вызов. Само "строительство" или же сборка происходит в файлах с немного нелепыми названиями, например [LevelOneSchedule](Assets/Scripts/InteractiveTutorial/LevelsTutorialsSchedules/LevelOneSchedule.cs) (можно было заменить буквы числами, но тогда я об этом не подумал):
```c#
IInteractiveTutorialStage stage3 = new InteractiveTutorialStage()
  .InsertCompletionCondition(new BallColoringCompleitionCondition(eightBall.BallController, BallColoring.Coloured))
  .InsertStepBackCondition(new BallUnavailableStepBackCondition(stashBall.BallController, stash, interactiveMessage))
  .InsertActions(
      new SwipePointerAction(pointer, stashBall.gameObject, eightBall.gameObject),
      new HighlightObjectAction(stashBall.gameObject, highlighter, Color.red),
      new HighlightObjectAction(eightBall.gameObject, highlighter),
      new MessageAction(interactiveMessage, EnumerableAnchor.MiddleUp, "Locale.InteractiveTutorial.1.HitEightBall"),
      new MessageAction(interactiveMessage, EnumerableAnchor.MiddleDown, new Vector2(0, 250), "Locale.InteractiveTutorial.FingerPower")
  )
  .Seal();

tutorial
  .InsertStages(stage1, stage2, stage3)
  .Seal()
  .StartTutorial();
```
#### О паттерне DI
Почему не Zenject? Потому что мне хотелось разобраться в том, как работает этот паттерн, и применить рефлексию на практике. И в целом было достаточно интересно попробовать придумать свою реализацию.
#### О паттерне EntryPoint
Я решил удостоить его короткой сноски здесь и не стал добавлять в резюме по нескольким причинам. Во-первых, он тесно связан с DI. Во-вторых, он ВРЕМЕННО работает как Singleton, ибо, как я упоминал ранее, он находится на стыке старой и новой архитектуры.
## Использованные библиотеки
* DOTween (абсолютно все анимации в игре)
* UniTask (используется в новой архитектуре вместо стандартного Coroutine)

## Интеграции
* Google UMP
* Unity In App Purchasing
* Unity LevelPlay
* Unity Analytics
* Firebase Analytics/Crashlytics
