using System.Collections.Generic;

using UnityEngine;

namespace BaseSDK.Utils {
	/// <summary>
	/// Generic class for maintaining ObjectPool of type <typeparamref name="T"/>
	/// </summary>
	/// <typeparam name="T">Type of ObjectPool</typeparam>
	public class ObjectPool<T> where T : Component {
#pragma warning disable S2743 // Static fields should not be used in generic types
		#region Custom DataTypes
		/// <summary>
		/// If an item is available or in use
		/// </summary>
		public enum ItemUsageState { FREE, IN_USE }
		#endregion Custom DataTypes

		#region Private Fields
		private const string pool = "Pool";

		/// <summary>
		/// Template object for instantiating. If this is null, a new empty GameObject will be created
		/// </summary>
		private static T templ;

		/// <summary>
		/// Controller Gameobject Pool<T> (usually dontdestroy and disabled)
		/// </summary>
		private static GameObject controller;

		/// <summary>
		/// Queue of free/available pool items for efficient O(1) access
		/// </summary>
		private static readonly Queue<T> freeItems = new();

		/// <summary>
		/// HashSet of in-use pool items for efficient O(1) tracking
		/// </summary>
		private static readonly HashSet<T> inUseItems = new();

		/// <summary>
		/// Default rotation for instantiating new objects
		/// </summary>
		private static Quaternion rotation = Quaternion.identity;

		/// <summary>
		/// Default scale for instantiating new objects
		/// </summary>
		private static Vector3 localScale = Vector3.one;
		#endregion Private Fields

		#region Properties
		/// <summary>
		/// All the Objects of this type which are in use
		/// </summary>
		public static List<T> AllInUseItems => new List<T>(inUseItems);

		/// <summary>
		/// All the Objects of this type which are free/available for use
		/// </summary>
		public static List<T> GetAllFree => new List<T>(freeItems);

		/// <summary>
		/// Set the max pool items allowed for this Type of ObjectPool.<br/>Ignored for BulkInstantiate call.
		/// </summary>
		public static int MaxPoolItems { get; set; }

		/// <summary>
		/// Count of free/available instantiated items for this ObjectPool<T>
		/// </summary>
		public static int NumberOfFreeItems => freeItems.Count;

		/// <summary>
		/// Count of in use instantiated items for this ObjectPool<T>
		/// </summary>
		public static int NumberOfInUseItems => inUseItems.Count;

		/// <summary>
		/// Count of instantiated items for this ObjectPool<T>
		/// </summary>
		public static int InstantiatedPoolItemsCount => freeItems.Count + inUseItems.Count;

		/// <summary>
		/// Controller GameObject which was instantiated and holds all the pool objects. Named Pool<T>
		/// </summary>
		public static GameObject GameObject {
			get {
				if (controller == null)
					CreateControllerGameObject(true);
				return controller;
			}
		}
		#endregion Properties

		#region Methods
		/// <summary>
		/// Creates the Pool<T> gameobject if it doesn't already exist.
		/// </summary>
		/// <param name="dontDestroy">Don't Destroy the controller GameObject</param>
		/// <param name="instantiateActive">Should Instantiate GameObject in (de)active state</param>
		/// <returns>Returns the Pool<T> gameobject.</returns>
#pragma warning disable S3241 // Methods should not return values that are never used
		private static GameObject CreateControllerGameObject (bool dontDestroy = true, bool instantiateActive = false) {
			if (controller == null) {
				controller = new GameObject($"{pool}<{typeof(T).Name}>");
				controller.SetActive(instantiateActive);
				if (dontDestroy)
                    Object.DontDestroyOnLoad(controller);
			}
			return controller;
		}
#pragma warning restore S3241 // Methods should not return values that are never used

		/// <summary>
		/// Set the instantiation template object. This template gameobject will be used to instantiate from Get.<br/>
		/// If template is not set, then new GameObject will be created with the Type T component<br/>
		/// Should be called in Awake or static constructor or something like that, before calling any other method/property of the class.
		/// </summary>
		/// <param name="template">Template</param>
		/// <param name="dontDestroy">Don't Destroy the controller GameObject</param>
		/// <param name="maxPoolItems">Max Pool items.</param>
		public static void SetTemplate (T template, bool dontDestroy = true, int maxPoolItems = -1) {
			templ = template;
			localScale = templ.transform.localScale;
			rotation = templ.transform.rotation;
			CreateControllerGameObject(dontDestroy);
			MaxPoolItems = maxPoolItems;
		}

		/// <summary>
		/// Get a pool item from the freed items list. If none exists, then it will be created.<br/>
		/// If instantiated pool items count is equal to MaxPoolItems, then no new item will be created & returns null.<br/>
		/// But if BulkInstantiate is called which makes pool instantiated items more than MaxPoolItems, then next call of Get will not create anything
		/// </summary>
		/// <param name="parentTransform">Which gameobject should this pool item be parented to?</param>
		/// <param name="itemUsageState">On creating a new or fetching an existing pool item, set it as an in-use item or a free to use item</param>
		/// <returns>Returns null if MaxPoolItems is reached, else finds an available object, else creates a new one and returns it.</returns>
		public static T Get (Transform parentTransform = null, ItemUsageState itemUsageState = ItemUsageState.IN_USE) {

            // Try to get a free item from the queue
            if (!freeItems.TryDequeue(out T item)) {
                // No free items available, check if we can create a new one
                if (MaxPoolItems != -1 && InstantiatedPoolItemsCount >= MaxPoolItems)
                    return null;

                // Create a new item
                if (templ != null) {
                    item = Object.Instantiate(templ, templ.transform.position, Quaternion.identity, controller.transform);
                	item.gameObject.name = templ.name;
				}
                else {
                    item = new GameObject().AddComponent<T>();
					item.gameObject.name = typeof(T).Name;
				}
            }

            // Set up the item
            item.transform.SetParent(parentTransform, true);
			item.transform.localScale = localScale;
			item.transform.rotation = rotation;

			// Track the item based on its usage state
			if (itemUsageState == ItemUsageState.IN_USE)
				inUseItems.Add(item);
			else
				freeItems.Enqueue(item);

			return item;
		}

		/// <summary>
		/// Instantiate Ojects in bulk.</br>
		/// Better memory management to bulk instantiate if you know how many items will be needed. Better than Get operations in some cases like scene load, etc.
		/// </summary>
		/// <param name="capacity"></param>
		public static void BulkInstantiate (int capacity) {
			for (var i = 0; i < capacity; i++) {
				T item;
				if (templ != null)
					item = Object.Instantiate(templ, templ.transform.position, Quaternion.identity, controller.transform);
				else
					item = new GameObject().AddComponent<T>();

				item.transform.SetParent(controller.transform, true);
				item.transform.localScale = localScale;
				item.transform.rotation = rotation;
				freeItems.Enqueue(item);
			}
		}

		/// <summary>
		/// Free up an item from the In use category to Freed up category.
		/// </summary>
		/// <param name="itemToFree">Item to free.</param>
		public static void FreeToPool (T itemToFree) {
			if (inUseItems.Remove(itemToFree)) {
				itemToFree.transform.SetParent(controller.transform, true);
				freeItems.Enqueue(itemToFree);
			}
		}

		/// <summary>
		/// Free up all pool items
		/// </summary>
		public static void FreeAllItemsToPool () {
			foreach (var item in inUseItems) {
				item.transform.SetParent(controller.transform, true);
				freeItems.Enqueue(item);
			}
			inUseItems.Clear();
		}
		#endregion Methods
#pragma warning restore S2743 // Static fields should not be used in generic types
	}
}