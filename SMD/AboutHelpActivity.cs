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
using Android.Graphics;
using Android.Preferences;
using UK.CO.Chrisjenx.Calligraphy;

namespace SMD
{
    [Activity(Label = "Help & About", Theme = "@android:style/Theme.Material.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class AboutHelpActivity : Activity
    {
        PowerManager dfpowermanager; PowerManager.WakeLock WLock;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AH);
            //Window.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Color.ParseColor("#f5f5dc")));
            dfpowermanager = (PowerManager)GetSystemService(Context.PowerService);
            LoadDataSetting();

            TextView TxNotesEdu = (TextView)FindViewById(Resource.Id.AHTextViewNotes);
            StringBuilder strb = new StringBuilder();

            strb.AppendLine("برنامه فلزیاب پیشرفته وظیفه پیدا کردن و کشف کردن انواع فلزات را برعهده دارد.");
            strb.AppendLine("البته این بستگی به سنسور تلفن همراه شما دارد که قدرت سنسور به کار رفته و نوع آن بستگی دارد.");
            strb.AppendLine("");
            strb.AppendLine("توضیح قسمت های مختلف برنامه :");
            strb.AppendLine("");
            strb.AppendLine("شما میتوانید از قسمت سنسور برای فهمیدن قدرت و برد و مشخصات سنسور تلفن همراه خوداستفاده کنید.");
            strb.AppendLine("و همین طور لیست تمامی سنسور های موجود تلفن همراه و مشخصات سیستمی تلفن همراه را متوجه شوید.");
            strb.AppendLine("");
            strb.AppendLine("قسمت تایمر هم برای زمان دار کردن استفاده از برنامه است.");
            strb.AppendLine("نمایشگر سنسور هم اطلاعاتی از مشخصات و تنظیمات آن را نشان می دهد.");
            strb.AppendLine("نمایشگرعمومی هم اطلاعات برخی از قسمت های برنامه را نمایان میکند.");
            strb.AppendLine("");
            strb.AppendLine("آموزش نحوه استفاده از برنامه :");
            strb.AppendLine("");
            strb.AppendLine("برای استفاده از برنامه باید مراحل زیر را انجام دهید.");
            strb.AppendLine("1)برنامه رافعال کنید (سنسور را از منو روشن کنید)");
            strb.AppendLine("2)ابتدا باید سنسور را کالیبره کنید که در منو برنامه توضیح داده شده است. ");
            strb.AppendLine("3)نوع اسکن محل را انتخاب کنید. ");
            strb.AppendLine("4)کاوش کردن محل فلزات ");
            strb.AppendLine("و با استفاده از نمایشگرها وضعیت برنامه را کنترل کنید.");
            strb.AppendLine("");
            strb.AppendLine("توضیحات در مورد اسکنر محل :");
            strb.AppendLine("برنامه دارای سه نوع اسکن هست : اتوماتیک و دستی و سفارشی");
            strb.AppendLine("شما میتوانید براساس نیازتون یکی را انتخاب میکنید.");
            strb.AppendLine("توضیحات اسکنر اتوماتیک : ");
            strb.AppendLine("این اسکن براساس تنطیمات برنامه که انجام شده عمل میکند و نمونه کامل برای پیدا کردن اجسام هست . از قسمت تنطیمات باید اعمال تغییرات کردو بیشتر برای  جست وجوی دقیق و برد کوتاه استفاده میشود.");
            strb.AppendLine("توضیحات اسکنر دستی : ");
            strb.AppendLine("این اسکنر براساس حساسیت به فلزات و تغییر قدرت سنسور برای کاوش با دقت متوسط و برد میانگین استفاده میشود.وهمین طور برای کاربران حرفه ای این قسمت تعبیه شده است که این قسمت نیازمند اپراتور حرفه ای است .");
            strb.AppendLine("توضیحات اسکنر سفارشی : ");
            strb.AppendLine("این اسکن محل برای جست وجوی  براساس دقت و برد می باشد که از نهایت قدرت سنسور استفاده میکند. و میتوانید بیشتر عمق یابی کنید ولی دقت پایین و عمق بالا داراست.");
            strb.AppendLine("");
            strb.AppendLine("البته نوع اسکن خوب و قابل اطمینان اسکن اتوماتیک است که از دقت و قدرت متوسط برخودار است پیشنهاد ما این اسکنر میباشد.");
            strb.AppendLine("");
            strb.AppendLine("نکات : احتمال دارد برخی از فلزات را برنامه پیدا نکند که به نوع سنسور شما بستگی دارد.");
            strb.AppendLine("نکات : برای بهتر عمل کردن برنامه بهتر است تمرین زیاد داشته باشد چون اپراتور خوب میتوانید نتیجه بهتر داشته باشد.");
            strb.AppendLine(" نکات : عمق برنامه بستگی به خیلی چیزها دارد مثل قدرت و سرعت و دقت و حساسیت و محل کاوش و خیلی چیز ها دیگه دارد ولی عمق نهایت این برنامه با آیده ال بودن موارد بالا کم از 1 متر است .");
            strb.AppendLine("نکات : میدان مغناطیسی آهنربا و آهن بسیار بالاست . ");
            strb.AppendLine("");
            strb.AppendLine("در صورت نیاز مشاوره بیشتر و انتقاد و پیشنهاد به ما ایمیل بزنید");
            strb.AppendLine("Email : DeveloperAndroid4444@gmail.com ");
            strb.AppendLine("");
            strb.AppendLine("اطلاعات درباره برنامه : ");
            strb.AppendLine("Visual Studio 2015 - Xamarin 4.1 - Andorid with C#");


            TxNotesEdu.Text = strb.ToString();

        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
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