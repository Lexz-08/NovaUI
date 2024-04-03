using NovaUI.Controls;

namespace NovaUI.Enums
{
	/// <summary>
	/// Specifies how the tabs of a <see cref="NovaTabControl"/> are drawn.
	/// </summary>
	public enum TabStyle
	{
		/// <summary>
		/// A solid-color rectangle is drawn to represent the tab.
		/// </summary>
		Block,

		/// <summary>
		/// A hollow solid-color rectangle is drawn to represent the tab.
		/// </summary>
		Box,

		/// <summary>
		/// A solid-color rectangle is drawn under the text to represent the tab.
		/// </summary>
		Underline,

		/// <summary>
		/// A solid-color rectangle is drawn to represent the tab with curves at the corners for the far left and right tabs.
		/// </summary>
		RadialBlock,

		/// <summary>
		/// A hollow solid-color rectangle with curves at the corners is drawn to represent the tab.
		/// </summary>
		RadialBox,

		/// <summary>
		/// A solid-color rectangle with curves at the ends is drawn under the text to represent the tab.
		/// </summary>
		RadialUnderline
	}
}
