using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match;
using Work_Directory.Denis.Scripts;

#pragma warning disable 4014
namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    /*
     * main script in latest GameLauncher responsible for transitions between categories,
     * running game scenes and scrolling
     * all objects that have components "folder controller" communicate through this script
     */
    public class LauncherController : MonoBehaviour
    {
        // trigger mouse in line if mouseOnLine is true do not move between categories
        public bool mouseOnLine;
        public static LauncherController Instance;
        //button that returns the user from the game selection to the category selection
        public Transform backButton;
        public Transform currentSelectGames;
        //UI element - sprite which is on top
        public Transform line;
        //Object in Scene which contains all the games
        public FolderController gamesHolder;
        //root folder containing all categories
        public FolderController startFolder;
        public FolderController currentFolder;
        //is used to check whether any async actions are performed on the gameLauncher scene
        public bool inTask;
        //checks selected game or category
        private bool selectGame;
        //scrolling - if player scroll games list
        //notScrolling - is true if games list is short to scrolling and player can't scrolling this game list
        private bool scrolling, notScrolling;
        //how strong was it scrolled up or down
        private float direction;
        //current position in games list
        private float currentHeight;
        //scrolling shift in frame
        private float  deltaShift;
        private Vector3 currentMousePos;
        //latest element position in games list and current selected games list length, used to prevent endless scrolling
        private Vector2 lastElement;
        private float gameListLength;
        private void Awake()
        {
            Instance = this;
            currentFolder = startFolder;
            gamesHolder.GetComponentsInChildren<FolderController>();
        }

        private void Start()
        {
            for (var i = 0; i < startFolder.innerFolders.Count; i++)
            {
                gamesHolder.innerFolders[i].folderId = startFolder.innerFolders[i].folderId =  i;
            }
        }
        
        //переход между выбором категорий и выбором игр и обратно 
        public async void MoveToFolder(bool isGame, FolderController folder)
        {
            inTask = true;
            if(isGame)
            {
                await StartGame(folder);
                mouseOnLine = false;
                inTask = false;
                return;
            }
            deltaShift = 0;
            scrolling = false;
            //проигрывание всех анимаций включение и выключение объектов для перехода из выбора категорий в выбор игр 
            currentMousePos = Input.mousePosition;
            
            backButton.gameObject.SetActive(true);
            var tmp = gamesHolder.transform.GetChild(folder.folderId);
            tmp.gameObject.SetActive(true);
            currentSelectGames = tmp;
            if(folder.categoriesName != null)
            {
                new ScaleIn(folder.categoriesName, AnimationCurve.EaseInOut(0, 0, .75f, 1)).RunTask();
            }
            line.gameObject.SetActive(true);
            new Fade(line,Mode.In,AnimationCurve.EaseInOut(0, 0, .75f, 1)).RunTask();
            new GameListLoadAnimation(currentSelectGames,AnimationCurve.EaseInOut(0,0,.75f,1 )).RunTask();
            new ScaleIn(backButton, AnimationCurve.EaseInOut(0, 0, .75f, 1)).RunTask();
            await new UnLoadAnimation(currentFolder.transform, AnimationCurve.EaseInOut(0, 0, .75f, 1),folder.transform.position).RunTask();
            selectGame = true;
            foreach (var currentFolderInnerFolder in currentFolder.innerFolders)
                currentFolderInnerFolder.gameObject.SetActive(false);
            
            foreach (var folderInnerFolder in folder.innerFolders)
                folderInnerFolder.gameObject.SetActive(true);
            
            currentFolder = folder;
            if(currentSelectGames.childCount != 0)
            {
                lastElement = currentSelectGames.GetChild(currentSelectGames.childCount - 1).localPosition;
            }
            //переменные используемые для прокрутки
            gameListLength = 3.5f - lastElement.y;
            currentHeight = 0;
            direction = 0;
            notScrolling = gameListLength < 8.25f;
            mouseOnLine = inTask = false;
            
        }
        
        //прокрутка за один кадр
        private void Scrolling()
        {
            if(!selectGame || inTask || notScrolling) return;
            //если игрок коснулся экрана и текущая высота не выходит за границы списка игр осуществить прокрутку
            if (scrolling && currentHeight + deltaShift < gameListLength + 2 && currentHeight + deltaShift > -3)
            {
                currentHeight += deltaShift;
                currentSelectGames.localPosition += new Vector3(0, deltaShift, 0);
                direction = deltaShift;
            }
            //возврат фокуса если текущая высота вышла за границы списка игр
            else if (currentHeight < 0 && !scrolling)
            {
                currentHeight += Time.deltaTime * 5;
                currentSelectGames.localPosition = new Vector3(0, currentHeight, 0);
                direction = 0;
            }
            else if (currentHeight > gameListLength && !scrolling)
            {
                currentHeight -= Time.deltaTime * 5;
                currentSelectGames.localPosition = new Vector3(0, currentHeight, 0);
                direction = 0;
            }
            //прокрутка по инерции
            else if(Mathf.Abs(direction) > Time.deltaTime && !scrolling)
            {
                direction = .95f * direction - Mathf.Sign(direction) * Time.deltaTime;
                currentHeight += direction;
                currentSelectGames.localPosition += new Vector3(0, direction, 0);
            }        
        }

        private void Update()
        {
            deltaShift = (Input.mousePosition.y - currentMousePos.y) / 2f * Time.deltaTime;
            currentMousePos = Input.mousePosition;
            scrolling = Input.GetKey(KeyCode.Mouse0);
            Scrolling();
        }

        //Загрузка игры
        private async Task StartGame(FolderController game)
        {
            scrolling = false;
            inTask = true;
            var tmpScene = SceneManager.LoadSceneAsync(game.gamePath, LoadSceneMode.Additive);
            //ожидание загрузки сцены 
            var time = Time.realtimeSinceStartup;
            await new WaitWhile(() => !tmpScene.isDone);
            await new WaitForUpdate();
            //устанавливает выбранную сцену как главную
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
            inTask = false;
            //деактивация сцены "GamesLauncher"
            gameObject.SetActive(false);
        }

        public async Task StartGame(LessonData lessonData)
        {
            const int delayMilliseconds = 100;
            scrolling = false;
            inTask = true;
            var loadProgress = SceneManager.LoadSceneAsync(lessonData.scenePath, LoadSceneMode.Additive);
            await SceneLoadProgress.Instance.StartLoadingProgress(loadProgress);
            await Task.Delay(delayMilliseconds);
            //устанавливает выбранную сцену как главную
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
            inTask = false;
            //деактивация сцены "GamesLauncher"
            gameObject.SetActive(false);
        }
       
        
        
        //Возврат из выбора игр в выбор категорий
        public async void GoBack()
        {
            if(inTask) return;
            inTask = true;
            selectGame = false;
            //проигрывание всех анимаций включение и выключение объектов для перехода из выбора игр в выбор категорий
            //Возврат переменных в начальное состояние
            foreach (var folderInnerFolder in startFolder.innerFolders)
                folderInnerFolder.gameObject.SetActive(true);
            
            if(currentFolder.categoriesName != null)
            {
                new ScaleOut(currentFolder.categoriesName, AnimationCurve.EaseInOut(0, 0, .75f, 1)).RunTask();
            }
            var tmp = gamesHolder.transform.GetChild(currentFolder.folderId);
            new Fade(line,Mode.Out,AnimationCurve.EaseInOut(0, 0, .75f, 1)).RunTask();
            new ScaleOut(backButton, AnimationCurve.EaseInOut(0, 0, .75f, 1)).RunTask();
            new LoadAnimation(startFolder.transform, AnimationCurve.EaseInOut(0, 0, .75f, 1)).RunTask();
            await new GameListUnLoadAnimation(tmp,AnimationCurve.EaseInOut(0,0,.75f,1 ),currentFolder.startPos).RunTask();
            line.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            foreach (var currentFolderInnerFolder in currentFolder.innerFolders)
            {
                currentFolderInnerFolder.gameObject.SetActive(false);
            }
            tmp.gameObject.SetActive(false);
            currentFolder = startFolder;
            mouseOnLine = false;
            inTask = false;
        }
    }
}