using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace MySQL_CSharp {
    public partial class Form1 : Form {

        MySqlConnection myCon;
        string id = "NULL";

        public Form1() {
            InitializeComponent();


            lbl_txt.Text = "Desconectado";
            lbl_txt.Left = (this.ClientSize.Width - lbl_txt.Size.Width) / 2;

            conectar();
            llenarTabla();
        }

        private void conectar() {
            try {
                string server = "localhost";
                string db = "empleados";
                string user = "root";
                string pwd = "123456";
                string con = $"server={server};Port={3306};database={db};Uid={user};pwd={pwd};";

                myCon = new MySqlConnection(con);
                myCon.Open();

                lbl_txt.Text = "Conectado";
                lbl_txt.ForeColor = System.Drawing.Color.Green;
                lbl_txt.Left = (this.ClientSize.Width - lbl_txt.Size.Width) / 2;
            } catch (Exception err) {
                lbl_txt.Text = $"Error de coneccion";
                lbl_txt.ForeColor = System.Drawing.Color.Red;
                lbl_txt.Left = (this.ClientSize.Width - lbl_txt.Size.Width) / 2;

                Console.WriteLine(err);
            }
        }

        private void llenarTabla(string query = "select * from empleados;") {

            MySqlCommand comandDB = new MySqlCommand(query, myCon);
            comandDB.CommandTimeout = 60;
            MySqlDataReader reader;

            dtg_empleados.Rows.Clear();
            dtg_empleados.Refresh();

            try {
                reader = comandDB.ExecuteReader();
                if (reader.HasRows) {
                    while (reader.Read()) {
                        int index = dtg_empleados.Rows.Add();
                        dtg_empleados.Rows[index].Cells[0].Value = reader.GetString(0);
                        dtg_empleados.Rows[index].Cells[1].Value = reader.GetString(1);
                        dtg_empleados.Rows[index].Cells[2].Value = reader.GetString(2);
                        dtg_empleados.Rows[index].Cells[3].Value = reader.GetString(3);
                    }
                }

                reader.Close();
                id = "NULL";
            } catch (Exception err) {
                MessageBox.Show($"{err}");
            }
            
        }

        private void btn_buscar_Click(object sender, EventArgs e) {
            string were = "WHERE 1=1 ";

            if (txtBox_nombre.Text != "") were += $"and Nombre LIKE '%{txtBox_nombre.Text}%'";
            if (txtBox_puesto.Text != "") were += $"and Puesto LIKE '%{txtBox_puesto.Text}%'";
            if (txtBox_edad.Text != "") were += $"and Edad LIKE '%{txtBox_edad.Text}%'";

            string query = $"SELECT * FROM empleados {were};";

            llenarTabla(query);
        }

        private void btn_agregar_Click(object sender, EventArgs e) {
            string msg = "";

            if (txtBox_nombre.Text == "") msg = "el nombre no puede estar vacio\n";
            if (txtBox_puesto.Text == "") msg += "el puesto no puede estar vacio\n";
            if (txtBox_edad.Text == "") msg += "la edad no puede estar vacia\n";

            if(msg == "") {
                string query = $"INSERT INTO empleados (Nombre, Puesto, Edad) VALUES " +
                    $"('{txtBox_nombre.Text}', '{txtBox_puesto.Text}', {txtBox_edad.Text});";

                txtBox_nombre.Text = "";
                txtBox_puesto.Text = "";
                txtBox_edad.Text = "";

                llenarTabla(query);
                llenarTabla();


            } else MessageBox.Show(msg);
        }

        private void dtg_empleados_CellClick(object sender, DataGridViewCellEventArgs e) {
            int index = e.RowIndex;

            dtg_empleados.Rows[index].Selected = true;

            id = dtg_empleados.Rows[index].Cells[0].Value.ToString();
            txtBox_nombre.Text = dtg_empleados.Rows[index].Cells[1].Value.ToString();
            txtBox_puesto.Text = dtg_empleados.Rows[index].Cells[2].Value.ToString();
            txtBox_edad.Text = dtg_empleados.Rows[index].Cells[3].Value.ToString();
        }

        private void btn_editar_Click(object sender, EventArgs e) {
            string query = $"UPDATE empleados SET nombre = '{txtBox_nombre.Text}', puesto = '{txtBox_puesto.Text}', edad = '{txtBox_edad.Text}' WHERE id = {id}";

            llenarTabla(query);
            llenarTabla();
        }

        private void btn_eliminar(object sender, EventArgs e) {
            MessageBox.Show(id);
            if(id != "NULL") {
                string query = $"DELETE FROM empleados WHERE id = {id};";
                llenarTabla(query);
                llenarTabla();
            }
        }
    }
}
