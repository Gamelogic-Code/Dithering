# Changes
## [4.4.2] 2024-11-28
### 🐞 Bug Fixes
- Fixed a bug where there was a conflict between the `Gamelogic.Extensions.Support.MustDisposeResourceAttribute` and the
  one in certain versions of `JetBrains.Annotations`.

## [4.4.1] 2024-90-26
### 🐞 Bug Fixes
- Fixed various issues related to editor code being included in builds. The following classes were
affected:
	- `Gamelogic.Extensions.Editor.Internal.Assets` (Note: this class has also moved namespaces.)
	- `Gamelogic.Extensions.LayerPopupAttribute`
	- `Gamelogic.Extensions.TagPopupAttribute`

### 🧪 Samples
- Fixed issues related to editor code being included in builds. The following class was affected:
	- `Gamelogic.Extensions.BuildScenePopupAttribute`
- `Gamelogic.Extensions.Samples.ValidatePowerOfTwoAttribute` had a bug where the compiler directives were misplaced, causing build errors.

## [4.4.0] - 2024-09-25
### 🎉 New Features
- Added `Gamelogic.Extensions.PropertyDrawerData.SeparatorColor` and `Gamelogic.Extensions.PropertyDrawerData.SeparatorHeight` to `Gamelogic.Extensions.PropertyDrawerData` to allow for customizing the appearance of separators.
- Added various grays in `Gamelogic.Extensions.Support.Branding` and `Gamelogic.Extensions.Support.Branding.Hex`.

### 🐞 Bug Fixes
- Fixed the constructors of `Gamelogic.Extensions.SeparatorAttribute` to avoid the ambiguous call when trying to use the default constructor.
- Re-added a constructor for `Gamelogic.Extensions.WarningIfNullAttribute` that takes a string message. (Note: the class is obsolete but retained for backwards compatibility. In the last update, the constructor that takes a string message was accidentally removed.)
- Renamed `Gamelogic.Extensions.NotNegativeAttribute` to `Gamelogic.Extensions.NonNegativeAttribute`, restoring the class's original name. (Note: the class is obsolete but retained for backwards compatibility. In the last update, the class was accidentally renamed.)

## [4.3.0] - 2024-09-22
### 🎉 New Features
- Added `Gamelogic.Extensions.VectorExtensions.ClampMagnitude`.
- Added `Gamelogic.Extensions.Support.Assets.FindByType`.
- Added methods to convert between colors and hexadecimal strings: `Gamelogic.Extensions.ColorExtensions.TryParseHex`, `Gamelogic.Extensions.ColorExtensions.ParseHex`, `Gamelogic.Extensions.ColorExtensions.ToRGBHex`, `Gamelogic.Extensions.ColorExtensions.ToRGBAHex`.
- Added a common base class for properties that validate their values: `Gamelogic.Extensions.ValidationAttribute`, and added different ways to do the validation through flags in `Gamelogic.Extensions.PropertyDrawerData`.
- Added `Gamelogic.Extensions.ValidateNotNullAttribute`, `Gamelogic.Extensions.ValidatePositiveAttribute`, `Gamelogic.Extensions.ValidateNonNegativeAttribute`, `Gamelogic.Extensions.ValidateMagnitudeRangeAttribute`, and deprecated `Gamelogic.Extensions.WarningIfNullAttribute`, `Gamelogic.Extensions.PositiveAttribute`, `Gamelogic.Extensions.NonNegativeAttribute`.
- `Gamelogic.Extensions.HighlightAttribute` now supports a color parameter and is drawn better in the non-pro skin of Unity.
- `Gamelogic.Extensions.WarningIfNullAttribute` now supports a color parameter.
- Added `Gamelogic.Extensions.PropertyDrawerData` that can be used to set global properties used by property drawers, such as default colors and list retrievers.
- Added property drawer support for drawing popups of various types, including strings, integers, colors, tags, and layers. See subclasses of `Gamelogic.Extensions.PopupListAttribute` and `Gamelogic.Extensions.Editor.PopupListPropertyDrawer`1`.
- Changed all relevant MonoBehaviours and ScriptableObjects to use the new validation attributes.
- Added `Gamelogic.Extensions.SeparatorAttribute` and `Gamelogic.Extensions.Editor.SeparatorDrawer` to draw separators in the inspector.
- Added `Gamelogic.Extensions.Support.SuppressReason` that has justifications for suppressing certain warnings, and deprecated `Gamelogic.Extensions.Support.Warning`.
  - Added `Gamelogic.Extensions.Support.Branding.HexColors` to provide string presentations of colors, and added `Gamelogic.Extensions.Support.Branding.White` and `Gamelogic.Extensions.Support.Branding.Black` for consistency.
- Added `Gamelogic.Extensions.Internal.EditorOnly` to mark symbols only available in the editor.
### 🐞 Bug Fixes
- Made `Gamelogic.Extensions.ThrowHelper.ThrowIfNegative` an extension method, like the other extension methods in this class.

### 🔧 Engineering
- Made `Gamelogic.Extensions.InspectorList` and all its subclasses obsolete for Unity 2020.1 and later, since Unity's new list provides similar (and more robust) functionality.

### 🧪 Samples
- Fixed warnings in the `Generators` sample.
- Expanded the `Property Drawers Example` to include all the new changes.
- Changed all relevant samples to use the new validation attributes.

### 📄 Documentation
- Added documentation for attributes used in property drawers.
- Expanded the documentation for `Property Drawers`.

### ⬆️ Upgrade Notes
- Replace:
	- `Gamelogic.Extensions.WarningIfNullAttribute` with `Gamelogic.Extensions.ValidateNotNullAttribute`
	- `Gamelogic.Extensions.NonNegativeAttribute` with `Gamelogic.Extensions.ValidateNotNegativeAttribute`
	- `Gamelogic.Extensions.PositiveAttribute` with `Gamelogic.Extensions.ValidatePositiveAttribute`
	- `Gamelogic.Extensions.Support.Warning` with `Gamelogic.Extensions.Support.SuppressReason`
- Replace any subclass of `Gamelogic.Extensions.InspectorList` with an ordinary `System.Collections.Generic.List`1` of the right type.

## [4.2.1] - 2024-07-24
### 🐞 Bug Fixes
- `Gamelogic.Extensions.ObservedValue` was accidentally moved to the wrong name space in a previous release. 
It has been moved back to `Gamelogic.Extensions`.

## [4.2.0] - 2024-07-21
### 🎉 New Features
- Added `Gamelogic.Extensions.Algorithms.RandomAccessQueue`.
- Added `Gamelogic.Extensions.Algorithms.RandomAccessPriorityQueue`.
- Added `Gamelogic.Extensions.Algorithms.IReadOnlyBuffer.Item` (indexing to `Gamelogic.Extensions.Algorithms.IReadOnlyBuffer`) 
(and so to the implementations of `Gamelogic.Extensions.Algorithms.IBuffer`).
- Added a property drawer for `Gamelogic.Extensions.MinMaxInt` (`Gamelogic.Extensions.Editor.MinMaxIntPropertyDrawer`).
- Added `Gamelogic.Extensions.MinMaxRangeAttribute` that can be applied tp on a field of type `Gamelogic.Extensions.MinMaxInt` or `Gamelogic.Extensions.MinMaxFloat` to support custom ranges in the inspector.
- Added `Gamelogic.Extensions.Support.Branding` to make it easier to make examples have a consistent look and feel. 

### 🧹 Refactors
- Deprecated `Gamelogic.Extensions.Algorithms.IndexPriorityQueue` (to be replaced with  `Gamelogic.Extensions.Algorithms.RandomAccessQueue`).

### ⬆️ Upgrade notes
- Replace `Gamelogic.Extensions.Algorithms.IndexPriorityQueue` with `Gamelogic.Extensions.Algorithms.RandomAccessQueue`.

### 🧪 Samples
- Added a more complex generators example to show how they can be used for procedural generation, and restructured the 
existing two example into folder. 
- Added a `Gamelogic.Extensions.MinMaxInt` to the property drawers sample.

## [4.1.0] - 2024-07-12
### 🎉 New Features
- Added `Gamelogic.Extensions.Algorithms.IndexPriorityQueue.Remove` method to `Gamelogic.Extensions.Algorithms.IndexPriorityQueue`.
- Added a ring buffer data structure and related classes:
	- `Gamelogic.Extensions.Algorithms.IBuffer`
	- `Gamelogic.Extensions.Algorithms.IResizeableBuffer`
	- `Gamelogic.Extensions.Algorithms.Buffer`
	- `Gamelogic.Extensions.Algorithms.Capacity2Buffer`
	- `Gamelogic.Extensions.Algorithms.RingBuffer`
- Added classes to implement PID control:
	- `Gamelogic.Extensions.Algorithms.PidController`
	- `Gamelogic.Extensions.Algorithms.Differentiator`
	- `Gamelogic.Extensions.Algorithms.Integrator`
- Added `Gamelogic.Extensions.VallueSnapshot` to represent a value and the previous value. 

### 🔧 Engineering
- Fixed various multiple enumeration issues in `Gamelogic.Extensions.Algorithms.CollectionExtensions`.

### 🐞 Bug Fixes
- Fixed warnings related to the pool samples. 
- Fixed warnings caused by `Gamelogic.Extensions.FpsCounter`.

### 📄 Documentation
- Improved pool documentation.

## [4.0.0] - 2024-07-08
### 🎉 New Features
- Added the `Gamelogic.Extensions.Algorithms.IndexPriorityQueue` data structure. 
- Added an experimental `Gamelogic.Extensions.FpsCounter` mostly to be used in example benchmarks.
- Added `Gamelogic.Extensions.ThrowHelper`.
- To `Gamelogic.Extensions.Algorithms.CollectionExtensions`, added:
	- `Gamelogic.Extensions.Algorithms.CollectionExtensions.Fill`
	- `Gamelogic.Extensions.Algorithms.CollectionExtensions.FillWithDefault`
- To `Gamelogic.Extensions.GLMathf` , added `Gamelogic.Extensions.GLMathf.Bilerp` and 
`Gamelogic.Extensions.GLMathf.Trilerp` for doing bilinear and trilinear interpolation.
	
### 🔧 Engineering
- Removed `Gamelogic.ObjectExtensions` (replaced with `Gamelogic.Extensions.ThrowHelper`).
- Replaced `Gamelogic.Extensions.PoolObject` with an interface `Gamelogic.Extensions.IPoolObject`
- Constrained type parameter of `Gamelogic.Extensions.MonoBehaviourPool` to `UnityEngine.Component` instead of 
`UnityEngine.MonoBehaviour` so it can also be used with Unity components. 

### ⬆️ Upgrade notes
- If you used `Gamelogic.ObjectExtensions.ThrowIfNull` replace it with `Gamelogic.Extensions.ThrowHelper.ThrowIfNull`.
- If you used `Gamelogic.ObjectExtensions.ThrowIfNegative` replace it with `Gamelogic.Extensions.ThrowHelper.ThrowIfNegative`.
- If you used `Gamelogic.Extensions.PoolObject` replace it with `Gamelogic.Extensions.IPoolObject`.

### 🧪 Samples
- Made the minimal pool example more compelling. 
- Added a pool benchmark example. 
- Added scripts that show how the pool types can be used to build more sophisticated pools. 

## [3.2.0]
### 🎉 New Features
- Added a new interface for pools `Gamelogic.Extensions.IPool`.
- Added methods to `Gamelogic.Extensions.Pool` and `Gamelogic.Extensions.MonoBehaviourPool` to implement the new
  `Gamelogic.Extensions.IPool` interface.
- Added `Gamelogic.Extensions.HashPool` for faster release of active objects.
- Added `Gamelogic.Extensions.PoolExtensions`.
- Added an `Gamelogic.Extensions.UnsafePool` for benchmarking
- Added a default message for `Gamelogic.Extensions.WarningIfNullAttribute`.
- Added a new pattern: `Gamelogic.Extensions.ImplementationFactory`, which creates instances of a class based on
  the type of a generic
  argument.
- Added `Gamelogic.Extensions.Algorithms.IResponseCurve`, a new interface for response curves.
- New collection extension methods:
	- `Gamelogic.Extensions.Algorithms.CollectionExtensions.HasSingle`
	- `Gamelogic.Extensions.Algorithms.CollectionExtensions.TrySingle`
	- `Gamelogic.Extensions.Algorithms.CollectionExtensions.MaxItemsBy`
	- `Gamelogic.Extensions.Algorithms.CollectionExtensions.MinItemsBy`
- New Image extension methods:
	- `Gamelogic.Extensions.ImageExtensions.SetAlpha`
	- `Gamelogic.Extensions.ImageExtensions.SetVisible`

### 🐞 Bug Fixes

- Fixed a bug where `Gamelogic.Extensions.InspectorButtonAttribute` would cause public or protected methods to be added
  for each subclass.
- Fixed a bug where `Gamelogic.Extensions.InspectorButtonAttribute` would also render a warning for fields that
  are of types that cannot be null.
- Fixed a bug in `Gamelogic.Extensions.TransformExtensions.SelfAndAllChildren` that caused an infinite loop.
- Now ensures the create action in the `Gamelogic.Extensions.Pool` constructor is not null.
- Now throws exceptions when calling `Gamelogic.Extensions.Pool.Release` on an item that is not in the pool.
- Capacities, increments and decrements for `Gamelogic.Extensions.Pool` methods are checked for being negative.

### 🔧 Engineering

- `Gamelogic.Extensions.ObjectExtensions.ThrowIfNull` now throws an `System.ArgumentNullException` instead of a
  `System.NullReferenceException` to reflect its intended use case.
- Removed a redundant overload of `Gamelogic.Extensions.Algorithms.CollectionExtensions.MaxBy`.

### 🚀 Performance Improvements

- Slightly improved the performance of `Gamelogic.Extensions.Pool.DecCapacity`.

### 📄 Documentation

- Improved the documentation for `Gamelogic.Extensions.Pool`.
- Added documentation for `Gamelogic.Extensions.Algorithms.CollectionExtensions.TryFirst` and
  `Gamelogic.Extensions.Algorithms.CollectionExtensions.TryLast`
  
### ⬆️ Upgrade notes

- If you relied on the `System.NullReferenceException` thrown by `Gamelogic.Extensions.ObjectExtensions.ThrowIfNull`, 
you need to update your code reflect that now it throws an `System.ArgumentNullException`.

## [3.1.0]
### 🎉 New Features
- 	Added `Gamelogic.Extensions.LifeCycleEvent` and `Gamelogic.Extensions.LifeCycleEventExtensions` to make it easier
to specify when logic should execute in the inspector.
- Disabled the GLMonoBehaviourEditor when the Odin inspector is present to avoid conflicts.

## [3.0.2] // Should be [3.1.0], go through code
### 🐞 Bug Fixes
- Set the editor assembly to only build for the editor. 

## [3.0.0]
### 🎉 New Features
- Added `Gamelogic.Extensions.Singleton<T>.FindAndConnectInstance` method.
- Added `Gamelogic.Extensions.Singleton<T>.IsReady` property.
- Added `Gamelogic.Extensions.Singleton<T>.HasInstanceInOpenScenes` property.
- Added the new types `Gamelogic.Extensions.Singleton` and `Gamelogic.Extensions.Singleton.FindResult`.
- Added `Gamelogic.Extensions.Warning` class to help deal with compiler and code-inspection warnings.
- Added `Gamelogic.Extensions.GameObjectExtensions.GetRequiredComponent` and `Gamelogic.Extensions.
GameObjectExtensions.GetRequiredComponentInChildren` extension methods.
- Added `Gamelogic.Extensions.GLMonoBehaviour.FindRequiredObjectOfType` method.
- Added `Gamelogic.Extensions.CollectionExtension` methods:
	- `Gamelogic.Extensions.CollectionExtension.CollectionExtension.TryFirst`
	- `Gamelogic.Extensions.CollectionExtension.CollectionExtension.TryLast`
	- `Gamelogic.Extensions.CollectionExtension.CollectionExtension.AsCountable` for cases where a count is needed
	  for an enumerable.
	- `Gamelogic.Extensions.CollectionExtension.CollectionExtension.AsList` for more convenient casting / enumerating
	  of an enumerable
	- `Gamelogic.Extensions.CollectionExtension.HasSameElementsAs` for comparing two collections for equality.
	- `Gamelogic.Extensions.CollectionExtension.Aggregate` that takes two aggregators, run them in parallel, and
	  returns the result as a tuple.
	- `Gamelogic.Extensions.CollectionExtension.MinMax` that return the minimum and maximum of a collection of
	  numbers as a tuple.
- Added `Gamelogic.Extensions.GLMathf.Equal` to centralize warning suppression when comparing floats using equality.

### 🔧 Engineering
- Made `Gamelogic.Extensions.Singleton<T>.OnDestroy` method public.
- Made `Gamelogic.Extensions.Singleton<T>.Instance` property public.
- Marked `Gamelogic.Extensions.InspectorButtonAttribute` as `MeansImplicitUse` so such methods can be private and not
  flag warnings.
- Assigned defaults to serialized fields in several example mono behaviours.
- Renamed `SingletonPrefabSpawner` to Gamelogic.Extensions.SingletonSpawner.
- Made `Gamelogic.Extensions.SingletonSpawner` more robust by checking for more than one existing instance and better
  error reporting.
- Restructured as a package.
