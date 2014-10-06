﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;
using System.Diagnostics;

namespace CountItemSets
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            try
            {
                textBoxFileName.Text = ConfigurationManager.AppSettings["TransactionFileName"];
            }
            catch (Exception) { }
        }

        private void browseButton1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBoxFileName.Text = openFileDialog1.FileName;

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            if (config.AppSettings.Settings["TransactionFileName"] != null)
                config.AppSettings.Settings.Remove("TransactionFileName");
            config.AppSettings.Settings.Add("TransactionFileName", openFileDialog1.FileName);
            config.Save(ConfigurationSaveMode.Modified);
        }

        static Dictionary<long, string> dictionaryEAN = new Dictionary<long, string>();
        static Dictionary<int, string> dictionaryVGR = new Dictionary<int, string>();
        static Dictionary<long, int> dictionaryEANtoVGR = new Dictionary<long, int>();
        static Dictionary<string, double> dictionaryRule = new Dictionary<string, double>();

        Dictionary<long, int> dictionaryLevel1 = new Dictionary<long, int>();
        Dictionary<string, int> dictionaryLevel2 = new Dictionary<string, int>();
        Dictionary<string, int> dictionaryLevel3 = new Dictionary<string, int>();
        Dictionary<string, int> dictionaryLevel4 = new Dictionary<string, int>();
        Dictionary<string, int> dictionaryLevel5 = new Dictionary<string, int>();

        int transactionCount = 0;

        private void CountItemSets()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            StreamReader reader = new StreamReader(textBoxFileName.Text);
            int rowCount = 0;
            int transNrLast = 0;

            dictionaryLevel1.Clear();
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                            transactionCount++;
                        }
                        if (dictionaryLevel1.ContainsKey(eanNr))
                            dictionaryLevel1[eanNr]++;
                        else
                            dictionaryLevel1.Add(eanNr, 1);
                        if (dictionaryLevel1.ContainsKey(-vgrNr))
                            dictionaryLevel1[-vgrNr]++;
                        else
                            dictionaryLevel1.Add(-vgrNr, 1);
                        if (transNrLast != transNr)
                        {
                            transNrLast = transNr;
                            transactionCount++;
                        }
                        if (!dictionaryEANtoVGR.ContainsKey(eanNr))
                            dictionaryEANtoVGR.Add(eanNr, vgrNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            textBoxTransactionCount.Text = transactionCount.ToString();
            dictionaryLevel1 = dictionaryLevel1.Where(item => (transactionCount / item.Value) <= 1000 && item.Key != 1 && item.Key != 2).ToDictionary(item => item.Key, item => item.Value);


            dictionaryLevel2.Clear();
            reader.Close();
            reader = new StreamReader(textBoxFileName.Text);
            transNrLast = 0;
            rowCount = 0;
            List<long> keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 1)
                                for (int i = 0; i < (keys.Count - 1); i++)
                                {
                                    long key1 = keys[i];
                                    if (dictionaryLevel1.ContainsKey(key1))
                                        for (int j = i + 1; j < keys.Count; j++)
                                        {
                                            long key2 = keys[j];
                                            if (dictionaryLevel1.ContainsKey(key2))
                                            {
                                                string keyName = key1 + "," + key2;
                                                if (dictionaryLevel2.ContainsKey(keyName))
                                                    dictionaryLevel2[keyName]++;
                                                else
                                                    dictionaryLevel2.Add(keyName, 1);
                                            }
                                        }
                                }
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            reader.Close();
            reader = new StreamReader(textBoxFileName.Text);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 1)
                                for (int i = 0; i < (keys.Count - 1); i++)
                                {
                                    long key1 = keys[i];
                                    if (dictionaryLevel1.ContainsKey(key1))
                                        for (int j = i + 1; j < keys.Count; j++)
                                        {
                                            long key2 = keys[j];
                                            if (dictionaryLevel1.ContainsKey(key2))
                                            {
                                                string keyName = key1 + "," + key2;
                                                if (dictionaryLevel2.ContainsKey(keyName))
                                                    dictionaryLevel2[keyName]++;
                                                else
                                                    dictionaryLevel2.Add(keyName, 1);
                                            }
                                        }
                                }
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(-vgrNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            dictionaryLevel2 = dictionaryLevel2.Where(item => (transactionCount / item.Value) <= 2000).ToDictionary(item => item.Key, item => item.Value);

            dictionaryLevel3.Clear();
            reader.Close();
            reader = new StreamReader(textBoxFileName.Text);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 2)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 2];
                                Parallel.For(0, keys.Count - 2, i =>
                                {
                                    keyNames[i] = new List<string>();
                                    long key1 = keys[i];
                                    if (dictionaryLevel1.ContainsKey(key1))
                                        for (int j = i + 1; j < (keys.Count - 1); j++)
                                        {
                                            long key2 = keys[j];
                                            if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                                for (int k = j + 1; k < keys.Count; k++)
                                                {
                                                    long key3 = keys[k];
                                                    if (dictionaryLevel2.ContainsKey(key2 + "," + key3) && dictionaryLevel2.ContainsKey(key1 + "," + key3))
                                                    {
                                                        string keyName = key1 + "," + key2 + "," + key3;
                                                        keyNames[i].Add(keyName);
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 2); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel3.ContainsKey(keyName))
                                            dictionaryLevel3[keyName]++;
                                        else
                                            dictionaryLevel3.Add(keyName, 1);
                                    }
                                }
                            }
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            dictionaryLevel3 = dictionaryLevel3.Where(item => (transactionCount / item.Value) <= 4000).ToDictionary(item => item.Key, item => item.Value);

            dictionaryLevel4.Clear();
            reader.Close();
            reader = new StreamReader(textBoxFileName.Text);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 3)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 3];
                                Parallel.For(0, keys.Count - 3, i =>
                                {
                                    keyNames[i] = new List<string>();
                                    long key1 = keys[i];
                                    if (dictionaryLevel1.ContainsKey(key1))
                                        for (int j = i + 1; j < (keys.Count - 2); j++)
                                        {
                                            long key2 = keys[j];
                                            if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                                for (int k = j + 1; k < (keys.Count-1); k++)
                                                {
                                                    long key3 = keys[k];
                                                    if (dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key3)) {
                                                        for (int l = j + 1; l < keys.Count; l++)
                                                        {
                                                            long key4 = keys[l];
                                                            if (dictionaryLevel3.ContainsKey(key2 + "," + key3 + "," + key4) && dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key4) && dictionaryLevel3.ContainsKey(key1 + "," + key3 + "," + key4))
                                                            {
                                                                string keyName = key1 + "," + key2 + "," + key3 + "," + key4;
                                                                keyNames[i].Add(keyName);
                                                            }
                                                        }
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 3); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel4.ContainsKey(keyName))
                                            dictionaryLevel4[keyName]++;
                                        else
                                            dictionaryLevel4.Add(keyName, 1);
                                    }
                                }
                            }
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            dictionaryLevel4 = dictionaryLevel4.Where(item => (transactionCount / item.Value) <= 8000).ToDictionary(item => item.Key, item => item.Value);

            dictionaryLevel5.Clear();
            reader.Close();
            reader = new StreamReader(textBoxFileName.Text);
            transNrLast = 0;
            rowCount = 0;
            keys = new List<long>(10);
            while (!reader.EndOfStream)
            {
                try
                {
                    String line = reader.ReadLine();
                    String[] columns = line.Split(';');
                    if (rowCount > 0)
                    {
                        int transNr = Int32.Parse(columns[0]);
                        long eanNr = Int64.Parse(columns[1]);
                        int vgrNr = Int32.Parse(columns[2]);
                        if (transNrLast == 0)
                        {
                            transNrLast = transNr;
                        }
                        if (transNrLast != transNr)
                        {
                            keys.Sort();
                            keys = new List<long>(keys.Distinct());
                            if (keys.Count > 4)
                            {
                                List<string>[] keyNames = new List<string>[keys.Count - 4];
                                Parallel.For(0, keys.Count - 4, i =>
                                {
                                    keyNames[i] = new List<string>();
                                    long key1 = keys[i];
                                    if (dictionaryLevel1.ContainsKey(key1))
                                        for (int j = i + 1; j < (keys.Count - 3); j++)
                                        {
                                            long key2 = keys[j];
                                            if (dictionaryLevel2.ContainsKey(key1 + "," + key2))
                                                for (int k = j + 1; k < (keys.Count - 2); k++)
                                                {
                                                    long key3 = keys[k];
                                                    if (dictionaryLevel3.ContainsKey(key1 + "," + key2 + "," + key3))
                                                    {
                                                        for (int l = j + 1; l < (keys.Count-1); l++)
                                                        {
                                                            long key4 = keys[l];
                                                            if (dictionaryLevel4.ContainsKey(key1 + "," + key2 + "," + key3 + "," + key4))
                                                            {
                                                                for (int m = l + 1; m < (keys.Count); m++)
                                                                {
                                                                    long key5 = keys[m];
                                                                    if (dictionaryLevel4.ContainsKey(key2 + "," + key3 + "," + key4 + "," + key5) && dictionaryLevel4.ContainsKey(key1 + "," + key3 + "," + key4 + "," + key5) && dictionaryLevel4.ContainsKey(key1 + "," + key2 + "," + key4 + "," + key5) && dictionaryLevel4.ContainsKey(key1 + "," + key2 + "," + key3 + "," + key5))
                                                                    {
                                                                        string keyName = key1 + "," + key2 + "," + key3 + "," + key4 + "," + key5;
                                                                        keyNames[i].Add(keyName);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                        }
                                }); //Parallel.For
                                for (int i = 0; i < (keys.Count - 4); i++)
                                {
                                    foreach (string keyName in keyNames[i])
                                    {
                                        if (dictionaryLevel5.ContainsKey(keyName))
                                            dictionaryLevel5[keyName]++;
                                        else
                                            dictionaryLevel5.Add(keyName, 1);
                                    }
                                }
                            }
                            keys.Clear();
                            transNrLast = transNr;
                        }
                        keys.Add(eanNr);
                    }
                    rowCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };
            }
            dictionaryLevel5 = dictionaryLevel5.Where(item => (transactionCount / item.Value) <= 8000).ToDictionary(item => item.Key, item => item.Value);

            stopwatch.Stop();
            textBoxTime.Text = stopwatch.Elapsed.ToString();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;

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
                config.AppSettings.Settings.Add("EANTable",eanPath);
                config.Save(ConfigurationSaveMode.Modified);
            }
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

            CountItemSets();

            /*
            dictionaryRule.Clear();
            foreach (KeyValuePair<string, int> pair in dictionaryLevel2)
            {
                String[] columns = pair.Key.Split(',');
                long eanNr1 = 0;
                Int64.TryParse(columns[0], out eanNr1);
                long eanNr2 = 0;
                Int64.TryParse(columns[1], out eanNr2);
                double value = (double)pair.Value / (double)dictionaryLevel1[eanNr1];
                dictionaryRule.Add(pair.Key, value);
            }
            */

            /*
            dictionaryRule.Clear();
            foreach (KeyValuePair<string, int> pair in dictionaryLevel2)
            {
                String[] columns = pair.Key.Split(',');
                long eanNr1 = 0;
                Int64.TryParse(columns[0], out eanNr1);
                long eanNr2 = 0;
                Int64.TryParse(columns[1], out eanNr2);
                dictionaryRule.Add(eanNr1 + "," + eanNr2, ((double)pair.Value / (double)dictionaryLevel1[eanNr1]) / ((double)dictionaryLevel1[eanNr2] / (double)transactionCount));
                dictionaryRule.Add(eanNr2 + "," + eanNr1, ((double)pair.Value / (double)dictionaryLevel1[eanNr2]) / ((double)dictionaryLevel1[eanNr1] / (double)transactionCount));
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
                dictionaryRule.Add(eanNr1 + "," + eanNr2 + "," + eanNr3, (double)pair.Value / (double)dictionaryLevel2[eanNr1 + "," + eanNr2]);
                dictionaryRule.Add(eanNr1 + "," + eanNr3 + "," + eanNr2, (double)pair.Value / (double)dictionaryLevel2[eanNr1 + "," + eanNr3]);
                dictionaryRule.Add(eanNr2 + "," + eanNr3 + "," + eanNr1, (double)pair.Value / (double)dictionaryLevel2[eanNr2 + "," + eanNr3]);
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
                dictionaryRule.Add(eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr4, (double)pair.Value / (double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr3]);
                dictionaryRule.Add(eanNr1 + "," + eanNr2 + "," + eanNr4 + "," + eanNr3, (double)pair.Value / (double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr4]);
                dictionaryRule.Add(eanNr1 + "," + eanNr3 + "," + eanNr4 + "," + eanNr2, (double)pair.Value / (double)dictionaryLevel3[eanNr1 + "," + eanNr3 + "," + eanNr4]);
                dictionaryRule.Add(eanNr2 + "," + eanNr3 + "," + eanNr4 + "," + eanNr1, (double)pair.Value / (double)dictionaryLevel3[eanNr2 + "," + eanNr3 + "," + eanNr4]);
            }

            List<KeyValuePair<string, double>> results = dictionaryRule.Where(item => item.Value >= 0.05).ToDictionary(item => TranslateEANpairs(item.Key), item => item.Value).OrderByDescending(item => item.Value).ToList();
            */

            List<AssociationRule> results = new List<AssociationRule>();
            foreach (KeyValuePair<string, int> pair in dictionaryLevel2)
            {
                String[] columns = pair.Key.Split(',');
                long eanNr1 = 0;
                Int64.TryParse(columns[0], out eanNr1);
                long eanNr2 = 0;
                Int64.TryParse(columns[1], out eanNr2);
                results.Add(new AssociationRule(eanNr1, 0, 0, 0, eanNr2, (double)pair.Value / (double)dictionaryLevel1[eanNr1], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel1[eanNr1] * (double)dictionaryLevel1[eanNr2]), (double)dictionaryLevel1[eanNr1] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr2, 0, 0, 0, eanNr1, (double)pair.Value / (double)dictionaryLevel1[eanNr2], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel1[eanNr2] * (double)dictionaryLevel1[eanNr1]), (double)dictionaryLevel1[eanNr2] / (double)transactionCount));
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
                results.Add(new AssociationRule(eanNr1, eanNr2, 0, 0, eanNr3, (double)pair.Value / (double)dictionaryLevel2[eanNr1 + "," + eanNr2], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel2[eanNr1 + "," + eanNr2] * (double)dictionaryLevel1[eanNr3]), (double)dictionaryLevel2[eanNr1 + "," + eanNr2] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr3, 0, 0, eanNr2, (double)pair.Value / (double)dictionaryLevel2[eanNr1 + "," + eanNr3], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel2[eanNr1 + "," + eanNr3] * (double)dictionaryLevel1[eanNr2]), (double)dictionaryLevel2[eanNr1 + "," + eanNr3] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr2, eanNr3, 0, 0, eanNr1, (double)pair.Value / (double)dictionaryLevel2[eanNr2 + "," + eanNr3], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel2[eanNr2 + "," + eanNr3] * (double)dictionaryLevel1[eanNr1]), (double)dictionaryLevel2[eanNr2 + "," + eanNr3] / (double)transactionCount));
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
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr3, 0, eanNr4, (double)pair.Value / (double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr3], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr3] * (double)dictionaryLevel1[eanNr4]), (double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr3] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr4, 0, eanNr3, (double)pair.Value / (double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr4], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr4] * (double)dictionaryLevel1[eanNr3]), (double)dictionaryLevel3[eanNr1 + "," + eanNr2 + "," + eanNr4] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr3, eanNr4, 0, eanNr2, (double)pair.Value / (double)dictionaryLevel3[eanNr1 + "," + eanNr3 + "," + eanNr4], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel3[eanNr1 + "," + eanNr3 + "," + eanNr4] * (double)dictionaryLevel1[eanNr2]), (double)dictionaryLevel3[eanNr1 + "," + eanNr3 + "," + eanNr4] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr2, eanNr3, eanNr4, 0, eanNr1, (double)pair.Value / (double)dictionaryLevel3[eanNr2 + "," + eanNr3 + "," + eanNr4], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel3[eanNr2 + "," + eanNr3 + "," + eanNr4] * (double)dictionaryLevel1[eanNr1]), (double)dictionaryLevel3[eanNr2 + "," + eanNr3 + "," + eanNr4] / (double)transactionCount));
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
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr3, eanNr4, eanNr5, (double)pair.Value / (double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr4], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr4] * (double)dictionaryLevel1[eanNr5]), (double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr4] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr3, eanNr5, eanNr4, (double)pair.Value / (double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr5], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr5] * (double)dictionaryLevel1[eanNr4]), (double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr3 + "," + eanNr5] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr2, eanNr4, eanNr5, eanNr3, (double)pair.Value / (double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr4 + "," + eanNr5], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr4 + "," + eanNr5] * (double)dictionaryLevel1[eanNr3]), (double)dictionaryLevel4[eanNr1 + "," + eanNr2 + "," + eanNr4 + "," + eanNr5] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr1, eanNr3, eanNr4, eanNr5, eanNr2, (double)pair.Value / (double)dictionaryLevel4[eanNr1 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr1 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5] * (double)dictionaryLevel1[eanNr2]), (double)dictionaryLevel4[eanNr1 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5] / (double)transactionCount));
                results.Add(new AssociationRule(eanNr2, eanNr3, eanNr4, eanNr5, eanNr1, (double)pair.Value / (double)dictionaryLevel4[eanNr2 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5], ((double)pair.Value * (double)transactionCount) / ((double)dictionaryLevel4[eanNr2 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5] * (double)dictionaryLevel1[eanNr1]), (double)dictionaryLevel4[eanNr2 + "," + eanNr3 + "," + eanNr4 + "," + eanNr5] / (double)transactionCount));
            }

            results = new List<AssociationRule>(results.Where(item => item.Confidence >= 0.05 && item.Lift >= 1.0).OrderByDescending(item => item.Lift).OrderBy(item => item.Then.ToString()));

            dataGridViewResults.DataSource = results;
            buttonStart.Enabled = true;

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

            public AssociationRule(long condition1, long condition2, long condition3, long condition4, long then, double confidence, double lift, double support)
            {
                Condition1 = new TransactionItem(condition1);
                Condition2 = new TransactionItem(condition2);
                Condition3 = new TransactionItem(condition3);
                Condition4 = new TransactionItem(condition4);
                Then = new TransactionItem(then);
                Confidence = confidence;
                Lift = lift;
                Support = support;
            }

            public class TransactionItem: IComparable
            {
                public long EANCode { get; set; }

                public string Text { get; set; }
                public TransactionItem(long eanCode)
                {
                    EANCode = eanCode;
                }

                public override string ToString()
                {
                    if (Text != null) return Text;
                    return Text = TranslateEANCode();
                }

                public string TranslateEANCode()
                {
                    if (EANCode == 0) return "";
                    string result;
                    if (EANCode < 0)
                    {
                        int vgrNr = (int) -EANCode;
                        if (dictionaryVGR.ContainsKey(vgrNr))
                            result = "Varugrupp " + dictionaryVGR[vgrNr] + "(" + vgrNr + ")";
                        else
                            result = "Varugrupp " + "(" + vgrNr + ")";

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
                                result = "Varugrupp " + vgrNr + " (" + EANCode + ")";
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
                        return this.EANCode.CompareTo(item.EANCode);
                    else 
                        throw new ArgumentException("Object is not a TransactionItem");
                }
                
            }

        }

    }
}
