using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba2
{
    public partial class Form1 : Form
    {
        public string ConnectionString { get; set; } 
            = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=TheWorld; Integrated Security=true;";

        public Form1()
        {
            InitializeComponent();


        }

        private async void uploadButton_Click(object sender, EventArgs e)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                var query = @"select * from part";
                var cmd = new SqlCommand(query, sqlConnection);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    TreeNode tn = new TreeNode(dr["part"].ToString());
                    tn.Tag = dr["id"];
                    treeView1.Nodes.Add(tn);
                    LoadCountries((int)dr["id"], tn);
                }

                dr.Close();
            }
        }

        private void LoadCountries(int partId, TreeNode parent)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                var query = @"select c.country, c.id from country c where c.part_id = @partId";
                var cmd = new SqlCommand(query, sqlConnection);
                cmd.Parameters.AddWithValue("@partId", partId);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    TreeNode tn = new TreeNode(dr["country"].ToString());
                    tn.Tag = dr["id"];
                    parent.Nodes.Add(tn);
                }

                dr.Close();
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 1)
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    var query = @"delete from country where id = @id";
                    var cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@id", treeView1.SelectedNode.Tag); 
                    
                    cmd.ExecuteNonQuery();  
                    treeView1.SelectedNode.Remove();  
                }
            }
            else if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 0)
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    var query = @"delete from part where id = @id";
                    var cmd = new SqlCommand(query, sqlConnection);
                    cmd.Parameters.AddWithValue("@id", treeView1.SelectedNode.Tag);

                    cmd.ExecuteNonQuery();
                    treeView1.SelectedNode.Remove();
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level < 2)
            {
                удалитьToolStripMenuItem.Enabled = true;
            }
            else
            {
                удалитьToolStripMenuItem.Enabled = false;
            }
        }
    }
}
