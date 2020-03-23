using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Work_Directory.Bobur._Scripts.IQSha_Games.Interfaces;
using _Scripts.Utility;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#pragma warning disable 4014

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Find_Difference
{
    [Serializable]
    public class FindDifference : IGame
    {
        //заранее должен быть создан префаб с дочерними обьектами на которых весят спрайты с отличиями тогда при создании
        //префаб накладывается на одно из изображений и игрок видит две картинки с отличиями
        private Camera _cameraMain;
        [TitleGroup("Find Difference game properties")]
        //Изображения
        public Sprite Image;
        //количество отличий
        public int NumberOfDifferences;
        //спрайт точки
        public Sprite pointSprite;
        //цвет точки
        public Color PointColor;
        //точки установленные игроком
        [HideInInspector]public List<Dif> Points;
        //скрипт который находится на обьектах которые лежат в местах отличия картинок
        [HideInInspector]public List<ClickEvent> ClickEvents;
        //желаемый размер спрайта 
        //спрайт будет максимально растянут до ImageSize с учетом соотношения сторон 
        public Vector2 ImageSize;
        //позиция оригинального изображения
        public Vector2 OriginalImagePos;
        //позиция изображения с отличиями
        public Vector2 ImageWithDifferencePos;
        //размер изображения
        private Vector2 _scaleImg;
        //Размер точки
        public Vector3 pointSize;
        //точка при нажатии на картинку
        private GameObject pointObject;
        //префаб изображения с отличиями
        public Transform ImageWithDifferencePref;
        //родительский обьект
        private GameConstructor gameConstructor;
        //родительский обьект
        private GameObject parentObject;
        //анимированные обьекты
        private List<Renderer> animatedObjects;
        //можно ли кликать
        private bool canClick;

        //метод вызывающийся в начале игры
        public IGame Init(GameConstructor constructor, GameObject parent)
        {
            gameConstructor = constructor;
            parentObject = new GameObject("Find Difference Elements");
            parentObject.transform.SetParent(parent.transform);
            animatedObjects = new List<Renderer>();
            canClick = true;
            
            _cameraMain = Camera.main;
            //создание начальной сцены
            CreateStartScene();
            gameConstructor.SetOnUpdate(Update);
            
            return this;
        }
        
        private void CreateStartScene()
        {
            ClickEvents = new List<ClickEvent>();
            Points = new List<Dif>();
            //создание изображения с отличиями
            var img1 = Object.Instantiate(ImageWithDifferencePref, parentObject.transform, true);
            
            img1.name = "Image With Difference";
            img1.gameObject.SetActive(false);
            //настройки спрайта
            var sr1 = img1.gameObject.AddComponent<SpriteRenderer>();
            sr1.sortingLayerName = SortingLayer.layers[SortingLayer.layers.Length - 2].name;
            sr1.sortingOrder = 0;
            animatedObjects.Add(sr1);
            //добовление дочерних обьектов к списку анимированных обьектов 
            foreach (Transform transform in img1.transform)
            {
                if (!(transform.GetComponent<SpriteRenderer>() is Renderer renderer)) continue;
                renderer.sortingLayerName = SortingLayer.layers[SortingLayer.layers.Length - 2].name;
                renderer.sortingOrder = 1;
                animatedObjects.Add(renderer);
            }
            img1.localPosition = ImageWithDifferencePos;
            //создание оригинального изображения
            var img2 = new GameObject("Original Image");
            img2.transform.SetParent(parentObject.transform);
            img2.SetActive(false);
            
            var sr2 = img2.AddComponent<SpriteRenderer>();
            animatedObjects.Add(sr2);
            //настройки спрайтов
            sr1.drawMode = SpriteDrawMode.Sliced;
            sr2.drawMode = SpriteDrawMode.Sliced;
            img2.transform.localPosition = OriginalImagePos;
            sr1.sprite = Image;
            sr2.sprite = Image;
            //изменения размера
            var ratio = Mathf.Min(ImageSize.x / sr1.size.x, ImageSize.y / sr2.size.y);
            var size = sr1.size;
            size *= ratio;
            sr1.size = size;
            sr2.size *= ratio;
            _scaleImg = size;
            var tmpList = new List<int>();
        
            for (var i = 0; i < img1.childCount; ++i)
            {
                tmpList.Add(i);
                img1.GetChild(i).gameObject.SetActive(false);
            }
            //берет NumberOfDifferences рандомных индексов
            tmpList = ShuffleIndex(NumberOfDifferences, tmpList);
            //активирует NumberOfDifferences отличий на картинке
            foreach (var i in tmpList)
            {
                img1.GetChild(i).gameObject.SetActive(true);
                //изменение размера изображений отличий в зависимости от изменения размера оригинального изображения
                var sr = img1.GetChild(i).GetComponent<SpriteRenderer>();
                sr.drawMode = SpriteDrawMode.Sliced;
                sr.transform.localScale *= ratio;
                //Добовление необходимых компонентов и изменение позиции в зависимости от изменения размера
                img1.GetChild(i).position = ImageWithDifferencePos + ratio * ((Vector2)img1.GetChild(i).position - ImageWithDifferencePos);
                var ce = img1.GetChild(i).gameObject.AddComponent<ClickEvent>();
                img1.GetChild(i).gameObject.AddComponent<PolygonCollider2D>();
                ce.id = i;
                ClickEvents.Add(ce);
            }
            //создания точки для копий
            pointObject = new GameObject("Point");
            pointObject.transform.SetParent(parentObject.transform);
            pointObject.transform.localScale = pointSize;
            pointSize.z = 0;
            var tmpSr = pointObject.AddComponent<SpriteRenderer>();
            tmpSr.sprite = pointSprite;
            tmpSr.sortingLayerName = SortingLayer.layers[SortingLayer.layers.Length - 2].name;
            tmpSr.sortingOrder = 3;
            
            pointObject.SetActive(false);
        }
    
        // Возвращает count элементов из списка indexes
        private static List<int> ShuffleIndex(int count, List<int> indexes)
        {
            var shuffleIndexList = indexes.GetRange(0, indexes.Count);
            var indexCount = shuffleIndexList.Count;
     
            for (var i = 0; i < indexCount - count; ++i)
                shuffleIndexList.Remove(shuffleIndexList[(int) (Random.value * shuffleIndexList.Count)]);
        
            return shuffleIndexList;
        }

        private void Update()
        {
            if(!canClick)
                return;
            //позиция мышки
            var tmpPos = _cameraMain.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
            var tmpVec = (Vector2)tmpPos - ImageWithDifferencePos;
            //если курсор не находится на изображении
            if (!Input.GetKeyDown(KeyCode.Mouse0) || !(_scaleImg.x / 2f > Mathf.Abs(tmpVec.x)) ||
                !(_scaleImg.y / 2f > Mathf.Abs(tmpVec.y)))
                return;

            //если курсор находится рядом с ранее поставленной точкой то удалить точку
            foreach (var point in Points)
            {
                var dist = point.Point.position - tmpPos;
                dist.z = 0;
                if (!((dist).magnitude < pointSize.magnitude)) continue;
                Object.Destroy(point.Point.gameObject);
                Points.Remove(point);
                gameConstructor.GetNextButton.interactable = (Points.Count == NumberOfDifferences);
                return;
            }
            //если количество точек равно количеству отличий то удалить первую поставленную точку перед тем как поставить новую
            if (Points.Count == NumberOfDifferences)
            {
                Object.Destroy(Points[0].Point.gameObject);
                Points.Remove(Points[0]);
            }

            //создание новой точки
            var newPointOb = Object.Instantiate(pointObject, parentObject.transform, true);
            newPointOb.SetActive(true);
            
            var tmpPoint = new Dif
            {
                PointId = -1, Point = newPointOb.transform
            };
            tmpPoint.Point.position = tmpPos;
            Points.Add(tmpPoint);
            tmpPoint.Point.GetComponent<SpriteRenderer>().color = PointColor;
            //установить id если точка находится над отличием
            foreach (var clickEvent in ClickEvents)
            {
                if (!clickEvent.mouseOver) continue;
                tmpPoint.PointId = clickEvent.id;
                break;
            }
            //сделать кнопку проверки активной если количество поставленных точек равно количеству отличий
            gameConstructor.GetNextButton.interactable = (Points.Count == NumberOfDifferences);
        }

        #region IGame
        //проверка ответа 
        public bool CheckAnswer()
        {
            //если все точки находятся на спрайтах с отличиями и нет пары точек находящихся на одном и том же отличии
            if (Points.Count != NumberOfDifferences) return false;
            for (var i = 0; i < Points.Count; ++i)
            {
                if (Points[i].PointId == -1) return false;
                for (var j = i + 1; j < Points.Count; ++j)
                    if (Points[i].PointId == Points[j].PointId)
                        return false;
            }

            return true;
        }

        //появление на сцену
        public async Task SceneIn()
        {
            foreach (var animatedObject in animatedObjects)
            {
                animatedObject.gameObject.SetActive(true);
                AnimationUtility.FadeIn(animatedObject);
            }

            await new WaitForSeconds(1f);
        }

        //исчезновение из сцены
        public async Task SceneOut()
        {
            AnimationUtility.ScaleOut(parentObject);

            await new WaitForSeconds(1f);
        }

        public void ChangeStateAfterAnswer() => canClick = false;

        #endregion
    }

    public class Dif
    {
        public Transform Point;
        public int PointId;
    }
}