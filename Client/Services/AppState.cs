using blazorissuesrepro.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace blazorissuesrepro.Services
{
    public class AppState
    {
        private HttpClient http;
        public AppState(HttpClient http)
        {
            this.http = http;

        }

        public WeatherForecast[] WeatherForecasts { get; private set; }

        public event Action OnChange;

        public async Task GetWeatherForecasts()
        {
            try
            {
                WeatherForecasts = await http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}