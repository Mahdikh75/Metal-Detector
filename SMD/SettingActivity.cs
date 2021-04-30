using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Android.Graphics;
using Android.Preferences;
using UK.CO.Chrisjenx.Calligraphy;

namespace SMD
{
    [Activity(Label = "Setting app", Theme = "@android:style/Theme.Material.Light",
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingActivity : PreferenceActivity 
    {
        PowerManager dfpowermanager; PowerManager.WakeLock WLock;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.DecorView.LayoutDirection = LayoutDirection.Rtl;
            AddPreferencesFromResource(Resource.Xml.PUi);
            //Window.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.ParseColor("#f5f5dc")));
            ListPreference ListTheme = (ListPreference)FindPreference("Theme");
            ListTheme.PreferenceChange += ListTheme_PreferenceChange;
            dfpowermanager = (PowerManager)GetSystemService(Context.PowerService);
            LoadDataSetting();

        }
        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        private void ListTheme_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            Finish();
            //Intent intent = new Intent(this, typeof(MainActivity));
            //intent.SetFlags(ActivityFlags.ClearTask);
            //StartActivity(intent);
            //dfam.RestartPackage("Com.Mkh_AppSMD");

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
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

    }

}