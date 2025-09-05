using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Data;
using Mottu.Api.Dtos;
using Mottu.Api.Entities;
using Mottu.Api.Enums;
using Mottu.Api.Services;
using System;
using System.Threading.Tasks;

namespace Mottu.Api.Controllers
{
    [ApiController]
    [Route("rentals")]
    public class RentalsController : ControllerBase
    {
        private readonly MottuDbContext _context;
        private readonly IDeliveryPersonService _deliveryPersonService;

        public RentalsController(MottuDbContext context, IDeliveryPersonService deliveryPersonService)
        {
            _context = context;
            _deliveryPersonService = deliveryPersonService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRental([FromBody] CreateRentalDto createRentalDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { mensagem = "Dados inválidos" });

            try
            {
                var deliveryPerson = await _context.DeliveryPeople
                    .FirstOrDefaultAsync(e => e.Identifier == createRentalDto.DeliveryPersonIdentifier);

                if (deliveryPerson == null)
                    return BadRequest(new { mensagem = "Entregador não encontrado" });

                var isQualified = await _deliveryPersonService.IsQualifiedForRentalAsync(deliveryPerson.Id);
                if (!isQualified)
                    return BadRequest(new { mensagem = "Entregador não qualificado para alugar moto" });

                var motorcycle = await _context.Motorcycles
                    .FirstOrDefaultAsync(m => m.Identifier == createRentalDto.MotorcycleIdentifier);

                if (motorcycle == null)
                    return BadRequest(new { mensagem = "Moto não encontrada" });

                var motorcycleAvailable = !await _context.Rentals.AnyAsync(r => r.MotorcycleId == motorcycle.Id && r.ReturnDate == null);
                if (!motorcycleAvailable)
                    return BadRequest(new { mensagem = "Moto não está disponível para locação" });


                decimal dailyRate = GetDailyRate((int)createRentalDto.Plan);

                var expectedStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
                if (createRentalDto.StartDate != expectedStartDate)
                    return BadRequest(new { mensagem = "Data de início deve ser o primeiro dia após a data de criação" });

                var expectedEndDate = createRentalDto.StartDate.AddDays((int)createRentalDto.Plan - 1);
                if (createRentalDto.ExpectedEndDate != expectedEndDate)
                    return BadRequest(new { mensagem = "Data de previsão de término deve ser igual à data calculada pelo plano" });

                if (createRentalDto.EndDate < createRentalDto.StartDate)
                    return BadRequest(new { mensagem = "Data de término não pode ser anterior à data de início" });

                var rental = new Rental
                {
                    Id = Guid.NewGuid(),
                    Identifier = $"locacao{Guid.NewGuid().ToString("N")[..8]}",
                    DailyRate = dailyRate,
                    DeliveryPersonIdentifier = createRentalDto.DeliveryPersonIdentifier,
                    MotorcycleIdentifier = createRentalDto.MotorcycleIdentifier,
                    StartDate = createRentalDto.StartDate,
                    EndDate = createRentalDto.EndDate,
                    ExpectedEndDate = createRentalDto.ExpectedEndDate,
                    DeliveryPersonId = deliveryPerson.Id,
                    MotorcycleId = motorcycle.Id,
                    TotalCost = (int)createRentalDto.Plan * dailyRate
                };

                await _context.Rentals.AddAsync(rental);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRentalById), new { id = rental.Id }, rental);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRentalById(Guid id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
                return NotFound(new { mensagem = "Locação não encontrada" });

            var isAdmin = (bool)(HttpContext.Items["IsAdmin"] ?? false);
            if (!isAdmin)
            {
                var username = HttpContext.Items["Username"]?.ToString();
                var deliveryPerson = await _context.DeliveryPeople
                    .FirstOrDefaultAsync(dp => dp.Identifier == username);
                
                if (deliveryPerson == null || rental.DeliveryPersonId != deliveryPerson.Id)
                {
                    return Forbid("Acesso negado: Só é possível visualizar as próprias locações");
                }
            }

            return Ok(rental);
        }

        [HttpGet]
        public async Task<IActionResult> ListRentals()
        {
            var isAdmin = (bool)(HttpContext.Items["IsAdmin"] ?? false);
            
            if (isAdmin)
            {
                var rentals = await _context.Rentals.ToListAsync();
                return Ok(rentals);
            }
            else
            {
                var username = HttpContext.Items["Username"]?.ToString();
                var deliveryPerson = await _context.DeliveryPeople
                    .FirstOrDefaultAsync(dp => dp.Identifier == username);
                
                if (deliveryPerson == null)
                    return NotFound(new { mensagem = "Entregador não encontrado" });
                
                var rentals = await _context.Rentals
                    .Where(r => r.DeliveryPersonId == deliveryPerson.Id)
                    .ToListAsync();
                
                return Ok(rentals);
            }
        }

        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnRental(Guid id, [FromBody] ReturnRentalDto returnDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { mensagem = "Dados inválidos" });

            try
            {
                var rental = await _context.Rentals.FindAsync(id);
                if (rental == null)
                    return NotFound(new { mensagem = "Locação não encontrada" });

                var isAdmin = (bool)(HttpContext.Items["IsAdmin"] ?? false);
                if (!isAdmin)
                {
                    var username = HttpContext.Items["Username"]?.ToString();
                    var deliveryPerson = await _context.DeliveryPeople
                        .FirstOrDefaultAsync(dp => dp.Identifier == username);
                    
                    if (deliveryPerson == null || rental.DeliveryPersonId != deliveryPerson.Id)
                    {
                        return Forbid("Acesso negado: Só é possível devolver as próprias locações");
                    }
                }

                if (returnDto.ReturnDate < rental.StartDate)
                    return BadRequest(new { mensagem = "Data de devolução não pode ser anterior à data de início" });

                var calculationResult = CalculateRentalTotal(rental, returnDto.ReturnDate);

                rental.ReturnDate = returnDto.ReturnDate;
                rental.TotalCost = calculationResult.TotalCost;
                _context.Rentals.Update(rental);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensagem = "Data de devolução informada com sucesso",
                    valor_total = calculationResult.TotalCost,
                    dias_utilizados = calculationResult.DaysUsed,
                    multa = calculationResult.Penalty,
                    dias_extras = calculationResult.ExtraDays
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{id}/total-value")]
        public async Task<IActionResult> GetRentalTotalValue(Guid id)
        {
            try
            {
                var rental = await _context.Rentals.FindAsync(id);
                if (rental == null)
                    return NotFound(new { mensagem = "Locação não encontrada" });

                var isAdmin = (bool)(HttpContext.Items["IsAdmin"] ?? false);
                if (!isAdmin)
                {
                    var username = HttpContext.Items["Username"]?.ToString();
                    var deliveryPerson = await _context.DeliveryPeople
                        .FirstOrDefaultAsync(dp => dp.Identifier == username);
                    
                    if (deliveryPerson == null || rental.DeliveryPersonId != deliveryPerson.Id)
                    {
                        return Forbid("Acesso negado: Só é possível visualizar as próprias locações");
                    }
                }

                if (!rental.ReturnDate.HasValue)
                    return BadRequest(new { mensagem = "Data de devolução não informada" });

                var calculationResult = CalculateRentalTotal(rental, rental.ReturnDate.Value);

                return Ok(new
                {
                    valor_total = calculationResult.TotalCost,
                    dias_utilizados = calculationResult.DaysUsed,
                    multa = calculationResult.Penalty,
                    dias_extras = calculationResult.ExtraDays,
                    valor_diaria = rental.DailyRate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        private decimal GetDailyRate(int plan)
        {
            return plan switch
            {
                7 => 30.00m,
                15 => 28.00m,
                30 => 22.00m,
                45 => 20.00m,
                50 => 18.00m,
                _ => throw new ArgumentException("Plano inválido.")
            };
        }

        private decimal GetPenaltyRate(int plan)
        {
            return plan switch
            {
                7 => 0.20m,
                15 => 0.40m,
                _ => 0
            };
        }

        private RentalCalculationResult CalculateRentalTotal(Rental rental, DateOnly returnDate)
        {
            var daysUsed = returnDate.DayNumber - rental.StartDate.DayNumber + 1;
            var plannedDays = rental.ExpectedEndDate.DayNumber - rental.StartDate.DayNumber + 1;

            var baseCost = daysUsed * rental.DailyRate;
            var penalty = 0m;
            var extraDays = 0;
            var extraCost = 0m;

            if (returnDate < rental.ExpectedEndDate)
            {
                var unusedDays = plannedDays - daysUsed;
                var unusedCost = unusedDays * rental.DailyRate;
                var penaltyRate = GetPenaltyRate(plannedDays);
                penalty = unusedCost * penaltyRate;
            }
            else if (returnDate > rental.ExpectedEndDate)
            {
                extraDays = returnDate.DayNumber - rental.ExpectedEndDate.DayNumber;
                extraCost = extraDays * 50.00m;
            }

            var totalCost = baseCost + penalty + extraCost;

            return new RentalCalculationResult
            {
                DaysUsed = daysUsed,
                BaseCost = baseCost,
                Penalty = penalty,
                ExtraDays = extraDays,
                ExtraCost = extraCost,
                TotalCost = totalCost
            };
        }
    }

    public class RentalCalculationResult
    {
        public int DaysUsed { get; set; }
        public decimal BaseCost { get; set; }
        public decimal Penalty { get; set; }
        public int ExtraDays { get; set; }
        public decimal ExtraCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}