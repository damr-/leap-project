using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PXL.Utility {

	public class AddComponentsToTargets : MonoBehaviour {
		/// <summary>
		/// All the GameObjects which the components will be added to
		/// </summary>
		public List<GameObject> Targets;

		/// <summary>
		/// All the components added to each one of the <see cref="Targets"/>
		/// </summary>
		public List<string> Components;

		private string assemblyName;

		private void Awake() {
			assemblyName = Assembly.GetExecutingAssembly().FullName;
			Targets.ForEach(t => t.AssertNotNull());
			Targets.ForEach(AddComponents);
		}

		private void AddComponents(GameObject target) {
			Components.ForEach(c => CreateAndAddComponent(target, c));
		}

		private void CreateAndAddComponent(GameObject target, string component) {
			System.Type componentType = Types.GetType(component, assemblyName);
			target.AddComponent(componentType);
		}
	}

}