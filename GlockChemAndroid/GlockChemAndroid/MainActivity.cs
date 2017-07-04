﻿using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;


namespace GlockChemAndroid
{
    [Activity(Label = "GlockChemAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            init();
        }
        protected void init() {
            SetContentView(Resource.Layout.Main);
            Button b = FindViewById<Button>(Resource.Id.BB);
            EditText t = FindViewById<EditText>(Resource.Id.editText1);
            TextView er = FindViewById<TextView>(Resource.Id.er);
            Core.Equation equation = null;
            Core.EquationCalculator calc = null;
            Core.EquationBalancer balance;
            b.Click += delegate {
                String strInput = t.Text;
                if (strInput == "")
                {
                    er.Text="错误：输入为空！";
                }
                try
                {
                    equation = new Core.Equation(strInput);
                }
                catch (Exception e)
                {
                    er.Text = "分析错误：" + e.GetBaseException();
                    return;
                }
                balance = new Core.EquationBalancer(equation);
                if (balance.checkBalance() == false)
                {
                    if (balance.balanceGaussian() != true)
                    {
                        er.Text = "配平失败";
                    }
                    else
                    {
                        cal("已配平");
                    }
                }
                else
                {
                    cal("已配平");
                }
            };
        }
        protected void cal(string s)
        {
            SetContentView(Resource.Layout.colv);
        }
    }
}

