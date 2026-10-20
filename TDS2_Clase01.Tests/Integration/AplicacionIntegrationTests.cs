using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TDS2_Clase01.Tests.Integration
{
    public class AplicacionIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AplicacionIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Swagger_PublicaElDocumentoOpenApi_ConLosEndpointsDeColaboradores()
        {
            var client = _factory.CreateClient();

            var respuesta = await client.GetAsync("/swagger/v1/swagger.json");

            var json = await respuesta.Content.ReadAsStringAsync();
            // Si falla, el mensaje incluye el detalle de la excepción que devolvió el servidor.
            Assert.True(respuesta.IsSuccessStatusCode,
                $"HTTP {(int)respuesta.StatusCode}: {json[..Math.Min(json.Length, 3000)]}");
            Assert.Contains("Colaboradores API", json);
            Assert.Contains("/api/ColaboradorAPI/listar", json);
            Assert.Contains("/api/ColaboradorAPI/editar/{id}", json);
        }

        [Fact]
        public async Task VistasMvc_SinAutenticacion_RedirigenAlLogin()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var respuesta = await client.GetAsync("/Colaborador/ListadoColaborador");

            Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
            Assert.Contains("Identity/Account/Login", respuesta.Headers.Location!.ToString());
        }
    }
}
