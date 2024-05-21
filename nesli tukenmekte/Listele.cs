using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nesli_tukenmekte
{
    public partial class Listele : Form
    {
        string baglanti = "Server=localhost;Database=canli_veritabani;Uid=root;Pwd=;";
        string yeniad = "";
        public Listele()
        {
            InitializeComponent();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "UPDATE canlilar SET ad =@ad, yasadigi_ulke=@yasadigi_ulke, populasyon=@populasyon, gorsel_ad=@gorsel_ad WHERE id = @satirid;";

               
              

                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                cmd.Parameters.AddWithValue("@ad", txtAdi.Text);
                cmd.Parameters.AddWithValue("@yasadigi_ulke", txtUlke.Text);
                cmd.Parameters.AddWithValue("@populasyon", txtPopulasyon.Text);
                cmd.Parameters.AddWithValue("@gorsel_ad", yeniad);



                int id = Convert.ToInt32(dgwCanli.SelectedRows[0].Cells["id"].Value);
                cmd.Parameters.AddWithValue("@satirid", id);



                cmd.ExecuteNonQuery();

                DgwDoldur(); //datagridviewi yenile





            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            DataGridViewRow dr = dgwCanli.SelectedRows[0];
            int satirId = Convert.ToInt32(dr.Cells[0].Value);

            DialogResult cevap = MessageBox.Show("hayvanı Silmek İstediğinizden Emin Misiniz?",
                                "Şarkı Sil",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Error);

            if (cevap == DialogResult.Yes)
            {

                string sorgu = "DELETE FROM canlilar where id = @satirid;";

                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    baglan.Open();
                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    cmd.Parameters.AddWithValue("@satirid", satirId);
                    cmd.ExecuteNonQuery();

                    DgwDoldur(); //tekrar doldurur
                }


            }
        }

        private void Listele_Load(object sender, EventArgs e)
        {
            string klasorYolu = @"gorsel_ad";
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
            }
            DgwDoldur();
        }
        void DgwDoldur()
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "SELECT * FROM canlilar;";

                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();


                da.Fill(dt);
                dgwCanli.DataSource = dt;
                // dgwFilmler.Columns["poster"].Visible = false;
            }
        }

        private void dgwCanli_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwCanli.SelectedRows.Count > 0)
            {
               txtAdi.Text = dgwCanli.SelectedRows[0].Cells["ad"].Value.ToString();
                txtUlke.Text = dgwCanli.SelectedRows[0].Cells["yasadigi_ulke"].Value.ToString();
                txtPopulasyon.Text = dgwCanli.SelectedRows[0].Cells["populasyon"].Value.ToString();
              
                // pbResim.Value = Convert.ToDateTime(dgwHayvanlar.SelectedRows[0].Cells["fotograf_adi"].Value);


                string dosyaYolu = Path.Combine(Environment.CurrentDirectory, "gorsel_ad", dgwCanli.SelectedRows[0].Cells["gorsel_ad"].Value.ToString());

                //önce resimi temizle
                pbResim.Image = null;

                //varsa zaten gösterir
                if (File.Exists(dosyaYolu))
                {
                    pbResim.Image = Image.FromFile(dosyaYolu);
                    pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
                }

            }
        }

        private void pbResim_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniad = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "gorsel_ad", yeniad);

            File.Copy(kaynakDosya, hedefDosya);

            pbResim.Image = null;

            if (File.Exists(hedefDosya))
            {
                pbResim.Image = Image.FromFile(hedefDosya);
                pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
