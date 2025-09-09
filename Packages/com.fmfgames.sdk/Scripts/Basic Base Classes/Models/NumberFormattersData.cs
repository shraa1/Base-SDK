using System;
using System.Collections.Generic;
using UnityEngine;

namespace FMF.DinoClickr.Models {
	[CreateAssetMenu(fileName = "Number Formatter Data", menuName = "Number Formatter Data File")]
	public class NumberFormattersData : ScriptableObject {
		public List<NumberFormattersDataObject> Data = new();
	}

	[Serializable]
	public struct NumberFormattersDataObject {
		public double MinValue;
		public double MaxValue;
		public string Formatter;
		public string ShortText;
		public string LongText;
		public double CachedMultiplier;
	}
}