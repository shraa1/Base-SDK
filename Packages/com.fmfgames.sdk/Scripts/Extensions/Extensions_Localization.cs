using System.Linq;
using System.Text;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace BaseSDK.Extension {
	/// <summary>
	/// Extension methods for working with <see cref="LocalizeStringEvent"/>.
	/// Provides helpers for setting keys and parameters in a convenient way.
	/// </summary>
	public static partial class Extensions {
		/// <summary>
		/// Sets the localization key and applies parameter values.
		/// </summary>
		/// <typeparam name="T">The type of the parameters (must match the variable type in the localization table).</typeparam>
		/// <param name="element">The target <see cref="LocalizeStringEvent"/> component.</param>
		/// <param name="key">The localization entry key to use.</param>
		/// <param name="parameters">Optional parameters for variable substitution.</param>
		public static void SetKey<T>(this LocalizeStringEvent element, string key, params T[] parameters) {
			element.SetEntry(key);                   // Assign the entry key
			SetParameters(element, parameters);      // Apply any provided parameters
		}

		/// <summary>
		/// Sets the localization key using a <see cref="StringBuilder"/> and applies parameters.
		/// </summary>
		/// <typeparam name="T">The type of the parameters (must match the variable type in the localization table).</typeparam>
		/// <param name="element">The target <see cref="LocalizeStringEvent"/> component.</param>
		/// <param name="builder">A <see cref="StringBuilder"/> containing the key string.</param>
		/// <param name="parameters">Optional parameters for variable substitution.</param>
		public static void SetKey<T>(this LocalizeStringEvent element, StringBuilder builder, params T[] parameters) =>
			SetKey(element, builder.ToString(), parameters);

		/// <summary>
		/// Sets the localization key without parameters.
		/// </summary>
		/// <param name="element">The target <see cref="LocalizeStringEvent"/> component.</param>
		/// <param name="key">The localization entry key to use.</param>
		public static void SetKey(this LocalizeStringEvent element, string key) => element.SetEntry(key);

		/// <summary>
		/// Sets the localization key from a <see cref="StringBuilder"/> without parameters.
		/// </summary>
		/// <param name="element">The target <see cref="LocalizeStringEvent"/> component.</param>
		/// <param name="builder">A <see cref="StringBuilder"/> containing the key string.</param>
		public static void SetKey(this LocalizeStringEvent element, StringBuilder builder) => SetKey(element, builder.ToString());

		/// <summary>
		/// Sets the values for parameters in a localized string event.
		/// </summary>
		/// <typeparam name="T">The type of the parameters (must match the variable type in the localization table).</typeparam>
		/// <param name="element">The target <see cref="LocalizeStringEvent"/> component.</param>
		/// <param name="parameters">The parameter values to assign to the variables.</param>
		public static void SetParameters<T>(this LocalizeStringEvent element, params T[] parameters) {
			if (parameters.Length <= 0)
				return; // No parameters provided, nothing to do

			// Convert the event's argument collection to a mutable list
			var args = element.StringReference.Values.ToList();
			if (args.Count <= 0)
				return;

			for(var i = 0; i < parameters.Length; i++) {
				if (i >= args.Count)
					break;
				((Variable<T>)args[i]).Value = parameters[i];
			}

			// Refresh to apply updated parameter values
			element.RefreshString();
		}
	}
}