using BlazorWasmCrud.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWasmCrud.Shared.VMModel
{
    public class PersonList
    {
        public List<Person>? Persons { get; set; }
        public string SearchTerm { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
    }
}
