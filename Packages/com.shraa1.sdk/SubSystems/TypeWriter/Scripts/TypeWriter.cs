using System;
using System.Collections.Generic;
using BaseSDK.Extension;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaseSDK.Utils {
	public class TypeWriter : MonoBehaviour {
		[Multiline] public string textToDisplay;
		public float typewriterTimeDelay;
		public int textMeshSortingOrder;
		public bool autoStartTyping;
		public List<AudioSource> audioSource = new List<AudioSource>();

		private Action onComplete;
		private TextMesh textMesh;
		private TextMeshProUGUI tmp;
		private TextMeshPro tmpro;
		private Text text;
		private Tweener tweener;

		private int TotalNewLineCount => textToDisplay.Occurrences(GameConstants.LINE_BREAK);

		private void Awake () {
			text = GetComponent<Text>();
			textMesh = GetComponent<TextMesh>();
			tmp = GetComponent<TextMeshProUGUI>();
			tmpro = GetComponent<TextMeshPro>();
			if (autoStartTyping) StartTyping();
		}

		public void StartTyping (Action onComplete = null) {
			StartTyping(0);
			if (textMesh != null)
				GetComponent<MeshRenderer>().sortingOrder = textMeshSortingOrder;
			this.onComplete = onComplete;
		}

		private void StartTyping (int index) {
			if (index == textToDisplay.Length) {
				onComplete?.Invoke();
				return;
			}
			if (index < textToDisplay.Length && textToDisplay[index] == GameConstants.LINE_BREAK)
				index++;

			string s = textToDisplay.Substring(0, index + 1);
			int count = TotalNewLineCount - s.Occurrences(GameConstants.LINE_BREAK);
			for (int i = 0; i < count; i++)
				s += GameConstants.LINE_BREAK;

			if (textMesh != null) textMesh.text = s;
			else if (text != null) text.text = s;
			else if (tmp != null) tmp.text = s;
			else if (tmpro != null) tmpro.text = s;

			if (audioSource.Count > 0)
				audioSource.Random().Play();

			index++;

			tweener = Utilities.WaitBeforeExecuting(typewriterTimeDelay, () => StartTyping(index));
		}

		public void StopTyping () => tweener?.Kill();
	}
}