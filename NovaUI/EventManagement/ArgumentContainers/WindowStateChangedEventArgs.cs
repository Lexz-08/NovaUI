using System;
using System.Windows.Forms;

namespace NovaUI.EventManagement.ArgumentContainers
{
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

		public WindowStateChangedEventArgs(FormWindowState previous, FormWindowState current)
		{
			_prevState = previous;
			_currentState = current;
		}
	}
}
