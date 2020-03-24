using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Mosaic
{
	public class MosaicColor : MonoBehaviour
	{
		private TaskMosaic _taskMosaic;
		private SpriteRenderer _sprite;
		private Animate _anim;

		private void Awake()
		{
			_taskMosaic = GetComponentInParent<TaskMosaic>();
			_sprite = GetComponent<SpriteRenderer>();
			_anim = new Animate(this, true);
		}
		
		private void OnMouseDown()
		{
			_taskMosaic.activeColor.Selected(false);
			_taskMosaic.activeColor = this;
			Selected(true);
		}

		public void Selected(bool value)
		{
			_anim.Scale((value) ? 1.4f : 1);
		}

		public Color GetColor()
		{
			return _sprite.color;
		}
	}
}
