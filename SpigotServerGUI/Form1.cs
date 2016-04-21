﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace SpigotServerGUI
{

    public partial class Form1 : Form
    {

        public int Xms;
        public int Xmx;
        public bool agreeEula;

        public string versionPath = Directory.GetCurrentDirectory() + @"\ServerVersions";
        public string jarPath = "";
        public string jarName = "";
        public string bufferLine;
        public string[] bufferArray;

        public const string MyJarPath = @"X:\Programming\Compile\Spigot 1.9\craftbukkit-1.9.jar";
        public const string version = "0.8.0";

        public int lineCount = 0;

        public ProcessStartInfo javaStartInfo;
        public Process javaProcess;

        public StreamWriter StrWr;
        public StreamReader StrRd;

        public FileDialog jarFileDialog = new OpenFileDialog();


        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {


            label6.Text = "Version: " + version;

            if (!Directory.Exists(versionPath))
            {

                Directory.CreateDirectory(versionPath);

            }

            if (!File.Exists(Directory.GetCurrentDirectory() + @"\ssgui.properties"))
            {

                File.Create(Directory.GetCurrentDirectory() + @"\ssgui.properties");

            }

            jarFileDialog.Filter = "Jar File (*.jar)|*.jar;";
            reloadVersions();

            readSettings(2);

        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            
            writeSettings();
            javaProcess.Kill();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            Process.Start("https://account.mojang.com/documents/minecraft_eula");

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {

                javaProcess.StandardInput.WriteLine("say" + textBox2.Text);

            }

            javaProcess.StandardInput.WriteLine(textBox2.Text);

        }

        private void button2_Click(object sender, EventArgs e)
        {

            startServer();

        }

        private void button4_Click(object sender, EventArgs e)
        {

            javaProcess.Kill();
            startServer();

        }

        private void button5_Click(object sender, EventArgs e)
        {

            createVersion();

        }

        private void button6_Click(object sender, EventArgs e)
        {

            setJarVersion();

        }

        private void button7_Click(object sender, EventArgs e)
        {

            deleteSelectedVersion();
            reloadVersions();

        }

        private void button8_Click(object sender, EventArgs e)
        {

            writeSettings();

        }

        private void button9_Click(object sender, EventArgs e)
        {
            //readSetthings:
            //Mode - 0   Only load the programm settings
            //Mode - 1   Only load the current jar settings
            //Mode - 2   Load Both

            readSettings(1);

        }

        public void startServer()
        {

            textBox1.Text = null;
            tabControl1.SelectedIndex = 1;

            jarPath = versionPath + @"\" + jarName + @"\" + jarName + ".jar";

            javaStartInfo = new ProcessStartInfo("java", "-Xms" + numericUpDown2.Value.ToString() + "M " + "-Xmx" + numericUpDown1.Value.ToString() + "M -jar " + '"' + jarPath + '"' + " nogui");
            javaStartInfo.RedirectStandardOutput = true;
            javaStartInfo.RedirectStandardInput = true;
            javaStartInfo.UseShellExecute = false;
            javaStartInfo.WorkingDirectory = Path.GetDirectoryName(jarPath);
            javaStartInfo.CreateNoWindow = true;
            javaProcess = Process.Start(javaStartInfo);

            javaProcess.OutputDataReceived += new DataReceivedEventHandler((consoleSender, consoleE) =>
            {
                // Prepend line numbers to each line of the output.
                if (!String.IsNullOrEmpty(consoleE.Data))
                {
                    this.Invoke((MethodInvoker)delegate { textBox1.AppendText(consoleE.Data + Environment.NewLine); });
                }
            });

            javaProcess.BeginOutputReadLine();

        }

        public void createVersion()
        {

            if (jarFileDialog.ShowDialog() == DialogResult.Cancel)
            {

                return;

            }

            if (!jarFileDialog.CheckFileExists)
            {

                MessageBox.Show("Couldn't find file", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            Directory.CreateDirectory(versionPath + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName));

            if (File.Exists(Path.GetDirectoryName(jarFileDialog.FileName) + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName) + @"\" + Path.GetFileName(jarFileDialog.FileName)))
            {

                File.Delete(Path.GetDirectoryName(jarFileDialog.FileName) + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName) + @"\" + Path.GetFileName(jarFileDialog.FileName));

            }

            Debug.Print(versionPath + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName) + @"\" + Path.GetFileName(jarFileDialog.FileName));
            File.Copy(jarFileDialog.FileName, versionPath + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName) + @"\" + Path.GetFileName(jarFileDialog.FileName)); /// + @"\" + Path.GetFileName(jarFileDialog.FileName)

            reloadVersions();
        }

        public void reloadVersions()
        {

            listBox1.Items.Clear();

            foreach (string directory in Directory.GetDirectories(versionPath))
            {

                listBox1.Items.Add(directory.Replace(versionPath + @"\", ""));

            }

        }

        public void deleteSelectedVersion()
        {

            Directory.Delete(versionPath + @"\" + listBox1.GetItemText(listBox1.SelectedItem), true);

        }

        public void setJarVersion()
        {

            jarName = listBox1.GetItemText(listBox1.SelectedItem);
            label5.Text = "Current Version: " + jarName;

        }

        public void writeSettings()
        {

            if (jarName == "")
            {

                MessageBox.Show("No Version Seleted", "No Version", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;

            }

            StrWr = new StreamWriter(Directory.GetCurrentDirectory() + @"\ssgui.properties", false);

            StrWr.WriteLine("usingVersion=" + jarName);

            StrWr.Flush();
            StrWr.Close();

            StrWr = new StreamWriter(versionPath + @"\" + jarName + @"\" + "ssguiv.properties", false);

            Xms = Convert.ToInt32(numericUpDown2.Value);
            Xmx = Convert.ToInt32(numericUpDown1.Value);
            agreeEula = checkBox2.Checked;

            StrWr.WriteLine("Xms=" + Xms + Environment.NewLine + "Xmx=" + Xmx + Environment.NewLine + "agreeEULA=" + agreeEula);

            StrWr.Flush();
            StrWr.Close();

        }

        public void readSettings(int mode)
        {

            //readSetthings:
            //Mode - 0   Only load the programm settings
            //Mode - 1   Only load the current jar settings
            //Mode - 2   Load Both

            if (mode == 0 || mode == 2)
            {

                StrRd = new StreamReader(Directory.GetCurrentDirectory() + @"\ssgui.properties");

                while ((bufferLine = StrRd.ReadLine()) != null)
                {

                    bufferArray = bufferLine.Split(Convert.ToChar("="));

                    try
                    {

                        switch (bufferArray[0])
                        {
                            case "usingVersion":
                                jarName = bufferArray[1];
                                break;
                                //select case for more settings later
                        }

                        label5.Text = "Current Version: " + jarName; 

                    }
                    catch (Exception e)
                    {

                        MessageBox.Show(e.Message + Environment.NewLine + "Please Contact Support", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                }

                StrRd.Close();

            }

            if (mode == 1 || mode == 2)
            {

                StrRd = new StreamReader(versionPath + @"\" + jarName + @"\ssguiv.properties");

                while ((bufferLine = StrRd.ReadLine()) != null)
                {

                    bufferArray = bufferLine.Split(Convert.ToChar("="));

                    try
                    {

                        switch (bufferArray[0])
                        {
                            case "xmx":
                                Xmx = Convert.ToUInt16(bufferArray[1]);
                                break;
                            case "xms":
                                Xms = Convert.ToUInt16(bufferArray[1]);
                                break;
                            case "agreeEULA=":
                                agreeEula = Convert.ToBoolean(bufferArray[1]);
                                break;

                        }

                    }
                    catch (Exception e)
                    {

                        MessageBox.Show(e.Message + Environment.NewLine + "Please Contact Support", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                }

                StrRd.Close();

            }

        }

    }
}
