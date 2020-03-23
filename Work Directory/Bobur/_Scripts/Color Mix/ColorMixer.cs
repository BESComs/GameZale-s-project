using System;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;
using Work_Directory.Bobur._Scripts.Color_Mix;

namespace _Scripts.Color_Mix
{
	public class ColorMixer : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _color1;
		[SerializeField] private SpriteRenderer _color2;
		private int colors;
		private Color defaultColor1;
		private Color defaultColor2;

		void Start()
		{
			defaultColor1 = _color1.color;
			defaultColor2 = _color2.color;
		}
		
		public async void AddColor(Color color)
		{
			colors++;
			SetColor(colors, color);
			if (colors == 2)
			{
				await Task.Delay(TimeSpan.FromSeconds(1.5f));
				Color mixedColor = MixColors(_color1.color, _color2.color);
				
				ColorLerp(mixedColor);
				
				ColorMixGameController.Instance.isAnimating = true;
				if (mixedColor == Colors.EnumToColor(ColorMixGameController.Instance.GetCurrentColorToMix()))
				{
					//ColorMixGameController.Instance.RegisterLessonEnd();
					await Task.Delay(TimeSpan.FromSeconds(2f));
					ColorMixGameController.Instance.RegisterAnswer(true);
					ColorMixGameController.Instance.NextColor();
				}
				else
				{
					ColorMixGameController.Instance.RegisterAnswer(false);
					await ColorMixGameController.Instance.WrongAnswerEffect();
					await Task.Delay(TimeSpan.FromSeconds(1f));
				}
				
				await ResetMixer();
				ColorMixGameController.Instance.isAnimating = false;
			}
		}

		private async Task ResetMixer()
		{
			colors = 0;
			
			Color color1 = _color1.color;
			Color color2 = _color2.color;
			for (float i = 0; i < 1f; i += Time.deltaTime)
			{
				i = (i > 1f) ? 1f : i;
				_color1.color = Color.Lerp(color1, defaultColor1, i);
				_color2.color = Color.Lerp(color2, defaultColor2, i);
				await Await.NextUpdate();
			}
		}

		private void SetColor(int colorIndex, Color color)
		{
			switch (colorIndex)
			{
				case 1: _color1.color = color; break;
				case 2: _color2.color = color; break;
			}
		}

		private Color MixColors(Color color1, Color color2)
		{
			Color mixedColor = new Color();
			if (color1 == Colors.Yellow && color2 == Colors.Blue || color1 == Colors.Blue && color2 == Colors.Yellow)
				mixedColor = Colors.Green;
			else if (color1 == Colors.Red && color2 == Colors.Yellow || color1 == Colors.Yellow && color2 == Colors.Red)
				mixedColor = Colors.Orange;
			else if(color1 == Colors.Blue && color2 == Colors.White || color1 == Colors.White && color2 == Colors.Blue)
				mixedColor = Colors.LightBlue;
			else if (color1 == Colors.Red && color2 == Colors.Blue || color1 == Colors.Blue && color2 == Colors.Red)
				mixedColor = Colors.Purple;
			else if (color1 == Colors.Red && color2 == Colors.White || color1 == Colors.White && color2 == Colors.Red)
				mixedColor = Colors.Pink;
			else if (color1 == Colors.Black && color2 == Colors.Blue || color1 == Colors.Blue && color2 == Colors.Black)
				mixedColor = Colors.DarkBlue;
			else if (color1 == Colors.Black && color2 == Colors.White || color1 == Colors.White && color2 == Colors.Black)
				mixedColor = Colors.Gray;
			else if (color1 == Colors.White && color2 == Colors.Yellow || color1 == Colors.Yellow && color2 == Colors.White)
				mixedColor = Colors.LightYellow;
			else if (color1 == Colors.Red && color2 == Colors.Black || color1 == Colors.Black && color2 == Colors.Red)
				mixedColor = Colors.Burgundy;
			else
			{
				mixedColor = Colors.Gray;
				print("Что то пошло не так. Не могу найти подходящий цвет.");
			}
				
			return mixedColor;
		}

		private async Task ColorLerp(Color targetColor)
		{
			Color color1 = _color1.color;
			Color color2 = _color2.color;
			for (float i = 0; i < 1f; i += Time.deltaTime)
			{
				i = (i > 1f) ? 1f : i;
				_color1.color = Color.Lerp(color1, targetColor, i);
				_color2.color = Color.Lerp(color2, targetColor, i);
				await Await.NextUpdate();
			}
		}
		
	}
}
