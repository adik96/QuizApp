using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TestMakerFreeWebApp.Data;
using Mapster;
namespace TestMakerFreeWebApp.Controllers
{
    [Route("api/[controller]")]
    public class BaseApiController : Controller
    {
        #region Konstruktor
        public BaseApiController(ApplicationDbContext context)
        {
            // Utwórz ApplicationDbContext, wykorzystując wstrzykiwanie zależności
            DbContext = context;
            // Utwórz pojedynczy obiekt JsonSerializerSettings,
            // który może być używany wielokrotnie
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion
        #region Współdzielone właściwości
        protected ApplicationDbContext DbContext { get; private set; }
        protected JsonSerializerSettings JsonSettings { get; private set; }
        #endregion
    }
}