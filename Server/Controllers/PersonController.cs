using BlazorWasmCrud.Server.Data;
using BlazorWasmCrud.Shared.Models;
using BlazorWasmCrud.Shared.VMModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlazorWasmCrud.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PersonController : ControllerBase
    {

        private DatabaseContext _ctx;

        public PersonController(DatabaseContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost]
        public IActionResult AddUpdate(Person Person)
        {
            Status status = new();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass valid data";
                return Ok(status);
            }
            try
            {
                if (Person.Id == 0)
                    _ctx.Person.Add(Person);
                else
                    _ctx.Person.Update(Person);
                _ctx.SaveChanges();
                status.StatusCode = 1;
                status.Message = "Saved successfully.";
            }
            catch (Exception ex)
            {
                status.StatusCode = 0;
                status.Message = "Server error";
            }
            return Ok(status);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Status status = new();
            try
            {
                Person person = _ctx.Person.Find(id);
                if (person is null)
                {
                    status.StatusCode = 0;
                    status.Message = "Person doesn't exist.";
                    return Ok(status);
                }
                _ctx.Person.Remove(person);
                _ctx.SaveChanges();
                status.StatusCode = 1;
                status.Message = "Deleted successfully.";
            }
            catch (Exception)
            {
                status.StatusCode = 0;
                status.Message = "Server error";
            }
            return Ok(status);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var data = _ctx.Person.Find(id);
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetAll(string? sTerm = "", int pageno = 1)
        {
            sTerm = sTerm?.ToLower();
            var data = (from person in _ctx.Person
                        where sTerm == null || person.Name.ToLower().StartsWith(sTerm)
                        select new Person
                        {
                            Name = person.Name,
                            Email = person.Email,
                            Id = person.Id
                        }).ToList();

            var totalRecords = data.Count;
            int pagesize = 3;
            var totalpages = (int)Math.Ceiling((double)totalRecords / pagesize);
            int skip = (pageno - 1) * pagesize;
            data = data.Skip(skip).Take(pagesize).ToList();
            var model = new PersonList()
            {
                Persons = data,
                SearchTerm = sTerm,
                TotalPages = totalpages,
                PageNumber = pageno
            };
            return Ok(model);
        }
    }
}
