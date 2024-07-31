using System;
using System.Windows.Forms;

using NovaUI.Controls;

namespace NovaUI.Events
{
	/// <summary>
	/// Provides data for the <see cref="NovaWindow.WindowStateChanged"/> and <see cref="NovaStrippedWindow.WindowStateChanged"/> events.
	/// </summary>
	public class WindowStateChangedEventArgs : EventArgs
	{
		private FormWindowState _prevState;
		private FormWindowState _currentState;

		/// <summary>
		/// The previous window state of the form.
		/// </summary>
		public FormWindowState PreviousState => _prevState;

		/// <summary>
		/// The current window state of the form.
		/// </summary>
		public FormWindowState CurrentState => _currentState;

		/// <summary>
		/// Creates a new <see cref="WindowStateChangedEventArgs"/>.
		/// </summary>
		/// <param name="previous">The previously set state of the window.</param>
		/// <param name="current">The currently set state of the window.</param>
		public WindowStateChangedEventArgs(FormWindowState PreviousState, FormWindowState CurrentState)
		{
			_prevState = PreviousState;
			_currentState = CurrentState;
		}
	}
}
