#pragma warning disable IDE0052 // Remove unread private members
using System.Runtime.InteropServices;
using BaseSDK;
using UnityEngine;
using UnityEngine.UI;

public class AdjustSysVol : MonoBehaviour {

	[SerializeField] private Slider volumeSlider;

	#region DLL Imports
	/// <summary>
	/// Gets the current volume
	/// </summary>
	/// <param name="vUnit">The unit to report the current volume in</param>
	[DllImport("ChangeVolumeWindows")]
	public static extern float GetSystemVolume (VolumeUnit vUnit);

	/// <summary>
	/// sets the current volume
	/// </summary>
	/// <param name="newVolume">The new volume to set</param>
	/// <param name="vUnit">The unit to set the current volume in</param>
	[DllImport("ChangeVolumeWindows")]
	public static extern void SetSystemVolume (double newVolume, VolumeUnit vUnit);
	#endregion

	private float volumeScalar;

	private void Awake () {
		volumeScalar = GetSystemVolume(VolumeUnit.Scalar);
		volumeSlider.value = volumeScalar;
		volumeSlider.onValueChanged.AddListener(val => SetSystemVolume(val));
	}

	public void SetSystemVolume (float volume) => SetSystemVolume(volumeScalar = volume, VolumeUnit.Scalar);
}