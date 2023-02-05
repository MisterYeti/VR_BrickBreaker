using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelLayout
{
	[System.Serializable]
	public struct rowData
	{
		public ScriptableBrick[] row;
	}

	public rowData[] rows = new rowData[7]; 
}

