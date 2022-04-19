using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Services.ShoppingCartAPI.Models
{
    public class Product
    {
        //Llave obtenida de la base de datos del producto
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, 1000)]
        public double Price { get; set; }
        
        public string Description { get; set; }
        
        public string CategoryName { get; set; }
        
        public string ImageUrl { get; set; }

    }
}
