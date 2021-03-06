﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Resources;

namespace CountItemSets
{
    public partial class Form1 : Form
    {
        private Thread updateThread;
        private ResourceManager localResourceManager;
        public Form1()
        {
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            InitializeComponent();

            localResourceManager = new ResourceManager("CountItemSets.Form1", typeof(Form1).Assembly);

            try
            {
                comboBoxCPUorGPU.SelectedIndex = 0;
                textBoxFileName.Text = ConfigurationManager.AppSettings["TransactionFileName"];
                textBoxFileNameItemsets.Text = ConfigurationManager.AppSettings["ItemsetsFileName"];
                fileNameItemsets = textBoxFileNameItemsets.Text;
                textBoxFileNameExcludeItems.Text = ConfigurationManager.AppSettings["ExcludeItemsFileName"];
                fileNameExcludeItems = textBoxFileNameExcludeItems.Text;
            }
            catch (Exception) { }
        }

        public void ThreadUpdateDataGridView()
        {
            IDictionary<long, int> dictionaryLevel1 = generator.Level1;
            int transactionCount = generator.GetTransactionCount();
            for (; ; )
            {
                WaitHandle.WaitAll(new WaitHandle[] { signalUpdateDataGridView });
                IEnumerable<AssociationRule> filter = results.Where(item => item.Confidence >= filterMinConfidence && item.Confidence <= filterMaxConfidence && item.Lift >= filterMinLift && item.Lift <= filterMaxLift && item.Support >= filterMinSupport && item.Support <= filterMaxSupport);
                if (filterConditionLevel1.Count() > 0)
                {
                    filter = filter.Where(item => (item.Condition1.EANCode < 0 ? filterConditionLevel1.Contains((int)-item.Condition1.EANCode) : filterConditionLevel1.Contains(dictionaryEANtoVGR[item.Condition1.EANCode])) 
                        && (item.Condition2.EANCode == 0 || (item.Condition2.EANCode < 0 ? filterConditionLevel1.Contains((int)-item.Condition2.EANCode) : filterConditionLevel1.Contains(dictionaryEANtoVGR[item.Condition2.EANCode])))
                        && (item.Condition3.EANCode == 0 || (item.Condition3.EANCode < 0 ? filterConditionLevel1.Contains((int)-item.Condition3.EANCode) : filterConditionLevel1.Contains(dictionaryEANtoVGR[item.Condition3.EANCode])))
                        && (item.Condition4.EANCode == 0 || (item.Condition4.EANCode < 0 ? filterConditionLevel1.Contains((int)-item.Condition4.EANCode) : filterConditionLevel1.Contains(dictionaryEANtoVGR[item.Condition4.EANCode])))
                        );
                }
                if (filterThenLevel1.Count() > 0)
                {
                    filter = filter.Where(item => item.Then.EANCode < 0 ? filterThenLevel1.Contains((int)-item.Then.EANCode) : filterThenLevel1.Contains(dictionaryEANtoVGR[item.Then.EANCode]));
                }
                if (filterConditionItemMaxSupport != 1.0)
                {
                    filter = filter.Where(item => ((double)dictionaryLevel1[item.Condition1.EANCode] / transactionCount) <= filterConditionItemMaxSupport
                    && (item.Condition2.EANCode == 0 || (((double)dictionaryLevel1[item.Condition2.EANCode] / transactionCount) <= filterConditionItemMaxSupport))
                    && (item.Condition3.EANCode == 0 || (((double)dictionaryLevel1[item.Condition3.EANCode] / transactionCount) <= filterConditionItemMaxSupport))
                    && (item.Condition4.EANCode == 0 || (((double)dictionaryLevel1[item.Condition4.EANCode] / transactionCount) <= filterConditionItemMaxSupport))
                    );
                }
                if (filterThenItemMaxSupport != 1.0)
                {
                    filter = filter.Where(item => ((double)dictionaryLevel1[item.Then.EANCode] / transactionCount) <= filterThenItemMaxSupport);
                }
                string textMatch;
                if ((textMatch = filterConditionTextMatch) != null)
                {
                    string text = textMatch.ToLower();
                    filter = filter.Where(item => item.Condition1.Text.ToLower().Contains(text)
                        || (item.Condition2.EANCode != 0 && (item.Condition2.Text.ToLower().Contains(text)))
                        || (item.Condition3.EANCode != 0 && (item.Condition3.Text.ToLower().Contains(text)))
                        || (item.Condition4.EANCode != 0 && (item.Condition4.Text.ToLower().Contains(text)))
                        );
                }
                if ((textMatch = filterThenTextMatch) != null)
                {
                    string text = textMatch.ToLower();
                    filter = filter.Where(item => item.Then.Text.ToLower().Contains(text));
                }
                if (filterConditionEANMatch != 0)
                {
                    long ean = filterConditionEANMatch;
                    filter = filter.Where(item => item.Condition1.EANCode == ean
                        || (item.Condition2.EANCode != 0 && (item.Condition2.EANCode == ean))
                        || (item.Condition3.EANCode != 0 && (item.Condition3.EANCode == ean))
                        || (item.Condition4.EANCode != 0 && (item.Condition4.EANCode == ean))
                        );
                }
                if (filterThenEANMatch != 0)
                {
                    long ean = filterThenEANMatch;
                    filter = filter.Where(item => item.Then.EANCode == ean);
                }
                List<AssociationRule> view = filter.ToList();
                Dictionary<AssociationRule.TransactionItem, int> groupsCondition1 = new Dictionary<AssociationRule.TransactionItem, int>();
                Dictionary<AssociationRule.TransactionItem, int> groupsCondition2 = new Dictionary<AssociationRule.TransactionItem, int>();
                Dictionary<AssociationRule.TransactionItem, int> groupsCondition3 = new Dictionary<AssociationRule.TransactionItem, int>();
                Dictionary<AssociationRule.TransactionItem, int> groupsCondition4 = new Dictionary<AssociationRule.TransactionItem, int>();
                Dictionary<AssociationRule.TransactionItem, int> groupsThen = new Dictionary<AssociationRule.TransactionItem, int>();
                for (int i = 0; i < view.Count; i++)
                {
                    AssociationRule rule = view[i];
                    if (!groupsCondition1.ContainsKey(rule.Condition1))
                        groupsCondition1.Add(rule.Condition1, i);
                    if (!groupsCondition2.ContainsKey(rule.Condition2))
                        groupsCondition2.Add(rule.Condition2, i);
                    if (!groupsCondition3.ContainsKey(rule.Condition3))
                        groupsCondition3.Add(rule.Condition3, i);
                    if (!groupsCondition4.ContainsKey(rule.Condition4))
                        groupsCondition4.Add(rule.Condition4, i);
                    if (!groupsThen.ContainsKey(rule.Then))
                        groupsThen.Add(rule.Then, i);
                }
                Invoke((Action)(() =>
                {
                    comboBoxRuleItemCondition1.Items.Clear();
                    comboBoxRuleItemCondition2.Items.Clear();
                    comboBoxRuleItemCondition3.Items.Clear();
                    comboBoxRuleItemCondition4.Items.Clear();
                    comboBoxRuleItemThen.Items.Clear();

                    comboBoxRuleGroupCondition1.Items.Clear();
                    comboBoxRuleGroupCondition2.Items.Clear();
                    comboBoxRuleGroupCondition3.Items.Clear();
                    comboBoxRuleGroupCondition4.Items.Clear();
                    comboBoxRuleGroupThen.Items.Clear();

                    comboBoxRuleItemCondition1.Items.AddRange(groupsCondition1.Where(pair => pair.Key.IsItem).Select(pair => new AssociationRule.ItemIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());
                    comboBoxRuleItemCondition2.Items.AddRange(groupsCondition2.Where(pair => pair.Key.IsItem).Select(pair => new AssociationRule.ItemIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());
                    comboBoxRuleItemCondition3.Items.AddRange(groupsCondition3.Where(pair => pair.Key.IsItem).Select(pair => new AssociationRule.ItemIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());
                    comboBoxRuleItemCondition4.Items.AddRange(groupsCondition4.Where(pair => pair.Key.IsItem).Select(pair => new AssociationRule.ItemIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());
                    comboBoxRuleItemThen.Items.AddRange(groupsThen.Where(pair => pair.Key.IsItem).Select(pair => new AssociationRule.ItemIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());

                    comboBoxRuleGroupCondition1.Items.AddRange(groupsCondition1.Where(pair => pair.Key.IsGroup).Select(pair => new AssociationRule.GroupIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());
                    comboBoxRuleGroupCondition2.Items.AddRange(groupsCondition2.Where(pair => pair.Key.IsGroup).Select(pair => new AssociationRule.GroupIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());
                    comboBoxRuleGroupCondition3.Items.AddRange(groupsCondition3.Where(pair => pair.Key.IsGroup).Select(pair => new AssociationRule.GroupIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());
                    comboBoxRuleGroupCondition4.Items.AddRange(groupsCondition4.Where(pair => pair.Key.IsGroup).Select(pair => new AssociationRule.GroupIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());
                    comboBoxRuleGroupThen.Items.AddRange(groupsThen.Where(pair => pair.Key.IsGroup).Select(pair => new AssociationRule.GroupIndexPair(pair.Key, pair.Value)).OrderBy(p => p).ToArray());

                    dataGridViewResults.DataSource = view;
                    groupBoxAssociationRules.Text = localResourceManager.GetString("groupBoxAssociationRules.Text") + " " + view.Count + " of " + results.Count;
                    Application.UseWaitCursor = false; // Cursor = Cursors.Default;
                    dataGridViewResults.UseWaitCursor = false; // dataGridViewResults.Cursor = Cursors.Default;
                }));
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            updateThread.Abort();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            updateThread = new Thread(new ThreadStart(ThreadUpdateDataGridView));
            updateThread.Start();
        }


        private void browseButton1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Csv files (*.csv)|*.csv";
            openFileDialog1.ShowDialog();
            textBoxFileName.Text = openFileDialog1.FileName;

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            if (config.AppSettings.Settings["TransactionFileName"] != null)
                config.AppSettings.Settings.Remove("TransactionFileName");
            config.AppSettings.Settings.Add("TransactionFileName", openFileDialog1.FileName);
            config.Save(ConfigurationSaveMode.Modified);
        }

        public IFrequentItemsetGenerator generator;
            //new ParallelTransactionFrequentItemsetGenerator();
            //new GPUTransactionFrequentItemsetGenerator();

        static Dictionary<long, string> dictionaryEAN = new Dictionary<long, string>();
        static Dictionary<int, string> dictionaryVGR = new Dictionary<int, string>();
        static Dictionary<long, int> dictionaryEANtoVGR = new Dictionary<long, int>();
        static Dictionary<string, double> dictionaryRule = new Dictionary<string, double>();

        //static Dictionary<long, int> dictionaryLevel1 = new Dictionary<long, int>();
        //Dictionary<string, int> dictionaryLevel2 = new Dictionary<string, int>();
        //Dictionary<string, int> dictionaryLevel3 = new Dictionary<string, int>();
        //Dictionary<string, int> dictionaryLevel4 = new Dictionary<string, int>();
        //Dictionary<string, int> dictionaryLevel5 = new Dictionary<string, int>();

        //static int transactionCount = 0;
        double pruningMinSupport = 0.0001;
        HashSet<long> pruningExcludeItems = new HashSet<long>();

        List<AssociationRule> results = new List<AssociationRule>();
        double filterMaxSupport = 1.0000;
        double filterMinSupport = 0.0010;
        double filterMaxLift = 1000.0;
        double filterMinLift = 1.0;
        double filterMaxConfidence = 1.00;
        double filterMinConfidence = 0.05;
        HashSet<int> filterConditionLevel1 = new HashSet<int>();
        HashSet<int> filterThenLevel1 = new HashSet<int>();
        double filterConditionItemMaxSupport = 1.00;
        double filterThenItemMaxSupport = 1.00;
        volatile string filterConditionTextMatch = null;
        volatile string filterThenTextMatch = null;
        long filterConditionEANMatch = 0;
        long filterThenEANMatch = 0;
        AutoResetEvent signalUpdateDataGridView = new AutoResetEvent(false);

        string fileNameItemsets = "";
        string fileNameExcludeItems = "";

        private void LoadPruningExcludeItems()
        {
            pruningExcludeItems.Clear();
            if(fileNameExcludeItems == "")
                return;
            StreamReader reader = new StreamReader(fileNameExcludeItems);
            int rowCount = 0;
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        long eanNr = Int64.Parse(columns[0]);
                        pruningExcludeItems.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            Application.UseWaitCursor = true;
            Application.DoEvents();
            // Cursor = Cursors.WaitCursor;

            InitStaticTables();

            //CountItemSets();
            generator.SetPruningMinSupport(pruningMinSupport);
            generator.BeginGenerate(textBoxFileName.Text, buttonStart_Click_Callback);
            Thread thread = new Thread(new ThreadStart(buttonStart_Click_Surveillance));
            thread.Name = "SurveillanceGeneratorThread";
            thread.Start();
        }

        private void buttonStart_Click_Surveillance()
        {
            try
            {
                for (; ; )
                {
                    int progress = generator.GetProgess();
                    Stopwatch stopwatch = generator.GetStopWatch();
                    Invoke((Action)(() =>
                    {
                        try
                        {
                            progressBarLoadingData.Value = progress;
                            textBoxTime.Text = stopwatch.Elapsed.ToString();
                        }
                        catch (Exception) { }
                    }));
                    if (progress == 100) break;
                    Thread.Sleep(100);
                }
            }
            catch (Exception) { }
        }

        private void buttonStart_Click_Callback()
        {
            dictionaryEANtoVGR = generator.GetDictionaryEANtoVGR();

            Invoke((Action)(() =>
            {
                InitFilters();
            }));

            GenerateRules();

            Invoke((Action)(() =>
            {
                UpdateMetaData();

                buttonStart.Enabled = true;
            }));            
        }

        void InitStaticTables()
        {
            String eanPath = "EAN.csv";
            try
            {
                eanPath = ConfigurationManager.AppSettings["EANTable"];
                if (eanPath == null) throw new Exception();
            }
            catch (Exception)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Select EAN translation table";
                dialog.ShowDialog();
                eanPath = dialog.FileName;
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                config.AppSettings.Settings.Add("EANTable", eanPath);
                config.Save(ConfigurationSaveMode.Modified);
            }
            dictionaryEAN.Clear();
            TextFieldParser parser = new TextFieldParser(eanPath);
            parser.SetDelimiters(";");
            parser.ReadLine();
            while (!parser.EndOfData)
            {
                try
                {
                    string[] fields = parser.ReadFields();
                    long eanNr = Int64.Parse(fields[0]);
                    string eanDesc = fields[1];
                    if (dictionaryEAN.ContainsKey(eanNr))
                    {
                        eanNr = 0;
                    }
                    else
                    dictionaryEAN.Add(eanNr, eanDesc);
                }
                catch (Exception) { }
            }


            String vgrPath = "VGR.csv";
            try
            {
                vgrPath = ConfigurationManager.AppSettings["VGRTable"];
                if (vgrPath == null) throw new Exception();
            }
            catch (Exception)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Select VGR translation table";
                dialog.ShowDialog();
                vgrPath = dialog.FileName;
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                config.AppSettings.Settings.Add("VGRTable", vgrPath);
                config.Save(ConfigurationSaveMode.Modified);
            }
            dictionaryVGR.Clear();
            parser = new TextFieldParser(vgrPath);
            parser.SetDelimiters(";");
            parser.ReadLine();
            while (!parser.EndOfData)
            {
                try
                {
                    string[] fields = parser.ReadFields();
                    int vgrNr = Int32.Parse(fields[0]);
                    string vgrDesc = fields[1];
                    dictionaryVGR.Add(vgrNr, vgrDesc);
                }
                catch (Exception) { }
            }
        }

        void InitFilters()
        {
            IDictionary<long, int> dictionaryLevel1 = generator.Level1;
            listBoxConditionFilterLevel1.Items.Clear();
            listBoxThenFilterLevel1.Items.Clear();
            List<KeyValuePair<int, string>> groupItems = new List<KeyValuePair<int, string>>();
            foreach (KeyValuePair<long, int> item in dictionaryLevel1)
            {
                if (item.Key < 0)
                {
                    groupItems.Add(new KeyValuePair<int, string>((int)-item.Key, dictionaryVGR[(int)-item.Key]));
                }
            }
            groupItems = groupItems.OrderBy(item => item.Key).ToList();
            foreach (KeyValuePair<int, string> item in groupItems)
            {
                listBoxConditionFilterLevel1.Items.Add(item);
                listBoxThenFilterLevel1.Items.Add(item);
            }

            comboBoxFilterCondition.Items.Clear();
            comboBoxFilterThen.Items.Clear();
            List<KeyValuePair<long, string>> items = new List<KeyValuePair<long, string>>();
            foreach (KeyValuePair<long, int> item in dictionaryLevel1)
            {
                if (item.Key > 0)
                {
                    items.Add(new KeyValuePair<long, string>(item.Key, dictionaryEAN.ContainsKey(item.Key) ? dictionaryEAN[item.Key] : "Unknown"));
                }
            }
            items = items.OrderBy(item => item.Key).ToList();
            foreach (KeyValuePair<long, string> item in items)
            {
                comboBoxFilterCondition.Items.Add(item);
                comboBoxFilterThen.Items.Add(item);
            }

            labelMaxConfidence.Text = filterMaxConfidence.ToString("P");
            labelMinConfidence.Text = filterMinConfidence.ToString("P");
            labelMaxSupport.Text = filterMaxSupport.ToString("P4");
            labelMinSupport.Text = filterMinSupport.ToString("P4");
            labelMaxLift.Text = filterMaxLift.ToString();
            labelMinLift.Text = filterMinLift.ToString();

            labelConditionItemMaxSupport.Text = filterConditionItemMaxSupport.ToString("P4");
            labelThenItemMaxSupport.Text = filterThenItemMaxSupport.ToString("P4");
        }

        void GenerateRules()
        {
            int transactionCount = generator.GetTransactionCount();
            IDictionary<long, int> dictionaryLevel1 = generator.Level1;
            IDictionary<string, int> dictionaryLevel2 = generator.Level2;
            IDictionary<string, int> dictionaryLevel3 = generator.Level3;
            IDictionary<string, int> dictionaryLevel4 = generator.Level4;
            IDictionary<string, int> dictionaryLevel5 = generator.Level5;
            // Building association rules
            AssociationRule.SetCurrentGenerator(generator); 
            results = new List<AssociationRule>();
            foreach (KeyValuePair<string, int> pair in dictionaryLevel2)
            {
                String[] columns = pair.Key.Split(',');
                long eanNr1 = 0;
                Int64.TryParse(columns[0], out eanNr1);
                long eanNr2 = 0;
                Int64.TryParse(columns[1], out eanNr2);
                results.Add(new AssociationRule(eanNr1, 0, 0, 0, eanNr2, (double)pair.Value / (double)dictionaryLevel1[eanNr1], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel1[eanNr1] * (double)dictionaryLevel1[eanNr2]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr2, 0, 0, 0, eanNr1, (double)pair.Value / (double)dictionaryLevel1[eanNr2], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel1[eanNr2] * (double)dictionaryLevel1[eanNr1]), (double)pair.Value / (double)transactionCount));
            }

            foreach (KeyValuePair<string, int> pair in dictionaryLevel3)
            {
                String[] columns = pair.Key.Split(',');
                long eanNr1 = 0;
                Int64.TryParse(columns[0], out eanNr1);
                long eanNr2 = 0;
                Int64.TryParse(columns[1], out eanNr2);
                long eanNr3 = 0;
                Int64.TryParse(columns[2], out eanNr3);
                results.Add(new AssociationRule(eanNr1, eanNr2, 0, 0, eanNr3, (double)pair.Value / (double)dictionaryLevel2[eanNr1 + "," + eanNr2], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel2[eanNr1 + "," + eanNr2] * (double)dictionaryLevel1[eanNr3]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr3, 0, 0, eanNr2, (double)pair.Value / (double)dictionaryLevel2[eanNr1 + "," + eanNr3], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel2[eanNr1 + "," + eanNr3] * (double)dictionaryLevel1[eanNr2]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr2, eanNr3, 0, 0, eanNr1, (double)pair.Value / (double)dictionaryLevel2[eanNr2 + "," + eanNr3], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel2[eanNr2 + "," + eanNr3] * (double)dictionaryLevel1[eanNr1]), (double)pair.Value / (double)transactionCount));
            }

            foreach (KeyValuePair<string, int> pair in dictionaryLevel4)
            {
                String[] columns = pair.Key.Split(',');
                long eanNr1 = 0;
                Int64.TryParse(columns[0], out eanNr1);
                long eanNr2 = 0;
                Int64.TryParse(columns[1], out eanNr2);
                long eanNr3 = 0;
                Int64.TryParse(columns[2], out eanNr3);
                long eanNr4 = 0;
                Int64.TryParse(columns[3], out eanNr4);
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr3, 0, eanNr4, (double)pair.Value / (double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr3], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr3] * (double)dictionaryLevel1[eanNr4]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr4, 0, eanNr3, (double)pair.Value / (double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr4], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr4] * (double)dictionaryLevel1[eanNr3]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr3, eanNr4, 0, eanNr2, (double)pair.Value / (double)dictionaryLevel3[eanNr1 + "," + eanNr3 + "," + eanNr4], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel3[eanNr1 + "," + eanNr3 + "," + eanNr4] * (double)dictionaryLevel1[eanNr2]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr2, eanNr3, eanNr4, 0, eanNr1, (double)pair.Value / (double)dictionaryLevel3[eanNr2 + "," + eanNr3 + "," + eanNr4], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel3[eanNr2 + "," + eanNr3 + "," + eanNr4] * (double)dictionaryLevel1[eanNr1]), (double)pair.Value / (double)transactionCount));
            }

            foreach (KeyValuePair<string, int> pair in dictionaryLevel5)
            {
                String[] columns = pair.Key.Split(',');
                long eanNr1 = 0;
                Int64.TryParse(columns[0], out eanNr1);
                long eanNr2 = 0;
                Int64.TryParse(columns[1], out eanNr2);
                long eanNr3 = 0;
                Int64.TryParse(columns[2], out eanNr3);
                long eanNr4 = 0;
                Int64.TryParse(columns[3], out eanNr4);
                long eanNr5 = 0;
                Int64.TryParse(columns[4], out eanNr5);
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr3, eanNr4, eanNr5, (double)pair.Value / (double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr4], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr4] * (double)dictionaryLevel1[eanNr5]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr3, eanNr5, eanNr4, (double)pair.Value / (double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr5], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr5] * (double)dictionaryLevel1[eanNr4]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr4, eanNr5, eanNr3, (double)pair.Value / (double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr4 + "," + eanNr5], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr4 + "," + eanNr5] * (double)dictionaryLevel1[eanNr3]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr3, eanNr4, eanNr5, eanNr2, (double)pair.Value / (double)dictionaryLevel4[eanNr1 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr1 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5] * (double)dictionaryLevel1[eanNr2]), (double)pair.Value / (double)transactionCount));
                results.Add(new AssociationRule(eanNr2, eanNr3, eanNr4, eanNr5, eanNr1, (double)pair.Value / (double)dictionaryLevel4[eanNr2 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr2 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5] * (double)dictionaryLevel1[eanNr1]), (double)pair.Value / (double)transactionCount));
            }

            results = new List<AssociationRule>(results.OrderByDescending(item => item.Lift).OrderBy(item => item.Then.ToString()));
            signalUpdateDataGridView.Set();
        }

        private void UpdateMetaData()
        {
            int transactionCount = generator.GetTransactionCount();
            IDictionary<string, int> dictionaryLevel2 = generator.Level2;
            IDictionary<string, int> dictionaryLevel3 = generator.Level3;
            IDictionary<string, int> dictionaryLevel4 = generator.Level4;
            IDictionary<string, int> dictionaryLevel5 = generator.Level5;
            textBoxTransactionCount.Text = transactionCount.ToString();
            textBoxNrFrequentItemsets.Text = (dictionaryLevel2.Count + dictionaryLevel3.Count + dictionaryLevel4.Count + dictionaryLevel5.Count).ToString();
            textBoxNrAssociationRules.Text = (dictionaryLevel2.Count * 2 + dictionaryLevel3.Count * 3 + dictionaryLevel4.Count * 4 + dictionaryLevel5.Count * 5).ToString();
        }

        static private string TranslateEANpairs(string textPair)
        {
            String[] columns = textPair.Split(',');
            List<string> results = new List<string>();
            foreach (string field in columns)
            {
                string result = field;
                long eanNr = 0;
                Int64.TryParse(field, out eanNr);
                if (dictionaryEAN.ContainsKey(eanNr))
                    result = dictionaryEAN[eanNr] + " (" + eanNr + ")";
                else
                {
                    if (dictionaryEANtoVGR.ContainsKey(eanNr))
                    {
                        int vgrNr = dictionaryEANtoVGR[eanNr];
                        if(dictionaryVGR.ContainsKey(vgrNr))
                            result = dictionaryVGR[vgrNr] + " (" + eanNr + ")";
                        else
                            result = "Varugrupp " + vgrNr + " (" + eanNr + ")";
                    }
                }
                results.Add(result);
            }
            return String.Join(",",results.ToArray());
        }

        public class AssociationRule
        {
            public TransactionItem Condition1 { get; set; }
            public TransactionItem Condition2 { get; set; }
            public TransactionItem Condition3 { get; set; }
            public TransactionItem Condition4 { get; set; }
            public TransactionItem Then { get; set; }
            public double Confidence { get; set; }
            public double Lift { get; set; }
            public double Support { get; set; }

            private static IFrequentItemsetGenerator generator;

            public static void SetCurrentGenerator(IFrequentItemsetGenerator generator) 
            {
                AssociationRule.generator = generator;
            }

            public AssociationRule(long condition1, long condition2, long condition3, long condition4, long then, double confidence, double lift, double support)
            {
                Condition1 = new TransactionItem(condition1);
                Condition2 = new TransactionItem(condition2);
                Condition3 = new TransactionItem(condition3);
                Condition4 = new TransactionItem(condition4);
                Then = new TransactionItem(then);
                if (confidence > 1.00)
                    confidence = 1.00;
                Confidence = confidence;
                Lift = lift;
                Support = support;
            }
            public int NumberOfConditions()
            {
                if (Condition2.EANCode == 0)
                    return 1;
                if (Condition3.EANCode == 0)
                    return 2;
                if (Condition4.EANCode == 0)
                    return 3;
                return 4;
            }

            public class TransactionItem: IComparable
            {
                public long EANCode { get; set; }
                public string Text { get { return TranslateEANCodeFull(); } }
                public string Name { get { return TranslateEANCodeShort(); } }
                public int GroupID { get { if (IsEmpty) return 0; else if (IsGroup) return (int)-EANCode; else return dictionaryEANtoVGR[EANCode]; } }
                public string GroupName { get { if (IsGroup || IsItem) return dictionaryVGR[GroupID]; else return string.Empty; } }
                public bool IsGroup { get { return EANCode < 0; } }
                public bool IsItem { get { return EANCode > 0; } }
                public bool IsEmpty { get { return EANCode == 0; } }
                public double Support { get { if (IsGroup || IsItem) return (double)generator.Level1[EANCode] / generator.GetTransactionCount(); else return 0.0; } }
                public TransactionItem(long eanCode)
                {
                    EANCode = eanCode;
                }
                public override int GetHashCode()
                {
                    return EANCode.GetHashCode();
                }
                public override bool Equals(object obj)
                {
                    var other = obj as TransactionItem;
                    if (other == null)
                        return false;

                    return EANCode == other.EANCode;
                }
                public override string ToString()
                {
                    return Text;
                }

                public string TranslateEANCodeShort()
                {
                    if (EANCode == 0) return string.Empty;
                    string result;
                    if (EANCode < 0)
                    {
                        int vgrNr = (int) -EANCode;
                        if (dictionaryVGR.ContainsKey(vgrNr))
                            result = dictionaryVGR[vgrNr];
                        else
                            result = vgrNr.ToString();

                    }
                    else if (dictionaryEAN.ContainsKey(EANCode))
                        result = dictionaryEAN[EANCode];
                    else
                    {
                        if (dictionaryEANtoVGR.ContainsKey(EANCode))
                        {
                            int vgrNr = dictionaryEANtoVGR[EANCode];
                            if (dictionaryVGR.ContainsKey(vgrNr))
                                result = dictionaryVGR[vgrNr];
                            else
                                result = vgrNr.ToString();
                        }
                        else result = EANCode.ToString();
                    }
                    return result;
                }
                public string TranslateEANCodeFull()
                {
                    if (EANCode == 0) return string.Empty;
                    string result;
                    if (EANCode < 0)
                    {
                        int vgrNr = (int)-EANCode;
                        if (dictionaryVGR.ContainsKey(vgrNr))
                            result = "[" + dictionaryVGR[vgrNr] + "(" + vgrNr + ")" + "]";
                        else
                            result = "[" + "(" + vgrNr + ")" + "]";

                    }
                    else if (dictionaryEAN.ContainsKey(EANCode))
                        result = dictionaryEAN[EANCode] + " (" + EANCode + ")";
                    else
                    {
                        if (dictionaryEANtoVGR.ContainsKey(EANCode))
                        {
                            int vgrNr = dictionaryEANtoVGR[EANCode];
                            if (dictionaryVGR.ContainsKey(vgrNr))
                                result = dictionaryVGR[vgrNr] + " (" + EANCode + ")";
                            else
                                result = vgrNr + " (" + EANCode + ")";
                        }
                        else result = EANCode.ToString();
                    }
                    return result;
                }
                public int CompareTo(Object obj)
                {
                    if (obj == null) return 1;
                    TransactionItem item = obj as TransactionItem;
                    if (item != null) 
                        return this.ToString().CompareTo(item.ToString());
                    else 
                        throw new ArgumentException("Object is not a TransactionItem");
                }
                
            }

            public class GroupIndexPair : IComparable
            {
                public TransactionItem Group { get; set; }
                public int Index { get; set; }
                public GroupIndexPair(TransactionItem group, int index) {
                    Group = group;
                    Index = index;
                }
                public override string ToString()
                {
                    return Group.GroupName;
                }
                public int CompareTo(Object obj)
                {
                    if (obj == null) return 1;
                    GroupIndexPair item = obj as GroupIndexPair;
                    if (item != null)
                    {
                        int value = this.Group.CompareTo(item.Group);
                        if (value == 0)
                            return Index.CompareTo(item.Index);
                        else return value;
                    }
                    else
                        throw new ArgumentException("Object is not a GroupIndexPair");
                }            
            }

            public class ItemIndexPair : IComparable
            {
                public TransactionItem Item { get; set; }
                public int Index { get; set; }
                public ItemIndexPair(TransactionItem item, int index)
                {
                    Item = item;
                    Index = index;
                }
                public override string ToString()
                {
                    return Item.Name;
                }
                public int CompareTo(Object obj)
                {
                    if (obj == null) return 1;
                    ItemIndexPair item = obj as ItemIndexPair;
                    if (item != null)
                    {
                        int value = this.Item.CompareTo(item.Item);
                        if (value == 0)
                            return Index.CompareTo(item.Index);
                        else return value;
                    }
                    else
                        throw new ArgumentException("Object is not a ItemIndexPair");
                }
            }
        }

        private void trackBarMaxSupport_Scroll(object sender, EventArgs e)
        {
            filterMaxSupport = 0.0001 * Math.Pow(10, trackBarMaxSupport.Value / 25.0);
            labelMaxSupport.Text = filterMaxSupport.ToString("P4");
        }

        private void trackBarMaxSupport_ValueChanged(object sender, EventArgs e)
        {
            filterMaxSupport = 0.0001 * Math.Pow(10, trackBarMaxSupport.Value / 25.0);
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }
        
        private void trackBarMinSupport_Scroll(object sender, EventArgs e)
        {
            filterMinSupport = 0.0001 * Math.Pow(10, trackBarMinSupport.Value / 25.0);
            labelMinSupport.Text = filterMinSupport.ToString("P4");
        }

        private void trackBarMinSupport_ValueChanged(object sender, EventArgs e)
        {
            filterMinSupport = 0.0001 * Math.Pow(10, trackBarMinSupport.Value / 25.0);
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void trackBarMaxLift_Scroll(object sender, EventArgs e)
        {
            filterMaxLift = (double)trackBarMaxLift.Value;
            labelMaxLift.Text = filterMaxLift.ToString("F");
        }

        private void trackBarMaxLift_ValueChanged(object sender, EventArgs e)
        {
            filterMaxLift = (double)trackBarMaxLift.Value;
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void trackBarMinLift_Scroll(object sender, EventArgs e)
        {
            filterMinLift = (double)trackBarMinLift.Value;
            labelMinLift.Text = filterMinLift.ToString("F");
        }

        private void trackBarMinLift_ValueChanged(object sender, EventArgs e)
        {
            filterMinLift = (double)trackBarMinLift.Value;
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void trackBarMaxConfidence_Scroll(object sender, EventArgs e)
        {
            filterMaxConfidence = trackBarMaxConfidence.Value / 100.0;
            labelMaxConfidence.Text = filterMaxConfidence.ToString("P");
        }

        private void trackBarMaxConfidence_ValueChanged(object sender, EventArgs e)
        {
            filterMaxConfidence = trackBarMaxConfidence.Value / 100.0;
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void trackBarMinConfidence_Scroll(object sender, EventArgs e)
        {
            filterMinConfidence = trackBarMinConfidence.Value / 100.0;
            labelMinConfidence.Text = filterMinConfidence.ToString("P");
        }

        private void trackBarMinConfidence_ValueChanged(object sender, EventArgs e)
        {
            filterMinConfidence = trackBarMinConfidence.Value / 100.0;
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void listBoxConditionFilterLevel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterConditionLevel1.Clear();
            foreach (KeyValuePair<int, string> pair in listBoxConditionFilterLevel1.SelectedItems)
            {
                filterConditionLevel1.Add(pair.Key);
            }
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void listBoxThenFilterLevel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterThenLevel1.Clear();
            foreach (KeyValuePair<int, string> pair in listBoxThenFilterLevel1.SelectedItems)
            {
                filterThenLevel1.Add(pair.Key);
            }
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void labelMaxLift_Click(object sender, EventArgs e)
        {

        }

        private void labelMinLift_Click(object sender, EventArgs e)
        {

        }

        private void trackBarPruningMinSupport_Scroll(object sender, EventArgs e)
        {
            pruningMinSupport = 0.0001 * Math.Pow(10, trackBarPruningMinSupport.Value / 25.0);
            labelPruningMinSupport.Text = pruningMinSupport.ToString("F4");
        }

        private void trackBarPruningMinSupport_ValueChanged(object sender, EventArgs e)
        {
            pruningMinSupport = 0.0001 * Math.Pow(10, trackBarPruningMinSupport.Value / 25.0);
        }

        private void selectAllToolStripMenuSelectAll_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                ContextMenuStrip menuStrip = menuItem.Owner as ContextMenuStrip;
                if (menuStrip != null)
                {
                    ListBox source = menuStrip.SourceControl as ListBox;
                    if (source != null)
                    {
                        for (int i = 0; i < source.Items.Count; i++)
                        {
                            source.SetSelected(i, true);
                        }
                    }
                }
            }
        }

        private void selectNoneToolStripMenuSelectNone_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                ContextMenuStrip menuStrip = menuItem.Owner as ContextMenuStrip;
                if (menuStrip != null)
                {
                    ListBox source = menuStrip.SourceControl as ListBox;
                    if (source != null)
                    {
                        source.ClearSelected();
                    }
                }
            }
        }

        private void trackBarConditionItemMaxSupport_ValueChanged(object sender, EventArgs e)
        {
            filterConditionItemMaxSupport = 0.0001 * Math.Pow(10, trackBarConditionItemMaxSupport.Value / 25.0);
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void trackBarConditionItemMaxSupport_Scroll(object sender, EventArgs e)
        {
            filterConditionItemMaxSupport = 0.0001 * Math.Pow(10, trackBarConditionItemMaxSupport.Value / 25.0);
            labelConditionItemMaxSupport.Text = filterConditionItemMaxSupport.ToString("P4");
        }

        private void trackBarThenItemMaxSupport_ValueChanged(object sender, EventArgs e)
        {
            filterThenItemMaxSupport = 0.0001 * Math.Pow(10, trackBarThenItemMaxSupport.Value / 25.0);
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void trackBarThenItemMaxSupport_Scroll(object sender, EventArgs e)
        {
            filterThenItemMaxSupport = 0.0001 * Math.Pow(10, trackBarThenItemMaxSupport.Value / 25.0);
            labelThenItemMaxSupport.Text = filterThenItemMaxSupport.ToString("P4");
        }

        private void dataGridViewResults_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewResults.SelectedRows.Count > 0)
            {
                AssociationRule rule = dataGridViewResults.SelectedRows[0].DataBoundItem as AssociationRule;
                if (rule != null)
                {
                    String text = @"{\rtf\ansi\b0"; 
                    text += @"{\b IF\b0} " + rule.Condition1.Name;
                    if (rule.Condition2.EANCode != 0)
                        text += @" {\b AND\b0} " + rule.Condition2.Name;
                    if (rule.Condition3.EANCode != 0)
                        text += @" {\b AND\b0} " + rule.Condition3.Name;
                    if (rule.Condition4.EANCode != 0)
                        text += @" {\b AND\b0} " + rule.Condition4.Name;
                    text += @" {\b THEN\b0} " + rule.Then.Name;
                    text += @"}";
                    try
                    {
                        richTextBoxSelectedRule.Rtf = text;
                    } catch(Exception) {};

                    textBoxRuleEANCondition1.Text = rule.Condition1.IsGroup ? "" : rule.Condition1.EANCode.ToString();
                    textBoxRuleEANCondition2.Text = rule.Condition2.IsItem ? rule.Condition2.EANCode.ToString() : "";
                    textBoxRuleEANCondition3.Text = rule.Condition3.IsItem ? rule.Condition3.EANCode.ToString() : "";
                    textBoxRuleEANCondition4.Text = rule.Condition4.IsItem ? rule.Condition4.EANCode.ToString() : "";
                    textBoxRuleEANThen.Text = rule.Then.IsGroup ? "" : rule.Then.EANCode.ToString();

                    if (!rule.Condition1.IsItem) comboBoxRuleItemCondition1.SelectedItem = null;
                    if (!rule.Condition2.IsItem) comboBoxRuleItemCondition2.SelectedItem = null;
                    if (!rule.Condition3.IsItem) comboBoxRuleItemCondition3.SelectedItem = null;
                    if (!rule.Condition4.IsItem) comboBoxRuleItemCondition4.SelectedItem = null;
                    if (!rule.Then.IsItem) comboBoxRuleItemThen.SelectedItem = null;

                    comboBoxRuleItemCondition1.Text = rule.Condition1.IsItem ? rule.Condition1.Name : "";
                    comboBoxRuleItemCondition2.Text = rule.Condition2.IsItem ? rule.Condition2.Name : "";
                    comboBoxRuleItemCondition3.Text = rule.Condition3.IsItem ? rule.Condition3.Name : "";
                    comboBoxRuleItemCondition4.Text = rule.Condition4.IsItem ? rule.Condition4.Name : "";
                    comboBoxRuleItemThen.Text = rule.Then.IsItem ? rule.Then.Name : "";
                    
                    comboBoxRuleGroupCondition1.Text = rule.Condition1.GroupName;
                    comboBoxRuleGroupCondition2.Text = rule.Condition2.GroupName;
                    comboBoxRuleGroupCondition3.Text = rule.Condition3.GroupName;
                    comboBoxRuleGroupCondition4.Text = rule.Condition4.GroupName;
                    comboBoxRuleGroupThen.Text = rule.Then.GroupName;

                    textBoxRuleSupportCondition1.Text = rule.Condition1.Support.ToString("P");
                    textBoxRuleSupportCondition2.Text = rule.Condition2.IsEmpty ? "" : rule.Condition2.Support.ToString("P");
                    textBoxRuleSupportCondition3.Text = rule.Condition3.IsEmpty ? "" : rule.Condition3.Support.ToString("P");
                    textBoxRuleSupportCondition4.Text = rule.Condition4.IsEmpty ? "" : rule.Condition4.Support.ToString("P");
                    textBoxRuleSupportThen.Text = rule.Then.Support.ToString("P");
                }
            }
        }

        private void buttonBrowseFileNameItemset_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Xml files (*.xml)|*.xml";
            openFileDialog1.ShowDialog();
            textBoxFileNameItemsets.Text = openFileDialog1.FileName;
            fileNameItemsets = textBoxFileNameItemsets.Text;

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            if (config.AppSettings.Settings["ItemsetsFileName"] != null)
                config.AppSettings.Settings.Remove("ItemsetsFileName");
            config.AppSettings.Settings.Add("ItemsetsFileName", openFileDialog1.FileName);
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void buttonSaveItemsets_Click(object sender, EventArgs e)
        {
            int transactionCount = generator.GetTransactionCount();
            IDictionary<long, int> dictionaryLevel1 = generator.Level1;
            IDictionary<string, int> dictionaryLevel2 = generator.Level2;
            IDictionary<string, int> dictionaryLevel3 = generator.Level3;
            IDictionary<string, int> dictionaryLevel4 = generator.Level4;
            IDictionary<string, int> dictionaryLevel5 = generator.Level5;
            saveFileDialog1.Filter = "Xml files (*.xml)|*.xml";
            saveFileDialog1.ShowDialog();
            string fileName = saveFileDialog1.FileName;
            XmlWriter writer = XmlWriter.Create(fileName);
            writer.WriteStartElement("Dictionaries");
            writer.WriteElementString("TransactionCount", transactionCount.ToString());
            writer.WriteStartElement("dictionaryEANtoVGR");
            foreach (KeyValuePair<long, int> pair in dictionaryEANtoVGR)
            {
                writer.WriteStartElement("Pair");
                writer.WriteElementString("Key", pair.Key.ToString());
                writer.WriteElementString("Value", pair.Value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("dictionaryLevel1");
            foreach (KeyValuePair<long, int> pair in dictionaryLevel1)
            {
                writer.WriteStartElement("Pair");
                writer.WriteElementString("Key", pair.Key.ToString());
                writer.WriteElementString("Value", pair.Value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("dictionaryLevel2");
            foreach (KeyValuePair<string, int> pair in dictionaryLevel2)
            {
                writer.WriteStartElement("Pair");
                writer.WriteElementString("Key", pair.Key.ToString());
                writer.WriteElementString("Value", pair.Value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("dictionaryLevel3");
            foreach (KeyValuePair<string, int> pair in dictionaryLevel3)
            {
                writer.WriteStartElement("Pair");
                writer.WriteElementString("Key", pair.Key.ToString());
                writer.WriteElementString("Value", pair.Value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("dictionaryLevel4");
            foreach (KeyValuePair<string, int> pair in dictionaryLevel4)
            {
                writer.WriteStartElement("Pair");
                writer.WriteElementString("Key", pair.Key.ToString());
                writer.WriteElementString("Value", pair.Value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("dictionaryLevel5");
            foreach (KeyValuePair<string, int> pair in dictionaryLevel5)
            {
                writer.WriteStartElement("Pair");
                writer.WriteElementString("Key", pair.Key.ToString());
                writer.WriteElementString("Value", pair.Value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();
        }

        private void buttonLoadItemsets_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;

            InitStaticTables();

            XMLFileGenerator generator = new XMLFileGenerator();
            generator.Load(fileNameItemsets);
            this.generator = generator;
            int transactionCount = generator.GetTransactionCount();
            textBoxTransactionCount.Text = transactionCount.ToString();
            dictionaryEANtoVGR = generator.GetDictionaryEANtoVGR();

            InitFilters();

            GenerateRules();

            UpdateMetaData();
        }

        private void buttonRuleExcludeGroup_Click(object sender, EventArgs e)
        {
            if (dataGridViewResults.SelectedRows.Count > 0)
            {
                AssociationRule rule = dataGridViewResults.SelectedRows[0].DataBoundItem as AssociationRule;
                if (rule != null)
                {
                    Button button = sender as Button;
                    if(button == buttonRuleExcludeGroupCondition1) {
                        ExcludeGroupFilterCondition(rule.Condition1.GroupID);
                    }
                    else if (button == buttonRuleExcludeGroupCondition2)
                    {
                        ExcludeGroupFilterCondition(rule.Condition2.GroupID);
                    }
                    else if (button == buttonRuleExcludeGroupCondition3)
                    {
                        ExcludeGroupFilterCondition(rule.Condition3.GroupID);
                    }
                    else if (button == buttonRuleExcludeGroupCondition4)
                    {
                        ExcludeGroupFilterCondition(rule.Condition4.GroupID);
                    }
                    else if (button == buttonRuleExcludeGroupThen)
                    {
                        ExcludeGroupFilterThen(rule.Then.GroupID);
                    }
                }
            }
        
        }

        private void ExcludeGroupFilterCondition(int groupID)
        {
            if (listBoxConditionFilterLevel1.SelectedItems.Count == 0) {
                for (int i = 0; i < listBoxConditionFilterLevel1.Items.Count; i++)
                {
                    listBoxConditionFilterLevel1.SetSelected(i, true);
                }
            }
            for (int i = 0; i < listBoxConditionFilterLevel1.Items.Count; i++)
            {
                KeyValuePair<int, string> pair = (KeyValuePair<int,string>)listBoxConditionFilterLevel1.Items[i];
                if(pair.Key == groupID)
                    listBoxConditionFilterLevel1.SetSelected(i, false);
            }
        }

        private void ExcludeGroupFilterThen(int groupID)
        {
            if (listBoxThenFilterLevel1.SelectedItems.Count == 0)
            {
                for (int i = 0; i < listBoxThenFilterLevel1.Items.Count; i++)
                {
                    listBoxThenFilterLevel1.SetSelected(i, true);
                }
            }
            for (int i = 0; i < listBoxThenFilterLevel1.Items.Count; i++)
            {
                KeyValuePair<int, string> pair = (KeyValuePair<int, string>)listBoxThenFilterLevel1.Items[i];
                if (pair.Key == groupID)
                    listBoxThenFilterLevel1.SetSelected(i, false);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
                try
                {
                    if (ActiveControl is DataGridView)
                        Clipboard.SetDataObject((ActiveControl as DataGridView).GetClipboardContent());
                    else if (ActiveControl is RichTextBox)
                    {
                        DataObject dataObject = new DataObject();
                        dataObject.SetData((ActiveControl as RichTextBox).SelectedText);
                        dataObject.SetData(DataFormats.Rtf,(ActiveControl as RichTextBox).SelectedRtf);
                        Clipboard.SetDataObject(dataObject);
                    }
                    
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                }
        }

        private void dataGridViewResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            AssociationRule rule = dataGridViewResults.Rows[e.RowIndex].DataBoundItem as AssociationRule;
            if (rule != null)
            {
                if (e.ColumnIndex <= 4)
                {
                    if ((e.ColumnIndex == 0 && rule.Condition1.IsGroup)
                        || (e.ColumnIndex == 1 && rule.Condition2.IsGroup)
                        || (e.ColumnIndex == 2 && rule.Condition3.IsGroup)
                        || (e.ColumnIndex == 3 && rule.Condition4.IsGroup)
                        || (e.ColumnIndex == 4 && rule.Then.IsGroup))
                    {
                        e.CellStyle.ForeColor = Color.DarkBlue;
                    }
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewResults_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridViewResults.Columns["Confidence"].DefaultCellStyle.Format = "P";
            dataGridViewResults.Columns["Lift"].DefaultCellStyle.Format = "F";
            dataGridViewResults.Columns["Support"].DefaultCellStyle.Format = "P";
            dataGridViewResults.Columns["Confidence"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewResults.Columns["Lift"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewResults.Columns["Support"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewResults.Columns["Lift"].DefaultCellStyle.Format = "F";
            dataGridViewResults.Columns["Support"].DefaultCellStyle.Format = "P";
            dataGridViewResults.Columns["Confidence"].FillWeight = 50;
            dataGridViewResults.Columns["Lift"].FillWeight = 50;
            dataGridViewResults.Columns["Support"].FillWeight = 50;
            //Translation
            dataGridViewResults.Columns["Condition1"].HeaderText = "Villkor1";
            dataGridViewResults.Columns["Condition2"].HeaderText = "Villkor2";
            dataGridViewResults.Columns["Condition3"].HeaderText = "Villkor3";
            dataGridViewResults.Columns["Condition4"].HeaderText = "Villkor4";
            dataGridViewResults.Columns["Then"].HeaderText = "Slutsats";            
            dataGridViewResults.Columns["Confidence"].HeaderText = "Förtroende";
            dataGridViewResults.Columns["Lift"].HeaderText = "Lyft";
            dataGridViewResults.Columns["Support"].HeaderText = "Stöd";

            dataGridViewResults.Columns["Condition1"].SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridViewResults.Columns["Condition2"].SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridViewResults.Columns["Condition3"].SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridViewResults.Columns["Condition4"].SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridViewResults.Columns["Then"].SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridViewResults.Columns["Confidence"].SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridViewResults.Columns["Lift"].SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridViewResults.Columns["Support"].SortMode = DataGridViewColumnSortMode.Programmatic;
        }

        private void comboBoxFilterThen_TextUpdate(object sender, EventArgs e)
        {
            filterThenEANMatch = 0;
            string text = comboBoxFilterThen.Text;
            if (text.Length == 0)
                filterThenTextMatch = null;
            else
                filterThenTextMatch = text;
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void comboBoxFilterCondition_TextUpdate(object sender, EventArgs e)
        {
            filterConditionEANMatch = 0;
            string text = comboBoxFilterCondition.Text;
            if (text.Length == 0)
                filterConditionTextMatch = null;
            else
                filterConditionTextMatch = text;
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void comboBoxFilterCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterConditionTextMatch = null;
            if (comboBoxFilterCondition.SelectedIndex >= 0) {
                KeyValuePair<long, string> item = (KeyValuePair<long, string>) comboBoxFilterCondition.SelectedItem;
                filterConditionEANMatch = item.Key;
            }
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void comboBoxFilterThen_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterThenTextMatch = null;
            if (comboBoxFilterThen.SelectedIndex >= 0)
            {
                KeyValuePair<long, string> item = (KeyValuePair<long, string>)comboBoxFilterThen.SelectedItem;
                filterThenEANMatch = item.Key;
            }
            Application.UseWaitCursor = true; Application.DoEvents(); // Cursor = Cursors.WaitCursor;
            signalUpdateDataGridView.Set();
        }

        private void buttonBrowseFileNameExcludeItems_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Csv files (*.csv)|*.csv";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxFileNameExcludeItems.Text = openFileDialog1.FileName;
                fileNameExcludeItems = textBoxFileNameExcludeItems.Text;

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                if (config.AppSettings.Settings["ExcludeItemsFileName"] != null)
                    config.AppSettings.Settings.Remove("ExcludeItemsFileName");
                config.AppSettings.Settings.Add("ExcludeItemsFileName", openFileDialog1.FileName);
                config.Save(ConfigurationSaveMode.Modified);
            }
        }

        private void dataGridViewResults_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            List<AssociationRule> view = dataGridViewResults.DataSource as List<AssociationRule>;
            if (view != null)
            {
                SortOrder sortOrder = dataGridViewResults.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection;
                if (sortOrder == SortOrder.Ascending)
                    sortOrder = SortOrder.Descending;
                else if (sortOrder == SortOrder.Descending)
                    sortOrder = SortOrder.Ascending;
                else sortOrder = SortOrder.Ascending;
                if(sortOrder == SortOrder.Ascending)
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            view.Sort((x, y) => x.Condition1.ToString().CompareTo(y.Condition1.ToString()));
                            break;
                        case 1:
                            view.Sort((x, y) => x.Condition2.ToString().CompareTo(y.Condition2.ToString()));
                            break;
                        case 2:
                            view.Sort((x, y) => x.Condition3.ToString().CompareTo(y.Condition3.ToString()));
                            break;
                        case 3:
                            view.Sort((x, y) => x.Condition4.ToString().CompareTo(y.Condition4.ToString()));
                            break;
                        case 4:
                            view.Sort((x, y) => x.Then.ToString().CompareTo(y.Then.ToString()));
                            break;
                        case 5:
                            view.Sort((x, y) => x.Confidence.CompareTo(y.Confidence));
                            break;
                        case 6:
                            view.Sort((x, y) => x.Lift.CompareTo(y.Lift));
                            break;
                        case 7:
                            view.Sort((x, y) => x.Support.CompareTo(y.Support));
                            break;
                        default:
                            break;
                    }
                else
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            view.Sort((y, x) => x.Condition1.ToString().CompareTo(y.Condition1.ToString()));
                            break;
                        case 1:
                            view.Sort((y, x) => x.Condition2.ToString().CompareTo(y.Condition2.ToString()));
                            break;
                        case 2:
                            view.Sort((y, x) => x.Condition3.ToString().CompareTo(y.Condition3.ToString()));
                            break;
                        case 3:
                            view.Sort((y, x) => x.Condition4.ToString().CompareTo(y.Condition4.ToString()));
                            break;
                        case 4:
                            view.Sort((y, x) => x.Then.ToString().CompareTo(y.Then.ToString()));
                            break;
                        case 5:
                            view.Sort((y, x) => x.Confidence.CompareTo(y.Confidence));
                            break;
                        case 6:
                            view.Sort((y, x) => x.Lift.CompareTo(y.Lift));
                            break;
                        case 7:
                            view.Sort((y, x) => x.Support.CompareTo(y.Support));
                            break;
                        default:
                            break;
                    }
                for (int i = 0; i < dataGridViewResults.Columns.Count; i++)
                {
                    if (i == e.ColumnIndex)
                        dataGridViewResults.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = sortOrder;
                    else dataGridViewResults.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                }
                dataGridViewResults.Refresh();
                if (dataGridViewResults.SelectedRows.Count > 0)
                    dataGridViewResults.SelectedRows[0].Selected = false;
                if (dataGridViewResults.Rows.Count > 0)
                    dataGridViewResults.Rows[0].Selected = true;
            }
        }

        private void comboBoxRuleGroup_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dataGridViewResults.Rows.Count > 0)
            {
                AssociationRule rule = dataGridViewResults.CurrentRow.DataBoundItem as AssociationRule;
                if (rule != null)
                {
                    AssociationRule.GroupIndexPair group = (sender as ComboBox).SelectedItem as AssociationRule.GroupIndexPair;
                    if (!group.Group.Equals(rule.Then))
                    {
                        dataGridViewResults.CurrentRow.Selected = false;
                        dataGridViewResults.FirstDisplayedScrollingRowIndex = group.Index;
                        dataGridViewResults.Rows[group.Index].Selected = true;
                        dataGridViewResults.CurrentCell = dataGridViewResults.Rows[group.Index].Cells[0];
                    }
                }
            }
        }

        private void comboBoxRuleItem_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dataGridViewResults.Rows.Count > 0)
            {
                AssociationRule rule = dataGridViewResults.CurrentRow.DataBoundItem as AssociationRule;
                if (rule != null)
                {
                    AssociationRule.ItemIndexPair item = (sender as ComboBox).SelectedItem as AssociationRule.ItemIndexPair;
                    if (!item.Item.Equals(rule.Then))
                    {
                        dataGridViewResults.CurrentRow.Selected = false;
                        dataGridViewResults.FirstDisplayedScrollingRowIndex = item.Index;
                        dataGridViewResults.Rows[item.Index].Selected = true;
                        dataGridViewResults.CurrentCell = dataGridViewResults.Rows[item.Index].Cells[0];
                    }
                }
            }
        }

        private void groupBoxFilters_Resize(object sender, EventArgs e)
        {
            int width = (groupBoxFilterConfidence.Left - groupBoxFilterCondition.Left - 12) / 2;
            groupBoxFilterCondition.Width = width;
            groupBoxFilterThen.Width = width;
            groupBoxFilterThen.Left = groupBoxFilterCondition.Left + width + 6;
        }

        private void buttonFilterReset_Click(object sender, EventArgs e)
        {
            trackBarMaxSupport.Value = 100;
            filterMaxSupport = 1.0000;
            trackBarMinSupport.Value = 25;
            filterMinSupport = 0.0010;

            trackBarMaxLift.Value = 100;
            filterMaxLift = 1000.0;
            trackBarMinLift.Value = 1;
            filterMinLift = 1.0;
            
            trackBarMaxConfidence.Value = 100;
            filterMaxConfidence = 1.00;
            trackBarMinConfidence.Value = 5;
            filterMinConfidence = 0.05;

            filterConditionLevel1 = new HashSet<int>();
            filterThenLevel1 = new HashSet<int>();

            trackBarConditionItemMaxSupport.Value = 100;
            filterConditionItemMaxSupport = 1.00;
            trackBarThenItemMaxSupport.Value = 100;
            filterThenItemMaxSupport = 1.00;

            comboBoxFilterCondition.SelectedItem = null;
            comboBoxFilterCondition.Text = "";
            filterConditionTextMatch = null;
            filterConditionEANMatch = 0;
            
            comboBoxFilterThen.SelectedItem = null;
            comboBoxFilterThen.Text = "";
            filterThenTextMatch = null;
            filterThenEANMatch = 0;

            InitFilters();

            signalUpdateDataGridView.Set();        
        }

        private void buttonExperiment_Click(object sender, EventArgs e)
        {
            InitStaticTables();

            buttonExperiment.Enabled = false;
            Thread thread = new Thread(new ThreadStart(ExperimentThread));
            thread.Name = "ExperimentThread";
            thread.Start();


        }

        private void ExperimentThread()
        {
            Stopwatch stopWatch;
            int nrTransactions = 0;
            int nrFrequentItemsets = 0;
            string fileName = textBoxFileNameExperimentLog.Text;
            fileName = @"C:\Temp\count-item-sets-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            StreamWriter writer = new StreamWriter(fileName);

            for (int type = 2; type < 4; type++)
            {
                Invoke((Action)(() =>
                {
                    String text = "";
                    switch (type)
                    {
                        case 0:
                            text = "Sequential Frequent Itemset Generator\n";
                            break;
                        case 1:
                            text = "Parallel Frequent Itemset Generator\n";
                            break;
                        case 2:
                            text = "Parallel Transaction Frequent Itemset Generator\n";
                            break;
                        case 3:
                            text = "GPU Transaction Frequent Itemset Generator\n";
                            break;
                    }
                    textBoxExperimentLog.Text += text + "\r\n";
                    writer.WriteLine(text);
                    text = "Transactions;PruningSupport;FrequentItemset;Time";
                    textBoxExperimentLog.Text += text + "\r\n";
                    writer.WriteLine(text);
                }));

                foreach (double minSupport in new double[] { 0.01, 0.005, /*0.0010, 0.0005*/ })
                {
                    for (int maxTransactions = 20000; maxTransactions <= 100000; maxTransactions += 20000)
                    {
                        switch (type)
                        {
                            case 0:
                                generator = new SequentialFrequentItemsetGenerator();
                                break;
                            case 1:
                                generator = new ParallelFrequentItemsetGenerator();
                                break;
                            case 2:
                                generator = new ParallelTransactionFrequentItemsetGenerator();
                                break;
                            case 3:
                                generator = new GPUTransactionFrequentItemsetGenerator();
                                break;
                        }
                        generator.SetPruningMinSupport(minSupport);
                        generator.SetMaxNrTransactions(maxTransactions);
                        Thread thread = new Thread(new ThreadStart(buttonStart_Click_Surveillance));
                        thread.Name = "SurveillanceGeneratorThread";
                        thread.Start();
                        generator.Generate(textBoxFileName.Text);
                        thread.Abort();
                        stopWatch = generator.GetStopWatch();
                        nrTransactions = generator.GetTransactionCount();
                        nrFrequentItemsets = generator.Level2.Count + generator.Level3.Count + generator.Level4.Count + generator.Level5.Count;
                        try
                        {
                            Invoke((Action)(() =>
                            {
                                String text = nrTransactions + ";" + minSupport + ";" + nrFrequentItemsets + ";" + stopWatch.ElapsedMilliseconds;
                                textBoxExperimentLog.Text += text + "\r\n";
                                writer.WriteLine(text);
                            }));
                        }
                        catch (Exception) { }
                    }
                }
            }
            writer.Close();
        }

        private void comboBoxCPUorGPU_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = (string)comboBoxCPUorGPU.Items[comboBoxCPUorGPU.SelectedIndex];
            if (selected == "On CPU")
            {
                generator = new ParallelTransactionFrequentItemsetGenerator();
            }
            else
            {
                generator = new GPUTransactionFrequentItemsetGenerator();
            }
        }
    }

}
