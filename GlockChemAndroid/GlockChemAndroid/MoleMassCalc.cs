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
using System.Collections;

namespace GlockChemAndroid
{
    [Activity(Label = "MoleMassCalc")]
    public class MoleMassCalc : Activity
    {
        static ArrayList history = new ArrayList();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.mmcv0);
            EditText t = FindViewById<EditText>(Resource.Id.editText1);
            TextView r = FindViewById<TextView>(Resource.Id.result);
            Button co = FindViewById<Button>(Resource.Id.co);
            Button ca = FindViewById<Button>(Resource.Id.ca);
            TextView er = FindViewById<TextView>(Resource.Id.er);
            Button cl = FindViewById<Button>(Resource.Id.cl);
            Spinner sp = FindViewById<Spinner>(Resource.Id.spinner1);

            var str = new List<string>();
            foreach (string i in history)
                str.Add(i);
            sp.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_equationSelected);
            var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, str);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            sp.Adapter = adapter;

            Core.Formula formula;

            cl.Click += delegate { t.Text = ""; r.Text = ""; };
            ca.Click += delegate {
                String strInput = t.Text;
                if (strInput == "")
                {
                    er.Text = Resources.GetText(Resource.String.formula_empty_imput);
                    Toast.MakeText(this, er.Text, ToastLength.Short).Show();
                }
                try
                {
                    formula = new Core.Formula(strInput);
                    Core.Equation eq = new Core.Equation(t.Text + "=" + t.Text);
                    Core.EquationBalancer eb = new Core.EquationBalancer(eq);

                    Core.EquationCalculator.EquationCondition condition = new Core.EquationCalculator.EquationConditionMole(eq.reactant[0], new Core.AdvNum(1));
                    Core.EquationCalculator calc = new Core.EquationCalculator(eq);
                    calc.calcMass(condition, eq.reactant[0]).toDouble();
                    r.Text = calc.calcMass(condition, eq.reactant[0]).toDouble() + " g/mol";
                }
                catch (Exception e)
                {
                    er.Text = Resources.GetText(Resource.String.parse_error) + e.Message;
                    Toast.MakeText(this, er.Text, ToastLength.Short).Show();

                    return;
                }
                history.Add(t.Text);
                str = new List<string>();
                foreach (string i in history)
                    str.Add(i);
                adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, str);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                sp.Adapter = adapter;
            };
            co.Click += delegate {
                if (t.Text != "")
                {
                    ClipboardManager mClipboardManager = (ClipboardManager)GetSystemService(ClipboardService);
                    mClipboardManager.Text = (r.Text);
                    Toast.MakeText(this, Resources.GetText(Resource.String.output_copied), ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, Resources.GetText(Resource.String.copy_null), ToastLength.Short).Show();
                }
            };
        }

        private void Spinner_equationSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            EditText t = FindViewById<EditText>(Resource.Id.editText1);
            Spinner spinner = (Spinner)sender;
            t.Text = (string)spinner.GetItemAtPosition(e.Position);
        }
    }
}