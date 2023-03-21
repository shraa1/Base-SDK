//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BaseSDK.Extension {
	public static partial class Extensions {
		#region Custom Operations DataType
		/// <summary>
		/// Direction to flip a texture
		/// </summary>
		public enum TextureFlipDirection { Horizontal, Vertical, Both }

		/// <summary>
		/// Direction to rotate a texture
		/// </summary>
		public enum RotateDirection { Left, Right }
		#endregion

		/// <summary>
		/// Flip a texture (Mirror image)
		/// Warning: Uses SetPixels method of a texture, which can be very slow. Might affect performance.
		/// </summary>
		/// <param name="texture">Texture to flip</param>
		/// <param name="flipDirection">Direction of flip</param>
		public static Texture2D Flip(this Texture2D texture, TextureFlipDirection flipDirection) {
			Color[] colors = texture.GetPixels();
			Color[] flipped = new Color[colors.Length];

			//Completely reverse the pixels
			if (flipDirection == TextureFlipDirection.Both) {
				Array.Copy(colors, flipped, colors.Length);
				Array.Reverse(flipped);
			}
			//Flip pixels in only the required axis if being flipped in only 1 direction
			else
				for (var i = 0; i < texture.width; i++)
					for (int j = 0; j < texture.height; ++j) {
						var index = flipDirection == TextureFlipDirection.Vertical ?
							(texture.width * (texture.height - j - 1)) + i :
							texture.width - 1 - i + (j * texture.width);

						flipped[index] = colors[i + (j * texture.width)];
					}

			texture.SetPixels(flipped);
			texture.Apply();
			return texture;
		}

		/// <summary>
		/// Fill a texture with a specific colour
		/// Warning: Uses SetPixels method of a texture, which can be very slow. Might affect performance.
		/// </summary>
		/// <param name="texture">Texture to fill</param>
		/// <param name="color">Colour to fill the texture with</param>
		public static Texture2D Fill(this Texture2D texture, Color32 color) => Fill(texture, (Color)color);

		/// <summary>
		/// Fill a texture with a specific Color
		/// Warning: Uses SetPixels method of a texture, which can be very slow. Might affect performance.
		/// </summary>
		/// <param name="texture">Texture to fill</param>
		/// <param name="color">Colour to fill the texture with</param>
		public static Texture2D Fill(this Texture2D texture, Color color) {
			Color[] colors = texture.GetPixels();
			for (var i = 0; i < colors.Length; i++)
				colors[i] = color;
			texture.SetPixels(colors);
			texture.Apply();
			return texture;
		}

		/// <summary>
		/// Rotates a texture 90° in a direction
		/// </summary>
		/// <param name="texture">Texture to Rotate</param>
		/// <param name="rotationDirection">Direction of Rotation, i.e. left or right</param>
#if UNITY_EDITOR
		/// <param name="pathToSaveTo">Provide an absolute path, so that the texture can be saved on the drive. Currently limited to Editor, but the wrapping code can be removed to implement at runtime.</param>
		public static Texture2D Rotate(this Texture2D texture, RotateDirection rotationDirection, string pathToSaveTo = null) {
#else
		public static Texture2D Rotate(this Texture2D texture, RotateDirection rotationDirection) {
#endif
			Color[] colors = texture.GetPixels();
			Color[] flipped = new Color[colors.Length];

			for (var i = 0; i < texture.width; i++)
				for (var j = 0; j < texture.height; j++) {
					var index = rotationDirection == RotateDirection.Left ?
						(i * texture.height) + texture.height - 1 - j :
						((texture.width - 1 - i) * texture.height) + j;
					flipped[index] = colors[i + (j * texture.width)];
				}

			texture = new Texture2D(texture.height, texture.width);
			texture.SetPixels(flipped);
			texture.Apply();

#if UNITY_EDITOR
			if (!pathToSaveTo.IsNullOrEmpty())
				System.IO.File.WriteAllBytes(pathToSaveTo, texture.EncodeToPNG());
#endif

			return texture;
		}

		/// <summary>
		/// Returns if the texture is Read/Write enabled or not
		/// </summary>
		/// <param name="texture">Texture to get the property Read/Write of</param>
		/// <returns>Returns if the texture is Read/Write enabled or not</returns>
		public static bool ReadWriteEnabled(this Texture2D texture) {
#if UNITY_EDITOR
			TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
			if (importer != null)
				return importer.isReadable;
#endif
			return false;
		}

		/// <summary>
		/// Change the Read/Write property of a texture
		/// </summary>
		/// <param name="texture">Texture to modify</param>
		/// <param name="isReadable">Should the texture be readable/writable</param>
		public static void EnableReadWrite(this Texture2D texture, bool isReadable) {
#if UNITY_EDITOR
				TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
				if (importer != null && importer.isReadable != isReadable) {
					importer.isReadable = isReadable;
					AssetDatabase.ImportAsset(importer.assetPath, ImportAssetOptions.ForceUpdate);
				}
#endif
		}
		
		/// <summary>
		/// Creates a sprite from the given texture.
		/// Note: Memory is not managed here, the caller if this method has to handle freeing memory if deleting the texture or sprite
		/// </summary>
		/// <param name="texture">Texture to create the sprite using</param>
		/// <returns>Returns the created Sprite</returns>
		public static Sprite ToSprite(this Texture2D texture) => Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(.5f, .5f));
	}
}