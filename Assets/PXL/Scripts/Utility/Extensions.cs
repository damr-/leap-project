﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap.Unity;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace PXL.Utility {

	public static class Extensions {

		/// <summary>
		/// Maps the value from the given input range to the desired output range
		/// </summary>
		public static float Remap(this float value, float inMin, float inMax, float outMin, float outMax) {
			return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
		}

		/// <summary>
		/// Returns the Component or throws an exception if it is missing
		/// </summary>
		public static T TryGetComponent<T>(this Component component) where T : Component {
			var tryComponent = component.GetComponent<T>();
			if (tryComponent != null)
				return tryComponent;

			var errorMessage = new StringBuilder();
			errorMessage.Append("Component ");
			errorMessage.Append(component.ToString());
			errorMessage.Append(" is trying to access Component ");
			errorMessage.Append(typeof(T).FullName);
			errorMessage.Append(", but it is missing");
			throw new MissingComponentException(errorMessage.ToString());
		}

		/// <summary>
		/// Returns a random item from the given list with optional minimum index.
		/// Maximum index is always the last element in the list.
		/// </summary>
		/// <param name="items">The array with all items</param>
		/// <param name="minIndex">The minimum possible index</param>
		public static T GetRandomElement<T>(this List<T> items, int minIndex = 0) where T : struct {
			return items.ElementAt(Random.Range(minIndex, items.Count));
		}

		/// <summary>
		/// Tries to retrieve the value for the given key (a Component) from the given dictionary.
		/// If there is no value for the given key, an entry is added.
		/// </summary>
		/// <param name="dictionary">The dictionary to look for the component</param>
		/// <param name="key">The Component as key for the dictionary</param>
		/// <returns>The Component </returns>
		public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key) where TK : Component where TV : class {
			TV value;

			if (dictionary.TryGetValue(key, out value))
				return value;

			value = key.GetComponent<TV>();
			if (value == null)
				throw new MissingReferenceException("GetOrAdd couldn't get the Component of the object!");
			dictionary.Add(key, value);

			return value;
		}

		/// <summary>
		/// Tries to retreive the value (a class) for the given key (a GameObject) from the given dictionary.
		/// If there is no value for the given key, an entry is added.
		/// </summary>
		public static TV GetOrAddFromGameObject<TV>(this IDictionary<GameObject, TV> dictionary, GameObject key) where TV : class {
			TV value;

			if (dictionary.TryGetValue(key, out value))
				return value;

			value = key.GetComponent<TV>();
			if (value == null)
				throw new MissingReferenceException("GetOrAddFromGameObject couldn't get the Component of the object!");
			dictionary.Add(key, value);

			return value;
		}

		/// <summary>
		/// Makes sure the given object is not null. If it is, a MissingReferenceException will be thrown with the given message.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="message"></param>
		public static void AssertNotNull(this Object o, string message = "Missing reference, object is null!") {
			if (o == null)
				throw new MissingReferenceException(message);
		}

		/// <summary>
		/// Compares two Vectors for near equality, optionally with the given epsilon.
		/// </summary>
		public static bool Equal(this Vector3 a, Vector3 b, float epsilon = 0.001f) {
			return Vector3.Magnitude(a - b) < epsilon;
		}

		/// <summary>
		/// Returns whether the given hand and it's leap hand are valid and visible
		/// </summary>
		public static bool IsHandValid(this HandModel hand) {
			return hand != null && hand.isActiveAndEnabled && hand.GetLeapHand() != null;
		}

		/// <summary>
		/// Returns the cleared list by removing all null entries and inactive scene objects. Also removes duplicates
		/// </summary>
		public static void PurgeIfNecessary<T>(ref List<T> list) where T : Component {
			if (list.PurgeNeeded())
				list = list.Where(c => c != null && c.gameObject != null && c.gameObject.activeInHierarchy).Distinct().ToList();
		}

		/// <summary>
		/// Returns whether the given list has to be purged
		/// </summary>
		private static bool PurgeNeeded<T>(this IEnumerable<T> list) where T : Component {
			return list.Any(element => element == null || element.gameObject == null || !element.gameObject.activeInHierarchy);
		}

		/// <summary>
		/// Removes the element with the given index from the list and destroys it's GameObject
		/// </summary>
		public static void DestroyElement<T>(this List<T> list, int index) where T : Component {
			if (index >= list.Count || index < 0)
				return;

			var o = list[index];
			list.RemoveAt(index);
			if (o != null)
				Object.DestroyImmediate(o.gameObject);
		}

		/// <summary>
		/// Tries to get the <see cref="Health.Health"/> Component of the given component and kill it
		/// </summary>
		public static void Kill<T>(this T component) where T : Component {
			var health = component.GetComponent<Health.Health>();
			if (health != null)
				health.Kill();
			else
				Debug.LogWarning(component.gameObject.name + " has no Health Component!");
		}

		/// <summary>
		/// Removes all children transforms of this transform
		/// </summary>
		public static void RemoveAllChildTransforms<T>(this T transform) where T : Transform {
			while (transform.childCount > 0) {
				foreach (Transform o in transform) {
					Object.DestroyImmediate(o.gameObject);
				}
			}
		}

		/// <summary>
		/// Truncates the given float value to a specified number of digits
		/// </summary>
		public static float Truncate(this float value, int digits) {
			var mult = Math.Pow(10.0, digits);
			var result = Math.Truncate(mult * value) / mult;
			return (float)result;
		}

	}

}