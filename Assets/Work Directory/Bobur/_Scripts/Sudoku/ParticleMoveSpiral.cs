using UnityAsync;
using UnityEngine;

namespace _Scripts.Sudoku
{
	public class ParticleMoveSpiral : MonoBehaviour {
		public async void RunParticle()
		{
			float size = 0f;
			float time = 0f;
			while (!(time >= 3.5f))
			{

				float xPos = size * Mathf.Sin(size) / 30;
				float yPos = size * Mathf.Cos(size) / 30;
				
				size += Time.deltaTime * 12;
				time += Time.deltaTime;
				
				transform.localPosition = new Vector3(transform.position.x + xPos, transform.position.y + yPos, 1f);
				
				
				await Await.NextUpdate();
			}
		}
	}
}