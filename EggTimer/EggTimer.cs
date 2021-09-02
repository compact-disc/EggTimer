using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;

namespace EggTimer
{
    public partial class mainForm : Form
    {
        private TimeSpan timeSpan;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint modifier, uint vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //ID for the F6, F7 Hotkey, F8 hotkey used later to check which hotkey is being used
        private const int F6HOTKEYID = 1;
        private const int F7HOTKEYID = 2;
        private const int F8HOTKEYID = 3;

        //Constant value for the hotkey value using WndProc
        private const int WM_HOTKEY = 0x0312;

        private Boolean reset = true;

        private Boolean running = false;

        //Modifiers for Hotkey registry
        private enum Modifiers
        {
            NONE = 0x0000,
            ALT = 0x0001,
            CONTROL = 0x0002,
            SHIFT = 0x0004,
            WINDOWS = 0x0008
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            //Check for key down press
            if (m.Msg == WM_HOTKEY)
            {
                //Switch over the ID assigned
                switch (m.WParam.ToInt32())
                {
                    case F6HOTKEYID:
                        if (running)
                        {
                            stopButton.PerformClick();
                        }
                        else if (!running)
                        {
                            startButton.PerformClick();
                        }
                        break;

                    case F7HOTKEYID:
                        restartButton.PerformClick();
                        break;

                    case F8HOTKEYID:
                        resetButton.PerformClick();
                        break;
                }
            }
            base.WndProc(ref m);
        }

        //Method to run when the Auto Clicker window loads
        private void AutoClickerFormLoad(object sender, EventArgs e)
        {
            //Register the F6 hotkey with the system
            RegisterHotKey(this.Handle, F6HOTKEYID, (uint)Modifiers.NONE, (uint)Keys.F6);

            //Register the F7 hotkey with the system
            RegisterHotKey(this.Handle, F7HOTKEYID, (uint)Modifiers.NONE, (uint)Keys.F7);

            //Register the F8 hotkey with the system
            RegisterHotKey(this.Handle, F8HOTKEYID, (uint)Modifiers.NONE, (uint)Keys.F8);
        }

        //Method to run when the Auto Clicker window closes
        private void AutoClickerFormClose(object sender, FormClosedEventArgs e)
        {
            //Unregister the F6 hotkey with the system
            UnregisterHotKey(this.Handle, F6HOTKEYID);

            //Unregister the F7 hotkey with the system
            UnregisterHotKey(this.Handle, F7HOTKEYID);

            //Unregister the F7 hotkey with the system
            UnregisterHotKey(this.Handle, F8HOTKEYID);
        }

        public mainForm()
        {
            InitializeComponent();

            //Set all the labels to transparent backgrounds
            titleLabel.BackColor = Color.Transparent;
            hoursLabel.BackColor = Color.Transparent;
            minutesLabel.BackColor = Color.Transparent;
            secondsLabel.BackColor = Color.Transparent;

            //Remove button borders
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.FlatAppearance.BorderSize = 0;

            resetButton.FlatStyle = FlatStyle.Flat;
            resetButton.FlatAppearance.BorderSize = 0;

            stopButton.FlatStyle = FlatStyle.Flat;
            stopButton.FlatAppearance.BorderSize = 0;

            restartButton.FlatStyle = FlatStyle.Flat;
            restartButton.FlatAppearance.BorderSize = 0;
        }

        private void startButton_Click(Object sender, EventArgs e)
        {
            double seconds = (double)secondsUpDown.Value;
            double minutes = (double)minutesUpDown.Value;
            double hours = (double)hoursUpDown.Value;

            double totalSeconds = seconds + (minutes * 60) + (hours * 3600);

            if (reset && !running && totalSeconds > 0)
            {

                timeSpan = TimeSpan.FromSeconds(totalSeconds);
                countdownTimer.Start();

                new ToastContentBuilder()
                    .AddText("Timer Started!")
                    .Show();

                reset = false;

                running = true;
            }
            else if (!reset && !running)
            {
                countdownTimer.Start();

                reset = false;

                running = true;
            }
        }

        private void restartButton_Click(Object sender, EventArgs e)
        {
            if (running)
            {
                countdownTimer.Stop();
                running = false;
            }

            double seconds = (double)secondsUpDown.Value;
            double minutes = (double)minutesUpDown.Value;
            double hours = (double)hoursUpDown.Value;
            double totalSeconds = seconds + (minutes * 60) + (hours * 3600);

            timeSpan = TimeSpan.FromSeconds(totalSeconds);
            countdownTimer.Start();

            new ToastContentBuilder()
                .AddText("Timer Restarted!")
                .Show();

            reset = false;

            running = true;
        }

        private void stopButton_Click(Object sender, EventArgs e)
        {
            running = false;

            countdownTimer.Stop();
        }

        private void resetButton_Click(Object sender, EventArgs e)
        {
            if (running)
            {
                countdownTimer.Stop();
                running = false;
            }

            secondsUpDown.Value = 0;
            minutesUpDown.Value = 0;
            hoursUpDown.Value = 0;

            timeSpan = TimeSpan.Zero;
            countdownLabel.Text = timeSpan.ToString(@"hh\:mm\:ss");

            reset = true;
        }

        private void countdownTimer_Tick(Object sender, EventArgs e)
        {
            if (timeSpan.TotalSeconds == 0)
            {
                countdownTimer.Stop();

                running = false;
                reset = true;

                new ToastContentBuilder()
                    .AddText("Timer Complete!")
                    .Show();
            }
            else
            {
                timeSpan = timeSpan.Subtract(TimeSpan.FromSeconds(1));
                countdownLabel.Text = timeSpan.ToString(@"hh\:mm\:ss");
            }
        }
    }
}
