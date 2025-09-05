using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mottu.Api.Dtos;
using Mottu.Api.Entities;
using Mottu.Api.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mottu.Api.Controllers
{
    [ApiController]
    [Route("motorcycles")]
    public class MotorcyclesController : ControllerBase
    {
        private readonly IMotorcycleService _motorcycleService;
        private readonly ILogger<MotorcyclesController> _logger;

        public MotorcyclesController(IMotorcycleService motorcycleService, ILogger<MotorcyclesController> logger)
        {
            _motorcycleService = motorcycleService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMotorcycle([FromBody] CreateMotorcycleDto createMotorcycleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { mensagem = "Dados inválidos" });

            try
            {
                var motorcycle = await _motorcycleService.CreateMotorcycleAsync(createMotorcycleDto);
                _logger.LogInformation("Moto criada via API com ID {MotoId}", motorcycle.Id);
                return CreatedAtAction(nameof(GetMotorcycleById), new { id = motorcycle.Id }, motorcycle);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Falha ao criar moto: {Error}", ex.Message);
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMotorcycleById(Guid id)
        {
            var motorcycle = await _motorcycleService.GetMotorcycleByIdAsync(id);
            if (motorcycle == null)
                return NotFound(new { mensagem = "Moto não encontrada" });
            return Ok(motorcycle);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Motorcycle>>> ListMotorcycles([FromQuery] string? plate = null)
        {
            var motorcycles = await _motorcycleService.ListMotorcyclesAsync(plate);
            return Ok(motorcycles);
        }

        [HttpPut("{id}/plate")]
        public async Task<IActionResult> UpdateMotorcyclePlateById(Guid id, [FromBody] UpdateMotorcyclePlateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { mensagem = "Dados inválidos" });

            try
            {
                var success = await _motorcycleService.UpdateMotorcyclePlateByIdAsync(id, updateDto);
                if (!success)
                    return NotFound(new { mensagem = "Moto não encontrada" });
                return Ok(new { mensagem = "Placa modificada com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotorcycle(Guid id)
        {
            try
            {
                var success = await _motorcycleService.DeleteMotorcycleAsync(id);
                if (!success)
                    return NotFound(new { mensagem = "Moto não encontrada" });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }
    }
}