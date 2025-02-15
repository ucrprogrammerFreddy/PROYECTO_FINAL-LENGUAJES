using EcoHogar_API.Services;
using Mar_Azul_API.Data;
using Mar_Azul_API.DTO;
using Mar_Azul_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mar_Azul_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ArticuloController : ControllerBase
    {
        private readonly DbContextEditorial _dbContext;
        private readonly FTPService _ftpService;

        public ArticuloController(DbContextEditorial dbContext, FTPService ftpService)
        {
            _dbContext = dbContext;
            _ftpService = ftpService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloDTO>>> GetArticulo()
        {
            try
            {
                var articulo = await _dbContext.Articulos
                    .Include(p => p.Categoria)
                    .ToListAsync();

                // Mapear articulos a ArticulosDTO 
                var ArticuloDto = articulo.Select(a => new ArticuloDTO
                {
                    IdArticulo = a.IdArticulo,  // en primer lugar se tiene el dto y luego p. de la bd
                    Nombre = a.Nombre,
                    Descripcion = a.Descripcion,
                    Contenido = a.Contenido,
                    Estado = a.Estado,
                    Categoria = a.Categoria.Nombre, // Solo devolver el nombre de la categoría
                    ImagenUrl = a.ImagenUrl
                  
                }).ToList();

                return Ok(ArticuloDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error al obtener los articulos: {ex.Message}" });
            }
        }



        [HttpPost]
        public async Task<ActionResult<Articulos>> PostArticulo([FromForm] Articulos articulo, [FromForm] IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0)
            {
                return BadRequest(new { error = "El campo 'imagen' es obligatorio." });
            }


            var categoria = await _dbContext.Categorias.FindAsync(articulo.IdCategoria);
            if (categoria == null)
            {
                return BadRequest(new { error = "La categoría especificada no existe." });
            }

            articulo.Categoria = categoria;

            try
            {

                string localFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(localFilePath, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }


                var (imagenUrl, errorMessage) = await _ftpService.UploadFileAsync(localFilePath, imagen.FileName);

                System.IO.File.Delete(localFilePath);

                if (string.IsNullOrEmpty(imagenUrl))
                {
                    return StatusCode(500, new { error = $"Ocurrió un error al subir la imagen al servidor FTP: {errorMessage}" });
                }

                articulo.ImagenUrl = imagenUrl;


                _dbContext.Articulos.Add(articulo);
                await _dbContext.SaveChangesAsync();


                var articuloDto = new ArticuloDTO
                {
                    IdArticulo = articulo.IdArticulo,
                    Nombre = articulo.Nombre,
                    Descripcion = articulo.Descripcion,
                    Contenido = articulo.Contenido,
                    Estado=articulo.Estado,
                    Categoria = articulo.Categoria.Nombre,
                    ImagenUrl = articulo.ImagenUrl,
                    

                };

                return CreatedAtAction(nameof(GetArticulo), new { id = articulo.IdArticulo }, articuloDto);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = $"Ocurrió un error al crear el articulo: {ex.Message}" });
            }
        }

        // Para obtener varios articulos de forma aleatoria 
        [HttpGet("action/random")]
        public async Task<ActionResult<IEnumerable<ArticuloDTO>>> GetRandomArticulo()
        {
            var productos = await _dbContext.Articulos
                .Include(p => p.Categoria)
                .OrderBy(x => Guid.NewGuid()) // Orden aleatorio
                .Take(3) // Seleccionar 3
                .ToListAsync();

            var articuloDto = productos.Select(p => new ArticuloDTO
            {
                IdArticulo = p.IdArticulo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Contenido = p.Contenido,
                Categoria = p.Categoria.Nombre,
                ImagenUrl = p.ImagenUrl,
                Estado = p.Estado 


            }).ToList();

            return Ok(articuloDto);
        }

       

        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticulo(int id, [FromForm] Articulos articulo, [FromForm] IFormFile imagen)
        {
            if (id != articulo.IdArticulo)
            {
                return BadRequest(new { error = "El ID del articulo no coincide con el ID proporcionado en la URL." });
            }


            var articuloExistente = await _dbContext.Articulos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.IdArticulo == id);
            if (articuloExistente == null)
            {
                return NotFound(new { error = "El articulo no existe." });
            }


            var categoria = await _dbContext.Categorias.FindAsync(articulo.IdCategoria);
            if (categoria == null)
            {
                return BadRequest(new { error = "La categoría especificada no existe." });
            }


            articuloExistente.Nombre = articulo.Nombre;
            articuloExistente.Descripcion = articulo.Descripcion;
            articuloExistente.Contenido = articulo.Contenido;
            articuloExistente.IdCategoria = articulo.IdCategoria;
            articuloExistente.Categoria = categoria;
            articuloExistente.Estado =articulo.Estado;

            try
            {
                if (imagen != null && imagen.Length > 0)
                {

                    string localFilePath = Path.GetTempFileName();
                    using (var stream = new FileStream(localFilePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(stream);
                    }

                    var (imagenUrl, errorMessage) = await _ftpService.UploadFileAsync(localFilePath, imagen.FileName);

                    System.IO.File.Delete(localFilePath);

                    if (string.IsNullOrEmpty(imagenUrl))
                    {
                        return StatusCode(500, new { error = $"Ocurrió un error al subir la imagen al servidor FTP: {errorMessage}" });
                    }

                    if (!string.IsNullOrEmpty(articuloExistente.ImagenUrl))
                    {
                        string remoteFileName = articuloExistente.ImagenUrl.Split('/').Last();
                        string folder = articuloExistente.ImagenUrl.Contains("/images/") ? "images/" : "";
                        await _ftpService.DeleteFileAsync($"{folder}{remoteFileName}");
                    }

                    articuloExistente.ImagenUrl = imagenUrl;
                }


                _dbContext.Entry(articuloExistente).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                var articuloDto = new ArticuloDTO
                {
                    IdArticulo = articuloExistente.IdArticulo,
                    Nombre = articuloExistente.Nombre,
                    Descripcion = articuloExistente.Descripcion,
                    Contenido = articuloExistente.Contenido,
                    Categoria = articuloExistente.Categoria.Nombre,
                    ImagenUrl = articuloExistente.ImagenUrl,
                    Estado = articuloExistente.Estado
                };

                return Ok(articuloDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error al actualizar el producto: {ex.Message}" });
            }
        }



        // DELETE: api/producto/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticulo(int id)
        {
            var articulo = await _dbContext.Articulos.FindAsync(id);
            if (articulo == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(articulo.ImagenUrl))
            {
                string remoteFileName = articulo.ImagenUrl.Split('/').Last();
                await _ftpService.DeleteFileAsync(remoteFileName);
            }

            _dbContext.Articulos.Remove(articulo);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticuloDTO>> GeArticulo(int id)
        {
            var articulo = await _dbContext.Articulos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.IdArticulo == id);

            if (articulo == null)
            {
                return NotFound();
            }

            // Mapear producto a ProductoDto
            var productoDto = new ArticuloDTO
            {
                


                 IdArticulo = articulo.IdArticulo,
                Nombre = articulo.Nombre,
                Descripcion = articulo.Descripcion,
                Contenido = articulo.Contenido,
                Categoria = articulo.Categoria.Nombre,
                ImagenUrl = articulo.ImagenUrl,
                Estado = articulo.Estado

            };

            return Ok(productoDto);
        }








    }
}
