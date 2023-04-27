﻿//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace BaseSDK.Utils {
    public class MessageBox : Singleton<MessageBox> {

        #region Variables
        private static Transform prefab;
        private const string Messagebox = "Messagebox";
        private const string ButtonStr = "Button";
        private const string TextStr = "Text";

        public List<GameObject> buttonsPanels = new List<GameObject>();
        public Image bg;
        public Text title;
        public Text body;
        public Font font;
        #endregion

        #region Instance override - Since we need to instantiate a prefab rather than just create a new gameobject
        private static MessageBox instance;
        new public static MessageBox Instance {
            get {
                if (instance == null) {
                    var allComponentsOfType = FindObjectsOfType<MessageBox>();
                    if (allComponentsOfType.Length > 0) {
                        instance = allComponentsOfType[0];
                        if (allComponentsOfType.Length > 1)
                            Debug.LogErrorFormat("We have a problem. Multiple instances of type {0} present in the scene", typeof(MessageBox));
                        return instance;
                    }

#if UNITY_EDITOR
                    Debug.LogWarningFormat($"An instance of {typeof(MessageBox)} is needed in the scene... Creating one right now");
#endif

                    var go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    go.name = prefab.name;
                    var component = go.Find(Messagebox).GetComponent<MessageBox>();
                    instance = component;
                }
                return instance;
            }
        }
        #endregion

        protected virtual void Awake() {
            if (font != null) {
                title.font = font;
                body.font = font;
            }
            Hide();
        }

        public static void SetPrefab(Transform pref) => prefab = pref;

        /// <summary>
        /// Shows a message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="bodyText"></param>
        /// <param name="onCompleteAction"></param>
        /// <param name="buttonsTexts"></param>
        public static void Show(string title, string bodyText, Action<sbyte> onCompleteAction = null, params string[] buttonsTexts) {
            Instance.gameObject.SetActive(true);
            Instance.bg.gameObject.SetActive(true);
            if (buttonsTexts.Length <= 5)
                Instance.buttonsPanels.ForEach(x => {
                    var setActive = Instance.buttonsPanels.IndexOf(x) == buttonsTexts.Length;
                    x.SetActive(setActive);
                    if (setActive) {
                        for (sbyte i = 0; i < buttonsTexts.Length; i++) {
                            var btn = x.transform.Find(ButtonStr + i).GetComponent<Button>();
                            var a = i;
                            btn.onClick.RemoveAllListeners();
                            btn.onClick.AddListener(() => {
                                Hide();
                                onCompleteAction(a);
                            });
                            Text txt = btn.transform.Find(TextStr).GetComponent<Text>();
                            if (Instance.font != null)
                                txt.font = Instance.font;
                            txt.text = buttonsTexts[i];
                        }
                    }
                });

            Instance.title.text = title;
            Instance.body.text = bodyText;
        }

        /// <summary>
        /// Hides the messagebox window
        /// </summary>
        public static void Hide() {
            Instance.bg.gameObject.SetActive(false);
            Instance.gameObject.SetActive(false);
        }
    }
}