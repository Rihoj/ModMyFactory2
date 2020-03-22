//  Copyright (C) 2020 Mathis Rech
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.

using ModMyFactory.BaseTypes;
using ModMyFactory.Game;
using ModMyFactory.Mods;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ModMyFactory
{
    /// <summary>
    /// Manages Factorio instances and their mods
    /// </summary>
    public sealed class Manager
    {
        private readonly ObservableCollection<ManagedFactorioInstance> _managedInstances;
        private readonly Dictionary<AccurateVersion, ModManager> _modManagers;

        /// <summary>
        /// The list of instances being managed
        /// </summary>
        public FactorioInstanceCollection ManagedInstances { get; }

        public Manager()
        {
            _managedInstances = new ObservableCollection<ManagedFactorioInstance>();
            ManagedInstances = new FactorioInstanceCollection(_managedInstances);
            _modManagers = new Dictionary<AccurateVersion, ModManager>();
        }

        /// <summary>
        /// Tries to gets the mod manager associated with a specific version of Factorio
        /// Fails if no mod manager has been created for the specified version yet
        /// </summary>
        public bool TryGetModManager(AccurateVersion factorioVersion, out ModManager result)
        {
            factorioVersion = factorioVersion.ToMajor();
            return _modManagers.TryGetValue(factorioVersion, out result);
        }

        /// <summary>
        /// Gets the mod manager associated with a specific version of Factorio
        /// </summary>
        public ModManager GetModManager(AccurateVersion factorioVersion)
        {
            factorioVersion = factorioVersion.ToMajor();

            if (!_modManagers.TryGetValue(factorioVersion, out var result))
            {
                result = new ModManager(factorioVersion);
                _modManagers.Add(factorioVersion, result);
            }
            return result;
        }

        /// <summary>
        /// Adds a Factorio instance to the manager
        /// Instances that are already managed are not valid
        /// </summary>
        /// <param name="instance">The instance to add</param>
        public ManagedFactorioInstance AddInstance(in IFactorioInstance instance)
        {
            var modManager = GetModManager(instance.Version);
            var managedInstance = ManagedFactorioInstance.CreateInternal(instance, modManager);

            _managedInstances.Add(managedInstance);
            return managedInstance;
        }

        /// <summary>
        /// Removes a Factorio instance from the manager
        /// </summary>
        /// <param name="instance">The instance to remove</param>
        public bool RemoveInstance(in ManagedFactorioInstance instance)
            => _managedInstances.Remove(instance);

        /// <summary>
        /// Removes all Factorio instances from the manager
        /// </summary>
        public void ClearInstances()
            => _managedInstances.Clear();

        /// <summary>
        /// Adds a mod to the corresponding mod manager
        /// </summary>
        public void AddMod(Mod mod)
        {
            var modManager = GetModManager(mod.FactorioVersion);
            modManager.Add(mod);
        }

        /// <summary>
        /// Removes a mod from its corresponding mod manager
        /// </summary>
        public bool RemoveMod(Mod mod)
        {
            if (TryGetModManager(mod.FactorioVersion, out var modManager))
            {
                return modManager.Remove(mod);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes all mods from their corresponding managers
        /// </summary>
        public void ClearMods()
        {
            foreach (var modManager in _modManagers.Values)
                modManager.Clear();
        }
    }
}
