using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TDRS
{
	/// <summary>
	/// A user-facing Unity component for associating a GameObject with as node within
	/// the TDRS Manager's social graph.
	/// </summary>
	[DefaultExecutionOrder(1)]
	public class TDRSEntity : MonoBehaviour
	{
		[Serializable]
		public struct StatInitializer
		{
			public string name;
			public float baseValue;
		}

		#region Attributes

		/// <summary>
		/// The ID of the entity within the TDRS Manager
		/// </summary>
		[SerializeField]
		public string entityID = "";

		[SerializeField]
		public StatSchemaScriptableObj statSchema;

		[SerializeField]
		public string[] traitsAtStart;

		[SerializeField]
		public StatInitializer[] baseStats;

		#endregion

		public StatSchemaScriptableObj StatSchema => statSchema;

		public IEnumerable<StatInitializer> BaseStats => baseStats;

		public TraitAddedEvent OnTraitAdded;

		public TraitRemovedEvent OnTraitRemoved;

		public StatChangeEvent OnStatChange;

		#region Unity Methods

		void Awake()
		{
			if (statSchema == null)
			{
				Debug.LogError(
					$"{gameObject.name} is missing stat schema for TDRSEntity component."
				);
			}
		}

		void Start()
		{
			TDRSManager manager = FindObjectOfType<TDRSManager>();
			var node = manager.RegisterEntity(this);

			node.Traits.OnTraitAdded += (traits, traitID) =>
			{
				if (OnTraitAdded != null) OnTraitAdded.Invoke(traitID);
			};

			node.Traits.OnTraitRemoved += (traits, traitID) =>
			{
				if (OnTraitRemoved != null) OnTraitRemoved.Invoke(traitID);
			};

			node.Stats.OnValueChanged += (stats, nameAndValue) =>
			{
				string statName = nameAndValue.Item1;
				float value = nameAndValue.Item2;
				if (OnStatChange != null) OnStatChange.Invoke(statName, value);
			};
		}

		#endregion

		#region Custom Event Classes

		/// <summary>
		/// Event dispatched when a trait is added to a social entity
		/// </summary>
		[System.Serializable]
		public class TraitAddedEvent : UnityEvent<string> { }

		/// <summary>
		/// Event dispatched when a trait is removed from a social entity
		/// </summary>
		[System.Serializable]
		public class TraitRemovedEvent : UnityEvent<string> { }

		/// <summary>
		/// Event dispatched when a social entity has a stat that is changed
		/// </summary>
		[System.Serializable]
		public class StatChangeEvent : UnityEvent<string, float> { }

		#endregion
	}
}
