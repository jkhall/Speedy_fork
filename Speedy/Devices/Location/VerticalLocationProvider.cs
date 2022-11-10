﻿#region References

using System;
using System.Threading.Tasks;
using Speedy.Serialization;

#endregion

namespace Speedy.Devices.Location;

/// <summary>
/// Represent a provider of vertical location also known as altitude.
/// </summary>
/// <typeparam name="T"> The type that implements IVerticalLocation. </typeparam>
public abstract class VerticalLocationProvider<T> : Bindable, IVerticalLocationProvider<T>
	where T : class, IVerticalLocation, ICloneable<T>, new()
{
	#region Constructors

	/// <summary>
	/// Create an instance of the altitude provider.
	/// </summary>
	/// <param name="dispatcher"> An optional dispatcher. </param>
	protected VerticalLocationProvider(IDispatcher dispatcher) : base(dispatcher)
	{
		LastReadLocation = new T();
		LastReadLocation.UpdateDispatcher(dispatcher);
	}

	#endregion

	#region Properties

	/// <inheritdoc />
	public virtual bool IsListening { get; protected set; }

	/// <inheritdoc />
	public T LastReadLocation { get; }

	#endregion

	#region Methods

	/// <inheritdoc />
	public T GetVerticalLocation()
	{
		return LastReadLocation;
	}

	/// <inheritdoc />
	public abstract Task StartListeningAsync();

	/// <inheritdoc />
	public abstract Task StopListeningAsync();

	/// <summary>
	/// Trigger the vertical location changed event.
	/// </summary>
	/// <param name="e"> The updated location. </param>
	protected virtual void OnLocationChanged(T e)
	{
		LocationChanged?.Invoke(this, e);
	}

	#endregion

	#region Events

	/// <inheritdoc />
	public event EventHandler<T> LocationChanged;

	#endregion
}