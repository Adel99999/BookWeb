using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Models;

namespace Bulky.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> shoppingCartList { get; set; }
        public double OrderTotal { get; set; }
    }
}
