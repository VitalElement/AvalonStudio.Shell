using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using AvalonStudio.Extensibility.Utils;
using System;
using System.Runtime.InteropServices;

namespace AvalonStudio.Shell.Controls
{
    public class MetroWindow : Window, IStyleable
    {
		public MetroWindow()
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				// do this in code or we get a delay in osx.
				HasSystemDecorations = false;
			}
		}

		public static readonly AvaloniaProperty<Control> TitleBarContentProperty =
            AvaloniaProperty.Register<MetroWindow, Control>(nameof(TitleBarContent));

        private Grid _bottomHorizontalGrip;
        private Grid _bottomLeftGrip;
        private Grid _bottomRightGrip;
        private Button _closeButton;
        private Image _icon;
        private Grid _leftVerticalGrip;
        private Button _minimiseButton;

        private bool _mouseDown;
        private Point _mouseDownPosition;
        private Button _restoreButton;
        private Grid _rightVerticalGrip;

        private Grid _titleBar;
        private Grid _topHorizontalGrip;
        private Grid _topLeftGrip;
        private Grid _topRightGrip;

        public Control TitleBarContent
        {
            get { return GetValue(TitleBarContentProperty); }
            set { SetValue(TitleBarContentProperty, value); }
        }

        Type IStyleable.StyleKey => typeof(MetroWindow);

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (topHorizontalGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.North);
            }
            else if (bottomHorizontalGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.South);
            }
            else if (leftVerticalGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.West);
            }
            else if (rightVerticalGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.East);
            }
            else if (topLeftGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.NorthWest);
            }
            else if (bottomLeftGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.SouthWest);
            }
            else if (topRightGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.NorthEast);
            }
            else if (bottomRightGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.SouthEast);
            }
            else if (titleBar.IsPointerOver)
            {
                mouseDown = true;
                mouseDownPosition = e.GetPosition(this);
            }
            else
            {
                mouseDown = false;
            }

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            mouseDown = false;
            base.OnPointerReleased(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if ((titleBar.IsPointerOver || topHorizontalGrip.IsPointerOver) && mouseDown)
            {
                if (mouseDownPosition.DistanceTo(e.GetPosition(this)) > 2)
                {
                    WindowState = WindowState.Normal;
                    BeginMoveDrag();
                    mouseDown = false;
                }
            }

            base.OnPointerMoved(e);
        }

        private void ToggleWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    ToolTip.SetTip(restoreButton, "Maximize");
                    break;

                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    ToolTip.SetTip(restoreButton, "Restore");
                    break;
            }
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            titleBar = e.NameScope.Find<Grid>("titlebar");
            minimiseButton = e.NameScope.Find<Button>("minimiseButton");
            restoreButton = e.NameScope.Find<Button>("restoreButton");
            closeButton = e.NameScope.Find<Button>("closeButton");
            icon = e.NameScope.Find<Image>("icon");

            topHorizontalGrip = e.NameScope.Find<Grid>("topHorizontalGrip");
            bottomHorizontalGrip = e.NameScope.Find<Grid>("bottomHorizontalGrip");
            leftVerticalGrip = e.NameScope.Find<Grid>("leftVerticalGrip");
            rightVerticalGrip = e.NameScope.Find<Grid>("rightVerticalGrip");

            topLeftGrip = e.NameScope.Find<Grid>("topLeftGrip");
            bottomLeftGrip = e.NameScope.Find<Grid>("bottomLeftGrip");
            topRightGrip = e.NameScope.Find<Grid>("topRightGrip");
            bottomRightGrip = e.NameScope.Find<Grid>("bottomRightGrip");

            minimiseButton.Click += (sender, ee) => { WindowState = WindowState.Minimized; };

            restoreButton.Click += (sender, ee) => { ToggleWindowState(); };

            titleBar.DoubleTapped += (sender, ee) => { ToggleWindowState(); };

            closeButton.Click += (sender, ee) => { Close(); };

            icon.DoubleTapped += (sender, ee) => { Close(); };

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				titleBar.IsVisible = false;

				BorderThickness = new Thickness();
			}
		}
    }
}