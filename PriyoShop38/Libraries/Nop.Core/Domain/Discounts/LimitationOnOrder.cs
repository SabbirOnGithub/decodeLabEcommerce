using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Discounts
{
    public enum LimitationOnOrder
    {
        NoLimitation = 0,
        ForNthOrder = 5,
        FromNthToMthOrder = 10
    }
}
