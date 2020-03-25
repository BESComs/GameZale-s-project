using System.Collections.Generic;
using UnityEngine;

namespace Tasks.Tangram
{
	public class TangramField : MonoBehaviour
	{
		public List<TangramPiece> pairedPieces = new List<TangramPiece>();
		public bool alignPieces = true;
		public bool parentPieces;
	}
}
