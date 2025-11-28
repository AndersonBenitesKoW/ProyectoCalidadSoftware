using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CapaEntidad;
using System;
using System.Linq; // Necesario para .Any()

namespace ProyectoCalidadSoftware.Services
{
    public interface IPdfService
    {
        byte[] GenerateCitaPdf(entCita cita);
        byte[] GenerateControlPrenatalPdf(entControlPrenatal control);
        byte[] GenerateSeguimientoPuerperioPdf(entSeguimientoPuerperio seguimiento);
        byte[] GeneratePartoPdf(entParto parto);
    }

    public class PdfService : IPdfService
    {
        // ==========================================
        // CONFIGURACIÓN DE ESTILOS Y COLORES
        // ==========================================
        static IContainer EstiloCeldaEtiqueta(IContainer container)
        {
            return container
                .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                .Background(Colors.Grey.Lighten4)
                .Padding(5);
        }

        static IContainer EstiloCeldaValor(IContainer container)
        {
            return container
                .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                .Padding(5);
        }

        // ==========================================
        // 1. REPORTE DE CITA MÉDICA
        // ==========================================
        public byte[] GenerateCitaPdf(entCita cita)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurarPagina(page, "FICHA DE CITA MÉDICA", Colors.Blue.Medium);

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        DefinirColumnas(table);

                        // Secciones
                        CrearTituloSeccion(table, "INFORMACIÓN GENERAL");
                        CrearFila(table, "ID Cita", cita.IdCita.ToString());
                        CrearFila(table, "Fecha y Hora", cita.FechaCita.ToString("dd/MM/yyyy HH:mm"));
                        CrearFila(table, "Estado Actual", cita.NombreEstado);
                        CrearFila(table, "Profesional", cita.NombreProfesional);

                        CrearTituloSeccion(table, "DATOS DEL PACIENTE");
                        CrearFila(table, "Nombre Completo", cita.NombrePaciente);
                        CrearFila(table, "ID Paciente", cita.IdPaciente.ToString());

                        CrearTituloSeccion(table, "DETALLES DE LA CONSULTA");
                        CrearFila(table, "Motivo de Consulta", cita.Motivo);
                        CrearFila(table, "Observaciones", cita.Observacion);

                        if (cita.FechaAnulacion.HasValue)
                        {
                            CrearTituloSeccion(table, "DATOS DE ANULACIÓN");
                            CrearFila(table, "Fecha Anulación", cita.FechaAnulacion?.ToString("dd/MM/yyyy"));
                            CrearFila(table, "Motivo Anulación", cita.MotivoAnulacion);
                        }
                    });

                    AgregarFooter(page);
                });
            }).GeneratePdf();
        }

        // ==========================================
        // 2. REPORTE DE CONTROL PRENATAL
        // ==========================================
        public byte[] GenerateControlPrenatalPdf(entControlPrenatal control)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurarPagina(page, "FICHA DE CONTROL PRENATAL", Colors.Green.Medium);

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        DefinirColumnas(table);

                        CrearTituloSeccion(table, "DATOS GENERALES");
                        CrearFila(table, "Paciente", control.NombrePaciente);
                        CrearFila(table, "Profesional", control.NombreProfesional);
                        CrearFila(table, "Fecha de Atención", control.Fecha.ToString("dd/MM/yyyy"));
                        CrearFila(table, "Nro. Control", control.NumeroControl?.ToString());
                        CrearFila(table, "Edad Gestacional", $"{control.EdadGestSemanas} sem. {control.EdadGestDias} días");
                        CrearFila(table, "Cálculo EG por", control.MetodoEdadGest);

                        CrearTituloSeccion(table, "ANTROPOMETRÍA");
                        CrearFila(table, "Peso Actual (kg)", control.PesoKg?.ToString("F2"));
                        CrearFila(table, "Peso Pre-Gestacional", control.PesoPreGestacionalKg?.ToString("F2"));
                        CrearFila(table, "Talla (m)", control.TallaM?.ToString("F2"));
                        CrearFila(table, "IMC Pre-Gestacional", control.IMCPreGestacional?.ToString("F2"));

                        CrearTituloSeccion(table, "SIGNOS VITALES");
                        CrearFila(table, "Presión Arterial", $"{control.PA_Sistolica}/{control.PA_Diastolica} mmHg");
                        CrearFila(table, "Pulso (lpm)", control.Pulso?.ToString());
                        CrearFila(table, "Frec. Respiratoria", control.FrecuenciaRespiratoria?.ToString());
                        CrearFila(table, "Temperatura (°C)", control.Temperatura?.ToString());

                        CrearTituloSeccion(table, "EXAMEN OBSTÉTRICO");
                        CrearFila(table, "Altura Uterina (cm)", control.AlturaUterina_cm?.ToString());
                        CrearFila(table, "Dinámica Uterina", control.DinamicaUterina);
                        CrearFila(table, "Presentación", control.Presentacion);
                        CrearFila(table, "Frec. Cardiaca Fetal", control.FCF_bpm?.ToString());
                        CrearFila(table, "Movimientos Fetales", "__________________"); // Campo manual sugerido

                        CrearTituloSeccion(table, "LABORATORIO Y EXÁMENES");
                        CrearFila(table, "Hemoglobina", control.Hemoglobina?.ToString());
                        CrearFila(table, "Grupo Sanguíneo", control.GrupoSanguineoRh);
                        CrearFila(table, "VIH", control.ResultadoVIH);
                        CrearFila(table, "Sífilis", control.ResultadoSifilis);
                        CrearFila(table, "Proteinuria", control.Proteinuria);

                        CrearTituloSeccion(table, "PLAN Y MANEJO");
                        CrearFila(table, "Micronutrientes", control.MicronutrientesEntregados);
                        CrearFila(table, "Consejerías", control.Consejerias);
                        CrearFila(table, "Próxima Cita", control.ProximaCitaFecha?.ToString("dd/MM/yyyy"));
                        CrearFila(table, "Observaciones", control.Observaciones);
                    });

                    AgregarFooter(page);
                });
            }).GeneratePdf();
        }

        // ==========================================
        // 3. REPORTE DE SEGUIMIENTO PUERPERIO
        // ==========================================
        public byte[] GenerateSeguimientoPuerperioPdf(entSeguimientoPuerperio seguimiento)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurarPagina(page, "FICHA DE SEGUIMIENTO PUERPERIO", Colors.Purple.Medium);

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        DefinirColumnas(table);

                        CrearTituloSeccion(table, "DATOS GENERALES");
                        CrearFila(table, "Paciente", seguimiento.NombrePaciente);
                        CrearFila(table, "Profesional", seguimiento.NombreProfesional);
                        CrearFila(table, "Fecha Atención", seguimiento.Fecha.ToString("dd/MM/yyyy"));
                        CrearFila(table, "Días Postparto", seguimiento.DiasPosparto?.ToString());

                        CrearTituloSeccion(table, "EVALUACIÓN CLÍNICA");
                        CrearFila(table, "Presión Arterial", $"{seguimiento.PA_Sistolica}/{seguimiento.PA_Diastolica}");
                        CrearFila(table, "Temperatura (°C)", seguimiento.Temp_C?.ToString());
                        CrearFila(table, "Altura Uterina (cm)", seguimiento.AlturaUterinaPP_cm?.ToString());
                        CrearFila(table, "Involución Uterina", seguimiento.InvolucionUterina);
                        CrearFila(table, "Loquios (Características)", seguimiento.Loquios);
                        CrearFila(table, "Hemorragia Residual", FormatBool(seguimiento.HemorragiaResidual));
                        CrearFila(table, "Signos de Infección", FormatBool(seguimiento.SignosInfeccion));

                        CrearTituloSeccion(table, "SALUD MENTAL Y LACTANCIA");
                        CrearFila(table, "Tamizaje Depresión", seguimiento.TamizajeDepresion);
                        CrearFila(table, "Lactancia Materna", seguimiento.Lactancia);
                        CrearFila(table, "Apoyo en Lactancia", FormatBool(seguimiento.ApoyoLactancia));

                        CrearTituloSeccion(table, "PLANIFICACIÓN Y OTROS");
                        CrearFila(table, "Consejería PF", FormatBool(seguimiento.ConsejoPlanificacion));
                        CrearFila(table, "Método Elegido", seguimiento.NombreMetodoPF);
                        CrearFila(table, "Complicaciones", seguimiento.ComplicacionesMaternas);
                        CrearFila(table, "Derivación", FormatBool(seguimiento.Derivacion));
                        CrearFila(table, "Observaciones", seguimiento.Observaciones);
                    });

                    AgregarFooter(page);
                });
            }).GeneratePdf();
        }

        // ==========================================
        // 4. REPORTE DE PARTO
        // ==========================================
        public byte[] GeneratePartoPdf(entParto parto)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurarPagina(page, "REGISTRO COMPLETO DE PARTO", Colors.Orange.Medium);

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // Usamos una tabla principal
                        col.Item().Table(table =>
                        {
                            DefinirColumnas(table);

                            CrearTituloSeccion(table, "DATOS GENERALES");
                            CrearFila(table, "Paciente", parto.NombrePaciente);
                            CrearFila(table, "Profesional Atiende", parto.NombreProfesional);
                            CrearFila(table, "Fecha del Parto", parto.Fecha.ToString("dd/MM/yyyy"));
                            CrearFila(table, "Establecimiento", parto.LugarNacimiento);
                            CrearFila(table, "Tipo de Seguro", parto.SeguroTipo);

                            CrearTituloSeccion(table, "CRONOLOGÍA");
                            CrearFila(table, "Hora Ingreso", parto.HoraIngreso?.ToString("HH:mm"));
                            CrearFila(table, "Hora Inicio Trabajo", parto.HoraInicioTrabajo?.ToString("HH:mm"));
                            CrearFila(table, "Hora Expulsión", parto.HoraExpulsion?.ToString("HH:mm"));
                            CrearFila(table, "Duración 2da Etapa", parto.DuracionSegundaEtapaMinutos?.ToString() + " min");

                            CrearTituloSeccion(table, "CARACTERÍSTICAS DEL PARTO");
                            CrearFila(table, "Vía de Parto", parto.NombreViaParto);
                            CrearFila(table, "Tipo de Parto", parto.TipoParto);
                            CrearFila(table, "Posición Materna", parto.PosicionMadre);
                            CrearFila(table, "Episiotomía", FormatBool(parto.Episiotomia));
                            CrearFila(table, "Desgarro", parto.Desgarro);
                            CrearFila(table, "Analgesia", parto.Analgesia);
                            CrearFila(table, "Acompañante", FormatBool(parto.Acompanante));

                            if (!string.IsNullOrEmpty(parto.IndicacionCesarea))
                                CrearFila(table, "Indicación Cesárea", parto.IndicacionCesarea);

                            CrearTituloSeccion(table, "LÍQUIDO Y MEMBRANAS");
                            CrearFila(table, "Estado Membranas", parto.Membranas);
                            CrearFila(table, "Tiempo Rotura (hrs)", parto.TiempoRoturaMembranasHoras?.ToString());
                            CrearFila(table, "Líquido Amniótico", parto.NombreLiquido);
                            CrearFila(table, "Aspecto Líquido", parto.AspectoLiquido);

                            CrearTituloSeccion(table, "ANTECEDENTES OBSTÉTRICOS");
                            CrearFila(table, "Hijos Previos", parto.NumeroHijosPrevios?.ToString());
                            CrearFila(table, "Cesáreas Previas", parto.NumeroCesareasPrevias?.ToString());
                            CrearFila(table, "Emb. Múltiple", FormatBool(parto.EmbarazoMultiple));

                            CrearTituloSeccion(table, "RESULTADOS Y COMPLICACIONES");
                            CrearFila(table, "Pérdidas Sangre (ml)", parto.PerdidasML?.ToString());
                            CrearFila(table, "Complicaciones", parto.ComplicacionesMaternas);
                            CrearFila(table, "Observaciones", parto.Observaciones);
                        });

                        // SECCIÓN DE BEBÉS (Fuera de la tabla principal para mejor manejo de lista)
                        if (parto.Bebes != null && parto.Bebes.Any())
                        {
                            col.Item().PaddingTop(15).Text("DATOS DEL RECIÉN NACIDO(S)").FontSize(12).Bold().FontColor(Colors.Orange.Darken2);

                            foreach (var bebe in parto.Bebes)
                            {
                                col.Item().PaddingTop(5).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(c =>
                                {
                                    c.Item().Text($"Bebé N° {bebe.NumeroBebe} ({bebe.Sexo})").Bold();
                                    c.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text($"Peso: {bebe.PesoGr} gr");
                                        row.RelativeItem().Text($"Estado: {bebe.EstadoBebe}");
                                        // Aquí puedes agregar más campos de la entidad Bebé si los tienes
                                    });
                                });
                            }
                        }
                        else
                        {
                            // Espacio vacío para llenar a mano si no se registraron bebés aún
                            col.Item().PaddingTop(15).Text("DATOS DEL RECIÉN NACIDO (A completar)").FontSize(12).Bold().FontColor(Colors.Grey.Darken2);
                            col.Item().PaddingTop(5).Border(1).BorderColor(Colors.Grey.Lighten2).Height(50);
                        }
                    });

                    AgregarFooter(page);
                });
            }).GeneratePdf();
        }

        // ==========================================
        // MÉTODOS AUXILIARES (LA CLAVE DEL DISEÑO)
        // ==========================================

        private void ConfigurarPagina(PageDescriptor page, string titulo, string colorHex)
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Header().PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(titulo).FontSize(16).Bold().FontColor(colorHex);
                    col.Item().Text("Reporte Detallado del Sistema").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void DefinirColumnas(TableDescriptor table)
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(140); // Ancho fijo para la etiqueta (Label)
                columns.RelativeColumn();    // El resto para el valor
            });
        }

        private void CrearTituloSeccion(TableDescriptor table, string titulo)
        {
            table.Cell().ColumnSpan(2).PaddingTop(10).PaddingBottom(5).BorderBottom(1.5f).BorderColor(Colors.Black)
                .Text(titulo).FontSize(11).Bold().FontColor(Colors.Black);
        }

        // ESTE ES EL MÉTODO IMPORTANTE:
        // Dibuja la fila SIEMPRE, tenga valor o sea NULL.
        private void CrearFila(TableDescriptor table, string etiqueta, string? valor)
        {
            // Celda Izquierda (Etiqueta)
            table.Cell().Element(EstiloCeldaEtiqueta).Text(etiqueta).SemiBold();

            // Celda Derecha (Valor)
            // Si el valor es nulo o vacío, ponemos un espacio en blanco " " para que QuestPDF dibuje la celda vacía correctamente.
            table.Cell().Element(EstiloCeldaValor).Text(valor ?? " ");
        }

        private string FormatBool(bool? valor)
        {
            if (!valor.HasValue) return " ";
            return valor.Value ? "Sí" : "No";
        }

        private void AgregarFooter(PageDescriptor page)
        {
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
                x.Span($" | Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}");
            });
        }
    }
}