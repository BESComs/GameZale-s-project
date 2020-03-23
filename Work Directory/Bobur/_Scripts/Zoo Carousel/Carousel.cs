using System;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;

namespace _Scripts.Zoo_Carousel
{
	public class Carousel : MonoBehaviour
	{

		[SerializeField] private Animal[] Placeholders;
		private AnimationCurve animCurve;
		
		// Use this for initialization
		void Start () {
			animCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		}
	
		// Update is called once per frame
		void Update () {
			
		}

		public async Task Spin()
		{
			for (float i = 0; i < 1f; i += Time.deltaTime / 6f)
			{
				i = (i > 1f) ? 1f : i;
				transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, animCurve.Evaluate(i) * 360f * -1f);

				for (int j = 0; j < Placeholders.Length; j++)
				{
					Placeholders[j].transform.parent.eulerAngles = new Vector3(Placeholders[j].transform.eulerAngles.x, Placeholders[j].transform.eulerAngles.y, 0f);
				}
				await Await.NextUpdate();
			}

			await Task.Delay(TimeSpan.FromSeconds(0.7f));
		}

		public Animal[] GetPlaceholders() => Placeholders;
	}
}
