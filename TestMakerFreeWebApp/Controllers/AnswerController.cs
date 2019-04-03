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
    public class AnswerController : BaseApiController
    {
        #region Konstruktor
        public AnswerController(ApplicationDbContext context)
            : base(context) { }
        #endregion

        // GET api/answer/all
        [HttpGet("All/{questionId}")]
        public IActionResult All(int questionId)
        {
            var answers = DbContext.Answers
                .Where(q => q.QuestionId == questionId)
                .ToArray();
            return new JsonResult(
            answers.Adapt<AnswerViewModel[]>(),
            JsonSettings);
        }
        #region Metody dostosowujące do konwencji REST
        /// <summary>
        /// Pobiera odpowiedź o podanym {id}
        /// </summary>
        /// <param name="id">identyfikator istniejącej odpowiedzi</param>
        /// <returns>odpowiedź o podanym {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var answer = DbContext.Answers.Where(i => i.Id == id)
                .FirstOrDefault();
            // Obsłuż żądania proszące o nieistniejącą odpowiedź
            if (answer == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono odpowiedzi o identyfikatorze { 0}", id)
                });
            }
            return new JsonResult(
            answer.Adapt<AnswerViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Dodaje nową odpowiedź do bazy danych
        /// </summary>
        /// <param name="model">obiekt AnswerViewModel z danymi do wstawienia</param>
        [HttpPut]
        public IActionResult Put([FromBody]AnswerViewModel model)
        {
            // Zwraca ogólny kod statusu HTTP 500 (Server Error),
            // jeśli dane przesłane przez klienta są niewłaściwe
            if (model == null) return new StatusCodeResult(500);
            // Odwzorowuje ViewModel na Model
            var answer = model.Adapt<Answer>();
            // Nadpisz właściwości,
            // które powinny być ustawiane tylko przez serwer
            answer.CreatedDate = DateTime.Now;
            answer.LastModifiedDate = answer.CreatedDate;
            // Dodaj nową odpowiedź
            DbContext.Answers.Add(answer);
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć nowo utworzoną odpowiedź klientowi
            return new JsonResult(answer.Adapt<AnswerViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Modyfikuje odpowiedź o podanym {id}
        /// </summary>
        /// <param name="model">obiekt AnswerViewModel z danymi do uaktualnienia</param>
        [HttpPost]
        public IActionResult Post([FromBody]AnswerViewModel model)
        {
            // Zwraca ogólny kod statusu HTTP 500 (Server Error),
            // jeśli dane przesłane przez klienta są niewłaściwe
            if (model == null) return new StatusCodeResult(500);
            // Pobierz odpowiedź do zmodyfikowania
            var answer = DbContext.Answers.Where(q => q.Id ==
            model.Id).FirstOrDefault();
            // Obsłuż żądania proszące o nieistniejącą odpowiedź
            if (answer == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono odpowiedzi o identyfikatorze { 0}", model.Id)});
        }
        // Obsłuż aktualizację (bez odwzorowania obiektów)
        // za pomocą ręcznego przepisania właściwości
        // otrzymanych w żądaniu od klienta
        answer.QuestionId = model.QuestionId;
        answer.Text = model.Text;
        answer.Value = model.Value;
        answer.Notes = model.Notes;
        // Właściwości ustawiane tylko przez serwer
        answer.LastModifiedDate = DateTime.Now;
        // Zapisz zmiany w bazie danych
        DbContext.SaveChanges();
        // Zwróć uaktualnioną odpowiedź klientowi
        return new JsonResult(answer.Adapt<AnswerViewModel>(),
        JsonSettings);
        }
        /// <summary>
        /// Usuwa odpowiedź o podanym {id} z bazy danych
        /// </summary>
        /// <param name="id">identyfikator istniejącej odpowiedzi</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Pobierz odpowiedź z bazy danych
            var answer = DbContext.Answers.Where(i => i.Id == id)
            .FirstOrDefault();
            // Obsłuż żądania proszące o nieistniejącą odpowiedź
            if (answer == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono odpowiedzi o identyfikatorze { 0}", id)
                });
        }
        // Usuń odpowiedź z bazy danych
        DbContext.Answers.Remove(answer);
        // Zapisz zmiany w bazie danych
        DbContext.SaveChanges();
        // Zwróć kod statusu HTTP 204
        return new NoContentResult();
        }
        #endregion
    }
}