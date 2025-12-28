using System;
using System.Collections.Generic;
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

		public const int WM_NCCALCSIZE = 0x0083;
		public const int WM_SYSCOMMAND = 0x0112;
		public const int SC_MINIMIZE = 0xF020;
		public const int SC_RESTORE = 0xF120;

		public enum RegistryCursor
		{
			AppStarting,
			Arrow,
			Crosshair,
			Hand,
			Help,
			IBeam,
			No,
			SizeAll,
			SizeNESW,
			SizeNS,
			SizeNWSE,
			SizeWE,
			UpArrow,
			Wait
		}

		private static readonly Dictionary<RegistryCursor, (Cursor Cursor, string Path)> cursors = new Dictionary<RegistryCursor, (Cursor Cursor, string Path)>();
		private static readonly Dictionary<RegistryCursor, IntPtr> formCursors = new Dictionary<RegistryCursor, IntPtr>()
		{
			{ RegistryCursor.AppStarting, Cursors.AppStarting.Handle },
			{ RegistryCursor.Arrow, Cursors.Arrow.Handle },
			{ RegistryCursor.Crosshair, Cursors.Cross.Handle },
			{ RegistryCursor.Hand, Cursors.Hand.Handle },
			{ RegistryCursor.Help, Cursors.Help.Handle },
			{ RegistryCursor.IBeam, Cursors.IBeam.Handle },
			{ RegistryCursor.No, Cursors.No.Handle },
			{ RegistryCursor.SizeAll, Cursors.SizeAll.Handle },
			{ RegistryCursor.SizeNESW, Cursors.SizeNESW.Handle },
			{ RegistryCursor.SizeNS, Cursors.SizeNS.Handle },
			{ RegistryCursor.SizeNWSE, Cursors.SizeNWSE.Handle },
			{ RegistryCursor.SizeWE, Cursors.SizeWE.Handle },
			{ RegistryCursor.UpArrow, Cursors.UpArrow.Handle },
			{ RegistryCursor.Wait, Cursors.WaitCursor.Handle }
		};

		public static void GetRegistryCursor(RegistryCursor cursor, Control control)
		{
			using (RegistryKey curKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors"))
			{
				string cursorPath = (string)curKey.GetValue(cursor.ToString());
				bool flag = cursors.TryGetValue(cursor, out (Cursor Cursor, string Path) pair);

				if (!flag || pair.Path != cursorPath)
				{
					if (flag) pair.Cursor.Dispose();

					IntPtr handle = !string.IsNullOrEmpty(cursorPath) ? LoadCursorFromFile(cursorPath) : formCursors[cursor];

					Cursor newCur = new Cursor(handle);
					cursors[cursor] = (newCur, null);
					control.Cursor = newCur;
				}
				else control.Cursor = pair.Cursor;
			}
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
	}
}
