﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class entPaciente
    {
        public int IdPaciente { get; set; }
        public int? IdUsuario { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string? DNI { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool Estado { get; set; }
        public string NombreCompleto => $"{Apellidos}, {Nombres}";
    }
}
