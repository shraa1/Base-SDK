using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentWindow : MonoBehaviour {

	[DllImport("Dwmapi.dll")]
	private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll")]
	private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

	private struct MARGINS {
		public int cxLeftWidth;
		public int cxRightWidth;
		public int cyTopHeight;
		public int cyBottomHeight;
	}

	private const int GWL_EXSTYLE = -20;
	private const uint WS_EX_LAYERED = 0x00080000;
	private const uint WS_EX_TRANSPARENT = 0x00000020;
	private const uint LWA_COLOURKEY = 0x00000001;
	private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
	private IntPtr hWnd;
	private Camera worldCamera;

	private void Awake() {
		hWnd = GetActiveWindow();
		worldCamera = Camera.main;
#if !UNITY_EDITOR
		var margin = new MARGINS { cxLeftWidth = -1 };
		DwmExtendFrameIntoClientArea(hWnd, ref margin);
		SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
		SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLOURKEY);
		SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif
		Application.runInBackground = true;
	}

	private void Update() {
		var pos = worldCamera.ScreenToWorldPoint(Input.mousePosition);
		pos.z = 0f;
		SetClickThrough(Physics2D.OverlapPoint(pos));
	}

	private void SetClickThrough(bool clickThrough) {
		if (clickThrough)
			SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
		else
			SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
	}
}