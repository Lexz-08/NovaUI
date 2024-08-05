using System.Windows.Forms.Design;

namespace NovaUI.Helpers.Designers
{
	internal class LengthResizeDesigner : ControlDesigner
	{
		public override SelectionRules SelectionRules => SelectionRules.Moveable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
	}
}
