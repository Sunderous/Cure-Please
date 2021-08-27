namespace CurePlease
{
    using CurePlease.Engine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml.Linq;

    public partial class PartyBuffs : Form
    {

        private Dictionary<string, IEnumerable<short>> ActiveBuffs;
        private Dictionary<string, IEnumerable<short>> ActiveDebuffs;

        public class BuffList
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }

        public List<BuffList> XMLBuffList = new List<BuffList>();

        public PartyBuffs(MainForm main, BuffEngine buffs, DebuffEngine debuffs)
        {
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();

            ActiveBuffs = buffs.ActiveBuffs;
            ActiveDebuffs = debuffs.ActiveDebuffs;

            if (main.setinstance2.Enabled == true)
            {
                // Create the required List

                // Read the Buffs file a generate a List to call.
                foreach (XElement BuffElement in XElement.Load("Resources/Buffs.xml").Elements("o"))
                {
                    XMLBuffList.Add(new BuffList() { ID = BuffElement.Attribute("id").Value, Name = BuffElement.Attribute("en").Value });
                }
            }
            else
            {
                MessageBox.Show("No character was selected as the power leveler, this can not be opened yet.");
            }
        }

        private void update_effects_Tick(object sender, EventArgs e)
        {
            ailment_list.Text = "BUFFS: \n";

            // Search through current active party buffs
            foreach (string name in ActiveBuffs.Keys)
            {
                // First add Character name and a Line Break.
                ailment_list.AppendText(name.ToUpper() + "\n");

                // Now create a list and loop through each buff and name them
                var buffIds = ActiveBuffs[name];

                int i = 1;
                int count = buffIds.Count();

                foreach (short acBuff in buffIds)
                {
                    i++;

                    var found_Buff = XMLBuffList.Find(r => r.ID == acBuff.ToString());

                    if (found_Buff != null)
                    {
                        if (i == count)
                        {
                            ailment_list.AppendText(found_Buff.Name + " (" + acBuff + ") ");
                        }
                        else
                        {
                            ailment_list.AppendText(found_Buff.Name + " (" + acBuff + "), ");
                        }
                    }
                }

                ailment_list.AppendText("\n\n");
            }

            ailment_list.AppendText("DEBUFFS: \n");

            foreach (string name in ActiveDebuffs.Keys)
            {
                // First add Character name and a Line Break.
                ailment_list.AppendText(name.ToUpper() + "\n");

                // Now create a list and loop through each buff and name them
                var buffIds = ActiveDebuffs[name];

                int i = 1;
                int count = buffIds.Count();

                foreach (short acBuff in buffIds)
                {
                    i++;

                    var found_Buff = XMLBuffList.Find(r => r.ID == acBuff.ToString());

                    if (found_Buff != null)
                    {
                        if (i == count)
                        {
                            ailment_list.AppendText(found_Buff.Name + " (" + acBuff + ") ");
                        }
                        else
                        {
                            ailment_list.AppendText(found_Buff.Name + " (" + acBuff + "), ");
                        }
                    }
                }

                ailment_list.AppendText("\n\n");
            }
        }
    }
}
