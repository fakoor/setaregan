using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Data;
using System.Diagnostics;

namespace Setaregan
{
    public partial class frmSetaregan : Form
    {
        public frmSetaregan()
        {
            InitializeComponent();
        }

        private void frmSetaregan_Load(object sender, EventArgs e)
        {
            this.Text += " - "+ txtMainClass.Text;

            var bCamUSer = GetCameraUser();
            var faYes = "بله";
            var faNo = "خیر";
            if (bCamUSer)
                lblCameraCurrentUser.Text = faYes;
            else
                lblCameraCurrentUser.Text = faNo;

            var bCamUSys = GetCameraSystem();
            if (bCamUSys)
                lblCameraSystem.Text = faYes;
            else
                lblCameraSystem.Text = faNo;

            if (bCamUSer && bCamUSys)
            {
                btnRepairCamPermission.Enabled = false;
            }
            else
            {
                var s = Environment.OSVersion.VersionString;
                MessageBox.Show(s);
                //if ()
                btnRepairCamPermission.Enabled = true;
            }
            tmrSetaregan.Start();
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void tmrSetaregan_Tick(object sender, EventArgs e)
        {
            var tNow = DateTime.Now;

            var tHour = tNow.Hour;
            var tMinute = tNow.Minute;
            var tSecond = tNow.Second;
            var nowStamp = new TimeSpan(tHour, tMinute, tSecond);
            lblClock.Text = nowStamp.ToString();
            var m = (int) nowStamp.TotalMinutes;
            var starts = new int[]
            {
                7 * 60 + 40,
                9 * 60,
                10 * 60 + 10,
                11 * 60 + 20,
                12 * 60 +35,
                13 * 60 + 30

            };
            var darRest = "استراحت";

            var daRiazi = "ریاضی";
            var darFarsiR = "فارسی‌خ";
            var daFarsiW = "فارسی‌ن";
            var daFarsiRW = "فارسی‌خ‌ن";

            var darEn = "انگلیسی";
            var darFr = "فرانسه";
            var darOloom = "علوم";
            var darQuran = "قرآن";
            var darEmla = "املا";
            var darHedie = "هدیه";
            var darArt = "هنر";
            var darSport = "ورزش";
            var darDummy = "-";

            var dars = new string [7][]{
                    new string[] {darRest, darSport , daRiazi, darEn, darFr, darFarsiR}, //Sunday
                    new string[] {darRest, darFarsiR, daRiazi, darOloom, darEn, darArt},
                    new string[] { daRiazi, darHedie , darQuran, darEn, darFr, darRest},
                    new string[] {darEmla, daRiazi, darOloom, darFr, darEn, daFarsiW},
                    new string[] { darDummy, darDummy, darDummy, darDummy, darDummy, darDummy}, //Thursday
                    new string[] { darDummy, darDummy, darDummy, darDummy, darDummy, darDummy}, //Friday
                    new string[] {darRest, daFarsiRW , daRiazi, darFarsiR, darEn, darOloom} //Saturday
            };
            var zangIndex = -1;
            while (zangIndex < starts.Length - 1 && m > starts[zangIndex + 1])
                zangIndex++;

            if (zangIndex == starts.Length - 1 && m > starts[zangIndex])
                zangIndex++;

            var tDay = tNow.DayOfWeek;
            var nDay = (int)tDay;

            var zangDur = 1 * 60;
            if (zangIndex < 0)
            {
                lblZang.Text = "پیش‌گشایش";

                lblLesson.Text = dars[nDay][0];
            }
            else if (zangIndex >= starts.Length)
            {
                lblZang.Text = "اتمام‌دروس";
                lblLesson.Text = dars[nDay][starts.Length - 1];
                if (nDay <= 2 || nDay >= 5)
                {
                    lblLocDescr.Text = "تا  فردا";
                }
                else
                {
                    lblLocDescr.Text = "آخر هفته خوبی داشته‌باش";

                }
            }
            else
            {

                if (m < starts[zangIndex] + zangDur)
                {
                    lblZang.Text = (zangIndex + 1).ToString()+ "زنگ";
                    lblLesson.Text = dars[nDay][zangIndex];
                    lblLocDescr.Text = "تا پایان";

                }
                else 
                {
                    lblZang.Text = darRest;
                    if (zangIndex == starts.Length - 1)
                    {
                        lblLocDescr.Text = "تا تعطیلی";
                    }
                    else
                    {
                        lblLocDescr.Text = dars[nDay][zangIndex + 1] + " تا زمان ";
                    }
                }
            }

            var classId = txtMainClass.Text;
            if (lblLocDescr.Text.Equals(darEn))
            {
                classId = "en" + txtEn.Text;
            }
            else if (lblLocDescr.Text.Equals(darEn))
            {
                classId = "fr" + txtFr.Text;
            }

            var url = "https://"+txtServerAddress.Text+ "/"
                + classId 
                + ((chkOpenInApp.Checked)?"?proto=true":"");

            lblUrl.Text = url;
            switch (tDay)
            {
                case DayOfWeek.Saturday:
                    break;
            }
        }
        private bool GetCameraSystem()
        {
            var baseKey = Registry.LocalMachine;
            return GetCameraStatus(baseKey);
        }
        private bool GetCameraUser()
        {
            var baseKey = Registry.CurrentUser;
            return GetCameraStatus(baseKey);
        }
        private bool GetCameraStatus(RegistryKey baseKey)
        {
            bool ret = false;
            try
            {
                var path = @"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam";
                RegistryKey key = baseKey.OpenSubKey(path);

                    if (key != null)
                    {
                        string regval = key.GetValue("Value") as string;
                        if (!string.IsNullOrEmpty(regval))
                        {
                            if (string.Equals(regval, "Allow", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ret =  true;
                            }
                        }
                    }
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                //react appropriately
            }
            return ret;
        }

        private void lblUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url;
            if (e.Link.LinkData != null)
                url = e.Link.LinkData.ToString();
            else
                url = lblUrl.Text.Substring(e.Link.Start, e.Link.Length);

            if (!url.Contains("://"))
                url = "https://" + url;

            var si = new ProcessStartInfo(url);
            Process.Start(si);
            lblUrl.LinkVisited = true;
        }
    }
}
