using Kafe21.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kafe21
{
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasindiEventArgs> MasaTasindi;

        KafeVeri db;
        Siparis siparis;
        public SiparisForm(KafeVeri kafeVeri, Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            InitializeComponent();
            tsslAcilisZamani.Text = "Açılış Zamanı: " + siparis.AcilisZamani.ToString();
            dgvSiparisDetaylar.AutoGenerateColumns = false;//Bizim olusturdugumuz sutunları göstermesi için kafasına göre sütun oluşturmayacak
            MasaNolariGuncelle();
            OdemeTutariGuncelle();
            UrunleriYukle();
            SiparisDetaylariYukle();
        }

        private void SiparisDetaylariYukle()
        {
            dgvSiparisDetaylar.DataSource = siparis.SiparisDetaylar.ToList();
        }

        private void UrunleriYukle()
        {
            cboUrun.DataSource = db.Urunler.ToList();
        }

        private void OdemeTutariGuncelle()
        {
            lblOdemeTutari.Text = siparis.ToplamTutarTL;
        }

        private void MasaNolariGuncelle()
        {
            Text = $"Masa {siparis.MasaNo:00} - Sipariş";
            lblMasaNo.Text = siparis.MasaNo.ToString("00");

            cboMasaNo.DataSource = Enumerable
                .Range(1, db.MasaAdet)
                .Where(x => !db.Siparisler.Any(s => s.MasaNo == x && s.Durum == SiparisDurum.Aktif))
                .ToList();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            Urun urun = (Urun)cboUrun.SelectedItem;
            int adet = (int)nudAdet.Value;

            siparis.SiparisDetaylar.Add(new SiparisDetay()
            {
                UrunAd = urun.UrunAd,
                BirimFiyat = urun.BirimFiyat,
                Adet = adet,
                UrunId = urun.Id
            }) ;          
            db.SaveChanges();
            dgvSiparisDetaylar.DataSource = siparis.SiparisDetaylar.ToList();
            OdemeTutariGuncelle();
        }

        private void dgvSiparisDetaylar_KeyDown(object sender, KeyEventArgs e)
        {
            //delete ye basıldıysa ve dgv üzerinde en az bir secili satır varsa
            if (e.KeyCode == Keys.Delete && dgvSiparisDetaylar.SelectedRows.Count > 0)
            {
                DialogResult dr = MessageBox.Show("Seçili detay siparişten kaldırılılacaktır. Onaylıyor musunuz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                if (dr == DialogResult.Yes)
                {
                    DataGridViewRow satir = dgvSiparisDetaylar.SelectedRows[0];
                    SiparisDetay sd = (SiparisDetay)satir.DataBoundItem;//kastetmek
                    db.SiparisDetaylar.Remove(sd);
                    db.SaveChanges();
                    dgvSiparisDetaylar.DataSource = siparis.SiparisDetaylar.ToList();
                    OdemeTutariGuncelle();
                }
            }

        }

        private void btnAnasayfaya_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;//içinde bulunan formun close metodunu çağır
        }
        // Close();

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            SiparisKapat(SiparisDurum.Iptal);
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            SiparisKapat(SiparisDurum.Odendi);
        }
        private void SiparisKapat(SiparisDurum durum)
        {
            if (durum == SiparisDurum.Odendi)
                siparis.OdenenTutar = siparis.ToplamTutar();

            siparis.KapanisZamani = DateTime.Now;
            siparis.Durum = durum;
            db.SaveChanges();
            DialogResult = DialogResult.OK;//bu formda işim bitti.
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedIndex > -1)
            {
                int eskiMasaNo = siparis.MasaNo;
                int yeniMasaNo = (int)cboMasaNo.SelectedItem;
                siparis.MasaNo = yeniMasaNo;
                db.SaveChanges();
                MasaNolariGuncelle();

                if (MasaTasindi != null)
                {
                    MasaTasindi(this, new MasaTasindiEventArgs(eskiMasaNo, yeniMasaNo));
                }
            }
        }
    }
}
