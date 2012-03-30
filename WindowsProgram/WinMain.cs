using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Reflection;

namespace WindowsProgram
{
	static class WinMain
	{
		delegate long WndProc(IntPtr hWnd, uint message, uint wParam, uint lParam);

		[DllImport("User32.dll")]
		static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

		[DllImport("User32.dll")]
		static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowname, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

		[DllImport("User32.dll")]
		static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("User32.dll")]
		static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

		[DllImport("User32.dll")]
		static extern long DispatchMessage(ref MSG lpmsg);

		[DllImport("User32.dll")]
		static extern int TranslateMessage (ref MSG lpmsg);

		[DllImport("User32.dll")]
		static extern long DefWindowProc(IntPtr hWnd, uint message, uint wParam, uint lParam);

		[DllImport("User32.dll")]
		static extern void PostQuitMessage(int nExitCode);

		[StructLayout(LayoutKind.Sequential)]
		private struct WNDCLASSEX
		{
			public uint cbSize;
			public uint style;
			public WndProc lpfnWndProc;
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			public string lpszMenuName;
			public string lpszClassName;
			public IntPtr hIconSm;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct MSG
		{
			public IntPtr hwnd;
			public uint message;
			public uint wParam;
			public uint lParam;
			public uint time;
			public POINT pt;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct POINT
		{
			public long x;
			public long y;
		}

		[STAThread]
		static void Main()
		{
			Module[] ms = Assembly.GetEntryAssembly().GetModules();
			IntPtr hInst = Marshal.GetHINSTANCE(ms[0]);

			WNDCLASSEX wc = new WNDCLASSEX();
			wc.cbSize = (uint)Marshal.SizeOf(wc);
			wc.style = 0;
			wc.lpfnWndProc = WindowProcedure;
			wc.cbClsExtra = 0;
			wc.cbWndExtra = 0;
			wc.hInstance = hInst;
			wc.hIcon = (IntPtr)null;
			wc.hCursor = (IntPtr)null;
			wc.hbrBackground = (IntPtr)(5 + 1);	// COLOR_WINDOW=5
			wc.lpszMenuName = null;
			wc.lpszClassName = "TestClass";
			wc.hIcon = (IntPtr)null;

			RegisterClassEx(ref wc);

			IntPtr hWnd = CreateWindowEx(
				0x00040000,				// WS_EX_APPWINDOW
				"TestClass",
				"WindowsProgram",
				(uint)(0x00000000 | 0x00C00000 | 0x00080000 | 0x00040000 | 0x00020000L | 0x00010000L) ,// WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX
				unchecked((int)0x80000000), unchecked((int)0x80000000), // CW_USEDEFAULT
				unchecked((int)0x80000000), unchecked((int)0x80000000), // CW_USEDEFAULT
				(IntPtr)null, (IntPtr)null, hInst, (IntPtr)null
			);

			ShowWindow(hWnd, 5);	// SW_SHOW

			MSG msg;
			while (GetMessage(out msg, (IntPtr)null, 0, 0) != 0)
			{
				Console.Write("(" +  msg.message.ToString("X") + ",");
				DispatchMessage(ref msg);
				TranslateMessage(ref msg);
			}
		}

		static private long WindowProcedure(IntPtr hWnd, uint message, uint wParam, uint lParam)
		{
			Console.Write(message.ToString("X")+"),");
			switch (message)
			{
				case 0x0002:	// WM_DESTROY
					PostQuitMessage(0);
					return 0;
				default:
					return DefWindowProc(hWnd, message, wParam, lParam);
			}
		}
	}
}
