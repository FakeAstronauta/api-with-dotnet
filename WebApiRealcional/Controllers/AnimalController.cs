using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using WebApiRealcional.Data;

namespace WebApiRealcional.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly DataBaseContext _DbContext;

        public AnimalController(DataBaseContext dataBaseContext)
        {
            _DbContext = dataBaseContext;
        }

        [HttpGet("get")]
        public IActionResult getAnimal()
        {
                //obtener a todos los animales con su relacion 
            return Ok(_DbContext.animal.Include(a => a.Tipos).ToArray()) ;
        }

        [HttpPost("Insert")]
        public IActionResult insertAnimal([FromForm] Animal animalRequest)
        {
            try
            {
               var tipo = _DbContext.tipos.Where(c => c.TiposId == animalRequest.TiposId).FirstOrDefault(); 
                if (tipo == null)
                {
                    return StatusCode(400, "No se encontro el objeto a relacionar");
                }
                animalRequest.Tipos = tipo;
                _DbContext.animal.Add(animalRequest);
                _DbContext.SaveChanges();
                return Ok(animalRequest);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "Error:=> " + ex);
            }
        }

        [HttpPut("update/")]
        public IActionResult updateAnimal([FromForm] Animal animalRequest)
        { // que es TiposAnimales
            var tipo = _DbContext.tipos.Where(t => t.TiposId == animalRequest.TiposId).FirstOrDefault();
            var animal = _DbContext.animal.Where(t => t.AnimalId == animalRequest.AnimalId).FirstOrDefault();
                
            if (tipo != null && animal != null)
            {
                animal.Name = animalRequest.Name;
                tipo.Name = animalRequest.Name;
                _DbContext.animal.Update(animal);
                _DbContext.SaveChanges();
                return Ok(tipo);

            }
            return StatusCode(400, $"El elemento con id {animalRequest.AnimalId} no se encuentra");
        }

        [HttpDelete("delete/")]
        public IActionResult deleteAnimal(int id)
        {
            var animal = _DbContext.animal.Where(t => t.AnimalId == id).FirstOrDefault();

            if (animal != null)
            {
                _DbContext.animal.Remove(animal);
                _DbContext.SaveChanges();
                return Ok(animal);

            }
            return StatusCode(400, "el id no pertenece a ningun registro");
        }

        //Consumiendo este metodo lo puse aqui por que tenia hueva de hacer mas
        [HttpPost("Pruebas/")]
        public IActionResult pruebas([FromBody] UserEmail request)
        {
            var response = new {
               mensaje = "Email generado correctamente",
               statusCode = 200,
               Email = request.generateEmail()                
            };

            return Ok(response);
        }

    }
}
