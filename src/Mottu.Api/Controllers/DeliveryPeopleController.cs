using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Mottu.Api.Dtos;
using Mottu.Api.Entities;
using Mottu.Api.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Mottu.Api.Controllers
{
    [ApiController]
    [Route("delivery-people")]
    public class DeliveryPeopleController : ControllerBase
    {
        private readonly IDeliveryPersonService _deliveryPersonService;
        private readonly IFileStorageService _fileStorageService;

        public DeliveryPeopleController(IDeliveryPersonService deliveryPersonService, IFileStorageService fileStorageService)
        {
            _deliveryPersonService = deliveryPersonService;
            _fileStorageService = fileStorageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeliveryPerson([FromBody] CreateDeliveryPersonDto createDeliveryPersonDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { mensagem = "Dados inválidos" });

            try
            {
                var deliveryPerson = await _deliveryPersonService.CreateDeliveryPersonAsync(createDeliveryPersonDto);
                return CreatedAtAction(nameof(GetDeliveryPersonById), new { id = deliveryPerson.Id }, deliveryPerson);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryPersonById(Guid id)
        {
            var deliveryPerson = await _deliveryPersonService.GetDeliveryPersonByIdAsync(id);
            if (deliveryPerson == null)
                return NotFound(new { mensagem = "Entregador não encontrado" });

            var isAdmin = (bool)(HttpContext.Items["IsAdmin"] ?? false);
            if (!isAdmin)
            {
                var username = HttpContext.Items["Username"]?.ToString();
                if (deliveryPerson.Identifier != username)
                {
                    return Forbid("Acesso negado: Só é possível visualizar o próprio perfil");
                }
            }

            return Ok(deliveryPerson);
        }

        [HttpPut("{id}/cnh")]
        public async Task<IActionResult> UpdateCnhImage(Guid id, IFormFile cnhImage)
        {
            if (cnhImage == null || cnhImage.Length == 0)
                return BadRequest(new { mensagem = "Arquivo de imagem é obrigatório" });

            try
            {
                var isAdmin = (bool)(HttpContext.Items["IsAdmin"] ?? false);
                if (!isAdmin)
                {
                    var username = HttpContext.Items["Username"]?.ToString();
                    var deliveryPerson = await _deliveryPersonService.GetDeliveryPersonByIdAsync(id);
                    if (deliveryPerson == null || deliveryPerson.Identifier != username)
                    {
                        return Forbid("Acesso negado: Só é possível atualizar o próprio perfil");
                    }
                }

                if (!_fileStorageService.IsValidImageFormat(cnhImage))
                    return BadRequest(new { mensagem = "Formato de arquivo inválido. Apenas PNG e BMP são permitidos" });

                var filePath = await _fileStorageService.SaveFileAsync(cnhImage, "cnh");

                var success = await _deliveryPersonService.UpdateCnhImageAsync(id, filePath);
                if (!success)
                    return NotFound(new { mensagem = "Entregador não encontrado" });

                return Ok(new
                {
                    mensagem = "Imagem da CNH atualizada com sucesso",
                    caminho_arquivo = filePath
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListDeliveryPeople()
        {
            var isAdmin = (bool)(HttpContext.Items["IsAdmin"] ?? false);
            
            if (isAdmin)
            {
                var deliveryPeople = await _deliveryPersonService.ListDeliveryPeopleAsync();
                return Ok(deliveryPeople);
            }
            else
            {
                var username = HttpContext.Items["Username"]?.ToString();
                var deliveryPerson = await _deliveryPersonService.GetDeliveryPersonByIdentifierAsync(username ?? "");
                
                if (deliveryPerson == null)
                    return NotFound(new { mensagem = "Entregador não encontrado" });
                
                return Ok(new[] { deliveryPerson });
            }
        }
    }
}