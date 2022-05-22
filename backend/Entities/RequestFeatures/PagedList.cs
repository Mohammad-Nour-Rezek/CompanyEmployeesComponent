using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaDataProp { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaDataProp = new MetaData { 
                TotalCount = count,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)               
            };

            AddRange(items);
        }

        public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize, int count)
        {                                   
            return new PagedList<T>(source.ToList(), count, pageNumber, pageSize);
        }
    }
}
