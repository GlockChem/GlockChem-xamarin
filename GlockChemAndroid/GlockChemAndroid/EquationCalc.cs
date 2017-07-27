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
    [Activity(Label = "EquationCalc")]
    public class EquationCalc : Activity
    {
        Core.Equation equation = null;
        Core.EquationCalculator calc = null;
        Core.EquationBalancer balance;
        Core.EquationCalculator.EquationCondition condition;
        static ArrayList history = new ArrayList();
        string pass, p;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Init();
        }
        protected void Init() {
            SetContentView(Resource.Layout.eqfo);
            Button b = FindViewById<Button>(Resource.Id.BB);
            Button r = FindViewById<Button>(Resource.Id.rma);
            Button ret = FindViewById<Button>(Resource.Id.ret);
            EditText t = FindViewById<EditText>(Resource.Id.editText1);
            TextView er = FindViewById<TextView>(Resource.Id.er);
            Spinner sp = FindViewById<Spinner>(Resource.Id.spinner1);
            equation = null;
            balance = null;
            var str = new List<string>();
            foreach (string i in history)
                str.Add(i);
            
            sp.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_equationSelected);
            var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, str);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            sp.Adapter = adapter;

            r.Click += delegate { t.Text = ""; };
            ret.Click += delegate {
                var intent = new Intent(this, typeof(MainActivity)).SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
                Finish();
            };
            b.Click += delegate {
                String strInput = t.Text;
                if (strInput == "")
                {
                    er.Text = Resources.GetText(Resource.String.equation_empty_imput);
                    Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                }
                try
                {
                    equation = new Core.Equation(strInput);
                }
                catch (Exception e)
                {
                    er.Text = Resources.GetText(Resource.String.parse_error) + e.Message;
                    Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                    
                    return;
                }
                balance = new Core.EquationBalancer(equation);
                if (balance.checkBalance() == false)
                {
                    if (balance.balanceGaussian() != true)
                    {
                        er.Text = Resources.GetText(Resource.String.balance_failed); 
                        Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                    }
                    else
                    {
                        Cal();
                    }
                }
                else
                {
                    Cal();
                }
            };
        }

        private void Spinner_equationSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            EditText t = FindViewById<EditText>(Resource.Id.editText1);
            Spinner spinner = (Spinner)sender;
            t.Text = (string)spinner.GetItemAtPosition(e.Position);
        }

        protected void Cal()
        {
            SetContentView(Resource.Layout.colv);
            TextView t1 = FindViewById<TextView>(Resource.Id.textView1);
            t1.Text = equation.ToString();
            Button b = FindViewById<Button>(Resource.Id.BB);
            Button cpb = FindViewById<Button>(Resource.Id.cpb);
            Spinner ch = FindViewById<Spinner>(Resource.Id.spinner1);
            Spinner mo = FindViewById<Spinner>(Resource.Id.spinner2);
            EditText t = FindViewById<EditText>(Resource.Id.editText1);
            TextView er = FindViewById<TextView>(Resource.Id.er);
            string w = "";
            var str = new List<string>();
            foreach (Core.Pair<Core.Formula, int?> pair in equation.reactant)
            {
                str.Add(pair.L1.RawString);
                w += pair.L1.RawString + "+";
            }
            w = w.TrimEnd('+');
            w += "=";
            foreach (Core.Pair<Core.Formula, int?> pair in equation.product)
            {
                str.Add(pair.L1.RawString);
                w += pair.L1.RawString + "+";
            }
            w = w.TrimEnd('+');
            history.Add(w);
            ch.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, str);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            ch.Adapter = adapter;

            var lis = new List<string> {
                Resources.GetText (Resource.String.condition_type_mass),
                Resources.GetText (Resource.String.condition_type_mole)};
            
            var adapter2 = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, lis);
            mo.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ConditionSelected);
            adapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            mo.Adapter = adapter2;

            cpb.Click += delegate {
                ClipboardManager mClipboardManager = (ClipboardManager)GetSystemService(ClipboardService);
                mClipboardManager.Text=(t1.Text);
                Toast.MakeText(this, Resources.GetText(Resource.String.equation_copied), ToastLength.Short).Show();
            };

            b.Click += delegate
            {
                double numCondition = 0;
                try
                {
                    numCondition = Double.Parse(t.Text);
                }
                catch (Exception e)
                {
                    er.Text = Resources.GetText(Resource.String.invalid_number);
                    Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                    return;
                }
                if (numCondition <= 0)
                {
                    er.Text = Resources.GetText(Resource.String.invalid_number);
                    Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                    return;
                }
                condition = new Core.EquationCalculator.EquationConditionMass(equation.reactant[0], new Core.AdvNum(numCondition));

                Core.Pair<Core.Formula, int?> temp = equation.reactant[0];

                bool flag = true;
                for (int i = 0; i < equation.reactant.Count; i++)
                {
                    if (pass == equation.reactant[i].L1.RawString)
                    {
                        temp = equation.reactant[i];
                        flag = false;
                    }
                }
                for (int i = 0; i < equation.product.Count; i++)
                {
                    if (pass == equation.product[i].L1.RawString)
                    {
                        temp = equation.product[i];
                        flag = false;
                    }
                }
                if (flag)
                {
                    er.Text = Resources.GetText(Resource.String.unknown_error) + "1";
                    Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                }
                else
                {
                    if (p == lis[0])
                    {
                        condition = new Core.EquationCalculator.EquationConditionMass(temp, new Core.AdvNum(numCondition));
                    }
                    else
                    {
                        condition = new Core.EquationCalculator.EquationConditionMole(temp, new Core.AdvNum(numCondition));
                    }
                    Fin();
                }
            };
        }

        private void Fin()
        {
            SetContentView(Resource.Layout.finv);
            TextView tx2 = FindViewById<TextView>(Resource.Id.textView2);
            TextView tx3 = FindViewById<TextView>(Resource.Id.textView3);
            Button b = FindViewById<Button>(Resource.Id.BB);
            Button ex = FindViewById<Button>(Resource.Id.BB2);
            TextView er = FindViewById<TextView>(Resource.Id.er);
            Button re = FindViewById<Button>(Resource.Id.Brec);
            Button cpb = FindViewById<Button>(Resource.Id.cpb);
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
                        pass = pass + String.Format("{0:F2}+{1:F}-{2:F2} (g)", calc.calcMass(condition, pair).toDouble(), calc.calcMass(condition, pair).ErrorMax, calc.calcMass(condition, pair).ErrorMin);
                    }
                    catch (Core.RMDatabase.AtomNameNotFoundException e)
                    {
                        er.Text = Resources.GetText(Resource.String.unknown_atom) + e.Atom;
                        Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                        return;
                    }
                    tx2.Text += pass.ToString() +"\n";
                }
                foreach (Core.Pair<Core.Formula, int?> pair in equation.product)
                {
                    pass = pair.L1.RawString + ": ";
                    try
                    {
                        pass = pass + String.Format("{0:F2}+{1:F}-{2:F2} (g)", calc.calcMass(condition, pair).toDouble(), calc.calcMass(condition, pair).ErrorMax, calc.calcMass(condition, pair).ErrorMin);
                    }
                    catch (Core.RMDatabase.AtomNameNotFoundException e)
                    {
                        er.Text = Resources.GetText(Resource.String.unknown_atom) + e.Atom;
                        Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                        return;
                    }
                    tx3.Text += pass.ToString() + "\n";
                }
            }
            catch (Exception e1)
            {
                er.Text = Resources.GetText(Resource.String.unknown_error) + "2";
                Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                return;
            }

            cpb.Click += delegate {
                ClipboardManager mClipboardManager = (ClipboardManager)GetSystemService(ClipboardService);
                mClipboardManager.Text = (tx2.Text+tx3.Text);
                Toast.MakeText(this, Resources.GetText(Resource.String.output_copied), ToastLength.Short).Show();
            };

            b.Click += delegate { Init(); };
            re.Click += delegate { Cal(); };
            ex.Click += delegate {
                var intent = new Intent(this, typeof(MainActivity)).SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
                Finish();
            };
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            TextView tx4 = FindViewById<TextView>(Resource.Id.textView4);
            Spinner spinner = (Spinner)sender;
            pass = (string)spinner.GetItemAtPosition(e.Position);
            tx4.Text = Resources.GetText(Resource.String.selected) + pass;
        }
        private void Spinner_ConditionSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            p = (string)spinner.GetItemAtPosition(e.Position);
        }
    }
}

