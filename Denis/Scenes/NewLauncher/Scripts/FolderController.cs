using System.Collections.Generic;
using UnityEngine;

namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    /*
     * Ирархия папок:
     * 1 Корневая папка содержит папки категории
     * 2 Папки категории - содержат папки игр
     * 3 папки игры - являются листьями содержат информацию для запуска игр
     * Скрипт висит на всех вершинах иерархии
     * Содержит информацию для переходов и запуска игр
     * Скрипт передаёт информацию при нажатии LauncherController где далее она обрабатывается 
     */
    public class FolderController : MonoBehaviour
    {
        //ссылка на объект содержащий название категорий используется для перехода из выбора категорий в выбор игр
        public Transform categoriesName;
        //начальная позиция объекта используется во время переходов из игр в категории и обратно
        public Vector3 startPos;
        //ID категории или игры
        public int folderId;
        //путь к сцене если это игра. Используется для загрузки игры. Назначается скриптом GetAllGameScene метод SetPath 
        public string gamePath;
        //если это игра при нажатии на объект осуществляется запуск игры иначе осуществляется преход из выбора игр в выбор категорий и обратно 
        public bool isGame;
        //список внутренних папок
        public List<FolderController> innerFolders = new List<FolderController>();
        //пройденная дистанция фиксируется от момента нажатия до момента отжатия
        private Vector3 distance;
        //фиксирование нажатия
        private bool mouseDown;
        //позиция мышки в предыдущем кадре
        private Vector3 lastFrameMousePos;
        private void Awake()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var fc = transform.GetChild(i).GetComponent<FolderController>();
                if(fc != null)
                    innerFolders.Add(transform.GetChild(i).GetComponent<FolderController>());
            }
        }
        
        
        private void Start()
        {
            startPos = transform.position;
        }

        //(*) проверка на сдвиг от момента нажатия до отжатия на экран для корректной прокрутки списка игра
        private float timer;
        private void OnMouseDown()
        {
            timer = 0;
            mouseDown = true;
            distance = Vector3.zero;
            lastFrameMousePos = Input.mousePosition;
        }

        private void Update()
        {
            timer += Time.deltaTime * 10f;
            if(!mouseDown) return;
            distance += (Input.mousePosition - lastFrameMousePos) * Time.deltaTime;
            lastFrameMousePos = Input.mousePosition;
            
        }

        //если главный скрипт не выполняет ассинхронных операций и выполнено условие (*) осуществляет переход или запуск игры  
        private void OnMouseUp()
        {
            mouseDown = false;
            if(LauncherController.Instance.inTask || (Mathf.Abs(distance.x) + Mathf.Abs(distance.y)) * timer > 2 || LauncherController.Instance.mouseOnLine)
                return;
            LauncherController.Instance.MoveToFolder(isGame,this); 
        }
    }
}