﻿using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

namespace Spray {
    internal class SprayThread {
        public const string Csgo = "Counter-Strike: Global Offensive";
        private const int DelaySpray = 98;
        private int _count;
        private INPUT _input;

        private const int RdX = 0;
        private const int RdY = 0;
        private bool _spraying;
        private bool _runThread = true;
        private readonly EventWaitHandle _signal;

        public SprayThread() {
            _signal = new EventWaitHandle(false, EventResetMode.AutoReset);
            var t = new Thread(() => {
                while (_runThread) {
                    if (_spraying == false) {
                        _signal.WaitOne();
                    }

                    if (!Program.IsRun) {
                        continue;
                    }

                    if (Program.OnlyCs && NativeMethods.GetActiveWindowTitle() != Csgo) {
                        continue;
                    }

                    Move();
                    Thread.Sleep(DelaySpray);
                }
            });
            t.Start();
        }

        public void Stop() {
            _count = 0;
            _spraying = false;
            _signal.Reset();
        }

        public void Start() {
            _spraying = true;
            _signal.Set();
        }

        public void Exit() {
            _runThread = false;
            _signal.Set();
            _signal.Close();
        }

        private void Move() {
            int x = 0, y = 0;
            if (_count < SettingsFm.NowGun.Pattern.Length / 2) {
                x = SettingsFm.NowGun.Pattern[_count, 0];
                y = SettingsFm.NowGun.Pattern[_count, 1];
            }

            _count++;
            _input.type = NativeMethods.INPUT_MOUSE;
            _input.mi.mouseData = 0;
            _input.mi.time = 0;

            x = x - RdX;
            y = y - RdY;
            //rdX = rand() % 4 - 2;
            //rdY = rand() % 4 - 2;
            x = x - RdX;
            y = y - RdY;
            _input.mi.dx = x;
            _input.mi.dy = y;
            _input.mi.dwFlags = NativeMethods.MOUSEEVENTF_MOVE;

            var keyList = new List<INPUT> {
                _input
            };
            NativeMethods.SendInput(1, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
