using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _RestaurantAPI_.Entities
{
    public class Restaurant
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivery { get; set; }
        public string ContactMail { get; set; }
        public string ContactNumber { get; set; }
        public int? CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }

        public int AddressID { get; set; }
        public virtual Address Address { get; set; }    //Umożliwia bezpośrednie odnoszenie sie do adresu restauracji
        public virtual List<Dish> Dishes { get; set; }   //Umożliwia bezpośrednie odnoszenie sie do listy dań w restauracji. Nie potrzebujemy 
                                                         //id dania, bo referencja będzie właśnie ze strony dania.
    }
}
