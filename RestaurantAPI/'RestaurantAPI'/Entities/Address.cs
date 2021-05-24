using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _RestaurantAPI_.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string City{ get; set; }
        public string Street{ get; set; }
        public string PostalCode{ get; set; }
        public virtual Restaurant Restaurant{ get; set; }   //odniesienie do bezpośredniego obiektu restauracji, z którą jest połączony adres

    }
}
