using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Collections;

namespace GlockChemAndroid
{
    [Activity(Label = "GlockChemAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Init();
        }
        protected void Init() {
            SetContentView(Resource.Layout.Main);
            ImageButton ecb = FindViewById<ImageButton>(Resource.Id.iB1);
            ecb.Click += delegate {
                var intent = new Intent(this, typeof(EquationCalc)).SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
            };
            ImageButton mmb = FindViewById<ImageButton>(Resource.Id.iB2);
            mmb.Click += delegate {
                var intent = new Intent(this, typeof(MoleMassCalc)).SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
            };
            ImageButton spb = FindViewById<ImageButton>(Resource.Id.iB3);
            spb.Click += delegate {
                var intent = new Intent(this, typeof(SolutionCalc)).SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
            };
            Button ex = FindViewById<Button>(Resource.Id.ex);
            ex.Click += delegate
            {
                Finish();
                Process.KillProcess(Process.MyPid());
            };
        }

    }
}

