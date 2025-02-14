using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mar_Azul_API.Models
{
    public class Categorias
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(600, ErrorMessage = "La descripción no puede exceder los 600 caracteres.")]
        public string Descripcion { get; set; }

        public string UrlImagen { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [RegularExpression("[A-Z]", ErrorMessage = "El estado debe ser un único carácter en mayúscula.")]
        public char Estado { get; set; }

        // categoria a una sola seccion 
       public int IdSeccion { get; set; }
        public Secciones Seccion { get; set; }
        

        // ✅ Relación Uno a Muchos: Una Categoría tiene muchos Artículos
        public ICollection<Articulos> Articulo { get; set; } = new List<Articulos>();

        // ✅ Relación con Secciones (Una Sección tiene muchas Categorías)

        

       
        
    }
}
