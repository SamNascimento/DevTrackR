using DevTrackR.Entities;
using DevTrackR.Models;
using DevTrackR.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace DevTrackR.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController : ControllerBase
    {
        private readonly DevTrackRContext _context;
        public PackagesController(DevTrackRContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca todos os pacotes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var packages = _context.Packages;

            return Ok(packages);
        }

        /// <summary>
        /// Retorna informação de um pacote específico
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("{code}")]
        public IActionResult GetByCode (string code)
        {
            var package = _context.Packages
                .SingleOrDefault(p => p.Code == code);

            if(package == null)
                return NotFound();

            return Ok(package);
        }

        /// <summary>
        /// Cria um pacote novo
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(AddPackageInputModel model)
        {
            if (model.Title.Length < 10)
                return BadRequest("Tamanho do título deve ser maior que 10 caracteres");

            var package = new Package(model.Title, model.Weight);

            _context.Packages.Add(package);

            return CreatedAtAction(
                "GetByCode", 
                new { code = package.Code },
                package
                );
        }

        /// <summary>
        /// Cria um novo update para um pacote específico
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost("{code}/updates")]
        public IActionResult PostUpdate(string code, AddPackageUpdateInputModel model)
        {
            var package = _context.Packages
                .SingleOrDefault(p => p.Code == code);

            if (package == null)
                return NotFound();

            package.AddUpdate(model.Status, model.Delivered);

            return NoContent();
        }
    }
}
