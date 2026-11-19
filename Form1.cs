using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NTP_ChatApp
{
    public partial class Form1 : Form
    {
        // Arayüz elemanlarý
        private TextBox txtIp = null!;
        private TextBox txtPort = null!;
        private TextBox txtAd = null!;
        private TextBox txtMesaj = null!;
        private Button btnSunucu = null!;
        private Button btnBaglan = null!;
        private Button btnYeniPencere = null!;
        private Button btnGonder = null!;
        private ListBox lstMesajlar = null!;
        private Label lblDurum = null!;

        // Að (TCP/IP) nesneleri
        private TcpListener? server;
        private TcpClient? client;
        private NetworkStream? stream;
        private bool bagliMi = false;

        public Form1()
        {
            InitializeComponent();
            ArayuzuKur();
        }

        private void ArayuzuKur()
        {
            this.Text = "YMH219 - TCP/IP ChatApp";
            this.Size = new Size(520, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += Form1_FormClosing;

            // Üst Baðlantý Paneli
            Label lblIp = new Label() { Text = "IP:", Location = new Point(15, 18), AutoSize = true };
            txtIp = new TextBox() { Text = "127.0.0.1", Location = new Point(40, 15), Size = new Size(95, 25) };

            Label lblPort = new Label() { Text = "Port:", Location = new Point(145, 18), AutoSize = true };
            txtPort = new TextBox() { Text = "5000", Location = new Point(180, 15), Size = new Size(55, 25) };

            Label lblAd = new Label() { Text = "Ad:", Location = new Point(245, 18), AutoSize = true };
            txtAd = new TextBox() { Text = "Kullanici", Location = new Point(275, 15), Size = new Size(100, 25) };

            btnYeniPencere = new Button() { Text = "+ Pencere", Location = new Point(385, 14), Size = new Size(100, 26) };
            btnYeniPencere.Click += (s, e) => { new Form1().Show(); };

            // Sunucu ve Baðlan Butonlarý
            btnSunucu = new Button() { Text = "Sunucuyu Baþlat (Server)", Location = new Point(15, 50), Size = new Size(225, 32) };
            btnSunucu.Click += BtnSunucu_Click;

            btnBaglan = new Button() { Text = "Sunucuya Baðlan (Client)", Location = new Point(250, 50), Size = new Size(235, 32) };
            btnBaglan.Click += BtnBaglan_Click;

            // Mesaj Listesi
            lstMesajlar = new ListBox() { Location = new Point(15, 95), Size = new Size(470, 275) };

            // Alt Mesaj Gönderme Alaný
            txtMesaj = new TextBox() { Location = new Point(15, 385), Size = new Size(375, 28) };
            txtMesaj.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    MesajGonder();
                }
            };

            btnGonder = new Button() { Text = "Gönder", Location = new Point(400, 384), Size = new Size(85, 28) };
            btnGonder.Click += (s, e) => MesajGonder();

            // Durum Etiketi
            lblDurum = new Label() { Text = "Durum: Baðlý deðil", Location = new Point(15, 425), ForeColor = Color.Red, AutoSize = true };

            this.Controls.AddRange(new Control[]
            {
                lblIp, txtIp, lblPort, txtPort, lblAd, txtAd, btnYeniPencere,
                btnSunucu, btnBaglan, lstMesajlar, txtMesaj, btnGonder, lblDurum
            });
        }

        // SUNUCU BAÞLATMA
        private async void BtnSunucu_Click(object? sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(txtPort.Text);
                server = new TcpListener(IPAddress.Any, port);
                server.Start();

                btnSunucu.Enabled = false;
                btnBaglan.Enabled = false;
                DurumGuncelle($"Durum: Sunucu {port} portunda dinliyor...", Color.DarkOrange);
                MesajEkle($"[Sistem] Sunucu baþlatýldý. Ýstemci bekleniyor...");

                client = await server.AcceptTcpClientAsync();
                stream = client.GetStream();
                bagliMi = true;

                DurumGuncelle("Durum: Ýstemci baðlandý (Sunucu Modu)", Color.Green);
                MesajEkle("[Sistem] Bir kullanýcý sohbete katýldý!");

                _ = Task.Run(MesajlariDinle);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sunucu hatasý: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ÝSTEMCÝ BAÐLANMA
        private async void BtnBaglan_Click(object? sender, EventArgs e)
        {
            try
            {
                string ip = txtIp.Text;
                int port = int.Parse(txtPort.Text);

                client = new TcpClient();
                await client.ConnectAsync(ip, port);
                stream = client.GetStream();
                bagliMi = true;

                btnSunucu.Enabled = false;
                btnBaglan.Enabled = false;
                DurumGuncelle($"Durum: {ip}:{port} sunucusuna baðlandý", Color.Green);
                MesajEkle("[Sistem] Sunucuya baþarýyla baðlanýldý!");

                _ = Task.Run(MesajlariDinle);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Baðlantý hatasý: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // GELEN MESAJLARI ARKA PLANDA DÝNLEME
        private async Task MesajlariDinle()
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (bagliMi && stream != null)
                {
                    int okunanByte = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (okunanByte == 0) break;

                    string gelenMesaj = Encoding.UTF8.GetString(buffer, 0, okunanByte);
                    MesajEkle(gelenMesaj);
                }
            }
            catch
            {
                // Baðlantý koptuðunda sessizce çýk
            }
            finally
            {
                bagliMi = false;
                DurumGuncelle("Durum: Baðlantý kesildi", Color.Red);
                MesajEkle("[Sistem] Karþý taraf baðlantýyý kapattý.");
            }
        }

        // MESAJ GÖNDERME
        private async void MesajGonder()
        {
            if (!bagliMi || stream == null)
            {
                MessageBox.Show("Önce baðlantý kurmalýsýnýz!", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string metin = txtMesaj.Text.Trim();
            if (string.IsNullOrEmpty(metin)) return;

            try
            {
                string formatliMesaj = $"[{DateTime.Now:HH:mm:ss}] {txtAd.Text}: {metin}";
                byte[] veri = Encoding.UTF8.GetBytes(formatliMesaj);
                await stream.WriteAsync(veri, 0, veri.Length);

                MesajEkle(formatliMesaj);
                txtMesaj.Clear();
                txtMesaj.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mesaj gönderilemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Thread-safe (arayüzü kilitlemeden) mesaj ekleme
        private void MesajEkle(string mesaj)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(MesajEkle), mesaj);
                return;
            }
            lstMesajlar.Items.Add(mesaj);
            lstMesajlar.TopIndex = lstMesajlar.Items.Count - 1;
        }

        private void DurumGuncelle(string durum, Color renk)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color>(DurumGuncelle), durum, renk);
                return;
            }
            lblDurum.Text = durum;
            lblDurum.ForeColor = renk;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            bagliMi = false;
            stream?.Close();
            client?.Close();
            server?.Stop();
        }
    }
}
