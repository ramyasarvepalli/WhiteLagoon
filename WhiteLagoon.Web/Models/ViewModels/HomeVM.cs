using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Villa> VillaList { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int Nights { get; set; }

    }
}
