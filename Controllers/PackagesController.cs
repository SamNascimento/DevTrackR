using DevTrackR.Entities;
using DevTrackR.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevTrackR.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController : ControllerBase
    {
        /// <summary>
        /// Busca todos os pacotes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var packages = new List<Package>
            {
                new Package("Pacote 1", 1.3M),
                new Package("Pacote 2", 0.2M)
            };

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
            var package = new Package("Pacote 2", 0.2M);

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
            var package = new Package(model.Title, model.Weight);

            return Ok(package);
        }

        /// <summary>
        /// Cria um novo update para um pacote específico
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost("{code}/updates")]
        public IActionResult PostUpdate(string code, AddPackageUpdateInputModel model)
        {
            var package = new Package("Pacote 1", 1.2M);

            package.AddUpdate(model.Status, model.Delivered);

            return Ok();
        }
    }
}
