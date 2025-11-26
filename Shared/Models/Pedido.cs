using System;

namespace Shared.Models
{
    public class Pedido
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NomeCliente { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; } = "pendente";
    }
}
