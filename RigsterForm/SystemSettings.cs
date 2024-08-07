using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RigsterForm
{
    public class SystemSettings
    {
        /* 補貼金額 */
        public int allowance_per_nb { get; set; }

        /* 建構式 */
        public SystemSettings() { }

        /*設定補貼金額*/
        public void SetAllowancePerNB(int amount)
        {
            allowance_per_nb = amount;
        }
    }
}
