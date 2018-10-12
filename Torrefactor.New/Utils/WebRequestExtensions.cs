using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Torrefactor.Utils
{
	public static class WebRequestExtensions
	{
		public static async Task<string> GetResponseStirngAsync(this HttpWebRequest request)
		{
			var response = await request.GetResponseAsync();
			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				return await reader.ReadToEndAsync();
			}
		}
	}
}