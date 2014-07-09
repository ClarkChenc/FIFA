using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFATestAdapter
{
    public interface IFIFASettingMapper
    {
        void MapSettings(FIFASettingUI settings);
        FIFASettingMgr Settings { get; }
    }
}
