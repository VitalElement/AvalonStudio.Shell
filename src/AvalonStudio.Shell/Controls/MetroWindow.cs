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
            if (_topHorizontalGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.North);
            }
            else if (_bottomHorizontalGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.South);
            }
            else if (_leftVerticalGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.West);
            }
            else if (_rightVerticalGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.East);
            }
            else if (_topLeftGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.NorthWest);
            }
            else if (_bottomLeftGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.SouthWest);
            }
            else if (_topRightGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.NorthEast);
            }
            else if (_bottomRightGrip.IsPointerOver)
            {
                BeginResizeDrag(WindowEdge.SouthEast);
            }
            else if (_titleBar.IsPointerOver)
            {
				_mouseDown = true;
				_mouseDownPosition = e.GetPosition(this);
            }
            else
            {
				_mouseDown = false;
            }

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
			_mouseDown = false;
            base.OnPointerReleased(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if ((_titleBar.IsPointerOver || _topHorizontalGrip.IsPointerOver) && _mouseDown)
            {
                if (_mouseDownPosition.DistanceTo(e.GetPosition(this)) > 2)
                {
                    WindowState = WindowState.Normal;
                    BeginMoveDrag();
					_mouseDown = false;
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
                    ToolTip.SetTip(_restoreButton, "Maximize");
                    break;

                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    ToolTip.SetTip(_restoreButton, "Restore");
                    break;
            }
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
			_titleBar = e.NameScope.Find<Grid>("titlebar");
			_minimiseButton = e.NameScope.Find<Button>("minimiseButton");
			_restoreButton = e.NameScope.Find<Button>("restoreButton");
			_closeButton = e.NameScope.Find<Button>("closeButton");
			_icon = e.NameScope.Find<Image>("icon");

			_topHorizontalGrip = e.NameScope.Find<Grid>("topHorizontalGrip");
			_bottomHorizontalGrip = e.NameScope.Find<Grid>("bottomHorizontalGrip");
			_leftVerticalGrip = e.NameScope.Find<Grid>("leftVerticalGrip");
			_rightVerticalGrip = e.NameScope.Find<Grid>("rightVerticalGrip");

			_topLeftGrip = e.NameScope.Find<Grid>("topLeftGrip");
			_bottomLeftGrip = e.NameScope.Find<Grid>("bottomLeftGrip");
			_topRightGrip = e.NameScope.Find<Grid>("topRightGrip");
			_bottomRightGrip = e.NameScope.Find<Grid>("bottomRightGrip");

			_minimiseButton.Click += (sender, ee) => { WindowState = WindowState.Minimized; };

			_restoreButton.Click += (sender, ee) => { ToggleWindowState(); };

			_titleBar.DoubleTapped += (sender, ee) => { ToggleWindowState(); };

			_closeButton.Click += (sender, ee) => { Close(); };

			_icon.DoubleTapped += (sender, ee) => { Close(); };

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				_titleBar.IsVisible = false;
				_topHorizontalGrip.IsVisible = false;
				_bottomHorizontalGrip.IsHitTestVisible = false;
				_leftVerticalGrip.IsHitTestVisible = false;
				_rightVerticalGrip.IsHitTestVisible = false;
				_topLeftGrip.IsHitTestVisible = false;
				_bottomLeftGrip.IsHitTestVisible = false;
				_topRightGrip.IsHitTestVisible = false;
				_bottomRightGrip.IsHitTestVisible = false;

				BorderThickness = new Thickness();
			}
		}
    }
}