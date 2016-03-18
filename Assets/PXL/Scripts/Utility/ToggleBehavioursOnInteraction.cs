using System;
using System.Collections.Generic;
using System.Linq;
using PXL.Interaction;
using UnityEngine;
using UniRx;

namespace PXL.Utility {

	public class ToggleBehavioursOnInteraction : InteractionHandSubscriber {

		[Serializable]
		public class ToggleSetting {
			public HandSide HandSide;
			public InteractionType InteractionType;
			public List<Behaviour> EnableBehaviours = new List<Behaviour>();
			public List<Behaviour> DisableBehaviours = new List<Behaviour>();
			public List<Behaviour> ToggleBehaviours = new List<Behaviour>();

			/// <summary>
			/// How often the toggle should be executed. -1 for infinite
			/// </summary>
			public int Executions = 1;

			private int currentExecutions = 0;

			public void Executed() {
				currentExecutions++;
			}

			public bool CanExecute() {
				return currentExecutions < Executions;
			}
		}

		public List<ToggleSetting> Settings = new List<ToggleSetting>();

		protected override void HandleGrabbed(Grabbable grabbable) {
			var validTypeSettings = Settings.Where(s => s.InteractionType == InteractionType.Grab).ToList();
			
			var validSideSettings = new List<ToggleSetting>();
			foreach (var setting in validTypeSettings) {
				if (setting.HandSide == HandSide.Both) {
					validSideSettings.Add(setting);
					continue;
				}

				var side = GetHandSideIfValid(grabbable);

				if(setting.HandSide == side)
					validSideSettings.Add(setting);
			}

			foreach (var validSideSetting in validSideSettings) {
				if (!validSideSetting.CanExecute()) {
					continue;
				}

				validSideSetting.EnableBehaviours.ForEach(b => b.enabled = true);
				validSideSetting.DisableBehaviours.ForEach(b => b.enabled = false);
				validSideSetting.ToggleBehaviours.ForEach(b => b.enabled = !b.enabled);
				validSideSetting.Executed();
			}
		}

		protected override void HandleDropped(Grabbable grabbable) {

		}

		protected override void HandleMoved(MovementInfo movementInfo) {

		}

	}

}