using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.Preferences;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware;
using UK.CO.Chrisjenx.Calligraphy;
//
//AHTextViewTitle
namespace SMD
{
    [Activity(Label = "Sensor L-T", Theme = "@android:style/Theme.Material.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SensorLTActivity : Activity,IDialogInterfaceOnClickListener 
    {
        TextView TVMain;Sensor dfsensor;SensorManager dfsm;Button btnRun;
        PowerManager dfpowermanager; PowerManager.WakeLock WLock;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SLT);
            //Window.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.ParseColor("#f5f5dc")));
            TVMain = (TextView)FindViewById(Resource.Id.SLTextViewNotes);
            btnRun = (Button)FindViewById(Resource.Id.BtnLTARun);
            btnRun.Click += BtnRun_Click;
            btnRun.SetTextColor(Color.WhiteSmoke);
            btnRun.Background.SetColorFilter(Color.ParseColor("#808080"), PorterDuff.Mode.Multiply);

            dfsm = (SensorManager)GetSystemService(Context.SensorService);
            List<Sensor> ls = new List<Sensor>(dfsm.GetSensorList(SensorType.MagneticField));
            if (ls.Count > 0)
            {
                dfsensor = dfsm.GetDefaultSensor(SensorType.MagneticField);
                TVMain.Text = "دستگاه شمااز سنسور مغاطیس سنج پشتیبانی میکند";
                LoadInfoDevice();
            }
            else
            {
                TVMain.Text = "دستگاه شمااز سنسور مغاطیس سنج پشتیبانی نمیکند";
            }

            dfpowermanager = (PowerManager)GetSystemService(Context.PowerService);
            LoadDataSetting();

        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            IbClick();
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        private void IbClick()
        {
            AlertDialog.Builder MsBox = new AlertDialog.Builder(this);
            MsBox.SetTitle("انتخاب کنید"); MsBox.SetCancelable(false);
            View VAlert = View.Inflate(this, Resource.Layout.LayoutLSTTH, null);
            string[] ItemLV = new string[] { "اطلاعات درباره دستگاه", "لیست سنسورها دستگاه", "مشخصات سنسور مغناطیس سنج" };
            MsBox.SetItems(ItemLV, this);
            MsBox.SetNegativeButton("لغو", delegate { return; });
            MsBox.Create(); MsBox.Show();
        }

        public void LoadInfoDevice()
        {
            TVMain.Gravity = GravityFlags.Left;
            string MsBoxText = "";

            MsBoxText = "CpuAbi : " + Android.OS.Build.CpuAbi + "\nCpuAbi2 : " + Android.OS.Build.CpuAbi2 + 
                "\nName Device : "+ Android.OS.Build.Device + "\nDisplay : " + Android.OS.Build.Display + 
                "\nHardware : " + Android.OS.Build.Hardware + "\nManufacturer : " + Android.OS.Build.Manufacturer + 
                "\nModel : " + Android.OS.Build.Model + "\nSerial : " + Android.OS.Build.Serial + "\nSdk : " +
              Android.OS.Build.VERSION.Sdk + "\nRelease : " + Android.OS.Build.VERSION.Release +
              "\nSdkInt : " + Android.OS.Build.VERSION.SdkInt;

            TVMain.Text = "Info Device :" + "\n\n" + MsBoxText;
        }

        public void ListSensor()
        {
            TVMain.Gravity = GravityFlags.Left;
            string pls = ""; int count = 1;
            List<Sensor> ls = new List<Sensor>(dfsm.GetSensorList(SensorType.All));
            foreach (var item in ls)
            {
                pls += count + ") " + item.Name + "\n";
                count++;
            }
            TVMain.Text = "List sensor :" + "\n\n" + pls;
        }

        public void InfoSensorMa()
        {
            string Px = "مشخصات سنسور : " + System.Environment.NewLine + System.Environment.NewLine;
            TVMain.Gravity = GravityFlags.Right;

            Px += "اسم : " + dfsensor.Name + System.Environment.NewLine +
                "حداکثر تاخیر : " + dfsensor.MaxDelay + System.Environment.NewLine + 
                "حداقل تاخیر : " + dfsensor.MinDelay + System.Environment.NewLine +
                "قدرت : " + dfsensor.Power + System.Environment.NewLine + 
                "وضوح : " + dfsensor.Resolution + System.Environment.NewLine +
                "برد حداکثر : " + dfsensor.MaximumRange + System.Environment.NewLine + "نسخه : " + dfsensor.Version;

            TVMain.Text = Px;
        }

        protected override void OnStart()
        {
            base.OnStart();
            LoadDataSetting();
        }

        public void LoadDataSetting()
        {
            var PManager = PreferenceManager.GetDefaultSharedPreferences(this);

            Color ColorActionBar = Color.ParseColor("#ff4500");
            string Theme = PManager.GetString("Theme", "نارنجی-قرمز");
            switch (Theme)
            {
                case "سبز کبریتی":
                    ColorActionBar = Color.ParseColor("#008b8b");
                    break;
                case "نارنجی-قرمز":
                    ColorActionBar = Color.ParseColor("#ff4500");
                    break;
                case "خردلی":
                    ColorActionBar = Color.ParseColor("#daa520");
                    break;
                case "عسلی پررنگ":
                    ColorActionBar = Color.ParseColor("#d2691e");
                    break;
                default:
                    break;
            }
            ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(ColorActionBar));

            try
            {
                bool OnOffDisplay = PManager.GetBoolean("PowerManager", false);
                if (OnOffDisplay == true)
                {
                    WLock = dfpowermanager.NewWakeLock(WakeLockFlags.Full, "DoNotSleep");
                    WLock.Acquire();
                }
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Error Power manager", ToastLength.Short).Show();
            }

        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            switch (which)
            {
                case 0:
                    LoadInfoDevice();
                    break;
                case 1:
                    ListSensor();
                    break;
                case 2:
                    InfoSensorMa();
                    break;
                default:
                    break;
            }
        }
    }
}