using Kafe21.Data;
using Kafe21.Properties;
using Newtonsoft.Json;
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

namespace Kafe21
{
    public partial class Form1 : Form
    {
        KafeVeri db;

        public Form1()
        {
            InitializeComponent();
            VerileriOku();
            //  OrnekVerileriYukle();
            MasalariYukle();
        }

        private void VerileriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json);
            }
            catch (Exception)
            {

                db = new KafeVeri();
            }
        }

        private void MasalariYukle()
        {
            ImageList imgList = new ImageList();
            imgList.Images.Add("bos", Resources.bos);
            imgList.Images.Add("dolu", Resources.dolu);
            imgList.ImageSize = new Size(70, 70);
            lvwMasalar.LargeImageList = imgList;

            for (int i = 1; i <= db.MasaAdet; i++)
            {
                ListViewItem lvi = new ListViewItem("Masa " + i);
                bool doluMu = db.AktifSiparisler.Any(x => x.MasaNo == i);
                lvi.ImageKey = doluMu ? "dolu" : "bos";
                lvi.Tag = i;
                lvwMasalar.Items.Add(lvi);
            }
        }

        private void OrnekVerileriYukle()
        {
            db.Urunler.Add(new Urun()
            {
                UrunAd = "Ayran",
                BirimFiyat = 4.50m
            });
            db.Urunler.Add(new Urun()
            {
                UrunAd = "Pepsi",
                BirimFiyat = 5.00m
            });

        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            UrunlerForm frmUrunler = new UrunlerForm(db);
            frmUrunler.ShowDialog();
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            var frmGecmisSiparislerForm = new GecmisSiparislerForm(db);
            frmGecmisSiparislerForm.ShowDialog();
        }

        private void lvwMasalar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //çift tıklama listviewitem
            ListViewItem lvi = lvwMasalar.SelectedItems[0];
            int masaNo = (int)lvi.Tag;
            Siparis siparis = db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);
            //henuz bu masa için sipariş oluşturulmamıssa bu siparişi şimdi oluşturalım
            if (siparis == null)
            {
                siparis = new Siparis() { MasaNo = masaNo };
                db.AktifSiparisler.Add(siparis);
                lvi.ImageKey = "dolu";
            }

            SiparisForm frmSiparis = new SiparisForm(db, siparis); //SiparisForm ekranında işlem yapmak için 
            frmSiparis.MasaTasindi += FrmSiparis_MasaTasindi;
            DialogResult dr = frmSiparis.ShowDialog();

            if (dr == DialogResult.OK)
            {
                lvi.ImageKey = "bos";
            }
        }

        private void FrmSiparis_MasaTasindi(object sender, MasaTasindiEventArgs e)
        {
            lvwMasalar.Items.Cast<ListViewItem>().First(x => (int)x.Tag == e.EskiMasaNo).ImageKey = "bos";

            lvwMasalar.Items.Cast<ListViewItem>().First(x => (int)x.Tag == e.YeniMasaNo).ImageKey = "dolu";

            //foreach (ListViewItem lvi in lvwMasalar.Items)
            //{
            //    if ((int)lvi.Tag == e.EskiMasaNo)
            //    {
            //        lvi.ImageKey = "bos";
            //    }
            //    else if ((int)lvi.Tag == e.YeniMasaNo)
            //    {
            //        lvi.ImageKey = "dolu";
            //    }
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            VerileriKaydet();
        }

        private void VerileriKaydet()
        {
            var json = JsonConvert.SerializeObject(db, Formatting.Indented);//verileri dosyaya düzgün kaydetmesi için --->  Formatting.Indented
            File.WriteAllText("veri.json", json);
        }
    }
}
