//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BaseSDK.Extension;
using TMPro;

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
		private Text text;
		private Tweener tweener;

		private int TotalNewLineCount => textToDisplay.Occurrences(GameConstants.LINE_BREAK);

		private void Awake() {
			text = GetComponent<Text>();
			textMesh = GetComponent<TextMesh>();
			tmp = GetComponent<TextMeshProUGUI>();
			if (autoStartTyping) StartTyping();
		}

		public void StartTyping(Action onComplete = null) {
			StartTyping(0);
			if (textMesh != null)
				GetComponent<MeshRenderer>().sortingOrder = textMeshSortingOrder;
			this.onComplete = onComplete;
		}

		private void StartTyping(int index) {
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

			audioSource.Random().Play();

			index++;

			tweener = Utilities.WaitBeforeExecuting(typewriterTimeDelay, () => StartTyping(index));
		}

		public void StopTyping() => tweener?.Kill();
	}
}