using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Models
{
    public class PagedList<T> : List<T>
    {

        public PagedList(List<T> items,int count,int pageNumber,int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            AddRange(items);
            //above method add all the items to underline List
        }
        public int CurrentPage { get;private set; }

        public int TotalPages { get;private set; }

        public int PageSize { get; private set; }
        public int TotalCount { get;private set; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext => (CurrentPage < TotalPages);


        //this mehod will create pagedlist for us
        public static async Task<PagedList<T>> CreateAsync(
            IQueryable<T> source,int pageNUmber,int pageSize)
        {
            var count = source.Count();
            var items = await source.Skip((pageNUmber - 1)* pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return new PagedList<T>(items, count, pageNUmber, pageSize);
        }

    }
   
}
