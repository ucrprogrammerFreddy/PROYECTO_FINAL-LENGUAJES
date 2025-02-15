using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mar_Azul_API.Models
{ 
    public class Articulos
    {
        [Key]
        public int IdArticulo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string Descripcion { get; set; }

        public string Contenido { get; set; }

        public string? ImagenUrl { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [RegularExpression("[A-Z]", ErrorMessage = "El estado debe ser un único carácter en mayúscula.")]
        public string  Estado { get; set; }

        //  Relación con Categoría (Un Artículo pertenece a una Categoría)
       public int IdCategoria { get; set; }
        public Categorias Categoria { get; set; }

        public DateTime FechaPublicacion { get; set; }
        // Relación Muchos a Muchos con Usuarios (Autores)
        public ICollection<ArticuloAutor> ArticuloAutor { get; set; } = new List<ArticuloAutor>();

        // Relación con Etiquetas
         

        public ICollection<ArticuloEtiqueta> ArticuloEtiqueta { get; set; }




    }
}