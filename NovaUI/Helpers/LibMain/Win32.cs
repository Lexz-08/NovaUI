using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Win32;

namespace NovaUI.Helpers.LibMain
{
	internal struct Win32
	{
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursorFromFile(string lpFilename);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

		[DllImport("dwmapi.dll")]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

		[DllImport("dwmapi.dll")]
		public static extern int DwmSetWindowAttribute(IntPtr hWnd, int attr, ref int attrValue, int attrSize);

		[DllImport("dwmapi.dll")]
		public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

		[StructLayout(LayoutKind.Sequential)]
		public struct MARGINS
		{
			public int leftWidth;
			public int rightWidth;
			public int topHeight;
			public int bottomHeight;
		}

		public const int CS_DROPSHADOW = 0x00020000;

		public static Cursor RegCursor(string cursor)
		{
			RegistryKey curKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors");
			Cursor cCursor;

			/*
			 
			AppStarting
			Arrow
			Cross
			Default
			Hand
			Help
			IBeam
			No
			SizeAll
			SizeNESW
			SizeNS
			SizeNWSE
			SizeWE
			UpArrow
			WaitCursor

			 */

			switch (cursor)
			{
				case "AppStarting":
					string cursorPath = (string)curKey.GetValue("AppStarting");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.AppStarting.Handle);
					break;
				case "Arrow":
					cursorPath = (string)curKey.GetValue("Arrow");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.Arrow.Handle);
					break;
				case "Cross":
					cursorPath = (string)curKey.GetValue("Crosshair");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.Cross.Handle);
					break;
				case "Hand":
					cursorPath = (string)curKey.GetValue("Hand");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.Hand.Handle);
					break;
				case "Help":
					cursorPath = (string)curKey.GetValue("Help");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.Help.Handle);
					break;
				case "IBeam":
					cursorPath = (string)curKey.GetValue("IBeam");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.IBeam.Handle);
					break;
				case "No":
					cursorPath = (string)curKey.GetValue("No");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.No.Handle);
					break;
				case "SizeAll":
					cursorPath = (string)curKey.GetValue("SizeAll");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.SizeAll.Handle);
					break;
				case "SizeNESW":
					cursorPath = (string)curKey.GetValue("SizeNESW");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.SizeNESW.Handle);
					break;
				case "SizeNS":
					cursorPath = (string)curKey.GetValue("SizeNS");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.SizeNS.Handle);
					break;
				case "SizeNWSE":
					cursorPath = (string)curKey.GetValue("SizeNWSE");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.SizeNWSE.Handle);
					break;
				case "SizeWE":
					cursorPath = (string)curKey.GetValue("SizeWE");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.SizeWE.Handle);
					break;
				case "UpArrow":
					cursorPath = (string)curKey.GetValue("UpArrow");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.UpArrow.Handle);
					break;
				case "WaitCursor":
					cursorPath = (string)curKey.GetValue("Wait");
					cCursor = new Cursor(!string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : Cursors.WaitCursor.Handle);
					break;
				default: case "Default": cCursor = Cursors.Default; break;
			}

			curKey.Dispose();
			return cCursor;
		}

		public static bool CheckAeroEnabled()
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				int enabled = 0;
				DwmIsCompositionEnabled(ref enabled);

				return enabled == 1;
			}

			return false;
		}

		public static string NumberToHex(int number, bool caps)
		{
			if (number > 0)
			{
				string result = "";

				while (number != 0)
				{
					int quotient = number / 16;
					int remainder = number % 16;

					Console.WriteLine(number);

					number = quotient;
					result = Constants.Hexadecimals[caps][remainder] + result;

					Console.Write(quotient);
					Console.WriteLine(remainder);
					Console.Write(result);
				}

				return result;
			}
			else return "0";
		}
	}
}
