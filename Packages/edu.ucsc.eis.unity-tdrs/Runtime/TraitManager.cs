using System;
using System.Collections.Generic;
using System.Linq;

namespace TDRS
{
	/// <summary>
	/// Manages the traits attached to a social entity or relationship.
	/// </summary>
	public class TraitManager
	{
		#region Fields

		/// <summary>
		/// Traits currently applied to the entity
		/// </summary>
		protected Dictionary<string, Trait> m_traits;

		/// <summary>
		/// Collection of TraitID's that conflict with the current traits
		/// </summary>
		protected HashSet<string> m_conflictingTraits;

		#endregion

		#region Events

		/// <summary>
		/// Event published when a trait is added to the collection.
		/// </summary>
		public event EventHandler<string> OnTraitAdded;

		/// <summary>
		/// Event published when a trait is removed from the collection.
		/// </summary>
		public event EventHandler<string> OnTraitRemoved;

		#endregion

		#region Properties

		/// <summary>
		/// All traits within the collection.
		/// </summary>
		public List<Trait> Traits => m_traits.Values.ToList();

		#endregion

		#region Constructors

		public TraitManager()
		{
			m_traits = new Dictionary<string, Trait>();
			m_conflictingTraits = new HashSet<string>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Add a trait to the entity
		/// </remarks>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool AddTrait(Trait trait, int duration = -1)
		{
			if (m_traits.ContainsKey(trait.TraitID))
			{
				return false;
			}

			if (HasConflictingTrait(trait))
			{
				return false;
			}

			m_traits[trait.TraitID] = trait;

			trait.SetDuration(duration);

			m_conflictingTraits.UnionWith(trait.ConflictingTraits);

			if (OnTraitAdded != null) OnTraitAdded.Invoke(this, trait.TraitID);

			return true;
		}

		/// <summary>
		/// Remove a trait
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool RemoveTrait(string traitID)
		{
			if (!m_traits.ContainsKey(traitID))
			{
				return false;
			}

			m_traits.Remove(traitID);

			m_conflictingTraits.Clear();
			foreach (var (_, trait) in m_traits)
			{
				m_conflictingTraits.UnionWith(trait.ConflictingTraits);
			}

			if (OnTraitRemoved != null) OnTraitRemoved.Invoke(this, traitID);

			return true;
		}

		/// <summary>
		/// Remove a trait
		/// </summary>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool RemoveTrait(Trait trait)
		{
			return RemoveTrait(trait.TraitID);
		}

		/// <summary>
		/// Check if a trait is already present
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public bool HasTrait(string traitID)
		{
			return m_traits.ContainsKey(traitID);
		}

		/// <summary>
		/// Check if a trait is already present
		/// </summary>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool HasTrait(Trait trait)
		{
			return m_traits.ContainsKey(trait.TraitID);
		}

		/// <summary>
		/// Check if the there is already a conflicting trait present
		/// </summary>
		/// <param name="trait"></param>
		/// <returns></returns>
		public bool HasConflictingTrait(Trait trait)
		{
			return m_conflictingTraits.Contains(trait.TraitID);
		}

		/// <summary>
		/// Get a trait instance from the collection
		/// </summary>
		/// <param name="traitID"></param>
		/// <returns></returns>
		public Trait GetTrait(string traitID)
		{
			return m_traits[traitID];
		}

		#endregion
	}
}
