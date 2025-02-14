using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;


namespace Mar_Azul_API.Models
{ 
    public class ArticuloAutor
    {
        
       
    

    
        public int IdArticulo { get; set; }
       public Articulos Articulo { get; set; }

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }




    }
}