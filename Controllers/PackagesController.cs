using DevTrackR.Entities;
using DevTrackR.Models;
using DevTrackR.Persistence.Repository;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DevTrackR.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageRepository _repository;
        private readonly ISendGridClient _client;
        public PackagesController(IPackageRepository repository, ISendGridClient client)
        {
            _repository = repository;
            _client = client;
        }

        /// <summary>
        /// Busca todos os pacotes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var packages = _repository.GetAll();

            return Ok(packages);
        }

        /// <summary>
        /// Retorna informação de um pacote específico
        /// </summary>
        /// <remarks>
        /// {
        ///  "code": 1234
        /// }
        /// </remarks>
        /// <param name="code">Código de rastreio do pacote</param>
        /// <returns>Retorna o objeto correspondente aquele código</returns>
        /// <response code="200">Objeto encontrado e retornado</response>
        /// <response code="404">Nenhum objeto com esse código foi encontrado</response>
        [HttpGet("{code}")]
        public IActionResult GetByCode (string code)
        {
            var package = _repository.GetByCode(code);

            if (package == null)
                return NotFound();

            return Ok(package);
        }

        /// <summary>
        /// Cria um pacote novo
        /// </summary>
        /// <remarks>
        /// {
        ///  "title": "Pacote teste para reprodução",
        ///  "weight": 1.8,
        ///  "senderName": "Samuel",
        ///  "senderEmail: "emailteste@teste.com"
        /// }
        /// </remarks>
        /// <param name="model">Dados do pacote</param>
        /// <returns>Objeto recém criado com caminho para o método de busca de detalhes</returns>
        /// <response code="201">Cadastro realizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        public async Task<IActionResult> Post(AddPackageInputModel model)
        {
            if (model.Title.Length < 10)
                return BadRequest("Tamanho do título deve ser maior que 10 caracteres");

            var package = new Package(model.Title, model.Weight);

            _repository.Add(package);

            var message = new SendGridMessage
            {
                From = new EmailAddress("samuelmaster10@hotmail.com", "SamNascimento"),
                Subject = "Seu pacote foi enviado",
                PlainTextContent = $"Seu pacote com o código {package.Code} foi enviado"
            };

            message.AddTo(model.SenderEmail, model.SenderName);

            await _client.SendEmailAsync(message);

            return CreatedAtAction(
                "GetByCode", 
                new { code = package.Code },
                package
                );
        }

        /// <summary>
        /// Cria um novo update para um pacote específico rastreando ele pelo code
        /// </summary>
        /// <remarks>
        /// {
        ///  "code": 1234
        ///  "Status": "Texto com o status atualizado do pacote",
        ///  "Delivered": True/False,
        ///  "senderName": "Samuel",
        ///  "senderEmail: "emailteste@teste.com"
        /// }
        /// </remarks>
        /// <param name="code">Código de rastreio do pacote</param>
        /// <param name="model">Dados do pacote</param>
        /// <returns>Não retorna conteúdo, apenas atualiza e persiste a mudança da atualização</returns>
        /// <response code="204">Atualização ocorreu com sucesso, não retorna dados</response>
        /// <response code="404">Nenhum objeto com esse código foi encontrado</response>
        [HttpPost("{code}/updates")]
        public async Task<IActionResult> PostUpdate(string code, AddPackageUpdateInputModel model)
        {
            var package = _repository.GetByCode(code);

            if (package == null)
            {
                return NotFound();
            }

            package.AddUpdate(model.Status, model.Delivered);

            _repository.Update(package);

            var message = new SendGridMessage
            {
                From = new EmailAddress("samuelmaster10@hotmail.com", "SamNascimento"),
                Subject = "Seu pacote foi atualizado",
                PlainTextContent = $"Seu pacote com o código {package.Code} foi atualizado e está com o status {model.Status}"
            };

            message.AddTo(model.SenderEmail, model.SenderName);

            await _client.SendEmailAsync(message);

            return NoContent();
        }
    }
}
