using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoteTaker2
{
    public partial class NoteTaker : Form
    {
        private SQLiteConnection sqliteConn;
        private SQLiteDataAdapter sqliteDataAdapter;
        DataTable notes;
        bool editing = false;
        public NoteTaker()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            // Connection string for SQLite database file (modify as needed)
            string connectionString = "Data Source=notes.db;Version=3;";

            // Create a new SQLite database connection
            sqliteConn = new SQLiteConnection(connectionString);

            // Create the notes table if it doesn't exist
            using (SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Notes (Title TEXT, Note TEXT)", sqliteConn))
            {
                sqliteConn.Open();
                command.ExecuteNonQuery();
            }

            // Load data from the database into the DataTable
            notes = new DataTable();
            sqliteDataAdapter = new SQLiteDataAdapter("SELECT * FROM Notes", sqliteConn);
            sqliteDataAdapter.Fill(notes);

            // Set the DataTable as the DataSource for the DataGridView
            previousNotes.DataSource = notes;

            sqliteConn.Close();
        }

        private void NoteTaker_Load(object sender, EventArgs e)
        {
            //notes.Columns.Add("Title");
            //notes.Columns.Add("Note");

            previousNotes.DataSource = notes;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            titleBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[0].ToString();
            noteBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[1].ToString();
            editing = true;
        }

        private void newNoteButton_Click(object sender, EventArgs e)
        {
            titleBox.Text = "";
            noteBox.Text = "";
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try {
                notes.Rows[previousNotes.CurrentCell.RowIndex].Delete();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Not a valid note");
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (editing)
                {
                    notes.Rows[previousNotes.CurrentCell.RowIndex]["Title"] = titleBox.Text;
                    notes.Rows[previousNotes.CurrentCell.RowIndex]["Note"] = noteBox.Text;

                    using (SQLiteCommand command = new SQLiteCommand("UPDATE Notes SET Title=@title, Note=@note WHERE Title=@oldTitle", sqliteConn))
                    {
                        sqliteConn.Open();
                        command.Parameters.AddWithValue("@title", titleBox.Text);
                        command.Parameters.AddWithValue("@note", noteBox.Text);
                        command.Parameters.AddWithValue("@oldTitle", notes.Rows[previousNotes.CurrentCell.RowIndex]["Title"]);
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    notes.Rows.Add(titleBox.Text, noteBox.Text);

                    using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Notes (Title, Note) VALUES (@title, @note)", sqliteConn))
                    {
                        sqliteConn.Open();
                        command.Parameters.AddWithValue("@title", titleBox.Text);
                        command.Parameters.AddWithValue("@note", noteBox.Text);
                        command.ExecuteNonQuery();
                    }
                }
                titleBox.Text = "";
                noteBox.Text = "";
                editing = false;
            }
            finally {
                sqliteConn.Close();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void titleBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void previousNotes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            titleBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[0].ToString();
            noteBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[1].ToString();
            editing = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void FilterNotes(string filterText)
        {
            notes.DefaultView.RowFilter = $"Title LIKE '%{filterText}%'";
        }
        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            FilterNotes(searchBox.Text);

            previousNotes.DataSource = notes.DefaultView.ToTable();
        }
    }
}
