#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Work_Directory.Bobur._Scripts.IQSha_Games;
using Work_Directory.Denis.Scripts;

namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    /*
     * При запуске сцены ScreenShooterScene автоматически по очереди загружаются сцены в том порядке в котором
     * они былои собраны методом GetGamesScene класса GetAllGamesScene
     * и делается скриншот
     * скрины сохраняются в папке C:\Users\User\Desktop\ScreenShoots
     */
    public class ScreenShoot : MonoBehaviour
    {
        public Transform findILesson;
        //размер изображения
        private const int ResWidth = 1920; 
        private const int ResHeight = 1280;
        
        public List<(string, int)> levelsCount;
        //перед запуском нужно заполнить лист методом GetGamesScene класса GetAllGamesScene
        public List<string> scenePath;
        
        private async void Awake()
        {
            if (gameObject.activeSelf == false) return;
            levelsCount = new List<(string, int)>();
            //ожидание ввода пробела
            await new WaitWhile(() => !Input.GetKeyDown(KeyCode.Space));
            var filePath = @"C:\Users\User\Desktop\ScreenShoots\New Text Document.txt";
            var sb = new StringBuilder();
            
            for (var i = 0; i < scenePath.Count; i++)
            {
                //загрузка игры
                await SceneManager.LoadSceneAsync(scenePath[i], LoadSceneMode.Additive);
                await new WaitForSeconds(.1f);
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
                /*
                 поиск кнопки в сцене которая отвечает за старт игры
                 ! Примечание на данный момент на всех сценах либо имеется кнопка с скриптом StartPlay
                 либо на старте игры имеются две кнопки 
                 1 кнопка выхода
                 2 кнопка старта
                 либо кнопоки нет
                */
                var startPlayButton = FindObjectOfType<StartPlay>();
                
                if (startPlayButton)
                    startPlayButton.GetComponent<Button>().onClick.Invoke();
                else
                {
                    if(FindObjectsOfType<GameConstructor>().Length == 0)
                    {
                        var buttons = FindObjectsOfType<Button>();
                        foreach (var b in buttons)
                        {
                            if (b.transform.GetComponent<ExitButton>() != null) continue;
                            b.onClick.Invoke();
                            break;
                        }
                    }
                }
                await new WaitForUpdate();
                var tmp = Instantiate(findILesson);
                sb.AppendLine(scenePath[i] + " " + tmp.GetComponent<FindILessonTimeObservable>().LessonCountOnScene());
                
                //Сделать скриншот сохранить скрин в C:\Users\User\Desktop\ScreenShoots и сжатый квадратный скрин в C:\Users\User\Desktop\ScreenShoots\Corp
                //todo await TakeScreenShoot(i);
                //Отгрузка игровой сцены и её ожидание
                await SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
              
            }
            File.WriteAllText(filePath, sb.ToString());
            
        }
        private static (string,string) ScreenShotName(int width, int height, int i) 
        {
            //возвращает имя файла и путь к нему
            //имя файла - путь _ разрешение _ дата _ время _ индекс
            return (@"C:\Users\User\Desktop\ScreenShoots" , $"/screen_{width}x{height}_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{i}.png");
        }

        private static async Task TakeScreenShoot(int i)
        {
            //пара путь имя
            var (item1, item2) = ScreenShotName(ResWidth, ResHeight, i);
            //сделать скрин
            ScreenCapture.CaptureScreenshot(item1 + item2);
            var screenShotTexture = new Texture2D(ResWidth, ResHeight);
            //сохранить в папку с выбранную папку C:\Users\User\Desktop\ScreenShoots
            var data = screenShotTexture.EncodeToPNG();
            File.WriteAllBytes(item1 + item2, data);
            //ожидание для сохранения и проигрывания анимаций 
            await new WaitForSeconds(5.4f);
            //берет последний сделанный скрин для сжатия и обрезки до квадрата
            var images = Directory.GetFiles(item1, "*.png");
            //путь к файлу
            var imagePath = images[images.Length - 1];
            //считать файл 
            var fileData = File.ReadAllBytes(imagePath);
            var tex = new Texture2D(ResWidth, ResHeight);
            tex.LoadImage(fileData);
            //квадрат по центру изображения размером высота X высота
            var pixels = tex.GetPixels(320, 0, ResHeight, ResHeight);
            //берет каждый пятый пиксель в строке и каждый пятый пиксель в столбце
            //новый пиксель равен сумме половине цвета центрального и половине среднего арифметического всех соседних цветов
            var newColors = new Color[ResHeight * ResHeight / 25];
            for (var j = 0; j < ResHeight / 5; j++)
            {
                for (var k = 0; k < ResHeight / 5; k++)
                {
                    var index = (j * 5 + 2) * ResHeight + k * 5 + 2;
                    newColors[j * ResHeight / 5 + k] = AverageColor(pixels, index);
                }
            }
            tex.Resize(ResHeight / 5, ResHeight / 5);
            tex.SetPixels(newColors);
            tex.Apply();
            //запись в C:\Users\User\Desktop\ScreenShoots\Corp
            var croppedTextureBytes = tex.EncodeToJPG(100);
            File.WriteAllBytes(item1 + @"\Crop" + item2, croppedTextureBytes);
        }

        private static Color AverageColor(IReadOnlyList<Color> pixels, int index)
        {
            var newColor = pixels[index] * .4f;
            for (var i = 0; i < 9; i++)
                newColor += pixels[index + ResHeight * (i < 3 ? -1  : i < 6 ? 0 : 1) + 1 * (i % 3 == 0 ? 1: i % 3 == 1 ? -1 : 0)] * 6f / 90f;
            return newColor;
        }
    }
}
#endif