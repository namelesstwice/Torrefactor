using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace Torrefactor.Controllers
{
	[AllowAnonymous]
	public class SpaController : Controller
	{
		private readonly IFileProvider _fileProvider;

		public SpaController(IFileProvider fileProvider)
		{
			_fileProvider = fileProvider;
		}

		public IActionResult Index()
		{
			var fileInfo = _fileProvider.GetFileInfo("index.html");
			var readStream = fileInfo.CreateReadStream();
			return File(readStream, "text/html");
		}
	}
}