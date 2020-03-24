using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work_Directory.Denis.Scripts;

#pragma warning disable 4014

namespace Work_Directory.Denis.Scenes.OldGameSelectionMenu
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance;
        public Color color1, color2, color3;
        public Transform classesButton, categoriesButton, buttonHome;
        private TextMeshPro _buttonText1, _buttonText2;
        [HideInInspector]public Vector2 scenePositionOfCategories, scenePositionOfGames;
        [HideInInspector]public int sceneIdOfCategories, sceneIdOfGames;
        [HideInInspector] public List<SceneOfChoice> categories = new List<SceneOfChoice>();
        [HideInInspector] public List<SceneOfChoice> games = new List<SceneOfChoice>();
        [HideInInspector] public SceneOfChoice classes = new SceneOfChoice();
        private SpriteRenderer _homeSr, _classSr, _categoriesSr;
        public bool inTask;
        private void Awake()
        {
            _homeSr = buttonHome.GetComponent<SpriteRenderer>();
            _classSr = classesButton.GetComponent<SpriteRenderer>();
            _categoriesSr = categoriesButton.GetComponent<SpriteRenderer>();
            _buttonText2 = categoriesButton.GetChild(0).GetComponent<TextMeshPro>();
            _buttonText1 = classesButton.GetChild(0).GetComponent<TextMeshPro>();
            classesButton.gameObject.SetActive(false);
            categoriesButton.gameObject.SetActive(false);
            _homeSr.color = color1;
            Instance = this;
        }

        public async Task TransitionBetweenScenes(int scene1Id, int scene2Id, FromTo fromTo, Vector2 selectScenePos, bool doubleTransition = false, Item item = null)
        {
            inTask = true;   
            SceneOfChoice tmpScene = null, tmpScene2 = null;
            switch (fromTo)
            {
                case FromTo.ClassesCategories:
                    scenePositionOfCategories = selectScenePos;
                    sceneIdOfCategories = scene2Id;
                    foreach (var sceneOfChoice in categories)
                    {
                        if (sceneOfChoice.sceneId != scene2Id) continue;
                        tmpScene = sceneOfChoice;
                        break;
                    }
                    if (tmpScene != null)
                    {
                        new SmoothColoring(_homeSr, _homeSr.color, color2, AnimationCurve.EaseInOut(0, 0, .85f, 1))
                            .RunTask();
                        new SmoothColoring(_classSr, _classSr.color, color1, AnimationCurve.EaseInOut(0, 0, .85f, 1))
                            .RunTask();
                        classesButton.gameObject.SetActive(true);
                        new Scripts.Move(classesButton, new Vector2(-5.5f,classesButton.position.y),false,AnimationCurve.EaseInOut(0,0,.85f,1) ).RunTask();
                        await LoadNewSceneAnimation(tmpScene,classes,selectScenePos);   
                        if(item != null)
                            _buttonText1.text = item.itemDescription; 
                    }
                    break;
                case FromTo.CategoriesGames:
                    scenePositionOfGames = selectScenePos;
                    sceneIdOfGames = scene2Id;
                    foreach (var sceneOfChoice in games)
                    {
                        if (sceneOfChoice.sceneId != scene2Id) continue;
                        tmpScene = sceneOfChoice;
                        break;
                    }
                    foreach (var sceneOfChoice in categories)
                    {
                        if (sceneOfChoice.sceneId != scene1Id) continue;
                        tmpScene2 = sceneOfChoice;
                        break;
                    }
                    if (tmpScene != null && tmpScene2 != null)
                    {
                        new SmoothColoring(_homeSr, _homeSr.color, color3, AnimationCurve.EaseInOut(0, 0, .85f, 1))
                            .RunTask();
                        new SmoothColoring(_classSr, _classSr.color, color2, AnimationCurve.EaseInOut(0, 0, .85f, 1))
                            .RunTask();
                        new SmoothColoring(_categoriesSr, _categoriesSr.color, color1, AnimationCurve.EaseInOut(0, 0, .85f, 1))
                            .RunTask();
                        categoriesButton.gameObject.SetActive(true);
                        new Scripts.Move(categoriesButton, new Vector2(-3f,categoriesButton.position.y),false,AnimationCurve.EaseInOut(0,0,.85f,1) ).RunTask();
                        await LoadNewSceneAnimation(tmpScene, tmpScene2, selectScenePos);
                        if(item != null)
                            _buttonText2.text = item.itemDescription; 
                    }
                    break;
                case FromTo.GamesGame:
                    foreach (var sceneOfChoice in games)
                    {
                        if(sceneOfChoice.sceneId != scene1Id)continue;
                        tmpScene = sceneOfChoice;
                        break;
                    }

                    if (tmpScene != null)
                        await new Scale(tmpScene.parent, true, true).RunTask();
                    if (item != null)
                        LoadGame(item.gameScenePath);
                    break;
                case FromTo.GamesCategories:
                    foreach (var sceneOfChoice in games)
                    {
                        if (sceneOfChoice.sceneId != scene2Id) continue;
                        tmpScene = sceneOfChoice;
                        break;
                    }
                    foreach (var sceneOfChoice in categories)
                    {
                        if (sceneOfChoice.sceneId != scene1Id) continue;
                        tmpScene2 = sceneOfChoice;
                        break;
                    }
                    if (tmpScene != null && tmpScene2 != null)
                    {
                        _buttonText2.text = "";
                        new SmoothColoring(_homeSr, _homeSr.color, color2, AnimationCurve.EaseInOut(0, 0, doubleTransition ? .375f : .85f, 1))
                            .RunTask();
                        new SmoothColoring(_classSr, _classSr.color, color1, AnimationCurve.EaseInOut(0, 0, doubleTransition ? .375f : .85f, 1))
                            .RunTask();

                        new Scripts.Move(categoriesButton, new Vector2(-8f,categoriesButton.position.y),false,AnimationCurve.EaseInOut(0,0,doubleTransition ? .375f : .85f,1) ).RunTask();
                        await LoadNewSceneReverseAnimation(tmpScene, tmpScene2, selectScenePos, doubleTransition);
                        categoriesButton.gameObject.SetActive(false);
                    }
                    break;
                case FromTo.CategoriesClasses:
                    tmpScene2 = classes;
                    foreach (var sceneOfChoice in categories)
                    {
                        if (sceneOfChoice.sceneId != scene2Id) continue;
                        tmpScene = sceneOfChoice;
                        break;
                    }
                    if (tmpScene != null && tmpScene2 != null)
                    {
                        new SmoothColoring(_homeSr, _homeSr.color, color1, AnimationCurve.EaseInOut(0, 0, doubleTransition ? .375f : .85f, 1))
                            .RunTask();
                        _buttonText1.text = "";
                        new Scripts.Move(classesButton, new Vector2(-10f,classesButton.position.y),false,AnimationCurve.EaseInOut(0,0,doubleTransition ? .375f : .85f,1) ).RunTask();
                        await LoadNewSceneReverseAnimation(tmpScene, tmpScene2, selectScenePos, doubleTransition);
                        classesButton.gameObject.SetActive(false);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fromTo), fromTo, null);
            }

            inTask = false;
        }

        public void ToHome()
        {
            var tmpScene = new SceneOfChoice();
            var tmpScene2 = new SceneOfChoice();
            foreach (var sceneOfChoice in games)
            {
                if (sceneOfChoice.sceneId != sceneIdOfGames) continue;
                tmpScene = sceneOfChoice;
                break;
            }
            foreach (var sceneOfChoice in categories)
            {
                if (sceneOfChoice.sceneId != sceneIdOfCategories) continue;
                tmpScene2 = sceneOfChoice;
                break;
            }
            _buttonText2.text = _buttonText1.text = "";
            categoriesButton.position = new Vector2(-8f, categoriesButton.position.y); 
            tmpScene2.parent.localScale /= 7f;
            tmpScene2.parent.position = Vector2.zero;
            var sr = tmpScene2.parent.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in sr)
            {
                var spriteRendererColor = spriteRenderer.color;
                spriteRendererColor.a = .75f;
                spriteRenderer.color = spriteRendererColor;
            }
            tmpScene.parent.gameObject.SetActive(false);
            categoriesButton.gameObject.SetActive(false);
            _classSr.color = color1;
            _homeSr.color = color1;
            classesButton.position = new Vector2(-10f,classesButton.position.y);
            classesButton.gameObject.SetActive(false);
            classes.parent.gameObject.SetActive(true);
            classes.parent.localScale /= 7f;
            classes.parent.position = Vector2.zero;
            sr = classes.parent.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in sr)
            {
                var spriteRendererColor = spriteRenderer.color;
                spriteRendererColor.a = .75f;
                spriteRenderer.color = spriteRendererColor;
            }
            tmpScene2.parent.gameObject.SetActive(false);
        }        

        private async void LoadGame(string gameName)
        {
            inTask = true;
            var tmpScene = SceneManager.LoadSceneAsync(gameName, LoadSceneMode.Additive);
            await new WaitWhile(() => tmpScene.progress < .9f);
            transform.GetComponentInParent<GamesLauncher>().gameObject.SetActive(false);
            await new WaitWhile(() => !tmpScene.isDone);
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(2));
            inTask = false;
        }

        private async Task LoadNewSceneReverseAnimation(SceneOfChoice sceneOfChoice1, SceneOfChoice sceneOfChoice2, Vector2 selectScenePos, bool doubleSpeed = false)
        {
            inTask = true;
            sceneOfChoice2.parent.gameObject.SetActive(true);
            var time = doubleSpeed ? .425f : .85f;
            new Scripts.Scaling(sceneOfChoice2.parent, false, AnimationCurve.EaseInOut(0, 1, time, 1 / 7f)).RunTask();
            new Scripts.Move(sceneOfChoice2.parent, Vector2.zero,false, AnimationCurve.EaseInOut(0, 0, time, 1))
                .RunTask();
            new Fade(sceneOfChoice2.parent, Mode.In,AnimationCurve.EaseInOut(0,0,time,.75f)).RunTask();
            new Scripts.Move(sceneOfChoice1.parent,selectScenePos ,true,AnimationCurve.EaseInOut(0,0,time,1)).RunTask();
            await new Scale(sceneOfChoice1.parent,true,true,Mode.Out,AnimationCurve.EaseInOut(0,0,time,1)).RunTask();
            sceneOfChoice1.parent.gameObject.SetActive(false);
            inTask = false;
        }

        private async Task LoadNewSceneAnimation(SceneOfChoice sceneOfChoice1, SceneOfChoice sceneOfChoice2, Vector2 selectScenePos)
        {
            inTask = true;
            new Scripts.Scaling(sceneOfChoice2.parent, false ,AnimationCurve.EaseInOut(0,1,.85f,7f)).RunTask();
            new Scripts.Move(sceneOfChoice2.parent, -selectScenePos * 7f, false ,AnimationCurve.EaseInOut(0,0,.85f,1))
                .RunTask();
            new Fade(sceneOfChoice2.parent,Mode.Out,AnimationCurve.EaseInOut(0,0,.85f,1)).RunTask();
            sceneOfChoice1.parent.gameObject.SetActive(true);
            var startPos = sceneOfChoice1.parent.position;
            sceneOfChoice1.parent.position = selectScenePos;
            new Scripts.Move(sceneOfChoice1.parent, startPos, false,AnimationCurve.EaseInOut(0,0,.85f,1)).RunTask();
            if (sceneOfChoice1.parent.localScale.x > .9f)
                sceneOfChoice1.parent.localScale /= 7f;
            await new Scripts.Scaling(sceneOfChoice1.parent,false,AnimationCurve.EaseInOut(0,1,.85f,7f)).RunTask();
            sceneOfChoice2.parent.gameObject.SetActive(false);
            inTask = false;
        }
    }
    
    public enum FromTo
    {
        ClassesCategories,
        CategoriesGames,
        GamesGame,
        GamesCategories,
        CategoriesClasses
    }
}