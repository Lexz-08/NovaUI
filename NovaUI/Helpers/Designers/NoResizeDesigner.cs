using System.Windows.Forms.Design;

namespace NovaUI.Helpers.Designers
{
	internal class NoResizeDesigner : ControlDesigner
	{
		public override SelectionRules SelectionRules => SelectionRules.Moveable;
	}
}
