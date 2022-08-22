using System;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;

namespace qUTei
{
    public partial class AdminForm : Form
    {
        public Protocol MyProtocol;

        public AdminForm()
        {
            InitializeComponent();
            this.GameComboBox.SelectedIndex = 0;
        }

        private void loadProtocolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenProtocolDialog = new OpenFileDialog();
            OpenProtocolDialog.Title = "Select a protocol file";
            OpenProtocolDialog.InitialDirectory = Application.StartupPath;
            OpenProtocolDialog.Filter = "XML File (*.xml)|*.xml";
            OpenProtocolDialog.RestoreDirectory = true;
            if (OpenProtocolDialog.ShowDialog() == DialogResult.OK)
            {
                // Validate the Protocol XML
                try
                {   
                    XDocument xdoc = XDocument.Load(OpenProtocolDialog.FileName);                        // open the validatee
                    XmlSchemaSet schemas = new XmlSchemaSet();                                           // create XSD schema collection with the validator
                    schemas.Add(null, Path.Combine(Application.StartupPath, "Protocol Schema.xsd"));     // add schema
                    xdoc.Validate(schemas, settings_ValidationEventHandler);                             // wire up handler to get any validation errors
                    XmlReader reader = xdoc.CreateReader();                                              // create a reader to roll over the file so validation fires
                    reader.Settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings; // report warnings as well as errors
                    reader.Settings.ValidationType = ValidationType.Schema;                              // use XML Schema
                    while (reader.Read())                                                                // roll over the validatee by dumping to console
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                Console.Write("<{0}", reader.Name);
                                while (reader.MoveToNextAttribute())
                                {
                                    Console.Write(" {0}='{1}'", reader.Name, reader.Value);
                                }
                                Console.WriteLine(">");
                                break;
                            case XmlNodeType.Text:
                                Console.WriteLine(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                Console.WriteLine("</{0}>", reader.Name);
                                break;
                        }
                    }
                }
                catch(Exception XMLReadException)
                {
                    MessageBox.Show(XMLReadException.Message, "XML Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(2);
                }

                // Load the Protocol XML
                try
                {
                    XmlDocument ProtocolXMLDoc = new XmlDocument();
                    ProtocolXMLDoc.Load(OpenProtocolDialog.FileName);

                    MyProtocol = new Protocol();

                    // Condition
                    MyProtocol.Condition = ProtocolXMLDoc.SelectSingleNode("//protocol/@condition").Value;

                    // Games
                    foreach(XmlNode GameNode in ProtocolXMLDoc.SelectNodes("//protocol/game"))
                    {
                        Game AGame = new Game();

                        // Form Name
                        AGame.FormName = GameNode.SelectSingleNode("@form_name").Value;

                        // Password
                        AGame.Password = GameNode.SelectSingleNode("password").InnerText;

                        // DemoRec
                        AGame.DemoRec = Boolean.Parse(GameNode.SelectSingleNode("demorec").InnerText);
                        
                        // Map Choice
                        qUTeiChoice AMapChoice = new qUTeiChoice();
                        AMapChoice.FormInstructions = GameNode.SelectSingleNode("map_choice/@form_instructions").Value;
                        AMapChoice.CmdLineParamter = GameNode.SelectSingleNode("map_choice/@cmd_line_parameter").Value;
                        foreach (XmlNode CmdLineOptionNode in GameNode.SelectNodes("map_choice/cmd_line_option"))
                        {
                            CmdLineOption ACmdLineOption = new CmdLineOption();
                            ACmdLineOption.FormName = CmdLineOptionNode.SelectSingleNode("@form_name").Value;
                            ACmdLineOption.Value = CmdLineOptionNode.InnerText;
                            AMapChoice.CmdLineOptions.Add(ACmdLineOption);
                        }
                        AGame.MapChoice = AMapChoice;

                        // Game Type Choice
                        qUTeiChoice ATypeChoice = new qUTeiChoice();
                        ATypeChoice.FormInstructions = GameNode.SelectSingleNode("type_choice/@form_instructions").Value;
                        ATypeChoice.CmdLineParamter = GameNode.SelectSingleNode("type_choice/@cmd_line_parameter").Value;
                        foreach (XmlNode CmdLineOptionNode in GameNode.SelectNodes("type_choice/cmd_line_option"))
                        {
                            CmdLineOption ACmdLineOption = new CmdLineOption();
                            ACmdLineOption.FormName = CmdLineOptionNode.SelectSingleNode("@form_name").Value;
                            ACmdLineOption.Value = CmdLineOptionNode.InnerText;
                            ATypeChoice.CmdLineOptions.Add(ACmdLineOption);
                        }
                        AGame.TypeChoice = ATypeChoice;

                        // Additional Choices
                        foreach (XmlNode AdditionalChoiceNode in GameNode.SelectNodes("generic_choice"))
                        {
                            qUTeiChoice AnAdditionalChoice = new qUTeiChoice();
                            AnAdditionalChoice.FormInstructions = AdditionalChoiceNode.SelectSingleNode("@form_instructions").Value;
                            AnAdditionalChoice.CmdLineParamter = AdditionalChoiceNode.SelectSingleNode("@cmd_line_parameter").Value;
                            foreach (XmlNode CmdLineOptionNode in AdditionalChoiceNode.SelectNodes("cmd_line_option"))
                            {
                                CmdLineOption ACmdLineOption = new CmdLineOption();
                                ACmdLineOption.FormName = CmdLineOptionNode.SelectSingleNode("@form_name").Value;
                                ACmdLineOption.Value = CmdLineOptionNode.InnerText;
                                AnAdditionalChoice.CmdLineOptions.Add(ACmdLineOption);
                            }
                            AGame.MoreChoices.Add(AnAdditionalChoice);
                        }

                        // Switches
                        foreach (XmlNode Switch in GameNode.SelectNodes("switch"))
                        {
                            AGame.Switches.Add(Switch.InnerText);
                        }

                        MyProtocol.Games.Add(AGame);
                    } // GameNode
                }
                catch (Exception ProtocolLoadException)
                {
                    MessageBox.Show(ProtocolLoadException.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(3);
                }

                // Fill Admin Game Menu
                this.GameComboBox.Items.Clear();
                foreach (Game g in MyProtocol.Games)
                {
                    this.GameComboBox.Items.Add(g.FormName);
                }
                this.GameComboBox.SelectedIndex = 0;
            }
        }

        private void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            MessageBox.Show(e.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.IDBox.Text == String.Empty)
                this.IDBox.Text = "Enter ID";

            if(this.IDBox.Text != "Enter ID")
                this.Hide();
        }

        public int GetCurrentGame()
        {
            return this.GameComboBox.SelectedIndex;
        }

        public void IncrementCurrentGame()
        {
            this.GameComboBox.SelectedIndex++;
        }

        public bool EndOfProtocol()
        {
            return this.GameComboBox.SelectedIndex + 1 == this.GameComboBox.Items.Count;
        }
    }
}