﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Spray {
    public partial class SettingsFm : Form {
        private const string StrStopping = "This app is stopping.";
        private const string StrRunning = "This app is running.";
        private static bool _closeToTray = true;
        private const int OffsetX = 20;
        private const int OffsetYHead = 30;
        private const int OffsetYEnd = 10;
        private const int CheckBoxWidth = 80;
        private const int CheckBoxHeight = 20;
        private const int GunsBoxWidth = 2 * (OffsetX + CheckBoxWidth);

        public static readonly Gun[] Guns =
            {Gun.Ak47, Gun.M4A4, Gun.M4A1S, Gun.Gali, Gun.Famas, Gun.Ump45, Gun.Aug, Gun.Sg};

        private readonly List<RadioButton> _rbs = new List<RadioButton>();
        public static Gun NowGun = Guns[0];

        public bool IsRun {
            get => cbRunning.Checked;
            set => cbRunning.Checked = value;
        }

        public bool OnlyCs => cbOnlyCs.Checked;

        public SettingsFm() {
            InitializeComponent();
            InitForm();
            InitGun();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown || cbExitToTray.Checked == false || !_closeToTray) {
                SavePreference();
                return;
            }

            e.Cancel = true;
            ntIcon.Visible = true;
            Hide();
        }

        private void InitForm() {
            LoadPreference();
            var ctxMn = new ContextMenu();

            ntIcon.ContextMenu = ctxMn;

            var menuItem = new MenuItem {
                Index = 0,
                Text = @"E&xit"
            };
            menuItem.Click += MenuExit;

            ctxMn.MenuItems.Add(menuItem);

            UpdateText();
        }

        private void SavePreference() {
            Properties.Settings.Default.OnlyCs = cbOnlyCs.Checked;
            Properties.Settings.Default.ExitToTray = cbExitToTray.Checked;
            Properties.Settings.Default.Running = cbRunning.Checked;
            Properties.Settings.Default.Save();
        }

        private void LoadPreference() {
            cbOnlyCs.Checked = Properties.Settings.Default.OnlyCs;
            cbExitToTray.Checked = Properties.Settings.Default.ExitToTray;
            cbRunning.Checked = Properties.Settings.Default.Running;
        }

        private void InitGun() {
            for (var i = 0; i < Guns.Length; i++) {
                var gun = Guns[i];
                var rb = new RadioButton {
                    Width = CheckBoxWidth,
                    Height = CheckBoxHeight,
                    Text = gun.Name,
                    Location = MyPoint(i)
                };

                rb.CheckedChanged += GunChange;

                _rbs.Add(rb);
                GunBox.Controls.Add(rb);
            }

            GunBox.Size = new Size(GunsBoxWidth,
                CheckBoxHeight * (Guns.Length + Guns.Length % 2) / 2 + OffsetYEnd + OffsetYHead);
            _rbs[0].Checked = true;
        }

        private static Point MyPoint(int i) {
            var x = i % 2;
            var y = i / 2;
            return new Point(OffsetX + x * GunsBoxWidth / 2, OffsetYHead + y * CheckBoxHeight);
        }

        public void TickGun(int a) {
            if (a >= Guns.Length || a < 0) {
                a = 0;
            }

            _rbs[a].Checked = true;
        }

        private void MenuExit(object sender, EventArgs e) {
            _closeToTray = false;
            Close();
        }

        private void GunChange(object sender, EventArgs e) {
            if (!(sender is RadioButton tCb) || !tCb.Checked) {
                return;
            }

            NowGun = Guns[_rbs.IndexOf(tCb)];
        }

        private void cbRunning_CheckedChanged(object sender, EventArgs e) {
            UpdateText();
        }

        private void UpdateText() {
            if (cbRunning.Checked) {
                cbRunning.Text = StrRunning;
                Console.Beep(1000, 200);
            } else {
                cbRunning.Text = StrStopping;
                Console.Beep(700, 50);
            }
        }

        private void ntIcon_DoubleClick(object sender, EventArgs e) {
            if (sender is NotifyIcon nc) {
                nc.Visible = false;
            }

            WindowState = FormWindowState.Normal;
            Show();
        }
    }
}
