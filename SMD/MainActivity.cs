using System;
using System.Collections.Generic;
using System.Timers;
using System.Text;

using Android.Hardware;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Preferences;
using UK.CO.Chrisjenx.Calligraphy;

namespace SMD
{
    [Activity(Label = "Smart Metal Detectors", Theme = "@android:style/Theme.Material.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener, View.IOnClickListener
    {
        Sensor dfsensor; SensorManager dfsensorManager; Vibrator vibrator; PowerManager dfpowermanager;
        PowerManager.WakeLock WLock; Timer TimerSensor; int ValueTS;
        TypeScanMD typeScan = TypeScanMD.Auto;bool SensorOnOff = false;
        //Sensor Setting
        SensorStatus AutoHanSpeedSensor = SensorStatus.AccuracyMedium; double AutoPowerSensor = 1.35; int AutoSenSensor = 50;
        bool AutoCVib = true; bool AutoCHieght = false; bool AutoSRotbat = false;
        ETypeKhak AutoTypeKhak = ETypeKhak.Medium; ETepWork AutoTypeWork = ETepWork.Medium;
        // Sensor handle 
        int HanSen = 25, HanPower = 25;
        // Sensor Custom
        int CusD = 0; int CusB = 0;
        // Ui Widget main
        Button BtnVPG, BtnVPS; TextView LabelPanelT1, LabelPanelT2; ProgressBar ProgressMain; Spinner SpinnerTypeScan; Switch SwitchOnOff;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            //Window.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.ParseColor("#f5f5dc")));

            dfpowermanager = (PowerManager)GetSystemService(Context.PowerService);
            vibrator = (Vibrator)GetSystemService(Context.VibratorService);
            dfsensorManager = (SensorManager)GetSystemService(Context.SensorService);
            // Other code sensor Service ===>
            // dfsensorManager = GetSystemService(Context.SensorService) as SensorManager;
            // FindbyidView :
            BtnVPG = (Button)FindViewById(Resource.Id.MainBtnTpG);
            BtnVPS = (Button)FindViewById(Resource.Id.MainBtnTpS);
            LoadDataSetting();

            LabelPanelT1 = (TextView)FindViewById(Resource.Id.MainTextViewT1);
            LabelPanelT2 = (TextView)FindViewById(Resource.Id.MainTextViewT2);
            BtnVPG.Click += BtnVPG_Click;
            BtnVPS.Click += BtnVPS_Click;

            ProgressMain = (ProgressBar)FindViewById(Resource.Id.MainProgressBarVS); ProgressMain.Progress = 0;
            ProgressMain.ProgressDrawable.SetColorFilter(Color.Rgb(112,128,144), PorterDuff.Mode.Overlay);
                
            ActionBar.SetDisplayShowCustomEnabled(true);
            View viewAction = View.Inflate(this, Resource.Layout.LayoutActionBarCus, null);
            ActionBar.SetCustomView(viewAction, new ActionBar.LayoutParams(GravityFlags.Right));
            SwitchOnOff = viewAction.FindViewById<Switch>(Resource.Id.LActionBarSwitchOnOff);
            SwitchOnOff.CheckedChange += SwitchOnOff_CheckedChange;
            SpinnerTypeScan = viewAction.FindViewById<Spinner>(Resource.Id.LActionBarSpinnerTypeScan);
            string[] itemTS = new string[] { "اسکن : اتوماتیک", "اسکن : دستی", "اسکن : سفارشی" };
            ArrayAdapter<string> AdapterSp = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, itemTS);
            SpinnerTypeScan.Adapter = AdapterSp;
            SpinnerTypeScan.ItemSelected += SpinnerTypeScan_ItemSelected;

            TimerSensor = new Timer(100);
            TimerSensor.Elapsed += OnTimedEvent;

            List<Sensor> CheckSensor = new List<Sensor>(dfsensorManager.GetSensorList(SensorType.MagneticField));
            if (CheckSensor.Count <= 0)
            {
                AlertDialog.Builder Message = new AlertDialog.Builder(this);
                Message.SetTitle("خطا سیستمی");
                Message.SetMessage("دستگاه شما از سنسور مغاطیس سنج پشتیبانی نمیکند");
                Message.SetCancelable(false);
                Message.SetPositiveButton("ادامه میدم", delegate { });
                Message.SetNegativeButton("خروج", delegate { Finish(); });
                Message.Create(); Message.Show();
            }
            else if (CheckSensor.Count > 0)
            {
                dfsensor = dfsensorManager.GetDefaultSensor(SensorType.MagneticField);
            }

            //LoadPriceToApp();

        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        public void LoadPriceToApp()
        {
            //"ir.mservices.market"
            //"com.farsitel.bazaar"
            if (CheckPriceToApp("Com.Mkh_AppSMD", "com.farsitel.bazaar") == false)
            {
                AlertDialog.Builder MsBox = new AlertDialog.Builder(this);
                MsBox.SetTitle("اخطار عدم پرداخت");
                StringBuilder strb = new StringBuilder();
                strb.AppendLine("' " + "برنامه فلزیاب پیشرفته" + " '");
                strb.AppendLine();
                strb.AppendLine("کاربر گرامی شمااول باید برنامه را از مارکت های موبایل بازار یا مایکت دانلود کنید سپس از برنامه استفاده کنید.");
                strb.AppendLine();
                strb.AppendLine("بدلیل عدم پرداخت وجه برنامه نمی توانید از برنامه استفاده کنید.");
                MsBox.SetMessage(strb.ToString());
                MsBox.SetNegativeButton("خروج از برنامه", delegate { Finish(); });
                MsBox.SetPositiveButton("کد قفل گشا", delegate { KeyCodeAppOpen(); });
                MsBox.SetCancelable(false);
                MsBox.Create(); MsBox.Show();
            }
        }

        public void KeyCodeAppOpen()
        {
            AlertDialog.Builder MsBox = new AlertDialog.Builder(this);
            MsBox.SetTitle("کد امنیتی را وارد کنید");
            View ViewAlert = View.Inflate(this, Resource.Layout.LayoutCPA, null);
            MsBox.SetView(ViewAlert);
            EditText TeText = (EditText)ViewAlert.FindViewById(Resource.Id.LayoutCPAEditTextCodeNumber);

            MsBox.SetPositiveButton("ورود برنامه", delegate 
            {
                if (TeText.Text != "489751022")
                {
                    Finish();
                    Toast.MakeText(this, "کد قفل گشا اشتباه بود", ToastLength.Short).Show();
                }
                
            });
            MsBox.SetNegativeButton("خروج", delegate { Finish(); });
            MsBox.SetCancelable(false);
            MsBox.Create(); MsBox.Show();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (ValueTS > 0)
            {
                ValueTS--;
            }
            else if (ValueTS == 0)
            {
                ValueTS = 0; TimerSensor.Stop();
                vibrator.Vibrate(300);

                Finish();
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.NewTask);
                StartActivity(intent);

                //dfsensorManager.UnregisterListener(this, dfsensor);
                //ProgressMain.Progress = 0;
                //LabelPanelT1.Text = "";
            }
        }

        private void SpinnerTypeScan_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    LoadTypeScanAuto();
                    break;
                case 1:
                    LoadtypeScanHandle();
                    break;
                case 2:
                    LoadTypeScanCustom();
                    break;
                default:
                    break;
            }
        }

        private void SwitchOnOff_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            switch (e.IsChecked)
            {
                case true:
                    dfsensorManager.RegisterListener(this, dfsensor, SensorDelay.Ui);
                    vibrator.Vibrate(80);
                    SensorOnOff = true;
                    BtnVPS_Click(sender, e);
                    break;
                case false:
                    dfsensorManager.UnregisterListener(this, dfsensor);
                    LabelPanelT1.Text = "";
                    ProgressMain.Progress = 0;
                    vibrator.Vibrate(80);
                    LabelPanelT2.Text = "";
                    SensorOnOff = false;
                    break;
                default:
                    break;
            }
        }

        private void BtnVPS_Click(object sender, EventArgs e)
        {
            LabelPanelT2.Gravity = GravityFlags.Top | GravityFlags.Right;
            if (SensorOnOff == true)
            {
                vibrator.Vibrate(30);
                var PManager = PreferenceManager.GetDefaultSharedPreferences(this);

                string TKh = PManager.GetString("TypeKhak", "معمولی");
                string SmfSpeed = PManager.GetString("SpeedSensor", "متوسط");
                string SmfPower = PManager.GetString("PowerSensor", "متوسط");
                string SmfSens = PManager.GetString("SenSensor", "متوسط");
                string TepWH = PManager.GetString("TepWork", "متعادل");

                string Root = "";
                if (AutoSRotbat)
                {
                    Root = "هست";
                }
                else
                {
                    Root = "نیست";
                }

                StringBuilder AddingText = new StringBuilder();
                AddingText.AppendLine("اطلاعات مربوط به سنسور مغناطیس سنج");
                AddingText.AppendLine("1) سرعت سنسور : " + SmfSpeed);
                AddingText.AppendLine("2) قدرت سنسور : " + SmfPower);
                AddingText.AppendLine("3) حساسیت نسبت به فلزات : " + SmfSens);
                AddingText.AppendLine();
                AddingText.AppendLine("اطلاعات مربوط به محل کاوش");
                AddingText.AppendLine("1) نوع خاک : " + TKh);
                AddingText.AppendLine("2) دما : " + TepWH);
                AddingText.AppendLine("3) روطوبت : " + Root);

                LabelPanelT2.Text = AddingText.ToString();

            }
            else
            {
               
            }
        }

        private void BtnVPG_Click(object sender, EventArgs e)
        {
            var PManager = PreferenceManager.GetDefaultSharedPreferences(this);
            LabelPanelT2.Gravity = GravityFlags.Top | GravityFlags.Right;
            if (SensorOnOff == true)
            {
                vibrator.Vibrate(30);
                StringBuilder strb = new StringBuilder();

                if (ValueTS > 0)
                {
                    strb.AppendLine("تایمر سنسور فعال است");
                }
                else
                {
                    strb.AppendLine("تایمر سنسور غیرفعال است");
                }
                bool DisplayPWl = PManager.GetBoolean("PowerManager", false);

                if (DisplayPWl)
                {
                    strb.AppendLine("ممانعت از به خواب رفتن دستگاه فعال است");
                }
                else
                {
                    strb.AppendLine("ممانعت از به خواب رفتن دستگاه غیرفعال است");
                }

                strb.AppendLine("نوع فلرات جست و جو : آهنی و غیرآهنی");
                List<Sensor> ls = new List<Sensor>(dfsensorManager.GetSensorList(SensorType.MagneticField));
                if (ls.Count > 0)
                {
                    strb.AppendLine("نوع سنسور در حال استفاده : مغناطیس سنج");
                }

                LabelPanelT2.Text = strb.ToString();
            }
            else
            {
            }
        }

        public void LoadClibratorSensor()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine("آموزش نحوه کالیبره کردن سنسور مغناطیس سنج : ");
            strBuilder.AppendLine();
            strBuilder.AppendLine("برای افزایش دقت سنسور بهتر است سنسور مغناطیس سنج دستگاه خویش را کالیبره کنید .");
            strBuilder.AppendLine();
            strBuilder.AppendLine("راه های مختلفی برای کالیبره کردن سنسور وجود دارد ولی این روش راحت تر و قابل فهم تر است .");
            strBuilder.AppendLine();
            strBuilder.AppendLine("تلفن خود را رو به آسمان گرفته ( صفحه نمایش رو به آسمان باشد ) یعنی بصورت افقی باشد وبا استفاده از الگو به شکل بالا ( شکل بینهایت ) را روی هوا ترسیم کنید این کار را میتوانید یک بار یا بیش هم در جهات مختلف هم انجام دهید تا سنسور مغناطیس کالیبره شود و عملکرد ودقت بهتری داشته باشد . ");

            AlertDialog.Builder alertCS = new AlertDialog.Builder(this);
            alertCS.SetTitle("کالیبره کردن سنسور ");
            alertCS.SetIcon(Resource.Drawable.infy);
            alertCS.SetMessage(strBuilder.ToString());
            alertCS.SetNegativeButton("خروج", delegate { return; });
            alertCS.SetCancelable(false);
            alertCS.Create(); alertCS.Show();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add("کالیبره").SetIcon(Resource.Drawable.Clibrator).SetShowAsAction(ShowAsAction.Always);
            MenuInflater.Inflate(Resource.Menu.MenuMain, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            if (item.TitleFormatted.ToString() == "کالیبره")
            {
                LoadClibratorSensor();
            }

            switch (item.ItemId)
            {
                case Resource.Id.ItemSetting:
                    Intent SASetting = new Intent(this, typeof(SettingActivity));
                    SASetting.SetFlags(ActivityFlags.NewTask);
                    StartActivity(SASetting);

                    break;
                case Resource.Id.ItemSensor:
                    Intent SASensor = new Intent(this, typeof(SensorLTActivity));
                    SASensor.SetFlags(ActivityFlags.NewTask);
                    StartActivity(SASensor);

                    break;
                case Resource.Id.ItemTimer:
                    LoadTimerSensor();
                    break;
                case Resource.Id.ItemAboutHelp:
                    Intent SAAH = new Intent(this, typeof(AboutHelpActivity));
                    SAAH.SetFlags(ActivityFlags.NewTask);
                    StartActivity(SAAH);

                    break;
                default:
                    break;
            }
            return base.OnMenuItemSelected(featureId, item);
        }

        public void LoadTimerSensor()
        {
            View viewAlert = View.Inflate(this, Resource.Layout.LayoutTimerSensor, null);
            AlertDialog.Builder Message = new AlertDialog.Builder(this);
            Message.SetTitle("زمان را برحسب دقیقه انتخاب کنید"); Message.SetView(viewAlert);
            Message.SetIcon(Resource.Drawable.Timer);
            NumberPicker np = (NumberPicker)viewAlert.FindViewById(Resource.Id.LTimerSensorNumberPickerTS);
            np.MaxValue = 25; np.MinValue = 1;
            np.SetOnClickListener(this);
            Message.SetPositiveButton("فعال کن", delegate
            {
                ValueTS = (np.Value * 60) * 10;
                TimerSensor.Start();
                Toast.MakeText(this, np.Value + " M " + "تایمر سنسور فعال شد", ToastLength.Short).Show();
                WLock = dfpowermanager.NewWakeLock(WakeLockFlags.Full, "DoNotSleep");
                WLock.Acquire();
            });
            Message.SetNegativeButton("لغو", delegate { });
            Message.SetNeutralButton("لغو تایمر", delegate
            {
                TimerSensor.Stop(); ValueTS = 0;
                Toast.MakeText(this, "تایمر سنسور غیر فعال شد", ToastLength.Short).Show();
                try
                {
                    WLock.Release();
                }
                catch { }

            });
            Message.Create(); Message.Show();

        }

        public void LoadTypeScanCustom()
        {
            AlertDialog.Builder MsBox = new AlertDialog.Builder(this);
            MsBox.SetTitle("اسکن سفارشی");
            View ViewMs = View.Inflate(this, Resource.Layout.LayoutCustomScan, null);
            MsBox.SetView(ViewMs);
            Spinner SpB = (Spinner)ViewMs.FindViewById(Resource.Id.LCusScanSpinnerBord);
            Spinner SpD = (Spinner)ViewMs.FindViewById(Resource.Id.LCusScanSpinnerDegat);
            string[] itemSB = new string[] { "حداقل", "متوسط", "حداکثر", "حداکثر ++" };
            string[] itemSD = new string[] { "کم", "متوسط", "زیاد" };
            ArrayAdapter<string> IB = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, itemSB);
            ArrayAdapter<string> ID = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, itemSD);
            SpB.Adapter = IB; SpD.Adapter = ID;

            SpB.ItemSelected += delegate
            {
                CusB = SpB.SelectedItemPosition;
            };
            SpD.ItemSelected += delegate
            {
                CusD = SpD.SelectedItemPosition;
            };

            MsBox.SetCancelable(false);
            MsBox.SetPositiveButton("اجرا", delegate
            {
                typeScan = TypeScanMD.Custom;
                Toast.MakeText(this, "اسکن سفارشی فعال شد", ToastLength.Short).Show();
                ProgressMain.Progress = 0;
            });
            MsBox.SetNegativeButton("لغو", delegate { SpinnerTypeScan.SetSelection(0); });
            MsBox.Create(); MsBox.Show();
        }

        public void LoadTypeScanAuto()
        {
            Toast.MakeText(this, "اسکن اتوماتیک فعال شد", ToastLength.Short).Show();
            typeScan = TypeScanMD.Auto;

        }

        public void LoadtypeScanHandle()
        {
            AlertDialog.Builder MsBox = new AlertDialog.Builder(this);
            MsBox.SetTitle("اسکن دستی");
            View ViewMs = View.Inflate(this, Resource.Layout.LayoutHandleScan, null);
            MsBox.SetView(ViewMs);
            //start view 
            TextView TextNat = (TextView)ViewMs.FindViewById(Resource.Id.LHandleScanTextViewMN);
            SeekBar SeekBarSen = (SeekBar)ViewMs.FindViewById(Resource.Id.LHandleScanSeekBarSen);
            SeekBar SeekBarPower = (SeekBar)ViewMs.FindViewById(Resource.Id.LHandleScanSeekBarPower);
            Spinner SpSpeedsensor = (Spinner)ViewMs.FindViewById(Resource.Id.LHandleScanSpinnerSpeed);

            string[] HanItemSpSpeed = new string[] { "کم", "متوسط", "زیاد" };
            ArrayAdapter<string> SIAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, HanItemSpSpeed);
            SpSpeedsensor.Adapter = SIAdapter;
            SpSpeedsensor.ItemSelected += delegate
              {
                  switch (SpSpeedsensor.SelectedItemPosition)
                  {
                      case 0:
                          AutoHanSpeedSensor = SensorStatus.AccuracyLow;
                          break;
                      case 1:
                          AutoHanSpeedSensor = SensorStatus.AccuracyMedium;
                          break;
                      case 2:
                          AutoHanSpeedSensor = SensorStatus.AccuracyHigh;
                          break;
                      default:
                          break;
                  }
              };

            SeekBarSen.ProgressChanged += delegate
            {
                TextNat.Text = "حساسیت فلزات : " + SeekBarSen.Progress + "  *  " + "قدرت سنسور : " + SeekBarPower.Progress;
                HanSen = SeekBarSen.Progress;
            };

            SeekBarPower.ProgressChanged += delegate
            {
                TextNat.Text = "حساسیت فلزات : " + SeekBarSen.Progress + "  *  " + "قدرت سنسور : " + SeekBarPower.Progress;
                HanPower = SeekBarPower.Progress;
            };

            //end view
            MsBox.SetCancelable(false);
            MsBox.SetPositiveButton("اجرا", delegate
            {
                typeScan = TypeScanMD.Handle;
                Toast.MakeText(this, "اسکن دستی فعال شد", ToastLength.Short).Show();
            });
            MsBox.SetNegativeButton("لغو", delegate { SpinnerTypeScan.SetSelection(0); });
            MsBox.Create(); MsBox.Show();
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("خروج");
                alert.SetMessage("آیا از برنامه خارج میشوید ؟");
                alert.SetPositiveButton("خروج", delegate
                {

                    Finish();

                });
                alert.SetNegativeButton("لغو", delegate { return; });
                alert.SetCancelable(false);
                alert.Create(); alert.Show();
            }
            return base.OnKeyDown(keyCode, e);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            dfsensorManager.UnregisterListener(this, dfsensor);
            TimerSensor.Stop(); ValueTS = 0;
            try
            {
                WLock.Release();
            }
            catch { }

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
            BtnVPG.Background.SetColorFilter(Color.ParseColor("#808080"), PorterDuff.Mode.Multiply);
            BtnVPS.Background.SetColorFilter(Color.ParseColor("#808080"), PorterDuff.Mode.Multiply);
            BtnVPG.SetTextColor(Color.White);BtnVPS.SetTextColor(Color.White);

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

            AutoCVib = PManager.GetBoolean("Vibrator", false);
            AutoCHieght = PManager.GetBoolean("HeightSeng", false);

            string SmfSpeed = PManager.GetString("SpeedSensor", "متوسط");

            switch (SmfSpeed)
            {
                case "کم":
                    AutoHanSpeedSensor = SensorStatus.AccuracyLow;
                    break;
                case "متوسط":
                    AutoHanSpeedSensor = SensorStatus.AccuracyMedium;
                    break;
                case "زیاد":
                    AutoHanSpeedSensor = SensorStatus.AccuracyHigh;
                    break;
                default:
                    break;
            }

            string SmfPower = PManager.GetString("PowerSensor", "متوسط");

            switch (SmfPower)
            {
                case "کم":
                    AutoPowerSensor = 1;
                    break;
                case "متوسط":
                    AutoPowerSensor = 1.1;
                    break;
                case "زیاد":
                    AutoPowerSensor = 1.25;
                    break;
                default:
                    break;
            }

            string SmfSens = PManager.GetString("SenSensor", "متوسط");
            switch (SmfSens)
            {
                case "کم":
                    AutoSenSensor = 80;
                    break;
                case "متوسط":
                    AutoSenSensor = 70;
                    break;
                case "زیاد":
                    AutoSenSensor = 60;
                    break;
                case "خیلی کم":
                    AutoSenSensor = 90;
                    break;
                case "خیلی زیاد":
                    AutoSenSensor = 50;
                    break;
                default:
                    break;
            }

            AutoSRotbat = PManager.GetBoolean("Rotobat", false);

            string TepWH = PManager.GetString("TepWork", "متعادل");

            switch (TepWH)
            {
                case "گرم":
                    AutoTypeWork = ETepWork.Low;
                    break;
                case "متعادل":
                    AutoTypeWork = ETepWork.Medium;
                    break;
                case "سرد":
                    AutoTypeWork = ETepWork.Top;
                    break;
                default:
                    break;
            }

            string TKh = PManager.GetString("TypeKhak", "معمولی");
            switch (TKh)
            {
                case "نرم":
                    AutoTypeKhak = ETypeKhak.Low;
                    break;
                case "معمولی":
                    AutoTypeKhak = ETypeKhak.Medium;
                    break;
                case "سفت":
                    AutoTypeKhak = ETypeKhak.Top;
                    break;
                default:
                    break;
            }

        }

        public void OnValueChange(NumberPicker picker, int oldVal, int newVal)
        {

        }

        public void OnClick(View v)
        {

        }

        public bool CheckPriceToApp(string MyPackage,string MarketPackage)
        {
            string installer = PackageManager.GetInstallerPackageName(MyPackage);
            return installer != null && installer.Equals(MarketPackage);
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            if (sensor.Type == SensorType.MagneticField)
            {
                switch (typeScan)
                {
                    case TypeScanMD.Handle:
                        accuracy = AutoHanSpeedSensor;
                        break;
                    case TypeScanMD.Auto:
                        accuracy = AutoHanSpeedSensor;
                        break;
                    case TypeScanMD.Custom:
                        accuracy = SensorStatus.AccuracyMedium;
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.MagneticField)
            {
                float x = e.Values[0]; float y = e.Values[1]; float z = e.Values[2];
                double vx = Math.Pow(Convert.ToDouble(x), 2);
                double vy = Math.Pow(Convert.ToDouble(y), 2);
                double vz = Math.Pow(Convert.ToDouble(z), 2);
                double xyz = Math.Sqrt((vx + vy + vz));
            
                switch (typeScan)
                {
                    case TypeScanMD.Handle:
                        MFHandleSensor(xyz, HanPower, HanSen);
                        break;
                    case TypeScanMD.Auto:
                        MFAutoSensorOC(xyz);
                        break;
                    case TypeScanMD.Custom:
                        MFCustomSensor(xyz, CusB, CusD);
                        break;
                    default:
                        break;
                }

            }

        }

        public void MFCustomSensor(double xyz, int itemBord, int itemDegat)
        {
            StringBuilder strbT1 = new StringBuilder();
            double SeedValue = (itemBord + 1) + Math.Log10(Convert.ToDouble(dfsensor.MaximumRange));
            double MRnat = xyz + (SeedValue + (dfsensor.Power));
            double seni = ((itemDegat + 1) * 10) + 50;

            int PorgBar = Convert.ToInt16((MRnat * 100) / seni);

            if (MRnat >= seni)
            {
                ProgressMain.Progress = ProgressMain.Max;
                LabelPanelT1.SetTextColor(Color.Red);

                strbT1.AppendLine("هدف یافت شد !");
                strbT1.AppendLine("میدان مغناطیسی : " + Convert.ToSingle(MRnat));
                LabelPanelT1.Text = strbT1.ToString();
            }
            else
            {
                LabelPanelT1.SetTextColor(Color.Black);
                ProgressMain.Progress = PorgBar;
               
                if (MRnat > (seni * 70) / 100 && MRnat < seni)
                {
                    LabelPanelT1.SetTextColor(Color.Orange);
                }
                strbT1.AppendLine("درحال جست و جو ...");
                strbT1.AppendLine("میدان مغناطیسی : " + Convert.ToSingle(MRnat));
                LabelPanelT1.Text = strbT1.ToString();
            }

        }

        public void MFHandleSensor(double xyz, int power, int sen)
        {
            int Sensivi = 120 - (sen * 3);
            StringBuilder strbT1 = new StringBuilder();
            double NatAutoxyz = xyz + (power);
      
            int PorgBar = Convert.ToInt16((NatAutoxyz * 100) / Sensivi);
            if (NatAutoxyz >= Sensivi)
            {
                ProgressMain.Progress = ProgressMain.Max;
                LabelPanelT1.SetTextColor(Color.Red);
              
                strbT1.AppendLine("هدف یافت شد !");
                strbT1.AppendLine("میدان مغناطیسی : " + Convert.ToSingle(NatAutoxyz));
                LabelPanelT1.Text = strbT1.ToString();
            }
            else
            {
                LabelPanelT1.SetTextColor(Color.Black);
                ProgressMain.Progress = PorgBar;
                int SeedValue = (Sensivi * 70) / 100;
                if (NatAutoxyz > SeedValue && NatAutoxyz < Sensivi)
                {
                    LabelPanelT1.SetTextColor(Color.Orange);
                }
                strbT1.AppendLine("درحال جست و جو ...");
                strbT1.AppendLine("میدان مغناطیسی : " + Convert.ToSingle(NatAutoxyz));
                LabelPanelT1.Text = strbT1.ToString();
            }
        }

        public void MFAutoSensorOC(double xyz)
        {
            StringBuilder strbT1 = new StringBuilder();
            double NatAutoxyz = xyz * AutoPowerSensor;

            if (AutoSRotbat)
            {
                NatAutoxyz += 5;
            }

            switch (AutoTypeWork)
            {
                case ETepWork.Low:
                    NatAutoxyz += 0;
                    break;
                case ETepWork.Medium:
                    NatAutoxyz += 0.5;
                    break;
                case ETepWork.Top:
                    NatAutoxyz += 1;
                    break;
                default:
                    break;
            }

            switch (AutoTypeKhak)
            {
                case ETypeKhak.Low:
                    NatAutoxyz += 0;
                    break;
                case ETypeKhak.Medium:
                    NatAutoxyz += 0.5;
                    break;
                case ETypeKhak.Top:
                    NatAutoxyz += 1;
                    break;
                default:
                    break;
            }


            int PorgBar = Convert.ToInt16((NatAutoxyz * 100) / AutoSenSensor);
            if (NatAutoxyz >= AutoSenSensor)
            {
                ProgressMain.Progress = ProgressMain.Max;
                LabelPanelT1.SetTextColor(Color.Red);
                if (AutoCVib)
                {
                    vibrator.Vibrate(200);
                }

                strbT1.AppendLine("هدف یافت شد !");
                strbT1.AppendLine("میدان مغناطیسی : " + Convert.ToSingle(NatAutoxyz));
                if (AutoCHieght)
                {
                    if (NatAutoxyz < 60)
                    {
                        strbT1.AppendLine("فاصله تقریبی : زیاد ");
                    }
                    else if (NatAutoxyz < 70 && NatAutoxyz >= 60)
                    {
                        strbT1.AppendLine("فاصله تقریبی : نزدیک ");
                    }
                    else if (NatAutoxyz > 80 && NatAutoxyz >= 70)
                    {
                        strbT1.AppendLine("فاصله تقریبی : خیلی نزدیک ");
                    }
                }

                LabelPanelT1.Text = strbT1.ToString();
            }
            else
            {
                LabelPanelT1.SetTextColor(Color.Black);
                ProgressMain.Progress = PorgBar;
                int SeedValue = (AutoSenSensor * 70) / 100;
                if (NatAutoxyz > SeedValue && NatAutoxyz < AutoSenSensor)
                {
                    LabelPanelT1.SetTextColor(Color.Orange);
                }
                strbT1.AppendLine("درحال جست و جو ...");
                strbT1.AppendLine("میدان مغناطیسی : " + Convert.ToSingle(NatAutoxyz));
                LabelPanelT1.Text = strbT1.ToString();
            }

        }

        public enum ETypeKhak
        {
            Low, Medium, Top
        }

        public enum ETepWork
        {
            Low, Medium, Top
        }

        public enum TypeScanMD
        {
            Handle, Auto, Custom
        }

    }
}