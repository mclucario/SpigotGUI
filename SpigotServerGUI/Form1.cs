using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace SpigotServerGUI
{


    public partial class Form1 : Form
    {

        public const string MyJarPath = @"X:\Programming\Compile\Spigot 1.9\craftbukkit-1.9.jar";
        public const string version = "1.0.0";
        public string jarPath = "";
        public string jarName = "";

        public int lineCount = 0;

        public string versionPath = Directory.GetCurrentDirectory() + @"\ServerVersions";
        
        public ProcessStartInfo javaStartInfo;
        public Process javaProcess;

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

            jarFileDialog.Filter = "Jar File (*.jar)|*.jar;";
            reloadVersions();

        }

        private void Form1_Closing(object sender, EventArgs e)
        {

            javaProcess.Kill();

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

        public void startServer()
        {

            textBox1.Text = null;

            jarPath = versionPath + @"\" + jarName + @"\" + jarName + ".jar";

            javaStartInfo = new ProcessStartInfo("java","-Xms" + numericUpDown2.Value.ToString() + "M " + "-Xmx" + numericUpDown1.Value.ToString() + "M -jar " + '"' + jarPath + '"' + " nogui");
            javaStartInfo.RedirectStandardOutput = true;
            javaStartInfo.RedirectStandardInput = true;
            javaStartInfo.UseShellExecute = false;
            javaStartInfo.WorkingDirectory = Path.GetDirectoryName(jarPath);
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

        public void createVersion() {

            if (jarFileDialog.ShowDialog() == DialogResult.Cancel) {

                return;

            }

            if (!jarFileDialog.CheckFileExists) {

                MessageBox.Show("Couldn't find file", "File missing" , MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            Directory.CreateDirectory(versionPath + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName));

            if (File.Exists(Path.GetDirectoryName(jarFileDialog.FileName) + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName) + @"\" + Path.GetFileName(jarFileDialog.FileName))) {

                File.Delete(Path.GetDirectoryName(jarFileDialog.FileName) + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName) + @"\" + Path.GetFileName(jarFileDialog.FileName));

            }

            Debug.Print(versionPath + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName) + @"\" + Path.GetFileName(jarFileDialog.FileName));
            File.Copy(jarFileDialog.FileName, versionPath + @"\" + Path.GetFileNameWithoutExtension(jarFileDialog.FileName) + @"\" + Path.GetFileName(jarFileDialog.FileName)); /// + @"\" + Path.GetFileName(jarFileDialog.FileName)

            reloadVersions();
        }

        public void reloadVersions() {

            listBox1.Items.Clear();

            foreach (string directory in Directory.GetDirectories(versionPath)) {

                listBox1.Items.Add(directory.Replace(versionPath + @"\", ""));

            }

        }

        public void setJarVersion() {

            jarName = listBox1.GetItemText(listBox1.SelectedItem);
            label5.Text = "Current Version: " + jarName;

        }

    }
}
