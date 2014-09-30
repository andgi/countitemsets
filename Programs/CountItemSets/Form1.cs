using System;
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
            config.AppSettings.Settings.Add("TransactionFileName", openFileDialog1.FileName);
            config.Save(ConfigurationSaveMode.Modified);
        }

        Dictionary<long, string> dictionaryEAN = new Dictionary<long, string>();
        Dictionary<int, string> dictionaryVGR = new Dictionary<int, string>();
        Dictionary<long, int> dictionaryEANtoVGR = new Dictionary<long, int>();
        Dictionary<string, double> dictionaryRule = new Dictionary<string, double>();

        Dictionary<long, int> dictionaryLevel1 = new Dictionary<long, int>();
        Dictionary<string, int> dictionaryLevel2 = new Dictionary<string, int>();
        Dictionary<string, int> dictionaryLevel3 = new Dictionary<string, int>();

        int transactionCount = 0;

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
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                };
            }
            textBoxTransactionCount.Text = transactionCount.ToString();
            dictionaryLevel1 = dictionaryLevel1.Where(item => (transactionCount/item.Value) <= 1000 && item.Key != 1 && item.Key != 2).ToDictionary(item => item.Key, item => item.Value);


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
                            for (int i = 0; i < (keys.Count-1); i++)
                            {
                                for (int j = i + 1; j < keys.Count; j++)
                                {
                                    long key1 = keys[i];
                                    long key2 = keys[j];
                                    if (dictionaryLevel1.ContainsKey(key1) && dictionaryLevel1.ContainsKey(key2) && key1 != key2)
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
            dictionaryLevel2 = dictionaryLevel2.Where(item => (transactionCount / item.Value) <= 1000).ToDictionary(item => item.Key, item => item.Value);

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
                            for (int i = 0; i < (keys.Count - 1); i++)
                            {
                                for (int j = i + 1; j < keys.Count; j++)
                                {
                                    for (int k = j + 1; k < keys.Count; k++)
                                    {
                                        long key1 = keys[i];
                                        long key2 = keys[j];
                                        long key3 = keys[k];
                                        if (dictionaryLevel1.ContainsKey(key1) && dictionaryLevel1.ContainsKey(key2) && dictionaryLevel1.ContainsKey(key3)
                                            && dictionaryLevel2.ContainsKey(key1 + "," + key2) && dictionaryLevel2.ContainsKey(key2 + "," + key3) && dictionaryLevel2.ContainsKey(key1 + "," + key3))
                                        {
                                            string keyName = key1 + "," + key2 + "," + key3;
                                            if (dictionaryLevel3.ContainsKey(keyName))
                                                dictionaryLevel3[keyName]++;
                                            else
                                                dictionaryLevel3.Add(keyName, 1);
                                        }
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

            dictionaryRule.Clear();
            foreach (KeyValuePair<string, int> pair in dictionaryLevel3)
            {
                String[] columns = pair.Key.Split(',');
                long eanNr1 = 0;
                Int64.TryParse(columns[0], out eanNr1);
                long eanNr2 = 0;
                Int64.TryParse(columns[1], out eanNr2);
                long eanNr3 = 0;
                Int64.TryParse(columns[1], out eanNr3);
                double value = (double)pair.Value / (double)dictionaryLevel2[eanNr1+","+eanNr2];
                dictionaryRule.Add(pair.Key, value);
            }




            /*
            dictionary2 = dictionary2.Where(item => item.Value >= 1000).ToDictionary(item => TranslateEANpairs(item.Key), item => item.Value);
            List<KeyValuePair<string,int>> results = dictionary2.OrderByDescending(item => item.Value).ToList();
             */
            List<KeyValuePair<string, double>> results = dictionaryRule.Where(item => item.Value >= 0.05).ToDictionary(item => TranslateEANpairs(item.Key), item => item.Value).OrderByDescending(item => item.Value).ToList();
            dataGridViewResults.DataSource = results;
            buttonStart.Enabled = true;

        }

        private string TranslateEANpairs(string textPair)
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

    }
}
