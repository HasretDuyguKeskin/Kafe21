using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafe21.Data
{
    [Table("Siparisler")]
    public class Siparis
    {
        public int Id { get; set; }
        public int MasaNo { get; set; }
        public decimal OdenenTutar { get; set; }
        public DateTime? AcilisZamani { get; set; } = DateTime.Now;
        public DateTime? KapanisZamani { get; set; }
        public virtual ICollection<SiparisDetay> SiparisDetaylar { get; set; } = new HashSet<SiparisDetay>();
        public SiparisDurum Durum { get; set; }
        public string ToplamTutarTL => ToplamTutar().ToString("c2");
        public decimal ToplamTutar()
        {
            return SiparisDetaylar.Sum(x => x.Tutar());
        }
    }
}
