using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop_site_parser.Model;

namespace Shop_site_parser.Interfaces
{
    interface IAvitoDB
    {
        int WriteItem(AvitoDBModel _newItem);
        List<AvitoDBModel> GetDataList();
    }
}
