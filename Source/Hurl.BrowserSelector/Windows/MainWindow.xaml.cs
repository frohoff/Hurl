﻿using Hurl.BrowserSelector.Globals;
using Hurl.BrowserSelector.Helpers;
using Hurl.Library;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Path = System.IO.Path;

namespace Hurl.BrowserSelector.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //private Library.Models.Settings settings
        //{
        //    get
        //    {
        //        return (DataContext as MainViewModel).settings;
        //    }
        //}

        public MainWindow()
        {
            var settings = Globals.SettingsGlobal.Value;

            InitializeComponent();

            var osbuild = Environment.OSVersion.Version.Build;
            var backtype = settings.AppSettings?.BackgroundType;

            if (settings.AppSettings?.NoWhiteBorder == true) WindowBorder.BorderThickness = new Thickness(0);
            if (osbuild < 22000) WindowBorder.CornerRadius = new CornerRadius(0);

            if (backtype == "acrylic" && osbuild >= 22523)
            {
                WindowBackdropType = Wpf.Ui.Controls.Window.WindowBackdropType.Acrylic;
            }
            else if (backtype == "none" || osbuild < 22000)
            {
                WindowBackdropType = Wpf.Ui.Controls.Window.WindowBackdropType.None;
                var brush = Color.FromRgb(150, 50, 50);
                Background = new SolidColorBrush(brush);
            }
            else
            {
                WindowBackdropType = Wpf.Ui.Controls.Window.WindowBackdropType.Mica;
            }
        }

        public void Init(CliArgs data)
        {
            var settings = SettingsGlobal.Value;

            var x = AutoRulesCheck.CheckAllBrowserRules(data.Url, settings.Browsers);
            if (x) return;

            if (data.IsRunAsMin)
            {
                Debug.WriteLine("Minimizing--------------------------------");
                MinimizeWindow();
            }
            else
            {
                if (!data.IsSecondInstance)
                {
                    try
                    {

                        Width = settings.AppSettings?.WindowSize[0] ?? 420;
                        Height = settings.AppSettings?.WindowSize[1] ?? 210;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        throw;
                    }
                }
                Show();
                PositionWindowUnderTheMouse();
                if (data.IsSecondInstance)
                {
                    this.WindowState = WindowState.Normal;

                    ChooseType.SelectedIndex = 0;
                }
            }

            linkpreview.Content = string.IsNullOrEmpty(UriGlobal.Value) ? "No Url Opened" : UriGlobal.Value;
        }

        private void Window_Esc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MinimizeWindow();
            }
            //var keyInt = (int)e.Key;
            //if(keyInt >= 35 && keyInt <= 43)
            //{
            //}
        }

        private void LinkCopyBtnClick(object sender, RoutedEventArgs e) => Clipboard.SetText(UriGlobal.Value);
        private void SettingsBtnClick(object sender, RoutedEventArgs e) => Process.Start("notepad.exe", Constants.APP_SETTINGS_MAIN);
        private void Draggable(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void CloseBtnClick(object sender, RoutedEventArgs e) => MinimizeWindow();

        private void MinimizeWindow()
        {
            this.WindowState = WindowState.Minimized;
            this.Hide();
        }

        private void MaximizeWindow()
        {
            this.Show();
            PositionWindowUnderTheMouse();
            this.WindowState = WindowState.Normal;
        }

        private void TrayMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var settings = Globals.SettingsGlobal.Value;

            string tag = (sender as MenuItem).Tag as string;
            try
            {
                switch (tag)
                {
                    case "open":
                        MaximizeWindow();
                        break;
                    case "timed":
                        new TimeSelectWindow(settings.Browsers).ShowDialog();
                        break;
                    case "exit":
                        Application.Current.Shutdown();
                        break;
                    case "reload":
                        var AppPath = Path.Combine(AppContext.BaseDirectory, "Hurl.exe");
                        Process.Start(AppPath);
                        Application.Current.Shutdown();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void NotifyIcon_LeftClick(object sender, RoutedEventArgs e) => MaximizeWindow();

        private void Window_Deactivated(object sender, EventArgs e)
        {
#if DEBUG
            // No minimize on debug when not in focus
#else
            MinimizeWindow();
#endif
        }

        private void PositionWindowUnderTheMouse()
        {
            var settings = Globals.SettingsGlobal.Value;

            if (settings.AppSettings != null && settings.AppSettings.LaunchUnderMouse)
            {
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var mouse = transform.Transform(CursorPosition.LimitCursorWithin((int)Width, (int)Height));
                Left = mouse.X;
                Top = mouse.Y;

                Debug.WriteLine($"{Left}x{Top} while screen res: {SystemParameters.FullPrimaryScreenWidth}x{SystemParameters.FullPrimaryScreenHeight}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => new TimeSelectWindow(Globals.SettingsGlobal.Value.Browsers).ShowDialog();

        private void UiWindow_SizeChanged(object sender, SizeChangedEventArgs e) => Globals.SettingsGlobal.AdjustWindowSize(e);

        async private void linkpreview_Click(object sender, RoutedEventArgs e)
        {
            var NewUrl = await URLEditor.ShowInputAsync(this, UriGlobal.Value);
            if (!string.IsNullOrEmpty(NewUrl))
            {
                UriGlobal.Value = NewUrl;
                (sender as Button).Content = NewUrl;
                (sender as Button).ToolTip = NewUrl;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO
            UriGlobal.UrlOpenType = (sender as ComboBox).SelectedIndex;
            Debug.WriteLine("Selected: " + (sender as ComboBox).SelectedValue);
        }
    }
}