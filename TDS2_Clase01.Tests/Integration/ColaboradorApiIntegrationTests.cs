using System.Net;
using System.Net.Http.Json;
using TDS2_Clase01.Models.Dtos;
using TDS2_Clase01.Models.Entidades;

namespace TDS2_Clase01.Tests.Integration
{
    public class ColaboradorApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private const string Ruta = "/api/ColaboradorAPI";

        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ColaboradorApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private object NuevoCuerpo(string nombres = "Ana", int? idEmpresa = null) => new
        {
            nombres,
            apellidos = "Torres Ruiz",
            dni = "12345678",
            sexo = "Femenino",
            direccion = "Av. Siempre Viva 742",
            idEmpresa = idEmpresa ?? _factory.EmpresaId
        };

        private async Task<int> RegistrarAsync(string nombres = "Ana")
        {
            var respuesta = await _client.PostAsJsonAsync(Ruta, NuevoCuerpo(nombres));
            respuesta.EnsureSuccessStatusCode();
            var cuerpo = await respuesta.Content.ReadFromJsonAsync<RegistroColaboradorRespuesta>();
            return cuerpo!.IdColaborador;
        }

        private async Task<Colaborador> ObtenerAsync(int id)
        {
            var colaborador = await _client.GetFromJsonAsync<Colaborador>($"{Ruta}/detalle/{id}");
            return colaborador!;
        }

        [Fact]
        public async Task Listar_SinAutenticacion_Devuelve200()
        {
            var respuesta = await _client.GetAsync($"{Ruta}/listar");

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        }

        [Fact]
        public async Task ListarEmpresa_DevuelveLaEmpresaSembrada()
        {
            var empresas = await _client.GetFromJsonAsync<List<Empresa>>($"{Ruta}/listarEmpresa");

            Assert.Contains(empresas!, e => e.IdEmpresa == _factory.EmpresaId);
        }

        [Fact]
        public async Task Registrar_Valido_Devuelve200_YElColaboradorSePuedeConsultar()
        {
            var id = await RegistrarAsync("Lucia");

            var colaborador = await ObtenerAsync(id);

            Assert.Equal("Lucia", colaborador.Nombres);
            Assert.NotEqual(default, colaborador.FechaRegistro);
            Assert.Null(colaborador.Modificacion);
        }

        [Fact]
        public async Task Registrar_EmpresaInexistente_Devuelve400()
        {
            var respuesta = await _client.PostAsJsonAsync(Ruta, NuevoCuerpo(idEmpresa: 9999));

            Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        }

        [Fact]
        public async Task Registrar_SinNombres_Devuelve400()
        {
            var cuerpo = new { apellidos = "Torres", dni = "12345678", sexo = "F", direccion = "X", idEmpresa = _factory.EmpresaId };

            var respuesta = await _client.PostAsJsonAsync(Ruta, cuerpo);

            Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        }

        [Fact]
        public async Task Detalle_IdInvalido_Devuelve400()
        {
            var respuesta = await _client.GetAsync($"{Ruta}/detalle/0");

            Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        }

        [Fact]
        public async Task Detalle_Inexistente_Devuelve404()
        {
            var respuesta = await _client.GetAsync($"{Ruta}/detalle/99999");

            Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
        }

        [Fact]
        public async Task Editar_Existente_ActualizaYConservaLaFechaDeRegistro()
        {
            var id = await RegistrarAsync();
            var antes = await ObtenerAsync(id);

            var cuerpo = new
            {
                idColaborador = id,
                nombres = "Ana Maria",
                apellidos = antes.Apellidos,
                dni = antes.DNI,
                sexo = antes.Sexo,
                direccion = "Calle Nueva 456",
                idEmpresa = _factory.EmpresaId
            };
            var respuesta = await _client.PutAsJsonAsync($"{Ruta}/editar/{id}", cuerpo);

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            var despues = await ObtenerAsync(id);
            Assert.Equal("Ana Maria", despues.Nombres);
            Assert.Equal("Calle Nueva 456", despues.Direccion);
            Assert.Equal(antes.FechaRegistro, despues.FechaRegistro);
            Assert.NotNull(despues.Modificacion);
        }

        [Fact]
        public async Task Editar_Inexistente_Devuelve404()
        {
            // Regresión del bug de la condición invertida en Editar.
            var cuerpo = new
            {
                idColaborador = 99999,
                nombres = "Ana",
                apellidos = "Torres",
                dni = "12345678",
                sexo = "Femenino",
                direccion = "X",
                idEmpresa = _factory.EmpresaId
            };

            var respuesta = await _client.PutAsJsonAsync($"{Ruta}/editar/99999", cuerpo);

            Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
        }

        [Fact]
        public async Task Eliminar_Existente_Devuelve200_YLuegoElDetalleDa404()
        {
            var id = await RegistrarAsync();

            var respuesta = await _client.DeleteAsync($"{Ruta}/eliminar/{id}");

            Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"{Ruta}/detalle/{id}")).StatusCode);
        }

        [Fact]
        public async Task Eliminar_Inexistente_Devuelve404()
        {
            var respuesta = await _client.DeleteAsync($"{Ruta}/eliminar/99999");

            Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
        }
    }
}
