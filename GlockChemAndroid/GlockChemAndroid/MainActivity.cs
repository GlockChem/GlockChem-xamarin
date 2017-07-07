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
        Core.Equation equation = null;
        Core.EquationCalculator calc = null;
        Core.EquationBalancer balance;
        Core.EquationCalculator.EquationConditionMass condition;
        string pass;

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
            equation = null;
            balance = null;
            b.Click += delegate {
                String strInput = t.Text;
                if (strInput == "")
                {
                    er.Text = "错误：输入为空！";
                }
                try
                {
                    equation = new Core.Equation(strInput);
                }
                catch (Exception e)
                {
                    er.Text = "分析错误：";// + e.ToString();
                    Console.WriteLine("Here's the trace::" + e.StackTrace);
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
            TextView t1 = FindViewById<TextView>(Resource.Id.textView1);
            t1.Text = equation.ToString();
            Button b = FindViewById<Button>(Resource.Id.BB);
            Spinner ch = FindViewById<Spinner>(Resource.Id.spinner1);
            EditText t = FindViewById<EditText>(Resource.Id.editText1);
            TextView er = FindViewById<TextView>(Resource.Id.er);
            var str = new List<string>();
            foreach (Core.Pair<Core.Formula, int?> pair in equation.reactant)
            {
                str.Add(pair.L1.RawString);
            }
            foreach (Core.Pair<Core.Formula, int?> pair in equation.product)
            {
                str.Add(pair.L1.RawString);
            }
            ch.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, str);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            ch.Adapter = adapter;

            b.Click += delegate
            {

                double numCondition = 0;
                try
                {
                    numCondition = Double.Parse(t.Text);
                }
                catch (Exception e)
                {
                    er.Text = "给定质量无效";
                    return;
                }
                condition = new Core.EquationCalculator.EquationConditionMass(equation.reactant[0], new Core.AdvNum(numCondition));
                bool flag = true;
                for (int i = 0; i < equation.reactant.Count; i++)
                {
                    if (pass == equation.reactant[i].L1.RawString)
                    {
                        condition = new Core.EquationCalculator.EquationConditionMass(equation.reactant[i], new Core.AdvNum(numCondition));
                        flag = false;
                    }
                }
                for (int i = 0; i < equation.product.Count; i++)
                {
                    if (pass == equation.product[i].L1.RawString)
                    {
                        condition = new Core.EquationCalculator.EquationConditionMass(equation.product[i], new Core.AdvNum(numCondition));
                        flag = false;
                    }
                }
                if (flag)
                {
                    er.Text = "未知错误1";
                }
                else
                {
                    fin();
                }
            };
        }

        private void fin()
        {
            SetContentView(Resource.Layout.finv);
            TextView tx2 = FindViewById<TextView>(Resource.Id.textView2);
            TextView tx3 = FindViewById<TextView>(Resource.Id.textView3);
            Button b = FindViewById<Button>(Resource.Id.BB);
            Button ex = FindViewById<Button>(Resource.Id.BB2);
            TextView er = FindViewById<TextView>(Resource.Id.er);
            try
            {
                calc = new Core.EquationCalculator(equation);
                //add(new Label(equation.toString()));
                //add(new Label(ch.getSelectedItem() + ": " + t.getText()));
                // result = "";
                foreach (Core.Pair<Core.Formula, int?> pair in equation.reactant)
                {
                    pass = pair.L1.RawString + ": ";
                    try
                    {
                        pass = pass + String.Format("{0:F2}+{1:F}-{2:F2}", calc.calcMass(condition, pair).toDouble(), calc.calcMass(condition, pair).ErrorMax, calc.calcMass(condition, pair).ErrorMin);
                    }
                    catch (Core.RMDatabase.AtomNameNotFoundException e)
                    {
                        cal("发生错误：未知原子：" + e.Atom);
                        return;
                    }
                    tx2.Text += pass.ToString() +"\n";
                   // result += pass + "\n";
                }
                foreach (Core.Pair<Core.Formula, int?> pair in equation.product)
                {
                    pass = pair.L1.RawString + ": ";
                    try
                    {
                        pass = pass + String.Format("{0:F2}+{1:F}-{2:F2}", calc.calcMass(condition, pair).toDouble(), calc.calcMass(condition, pair).ErrorMax, calc.calcMass(condition, pair).ErrorMin);
                    }
                    catch (Core.RMDatabase.AtomNameNotFoundException e)
                    {
                        er.Text = "发生错误：未知原子：" + e.Atom;
                        return;
                    }
                    tx3.Text += pass.ToString() + "\n";
                    //result += pass + "\n";
                }
            }
            catch (Exception e1)
            {
                er.Text = "未知错误2";
                return;
            }
            b.Click += delegate { init(); };
            ex.Click += delegate { Finish(); Process.KillProcess(Process.MyPid()); };
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            TextView tx4 = FindViewById<TextView>(Resource.Id.textView4);
            Spinner spinner = (Spinner)sender;
            pass = (string)spinner.GetItemAtPosition(e.Position);
            tx4.Text = "已选择 " + pass;
        }
    }
}

