﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending.Modularity;

namespace Volo.Abp.ObjectExtending
{
    public class ObjectExtensionManager
    {
        public static ObjectExtensionManager Instance { get; set; } = new ObjectExtensionManager();

        [NotNull]
        public Dictionary<object, object> Configuration { get; }

        protected Dictionary<Type, ObjectExtensionInfo> ObjectsExtensions { get; }
        
        public ModuleObjectExtensionConfigurationDictionary Modules { get; }

        protected internal ObjectExtensionManager()
        {
            ObjectsExtensions = new Dictionary<Type, ObjectExtensionInfo>();
            Configuration = new Dictionary<object, object>();
            Modules = new ModuleObjectExtensionConfigurationDictionary();
        }

        [NotNull]
        public virtual ObjectExtensionManager AddOrUpdate<TObject>(
            [CanBeNull] Action<ObjectExtensionInfo> configureAction = null)
            where TObject : IHasExtraProperties
        {
            return AddOrUpdate(typeof(TObject), configureAction);
        }

        [NotNull]
        public virtual ObjectExtensionManager AddOrUpdate(
            [NotNull] Type[] types,
            [CanBeNull] Action<ObjectExtensionInfo> configureAction = null)
        {
            Check.NotNull(types, nameof(types));

            foreach (var type in types)
            {
                AddOrUpdate(type, configureAction);
            }

            return this;
        }

        [NotNull]
        public virtual ObjectExtensionManager AddOrUpdate(
            [NotNull] Type type,
            [CanBeNull] Action<ObjectExtensionInfo> configureAction = null)
        {
            Check.AssignableTo<IHasExtraProperties>(type, nameof(type));
            
            var extensionInfo = ObjectsExtensions.GetOrAdd(
                type,
                () => new ObjectExtensionInfo(type)
            );

            configureAction?.Invoke(extensionInfo);

            return this;
        }

        [CanBeNull]
        public virtual ObjectExtensionInfo GetOrNull<TObject>()
            where TObject : IHasExtraProperties
        {
            return GetOrNull(typeof(TObject));
        }

        [CanBeNull]
        public virtual ObjectExtensionInfo GetOrNull([NotNull] Type type)
        {
            Check.AssignableTo<IHasExtraProperties>(type, nameof(type));

            return ObjectsExtensions.GetOrDefault(type);
        }

        [NotNull]
        public virtual ImmutableList<ObjectExtensionInfo> GetExtendedObjects()
        {
            return ObjectsExtensions.Values.ToImmutableList();
        }
    }
}