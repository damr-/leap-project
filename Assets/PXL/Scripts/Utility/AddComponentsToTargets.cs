using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace PXL.Utility {

	/// <summary>
	/// This script adds the given components (their whole namespace needs to be given! to the given objects on start
	/// </summary>
	public class AddComponentsToTargets : MonoBehaviour {

		/// <summary>
		/// All the GameObjects which the components will be added to
		/// </summary>
		public List<GameObject> Targets;

		/// <summary>
		/// The namespaces of the components added to each one of the <see cref="Targets"/>
		/// </summary>
		public List<string> Components;

		private string assemblyName;

		private void Awake() {
			assemblyName = Assembly.GetExecutingAssembly().FullName;

			if (Targets.Count == 0)
				throw new MissingReferenceException("No targets set!");
			Targets.ForEach(t => t.AssertNotNull("Target is missing or null!"));

			Targets.ForEach(ApplyComponents);
		}

		private void ApplyComponents(GameObject target) {
			Components.ForEach(c => {
				var componentType = Types.GetType(c, assemblyName);
				RemoveExistingComponents(target, componentType);
				target.AddComponent(componentType);
			});
		}

		private static void RemoveExistingComponents(GameObject target, System.Type componentType) {
			var c = target.GetComponent(componentType);
			while (c != null) {
				Destroy(c);
				c = target.GetComponent(componentType);
			}
		}

	}

}