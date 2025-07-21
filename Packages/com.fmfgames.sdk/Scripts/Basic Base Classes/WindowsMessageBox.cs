using System;
using System.Runtime.InteropServices;

namespace BaseSDK.Utils.Helpers {
	public class WindowsMessageBox {
		[DllImport("user32.dll")]
		public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

		#region Custom DataTypes
		/// <summary>
		/// Buttons to show on the messagebox
		/// </summary>
		public enum MessageBoxButtons {
			Ok = 0x00000000, OkCancel = 0x00000001, AbortRetryIgnore = 0x00000002, YesNoCancel = 0x00000003, YesNo = 0x00000004, RetryCancel = 0x00000005, CancelTryIgnore = 0x00000006, Help = 0x00004000
		}

		/// <summary>
		/// The message box returns an integer value that indicates which button the user clicked.
		/// </summary>
		public enum MessageBoxResult {
			Ok = 1, Cancel = 2, Abort = 3, Ignore = 5, Yes = 6, No = 7, Retry = 10
		}

		/// <summary>
		/// To indicate the default button, specify one of the following values.
		/// </summary>
		public enum MessageBoxDefaultButton : uint {
			Button1 = 0x00000000, Button2 = 0x00000100, Button3 = 0x00000200, Button4 = 0x00000300,
		}

		/// <summary>
		/// To indicate the modality of the dialog box, specify one of the following values.
		/// </summary>
		public enum MessageBoxModal : uint {
			/// <summary>
			/// The user must respond to the message box before continuing work in the window identified by the hWnd parameter. However, the user can move to the windows of other threads and work in those windows. Depending on the hierarchy of windows in the application, the user may be able to move to other windows within the thread. All child windows of the parent of the message box are automatically disabled, but pop-up windows are not.
			/// </summary>
			Application = 0x00000000,

			/// <summary>
			/// Same as <see cref="Application"/> except that the message box has the Top Most style. Use system-modal message boxes to notify the user of serious, potentially damaging errors that require immediate attention (for example, running out of memory).
			/// </summary>
			System = 0x00001000,

			/// <summary>
			/// Same as <see cref="Application"/> except that all the top-level windows belonging to the current thread are disabled if the hWnd parameter is NULL. Use this flag when the calling application or library does not have a window handle available but still needs to prevent input to other windows in the calling thread without suspending other threads.
			/// </summary>
			Task = 0x00002000
		}

		/// <summary>
		/// To display an icon in the message box, specify one of the following values.
		/// </summary>
		public enum MessageBoxIcon : uint {
			/// <summary>
			/// An exclamation-point icon appears in the message box.
			/// </summary>
			Warning = 0x00000030,

			/// <summary>
			/// An icon consisting of a lowercase letter `i` in a circle appears in the message box.
			/// </summary>
			Information = 0x00000040,

			/// <summary>
			/// A question-mark icon appears in the message box.
			/// </summary>
			/// <remarks>
			/// The question-mark message icon is no longer recommended because it does not clearly represent a specific type of message and because the phrasing of a message as a question could apply to any message type. In addition, users can confuse the message symbol question mark with Help information. Therefore, do not use this question mark message symbol in your message boxes. The system continues to support its inclusion only for backward compatibility.
			/// </remarks>
			Question = 0x00000020,

			/// <summary>
			/// A stop-sign icon appears in the message box.
			/// </summary>
			Error = 0x00000010
		}
		#endregion

		/// <summary>
		/// Show a messagebox with this title and body.
		/// </summary>
		/// <param name="title">Title message.</param>
		/// <param name="body">Text to show in the body of the MessageBox.</param>
		/// <returns>Returns the clicked button value. Returns cancel if MessageBox was closed, and the MessageBoxButtons type included cancel.</returns>
		public static MessageBoxResult Show(string title, string body) => (MessageBoxResult)MessageBox(IntPtr.Zero, body, title, (uint)MessageBoxButtons.Ok);

		/// <summary>
		/// Show a messagebox with this title and body.
		/// </summary>
		/// <param name="title">Title message.</param>
		/// <param name="body">Text to show in the body of the MessageBox.</param>
		/// <param name="buttons">Buttons to show on the MessageBox.</param>
		/// <returns>Returns the clicked button value. Returns cancel if MessageBox was closed, and the MessageBoxButtons type included cancel.</returns>
		public static MessageBoxResult Show(string title, string body, MessageBoxButtons buttons = MessageBoxButtons.Ok) => (MessageBoxResult)MessageBox(IntPtr.Zero, body, title, (uint)buttons);

		/// <summary>
		/// Show a messagebox with this title and body.
		/// </summary>
		/// <param name="title">Title message.</param>
		/// <param name="body">Text to show in the body of the MessageBox.</param>
		/// <param name="buttons">Buttons to show on the MessageBox.</param>
		/// <param name="icon">The MessageType icon to show on the MessageBox.</param>
		/// <returns>Returns the clicked button value. Returns cancel if MessageBox was closed, and the MessageBoxButtons type included cancel.</returns>
		public static MessageBoxResult Show(string title, string body, MessageBoxButtons buttons = MessageBoxButtons.Ok, MessageBoxIcon icon = MessageBoxIcon.Information) =>
			(MessageBoxResult)MessageBox(IntPtr.Zero, body, title, ((uint)buttons) | ((uint)icon));


		/// <summary>
		/// Show a messagebox with this title and body.
		/// </summary>
		/// <param name="title">Title message.</param>
		/// <param name="body">Text to show in the body of the MessageBox.</param>
		/// <param name="buttons">Buttons to show on the MessageBox.</param>
		/// <param name="icon">The MessageType icon to show on the MessageBox.</param>
		/// <param name="defaultButton">Default button that is selected at the beginning.</param>
		/// <returns>Returns the clicked button value. Returns cancel if MessageBox was closed, and the MessageBoxButtons type included cancel.</returns>
		public static MessageBoxResult Show(string title, string body, MessageBoxButtons buttons = MessageBoxButtons.Ok, MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1) =>
			(MessageBoxResult)MessageBox(IntPtr.Zero, body, title, ((uint)buttons) | ((uint)icon) | ((uint)defaultButton));

		/// <summary>
		/// Show a messagebox with this title and body.
		/// </summary>
		/// <param name="title">Title message.</param>
		/// <param name="body">Text to show in the body of the MessageBox.</param>
		/// <param name="buttons">Buttons to show on the MessageBox.</param>
		/// <param name="icon">The MessageType icon to show on the MessageBox.</param>
		/// <param name="defaultButton">Default button that is selected at the beginning.</param>
		/// <param name="modal">Sets the type of MessageBox.</param>
		/// <returns>Returns the clicked button value. Returns cancel if MessageBox was closed, and the MessageBoxButtons type included cancel.</returns>
		public static MessageBoxResult Show(string title, string body, MessageBoxButtons buttons = MessageBoxButtons.Ok, MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, MessageBoxModal modal = MessageBoxModal.Application) =>
			(MessageBoxResult)MessageBox(IntPtr.Zero, body, title, ((uint)buttons) | ((uint)icon) | ((uint)defaultButton) | ((uint)modal));
	}
}