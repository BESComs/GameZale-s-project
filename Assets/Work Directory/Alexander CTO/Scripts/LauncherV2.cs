using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work_Directory.Denis.Scenes.NewLauncher.Scripts;

public class LauncherV2 : MonoBehaviour
{
    private enum ViewState
    {
        GradeList,
        CategoryList,
        LessonList
    }

    public static LauncherV2 Instance { get; private set; }

    private ViewState currentViewState;

    public GameObject gradeScrollView;
    public GameObject categoryScrollView;
    public GameObject lessonScrollView;

    public Transform categoryContent;
    public Transform lessonContent;

    public CategoryView categoryPrefab;
    public LessonView lessonPrefab;

    public GradeView mainGrade;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OpenGrade(mainGrade);
    }

    public void OpenGrade(GradeView gradeView)
    {
        currentViewState = ViewState.CategoryList;
        gradeScrollView.SetActive(false);
        categoryScrollView.SetActive(true);
        foreach (Transform t in categoryContent)
            Destroy(t.gameObject);
        foreach (var category in gradeView.categories)
        {
            var categoryView = Instantiate(categoryPrefab, categoryContent);
            categoryView.Init(category);
        }
    }

    public void OpenCategory(CategoryData categoryData)
    {
        currentViewState = ViewState.LessonList;
        categoryScrollView.SetActive(false);
        lessonScrollView.SetActive(true);
        foreach (Transform t in lessonContent)
            Destroy(t.gameObject);
        foreach (var lesson in categoryData.lessons)
        {
            var lessonView = Instantiate(lessonPrefab, lessonContent);
            lessonView.Init(lesson);
        }

        LauncherBackButtonV2.Instance.Enable();
    }

    private bool loadingScene;

    public async void OpenLesson(LessonData lessonData)
    {
        if (AvailabilityVar.Instance.canPlay == false) return;
        
        if (loadingScene) return;

        loadingScene = true;
        const int delayMilliseconds = 100;
        var loadProgress = SceneManager.LoadSceneAsync(lessonData.scenePath, LoadSceneMode.Additive);
        await SceneLoadProgress.Instance.StartLoadingProgress(loadProgress);
        await Task.Delay(delayMilliseconds);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        gameObject.SetActive(false);
        loadingScene = false;
    }

    public void OnBackPressed()
    {
        switch (currentViewState)
        {
            case ViewState.GradeList:
                break;
            case ViewState.CategoryList:
                /*
                categoryScrollView.SetActive(false);
                gradeScrollView.SetActive(true);
                currentViewState = ViewState.GradeList;
                */
                break;
            case ViewState.LessonList:
                lessonScrollView.SetActive(false);
                categoryScrollView.SetActive(true);
                currentViewState = ViewState.CategoryList;
                LauncherBackButtonV2.Instance.Disable();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    
    [Obsolete("Теперь отправка статистики работает только в онлайн режиме, чтобы предотвратить подделку статистики")]
    public async void BootsOfTravelKotoriyChetireTisyachiStoit()
    {
        return;
        cts = new CancellationTokenSource();
        var token = cts.Token;
        while (true)
        {
            if (StatisticsCache.HasCachedStatistics())
            {
                await ServerRequests.PostCachedStatistics();
            }
            await Task.Delay(5000);

            if (token.IsCancellationRequested) return;
        }
    }

    private void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }

    private CancellationTokenSource cts;


    public void Enable()
    {
        gameObject.SetActive(true);
    }
}