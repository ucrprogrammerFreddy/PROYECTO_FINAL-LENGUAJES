using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using Mar_Azul_API.Data;
using Mar_Azul_API.Models;


namespace Mar_Azul_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class EtiquetasController : ControllerBase
    {
        private readonly DbContextEditorial _context;


        public EtiquetasController(DbContextEditorial context)
        {
            _context = context;
        }



        /// Método auxiliar para normalizar un string:
        /// elimina acentos, espacios y convierte a minúsculas.
        /// Esto permite comparar nombres de forma uniforme.
        /// </summary>
        /// <param name="input">Texto a normalizar</param>
      
        private static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Normalizar a FormD para descomponer caracteres con diacríticos
            string normalized = input.Normalize(NormalizationForm.FormD);

            // Usar StringBuilder para eliminar acentos
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(char.ToLowerInvariant(c)); // Convertir a minúsculas directamente
            }

            // Convertir a string y eliminar espacios en blanco
            return Regex.Replace(sb.ToString(), @"\s+", "");
        }



        /// <summary>
        /// GET: api/Etiquetas/GetEtiquetas  
        /// Retorna la lista completa de etiquetas.
        /// </summary>
        [HttpGet("GetEtiquetas")]
        public async Task<ActionResult<IEnumerable<Etiquetas>>> GetEtiquetas()
        {
            var etiquetas = await _context.Etiquetas.ToListAsync();
            return Ok(etiquetas);
        }

        /// <summary>
        /// GET: api/Etiquetas/GetEtiquetaForId/{idEtiqueta}  
        /// Retorna la etiqueta cuyo ID coincide con el proporcionado.
        /// </summary>
        [HttpGet("GetEtiquetaForId/{idEtiqueta}")]
        public async Task<ActionResult<Etiquetas>> GetEtiquetaForId(int idEtiqueta)
        {
            var etiqueta = await _context.Etiquetas.FindAsync(idEtiqueta);
            if (etiqueta == null)
            {
                return NotFound(new { message = "Etiqueta no encontrada." });
            }
            return Ok(etiqueta);
        }

        /// <summary>
        /// GET: api/Etiquetas/GetEtiquetaForName/{nombre}  
        /// Retorna las etiquetas que tengan el nombre especificado, 
        /// considerando la comparación sin distinguir mayúsculas, acentos y espacios.
        /// </summary>
        [HttpGet("GetEtiquetaForName/{nombre}")]
        public async Task<ActionResult<IEnumerable<Etiquetas>>> GetEtiquetaForName(string nombre)
        {
            // Normalizar el nombre de búsqueda
            string normalizedNombre = Normalize(nombre);

            // Obtener los datos de la base de datos primero y luego filtrar en memoria
            var etiquetas = new List<Etiquetas>();
                etiquetas = (await _context.Etiquetas
                .AsNoTracking() // Optimización: evita tracking en la consulta
                .ToListAsync()) // Se ejecuta la consulta en la base de datos
                .Where(e => Normalize(e.Nombre) == normalizedNombre) // Filtra en memoria
                .ToList(); // Convierte a lista final

            // En lugar de `NotFound()`, devuelve una lista vacía para evitar el error en el cliente.
            return Ok(etiquetas);

        }

        /// <summary>
        /// POST: api/Etiquetas/AddEtiqueta  
        /// Crea una nueva etiqueta en la base de datos, validando que no exista 
        /// otra etiqueta con el mismo nombre (ignorando mayúsculas, acentos y espacios).
        /// </summary>
        [HttpPost("AddEtiqueta")]
        //FromBody
        //Sirve para que la api 
        public async Task<ActionResult<Etiquetas>> AddEtiqueta([FromBody] Etiquetas etiqueta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            etiqueta.IdEtiqueta = 0;

            // Normalizar el nombre de la nueva etiqueta
            string normalizedNewName = Normalize(etiqueta.Nombre);

            // Obtener solo los nombres normalizados en memoria y verificar si existe duplicado
            bool exists = (await _context.Etiquetas
                .Select(e => Normalize(e.Nombre)) // Solo obtener nombres normalizados
                .ToListAsync()) // Ejecutar en memoria
                .Contains(normalizedNewName); // Comparar con el nuevo nombre
            
            if (exists)
            {
                return Conflict(new { message = "No se pueden repetir nombres de etiquetas." });
            }

            _context.Etiquetas.Add(etiqueta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEtiquetaForId), new { idEtiqueta = etiqueta.IdEtiqueta }, etiqueta);
        }

        /// <summary>
        /// PUT: api/Etiquetas/UpdateEtiqueta/{idEtiqueta}  
        /// Actualiza la información de una etiqueta existente, validando que el nuevo nombre 
        /// no genere duplicados (ignorando mayúsculas, acentos y espacios).
        /// </summary>
        [HttpPut("UpdateEtiqueta/{idEtiqueta}")]
        public async Task<IActionResult> UpdateEtiqueta(int idEtiqueta, [FromBody] Etiquetas etiqueta)
        {
            if (idEtiqueta != etiqueta.IdEtiqueta)
            {
                return BadRequest(new { message = "El ID de la etiqueta no coincide." });
            }

            // Verificar que no exista otra etiqueta (con distinto id) con el mismo nombre normalizado
            // Normalizar el nombre de la etiqueta que se va a actualizar
            string normalizedUpdatedName = Normalize(etiqueta.Nombre);

            // Obtener todas las etiquetas y hacer la verificación en memoria
            bool duplicateExists = (await _context.Etiquetas
                .Where(e => e.IdEtiqueta != idEtiqueta) // Filtrar antes de traer datos a memoria
                .Select(e => Normalize(e.Nombre)) // Obtener solo nombres normalizados
                .ToListAsync()) // Ejecutar en la base de datos antes de aplicar Normalize()
                .Contains(normalizedUpdatedName); // Comparar en memoria

            if (duplicateExists)
            {
                return Conflict(new { message = "No se pueden repetir nombres de etiquetas." });
            }


            _context.Entry(etiqueta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EtiquetaExists(idEtiqueta))
                {
                    return NotFound(new { message = "Etiqueta no encontrada." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// DELETE: api/Etiquetas/DeleteEtiqueta/{idEtiqueta}  
        /// Elimina la etiqueta con el ID especificado.
        /// </summary>
        [HttpDelete("DeleteEtiqueta/{idEtiqueta}")]
        public async Task<IActionResult> DeleteEtiqueta(int idEtiqueta)
        {
            var etiqueta = await _context.Etiquetas.FindAsync(idEtiqueta);
            if (etiqueta == null)
            {
                return NotFound(new { message = "Etiqueta no encontrada." });
            }

            _context.Etiquetas.Remove(etiqueta);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Etiqueta eliminada correctamente." });
        }

        /// <summary>
        /// Verifica si existe una etiqueta con el ID proporcionado.
        /// </summary>
        /// <param name="id">ID de la etiqueta</param>
        /// <returns>True si existe, false en caso contrario</returns>
        private bool EtiquetaExists(int id)
        {
            return _context.Etiquetas.Any(e => e.IdEtiqueta == id);
        }


    }
}//Cierre de namespace
