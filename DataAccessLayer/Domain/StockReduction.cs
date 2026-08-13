using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Domain
{
    public record StockReduction(
        Guid ProductID,
        int Quantity
    );
}
