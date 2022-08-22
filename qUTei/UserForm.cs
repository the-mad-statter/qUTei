using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace qUTei
{
    public partial class UserForm : Form
    {
        private static bool DEBUG = false;

        private AdminForm MyAdminForm;
        private PasswordForm MyPasswordForm;
        private ChoiceForm MyChoiceForm;

        public UserForm()
        {
            InitializeComponent();
            MyAdminForm = new AdminForm();
            MyAdminForm.Hide();
            MyPasswordForm = new PasswordForm();
            MyPasswordForm.Hide();
            MyChoiceForm = new ChoiceForm();
            MyChoiceForm.Hide();

            Thread th = new Thread(new ThreadStart(DoSplash));
            th.Start();
            Thread.Sleep(3000);
            th.Abort();
            Thread.Sleep(1000);
        }

        private void DoSplash()
        {
            Splash sp = new Splash();
            sp.ShowDialog();
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
        }

        private void administratorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MyPasswordForm.PasswordBox.Text = String.Empty;
            MyPasswordForm.ShowDialog();
            if (MyPasswordForm.PasswordBox.Text == Properties.Settings.Default.ADMIN_PASS)
                MyAdminForm.ShowDialog();

            if(MyAdminForm.MyProtocol != null)
                this.ToolStripStatusLabel.Text = MyAdminForm.MyProtocol.Games[MyAdminForm.GetCurrentGame()].FormName;
        }

        private void UserFormNextButton_Click(object sender, EventArgs e)
        {
            if (MyAdminForm.MyProtocol != null)
            {
                this.NextButton.Hide();
                new Thread(this.RunAGame).Start();
            }
        }

        private void RunAGame()
        {
            Game CurrentGame = MyAdminForm.MyProtocol.Games[MyAdminForm.GetCurrentGame()];

            if (CurrentGame.Password != String.Empty)
            {
                MyPasswordForm.PasswordBox.Text = String.Empty;
                MyPasswordForm.ShowDialog();
                if (MyPasswordForm.PasswordBox.Text != CurrentGame.Password)
                    this.UserFormNextButton_Click(new object(), new EventArgs());
                    //this.UserFormNextButton_Click(sender, e);
            }

            string CmdArguments;       // to pass to server
            string MapName;            // used for demorec
            string Difficulty = "U";   // used for demorec
            int choice;                // reused with each choice

            // Map Choice
            choice = 0;
            if (CurrentGame.MapChoice.CmdLineOptions.Count > 1)
                choice = MakeChoice(CurrentGame.MapChoice);
            MapName = CurrentGame.MapChoice.CmdLineOptions[choice].Value;
            CmdArguments = MapName;


            // Game Type Choice
            choice = 0;
            if (CurrentGame.TypeChoice.CmdLineOptions.Count > 1)
                choice = MakeChoice(CurrentGame.TypeChoice);
            CmdArguments = CmdArguments + "?" +
                CurrentGame.TypeChoice.CmdLineParamter +
                CurrentGame.TypeChoice.CmdLineOptions[choice].Value;

            // Additional Choices with special lookout for mutators
            List<string> MutatorSelections = new List<string>();
            foreach (qUTeiChoice c in CurrentGame.MoreChoices)
            {
                choice = 0;
                if (c.CmdLineOptions.Count > 1)
                    choice = MakeChoice(c);
                // make note of difficutly for possible demorec
                if (c.CmdLineParamter == "Difficulty=")
                    Difficulty = c.CmdLineOptions[choice].Value;
                // special handling of mutators
                if (c.CmdLineParamter == "Mutator=")
                    MutatorSelections.Add(c.CmdLineOptions[choice].Value);
                else
                {
                    CmdArguments = CmdArguments + "?" +
                        c.CmdLineParamter +
                        c.CmdLineOptions[choice].Value;
                }
            }
            string MutatorCodes = "?Mutator=";
            for (int i = 0; i < MutatorSelections.Count; i++)
            {
                if (i == 0)
                    MutatorCodes = MutatorCodes + MutatorSelections[i];
                else
                    MutatorCodes = MutatorCodes + "," + MutatorSelections[i];

                if (i == MutatorSelections.Count - 1)
                    CmdArguments = CmdArguments + MutatorCodes;
            }

            // DemoRec
            if (CurrentGame.DemoRec == true)
            {
                CmdArguments = CmdArguments + "?Demorec=" +
                    generateDemoRecName(MyAdminForm.IDBox.Text, MyAdminForm.MyProtocol.Condition, CurrentGame.FormName, MapName, Difficulty);
            }

            // Switches
            foreach (string s in CurrentGame.Switches)
            {
                CmdArguments = CmdArguments + " " + s;
            }

            if (!MyAdminForm.EndOfProtocol())
            {
                if (DEBUG)
                    Console.Out.WriteLine(CmdArguments);
                else
                    this.ExecuteGame(CmdArguments);

                MyAdminForm.IncrementCurrentGame();
                this.ToolStripStatusLabel.Text = MyAdminForm.MyProtocol.Games[MyAdminForm.GetCurrentGame()].FormName;
                this.NextButton.Show();
            }
            else
            {
                if (DEBUG)
                    Console.Out.WriteLine(CmdArguments);
                else
                    this.ExecuteGame(CmdArguments);

                this.ToolStripStatusLabel.Text = "End of Protocol";
            }
        }





        private int MakeChoice(qUTeiChoice c)
        {
            MyChoiceForm.InstructionsLabel.Text = c.FormInstructions;

            MyChoiceForm.ChoicesComboBox.Items.Clear();
            foreach (CmdLineOption clo in c.CmdLineOptions)
            {
                MyChoiceForm.ChoicesComboBox.Items.Add(clo.FormName);
            }
            MyChoiceForm.ChoicesComboBox.SelectedIndex = 0;

            MyChoiceForm.ShowDialog();

            return MyChoiceForm.ChoicesComboBox.SelectedIndex;
        }

        private String generateDemoRecName(string id, string condition, string game, string map, string diff)
        {
            DateTime d = DateTime.Now;
            // ensure demorec name will be file system compatible
            char[] InvalidChars = new char[System.IO.Path.GetInvalidFileNameChars().Length + 1];
            System.IO.Path.GetInvalidFileNameChars().CopyTo(InvalidChars, 1);
            InvalidChars[0] = ' ';
            string[] TempID = id.Split(InvalidChars, StringSplitOptions.RemoveEmptyEntries);
            string[] TempCondition = condition.Split(InvalidChars, StringSplitOptions.RemoveEmptyEntries);
            string SystemCompatibleID = String.Concat(TempID);
            string SystemCompatibleCondition = String.Concat(TempCondition);

            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}", SystemCompatibleID, SystemCompatibleCondition, game, map, diff, d.ToString("yyyy_MM_dd_HH_mm_ss"));
        }

        private void ExecuteGame(string ServerArgs)
        {
            string d = Path.Combine(Properties.Settings.Default.UT2004_HOME, "System");

            Process ServerProcess = new Process();
            ServerProcess.StartInfo.WorkingDirectory = d;
            ServerProcess.StartInfo.FileName = "UT2004.exe";
            ServerProcess.StartInfo.Arguments = ServerArgs;
            ServerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ServerProcess.Start();
            ServerProcess.EnableRaisingEvents = true; // not sure if this one is necessary but here it is anyway

            Thread.Sleep(5000); // give server some time to startup

            Process ClientProcess = new Process();
            ClientProcess.StartInfo.WorkingDirectory = d;
            ClientProcess.StartInfo.FileName = "UT2004.exe";
            // previously working on xp machines: ClientProcess.StartInfo.Arguments = "ut2004://127.0.1.1:7777";
            ClientProcess.StartInfo.Arguments = "127.0.0.1";
            ClientProcess.Start();
            ClientProcess.EnableRaisingEvents = true;
            ClientProcess.WaitForExit();
            if (!ServerProcess.HasExited)
            {
                ServerProcess.Kill();                            // original force quit method - may prevent log files from being written but otherwise works
                //ServerProcess.StandardInput.WriteLine("quit"); // more graceful method - but could not secure standard output of server and results in crash
                //ServerProcess.CloseMainWindow();               // another attempt at grace - does not work but also does not crash
            }
        }
    }
}