using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestMakerFreeWebApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using TestMakerFreeWebApp.Data;
using Mapster;

namespace TestMakerFreeWebApp.Controllers
{
    public class ResultController : BaseApiController
    {
        #region Konstruktor
        public ResultController(ApplicationDbContext context)
            : base(context) { }
        #endregion

        // GET api/result/all
        [HttpGet("All/{quizId}")]
        public IActionResult All(int quizId)
        {
            var results = DbContext.Results
                .Where(q => q.QuizId == quizId)
                .ToArray();
            return new JsonResult(
            results.Adapt<ResultViewModel[]>(),
            JsonSettings);
        }
        #region Metody dostosowujące do konwencji REST
        /// <summary>
        /// Pobiera wynik o podanym {id}
        /// </summary>
        /// <param name="id">identyfikator istniejącego wyniku</param>
        /// <returns>wynik o podanym {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = DbContext.Results.Where(i => i.Id == id)
            .FirstOrDefault();
            // Obsłuż żądania proszące o nieistniejący wynik
            if (result == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono wyniku o identyfikatorze { 0}", id)
                });
            }
            return new JsonResult(
            result.Adapt<ResultViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Dodaje nowy wynik do bazy danych
        /// </summary>
        /// <param name="model">obiekt ResultViewModel z danymi do wstawienia</param>
        [HttpPut]
        public IActionResult Put([FromBody]ResultViewModel model)
        {
            // Zwraca ogólny kod statusu HTTP 500 (Server Error),
            // jeśli dane przesłane przez klienta są niewłaściwe
            if (model == null) return new StatusCodeResult(500);
            // Odwzorowuje ViewModel na Model
            var result = model.Adapt<Result>();
            // Nadpisz właściwości,
            // które powinny być ustawiane tylko przez serwer
            result.CreatedDate = DateTime.Now;
            result.LastModifiedDate = result.CreatedDate;
            // Dodaj nowy wynik
            DbContext.Results.Add(result);
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć nowo utworzony wynik klientowi
            return new JsonResult(result.Adapt<ResultViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Modyfikuje wynik o podanym {id}
        /// </summary>
        /// <param name="model">obiekt ResultViewModel z danymi do uaktualnienia</param>
        [HttpPost]
        public IActionResult Post([FromBody]ResultViewModel model)
        {
            // Zwraca ogólny kod statusu HTTP 500 (Server Error),
            // jeśli dane przesłane przez klienta są niewłaściwe
            if (model == null) return new StatusCodeResult(500);
            // Pobierz wynik do zmodyfikowania
            var result = DbContext.Results.Where(q => q.Id ==
            model.Id).FirstOrDefault();
            // Obsłuż żądania proszące o nieistniejący wynik
            if (result == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono wyniku o identyfikatorze {0}",
                model.Id)
                });
            }
            // Obsłuż aktualizację (bez odwzorowania obiektów)
            // za pomocą ręcznego przepisania właściwości
            // otrzymanych w żądaniu od klienta
            result.QuizId = model.QuizId;
            result.Text = model.Text;
            result.MinValue = model.MinValue;
            result.MaxValue = model.MaxValue;
            result.Notes = model.Notes;
            // Właściwości ustawiane tylko przez serwer
            result.LastModifiedDate = DateTime.Now;
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć uaktualniony wynik klientowi
            return new JsonResult(result.Adapt<ResultViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Usuwa wynik o podanym {id} z bazy danych
        /// </summary>
        /// <param name="id">identyfikator istniejącego wyniku</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Pobierz wynik z bazy danych
            var result = DbContext.Results.Where(i => i.Id == id)
            .FirstOrDefault();
            // Obsłuż żądania proszące o nieistniejący wynik
            if (result == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono wyniku o identyfikatorze { 0}", id)
            });
            }
            // Usuń wynik z bazy danych
            DbContext.Results.Remove(result);
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć kod statusu HTTP 204
            return new NoContentResult();
        }
        #endregion
    }
}