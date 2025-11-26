using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;
using PedidoApi.Services;

namespace PedidoApi.Controllers
{
    [ApiController]
    [Route("pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RabbitMqService _rabbit;

        public PedidosController(AppDbContext context, RabbitMqService rabbit)
        {
            _context = context;
            _rabbit = rabbit;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] Pedido pedido)
        {
            try
            {
                if (pedido == null)
                    return BadRequest("Pedido não pode ser nulo.");

                if (string.IsNullOrWhiteSpace(pedido.Descricao))
                    return BadRequest("Descrição é obrigatória.");

                if (pedido.Valor < 0)
                    return BadRequest("Valor do pedido inválido!");

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                await _rabbit.PublishAsync("pedidos_queue", pedido.Id.ToString());

                return Ok(pedido);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao criar pedido: {e.Message}");
                return StatusCode(500, "Erro interno ao processar o pedido.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var pedidos = await _context.Pedidos.ToListAsync();
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }

    }
}
