using CapaLogica;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoCalidadSoftware.Controllers
{
    public class bebeController : Controller
    {
        public IActionResult listar()
        {
         
            var listaBebes = logBebe.Instancia.ListarBebe();
            ViewBag.Lista = listaBebes;
            return View(listaBebes);


           
        }
    
    }
}
