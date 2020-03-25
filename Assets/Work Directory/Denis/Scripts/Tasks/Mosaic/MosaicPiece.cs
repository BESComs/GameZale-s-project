using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Mosaic
{
	public class MosaicPiece : MonoBehaviour
	{
		private TaskMosaic _taskMosaic;
		public MosaicColor RequredColor;
		public bool SetOnStart = false;
		
		private MosaicColor _color;
		private Animate _anim;

		private void Awake()
		{
			_taskMosaic = GetComponentInParent<TaskMosaic>();
			_anim = new Animate(this, true);
		}

		private void Start()
		{
			if (!SetOnStart) return;
			_color =  RequredColor;
			_anim.SetColor(_color.GetColor());
		}
		
		private void OnMouseDown()
		{
			_color = _taskMosaic.activeColor;
			_anim.Colorize(_color.GetColor(), 0.5f);
		}

		public MosaicColor Color()
		{
			return _color;
		}
	}
}
