
using EcoHogar_API.Models;
using EcoHogar_API.Services;
using Mar_Azul_API.Data;
using Mar_Azul_API.DTO;
using Mar_Azul_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EcoHogar_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DbContextEditorial _context;
        private readonly FTPService _ftpService;

        public UsuarioController(DbContextEditorial context, FTPService ftpService)
        {
            _context = context;
            _ftpService = ftpService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                                         .Select(u => new UsuarioDTO
                                         {
                                             IdUsuario = u.IdUsuario,
                                             Nombre = u.Nombre,
                                             Rol = u.Rol,
                                             Email = u.Email,
                                             Estado = u.Estado
                                         })
                                         .ToListAsync();

            return Ok(usuarios);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<UsuarioDTO>> PatchUsuario(int id, [FromForm] UsuarioDTO usuarioDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(usuarioDto.Nombre))
            {
                usuario.Nombre = usuarioDto.Nombre;
            }

            if (!string.IsNullOrEmpty(usuarioDto.Estado))
            {
                usuario.Estado = usuarioDto.Estado;
            }

            if (!string.IsNullOrEmpty(usuarioDto.Rol))
            {
                usuario.Rol = usuarioDto.Rol;
            }

            if (!string.IsNullOrEmpty(usuarioDto.Email))
            {
                usuario.Email = usuarioDto.Email;
            }

            // Marcar el usuario como modificado
            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Mapear el usuario actualizado a UsuarioDTO
            var updatedUsuarioDto = new UsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Estado = usuario.Estado
            };

            return Ok(updatedUsuarioDto);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }




    }
}
